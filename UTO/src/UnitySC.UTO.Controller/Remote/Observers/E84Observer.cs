using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.Saliences;
using Agileo.GUI.Services.UserMessages;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Efem;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Devices.Controller;
using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.Services;
using UnitySC.UTO.Controller.Views.Panels.Production.Equipment;

using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class E84Observer : E30StandardSupport
    {
        #region Fields

        private readonly IE87Standard _e87Standard;
        private Equipment.Devices.Controller.Controller _controller;
        private Efem _efem;
        private bool _lightCurtainErrorDetected;
        private List<LoadPort> _loadPorts;

        private bool _unloadAccessModeViolationInProgress;
        private readonly Dictionary<int, UserMessage> _loadPortMessage = new();

        public List<int> AccessViolationLoadPorts { get; } = new();
        #endregion

        #region Constructor

        public E84Observer(IE87Standard e87Standard, IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
            _e87Standard = e87Standard;
        }

        #endregion

        #region Overrides

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            _controller = Equipment.AllOfType<Equipment.Devices.Controller.Controller>().First();
            _controller.StatusValueChanged += Controller_StatusValueChanged;

            _efem = Equipment.AllOfType<Efem>().First();
            _efem.StatusValueChanged += Efem_StatusValueChanged;

            _loadPorts = Equipment.AllOfType<LoadPort>().ToList();

            foreach (var loadPort in _loadPorts)
            {
                loadPort.StatusValueChanged += LoadPort_StatusValueChanged;
                loadPort.E84ErrorOccurred += LoadPort_E84ErrorOccurred;
                loadPort.CommandExecutionStateChanged += LoadPort_CommandExecutionStateChanged;
            }

            _e87Standard.TransferStateChanged += E87Standard_TransferStateChanged;
        }

        private void LoadPort_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            switch (e.NewState)
            {
                case ExecutionState.Success:
                    if (e.Execution.Context.Command.Name == nameof(ILoadPort.Initialize))
                    {
                        if (AccessViolationLoadPorts.Contains(loadPort.InstanceId))
                        {
                            if (loadPort.CarrierPresence != CassettePresence.Absent)
                            {
                                LoadAccessModeViolation(loadPort, true);
                            }
                            else
                            {
                                AccessViolationLoadPorts.Remove(loadPort.InstanceId);
                            }
                        }
                    }
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _controller.StatusValueChanged -= Controller_StatusValueChanged;
                _efem.StatusValueChanged -= Efem_StatusValueChanged;
                foreach (var loadPort in Equipment.AllOfType<LoadPort>())
                {
                    loadPort.StatusValueChanged -= LoadPort_StatusValueChanged;
                    loadPort.E84ErrorOccurred -= LoadPort_E84ErrorOccurred;
                    loadPort.CommandExecutionStateChanged -= LoadPort_CommandExecutionStateChanged;
                }

                _e87Standard.TransferStateChanged -= E87Standard_TransferStateChanged;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers

        private void LoadPort_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            //Do not raise event when controller is in maintenance state
            if (_controller.State is OperatingModes.Maintenance or OperatingModes.Engineering)
            {
                return;
            }

            if (sender is not LoadPort loadPort)
            {
                return;
            }

            if (loadPort.AccessMode != LoadingType.Auto)
            {
                return;
            }

            var e87LoadPort = _e87Standard.GetLoadPort(loadPort.InstanceId);

            switch (e.Status.Name)
            {
                case nameof(LoadPort.O_L_REQ):
                    if (loadPort.O_L_REQ)
                    {
                        try
                        {
                            _e87Standard.IntegrationServices.NotifyCarrierLoadingStarted(loadPort.InstanceId);
                        }
                        catch (Exception exception)
                        {
                            Logger.Error(exception, "Error occurred in E87 services.");
                        }
                    }
                    break;

                case nameof(LoadPort.O_U_REQ):
                    if (loadPort.O_U_REQ)
                    {
                        try
                        {
                            _e87Standard.IntegrationServices.NotifyCarrierUnloadingStarted(loadPort.InstanceId);
                        }
                        catch (Exception exception)
                        {
                            Logger.Error(exception, "Error occurred in E87 services.");
                        }
                    }
                    break;

                case nameof(LoadPort.E84TransferInProgress):
                    if (!loadPort.E84TransferInProgress
                        && !_lightCurtainErrorDetected
                        && loadPort.CurrentCommand != nameof(LoadPort.Initialize)
                        && !loadPort.NeedsInitAfterE84Error())
                    {
                        if (loadPort.CarrierPresence == CassettePresence.Absent)
                        {
                            try
                            {
                                _e87Standard.IntegrationServices.NotifyCarrierUnloadingFinished(loadPort.InstanceId);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }
                        }

                        if (loadPort.CarrierPresence == CassettePresence.Correctly
                            && !loadPort.IsClamped
                            && loadPort.CanExecute(nameof(ILoadPort.Clamp), out _)
                            && e87LoadPort.TransferState == TransferState.TransferBlocked)
                        {
                            loadPort.ClampAsync();
                        }
                    }
                    break;

                case nameof(LoadPort.I_CS_0):
                case nameof(LoadPort.I_CS_1):
                case nameof(LoadPort.I_VALID):
                case nameof(LoadPort.I_TR_REQ):
                case nameof(LoadPort.I_BUSY):
                case nameof(LoadPort.I_COMPT):
                case nameof(LoadPort.I_CONT):
                    CheckActiveSignalsOff(loadPort);
                    break;

                case nameof(LoadPort.CarrierPresence):

                    if (_unloadAccessModeViolationInProgress && loadPort.CarrierPresence is CassettePresence.Absent or CassettePresence.Correctly)
                    {
                        _unloadAccessModeViolationInProgress = false;

                        if (loadPort.NeedsInitAfterE84Error())
                        {
                            loadPort.InitializeAsync(true);
                        }
                    }

                    //Carrier presence should not change in auto mode when O_HO_AVLB is ON
                    if (loadPort.State is not OperatingModes.Initialization
                        && loadPort.AccessMode == LoadingType.Auto
                        && e87LoadPort.AccessMode == AccessMode.Auto
                        && !loadPort.I_BUSY)
                    {
                        if ((loadPort.CarrierPresence == CassettePresence.Correctly
                             || loadPort.CarrierPresence == CassettePresence.NoPresentPlacement
                             || loadPort.CarrierPresence == CassettePresence.PresentNoPlacement)
                            && e87LoadPort.TransferState == TransferState.ReadyToLoad)
                        {
                            LoadAccessModeViolation(loadPort);
                        }
                        else if ((loadPort.CarrierPresence == CassettePresence.Absent
                                 || loadPort.CarrierPresence == CassettePresence.NoPresentPlacement
                                 || loadPort.CarrierPresence == CassettePresence.PresentNoPlacement)
                                 && e87LoadPort.TransferState == TransferState.ReadyToUnload)
                        {
                            UnloadAccessModeViolation(loadPort);
                        }
                    }
                    break;
            }
        }

        private void LoadPort_E84ErrorOccurred(object sender, E84ErrorOccurredEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            if (GetE84Alarm(e.Error, loadPort.InstanceId) is not { } alarm)
            {
                return;
            }

            var variableUpdate = new List<VariableUpdate>
            {
                    new(DVs.AlarmExtendedText, $"{alarm.Name} alarm occurred on LoadPort{loadPort.InstanceId}"),
                    new(DVs.AlarmDescription, alarm.Text)
            };

            E30Standard.AlarmServices.SetAlarm(alarm.ID, variableUpdate);

            if (e.Error is E84Errors.Tp3Timeout or E84Errors.Tp4Timeout)
            {
                DisplayE84TimeoutPopup(e.Error, loadPort);
            }
            else
            {
                CheckActiveSignalsOff(loadPort);
            }
        }

        private void E87Standard_TransferStateChanged(object sender, TransferStateChangedArgs e)
        {
            if (e.LoadPort.AccessMode == AccessMode.Manual)
            {
                return;
            }

            Task.Factory.StartNew(
                () =>
                {
                    var deviceLoadPort = _loadPorts
                        .First(lp => lp.InstanceId == e.LoadPort.PortID);

                    UpdateTransferState(deviceLoadPort, e.CurrentState, e.PreviousSate);
                });
        }

        private void Controller_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(IController.State))
            {
                switch (_controller.State)
                {
                    case OperatingModes.Engineering:
                    case OperatingModes.Maintenance:
                        foreach (var loadPort in _e87Standard.LoadPorts)
                        {
                            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                                .First(lp => lp.Value.InstanceId == loadPort.PortID)
                                .Value;

                            if (deviceLoadPort.E84TransferInProgress)
                            {
                                deviceLoadPort.DisableE84();
                            }
                        }
                        break;
                }
            }
        }

        private void Efem_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status.Name)
            {
                case nameof(IEfem.LightCurtainBeam):

                    var loadPortsAtRisk = _loadPorts.Where(
                        lp => lp.IsInService && lp.AccessMode is LoadingType.Auto && !lp.IsDocked && lp.O_HO_AVBL).ToList();

                    if (!_efem.LightCurtainBeam && loadPortsAtRisk.Any()) 
                    {
                        _lightCurtainErrorDetected = true;
                        App.ControllerInstance.ControllerEquipmentManager.LightTower
                            .DefineBuzzerModeAsync(BuzzerState.Slow);

                        var alarm = E30Standard.AlarmServices.GetAlarm(E84Alarms.LightCurtainError);
                        var variableUpdate = new List<VariableUpdate>()
                        {
                            new(DVs.AlarmDescription, alarm.Text),
                            new(DVs.AlarmExtendedText, "Light curtain alarm occurred")
                        };

                        E30Standard.AlarmServices.SetAlarm(alarm.ID, variableUpdate);

                        foreach (var loadPort in loadPortsAtRisk)
                        {
                            if (_e87Standard.GetLoadPort(loadPort.InstanceId) is { } e87LoadPort
                                && e87LoadPort.TransferState is not TransferState.OutOfService
                                    or TransferState.TransferBlocked)
                            {
                                loadPort.DisableE84Async().ContinueWith(_ => loadPort.ManageEsSignalAsync(false));

                                try
                                {
                                    _e87Standard.IntegrationServices.NotifyLoadPortTransferBlocked(
                                        loadPort.InstanceId);
                                }
                                catch (Exception exception)
                                {
                                    Logger.Error(exception, "Error occurred in E87 services.");
                                }
                            }
                        }

                        if (!IsLocalMode())
                        {
                           App.UtoInstance.MainUserMessageDisplayer.Show(new UserMessage(MessageLevel.Warning ,"Light curtain is crossed"));
                           return;
                        }

                        GUI.Common.App.Instance.Dispatcher.Invoke(
                            () =>
                            {
                                Popup popup = new Popup(
                                    new LocalizableText(nameof(E84Resources.E84_LIGHTCURTAIN_ERROR)),
                                    new LocalizableText(nameof(E84Resources.E84_LIGHTCURTAIN_ERROR_MESSAGE)));

                                popup.Commands.Add(
                                    new PopupCommand(
                                        nameof(E84Resources.E84_RESET_ES),
                                        new DelegateCommand(
                                            () =>
                                            {
                                                Task.Run(RecoveryLightCurtainError);
                                            },
                                            () => _efem.LightCurtainBeam && _lightCurtainErrorDetected)));
                                AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel.Popups.Show(popup);
                            });
                    }

                    if (_efem.LightCurtainBeam && _lightCurtainErrorDetected)
                    {
                        App.ControllerInstance.ControllerEquipmentManager.LightTower
                            .DefineBuzzerModeAsync(BuzzerState.Off);

                        if (!IsLocalMode())
                        {
                            App.UtoInstance.MainUserMessageDisplayer.HideAll();
                            RecoveryLightCurtainError();
                        }
                    }
          
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void RecoveryLightCurtainError()
        {
            var loadPortsToRecover = _loadPorts.Where(
                lp => lp.IsInService && lp.AccessMode is LoadingType.Auto && !lp.IsDocked && !lp.O_HO_AVBL).ToList();

            if (loadPortsToRecover.Any())
            {
                if (!loadPortsToRecover.All(CheckResetActiveSignals))
                {
                    return;
                }

                foreach (var loadPort in loadPortsToRecover)
                {
                    if (_e87Standard.GetLoadPort(loadPort.InstanceId) is { } e87LoadPort
                        && e87LoadPort.TransferState is TransferState.TransferBlocked)
                    {
                        try
                        {
                            loadPort.EnableE84Async();
                            _e87Standard.IntegrationServices.NotifyLoadPortTransferReady(
                                loadPort.InstanceId);
                        }
                        catch (Exception exception)
                        {
                            Logger.Error(exception, "Error occurred in E87 services.");
                        }
                    }
                }
            }

            _lightCurtainErrorDetected = false;
            E30Standard.AlarmServices.ClearAlarm(E84Alarms.LightCurtainError);
        }

        private E30Alarm GetE84Alarm(E84Errors error, int loadPortId)
        {
            switch (error)
            {
                case E84Errors.Tp1Timeout:
                    return E30Standard.AlarmServices.GetAlarm($"E84:TP1:LoadPort:{loadPortId}");
                case E84Errors.Tp2Timeout:
                    return E30Standard.AlarmServices.GetAlarm($"E84:TP2:LoadPort:{loadPortId}");
                case E84Errors.Tp3Timeout:
                    return E30Standard.AlarmServices.GetAlarm($"E84:TP3:LoadPort:{loadPortId}");
                case E84Errors.Tp4Timeout:
                    return E30Standard.AlarmServices.GetAlarm($"E84:TP4:LoadPort:{loadPortId}");
                case E84Errors.Tp5Timeout:
                    return E30Standard.AlarmServices.GetAlarm($"E84:TP5:LoadPort:{loadPortId}");
                case E84Errors.SignalError:
                    return E30Standard.AlarmServices.GetAlarm($"E84:HandoffError:LoadPort:{loadPortId}");
                default:
                    return null;
            }
        }

        private bool CheckResetActiveSignals(LoadPort loadPort)
        {
            return !loadPort.I_CS_0
                   && !loadPort.I_CS_1
                   && !loadPort.I_VALID
                   && !loadPort.I_TR_REQ
                   && !loadPort.I_BUSY
                   && !loadPort.I_COMPT
                   && !loadPort.I_CONT;
        }

        private void CheckActiveSignalsOff(LoadPort loadPort)
        {
            if (CheckResetActiveSignals(loadPort)
                && loadPort.State == OperatingModes.Maintenance
                && loadPort.NeedsInitAfterE84Error())
            {
                if (loadPort.CurrentE84Error == E84Errors.Tp3Timeout
                    || loadPort.CurrentE84Error == E84Errors.Tp4Timeout)
                {
                    return;
                }
                loadPort.InitializeAsync(true);
            }
        }

        private void DisplayE84TimeoutPopup(E84Errors error, LoadPort loadPort)
        {
            Logger.Info($"Displaying E84 recovery popup following {error} timeout error.");

            UserMessage userMessage;
            GUI.Common.App.Instance.Dispatcher.Invoke(
                () =>
                {
                    string errorTitle;
                    switch (error)
                    {
                        case E84Errors.Tp3Timeout:
                            errorTitle = "TP3 Timeout";
                            break;
                        case E84Errors.Tp4Timeout:
                            errorTitle = "TP4 Timeout";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(error));
                    }
                    userMessage = new UserMessage(
                        MessageLevel.Error,
                        new LocalizableText(nameof(E84Resources.E84_RECOVER_ERROR_MESSAGE), errorTitle));

                    userMessage.Commands.Add(
                        new UserMessageCommand(
                            nameof(E84Resources.E84_RESET_PIO),
                            new DelegateCommand(
                                () =>
                                {
                                    Logger.Info(
                                        $"User triggered E84 recovery procedure via the popup.");

                                    HideMessage(loadPort);

                                    Task.Run(
                                        () =>
                                        {
                                            loadPort.Initialize(true);
                                        });
                                },
                                () => CheckResetActiveSignals(loadPort)),
                            PathIcon.Check));

                    DisplayMessage(userMessage, loadPort);
                });
        }

        private void LoadAccessModeViolation(LoadPort loadPort, bool reactivate = false)
        {
            if (AccessViolationLoadPorts.Contains(loadPort.InstanceId)
                && !reactivate)
            {
                return;
            }

            if (!reactivate)
            {
                AccessViolationLoadPorts.Add(loadPort.InstanceId);
            }

            try
            {
                _e87Standard.IntegrationServices.NotifyCarrierLoadingStarted(loadPort.InstanceId);
                _e87Standard.IntegrationServices.NotifyAccessModeViolationOccurred(loadPort.InstanceId, true);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error occurred in E87 services.");
            }

            Task.Factory.StartNew(() =>
            {
                loadPort.DisableE84();

                GUI.Common.App.Instance.Dispatcher.Invoke(
                    () =>
                    {
                        EventHandler<StatusChangedEventArgs> loadPortHandler = null;
                        loadPortHandler = (_, e) =>
                        {
                            if (e.Status.Name == nameof(LoadPort.CarrierPresence)
                                && loadPort.CarrierPresence == CassettePresence.Absent)
                            {
                                loadPort.StatusValueChanged -= loadPortHandler;
                                HideMessage(loadPort);
                                loadPort.InitializeAsync(true);
                            }
                        };


                        var userMessage = new UserMessage(
                            MessageLevel.Error,
                            new LocalizableText(
                                nameof(E84Resources.E84_LOAD_ACCESS_MODE_VIOLATION_MESSAGE),
                                nameof(E84Resources.E84_LOAD_ACCESS_MODE_VIOLATION)));

                        userMessage.Commands.Add(
                            new UserMessageCommand(
                                nameof(E84Resources.E84_CONTINUE),
                                new DelegateCommand(
                                    () =>
                                    {
                                        loadPort.StatusValueChanged -= loadPortHandler;
                                        HideMessage(loadPort);
                                        AccessViolationLoadPorts.Remove(loadPort.InstanceId);
                                        loadPort.InitializeAsync(true);
                                    },
                                    () => loadPort.CarrierPresence == CassettePresence.Correctly
                                    && loadPort.State != OperatingModes.Initialization && loadPort.State != OperatingModes.Executing),
                                PathIcon.Check));

                        loadPort.StatusValueChanged += loadPortHandler;
                        DisplayMessage(userMessage, loadPort);
                    });
            });
        }

        private void UnloadAccessModeViolation(LoadPort loadPort)
        {
            try
            {
                _e87Standard.IntegrationServices.NotifyCarrierUnloadingStarted(loadPort.InstanceId);
                _e87Standard.IntegrationServices.NotifyAccessModeViolationOccurred(loadPort.InstanceId, false);

                if (loadPort.CarrierPresence == CassettePresence.Absent)
                {
                    loadPort.InitializeAsync(true);
                }
                else
                {
                    _unloadAccessModeViolationInProgress = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error occurred in E87 services.");
            }
        }

        private void UpdateTransferState(LoadPort loadPort, TransferState currentState, TransferState? previousState)
        {
            if (!loadPort.Configuration.IsE84Enabled)
            {
                return;
            }

            switch (currentState)
            {
                case TransferState.ReadyToLoad:
                    if (loadPort.E84TransferInProgress)
                    {
                        loadPort.DisableE84();
                    }
                    loadPort.EnableE84();
                    if (loadPort.CarrierPresence == CassettePresence.Absent)
                    {
                        loadPort.RequestLoad();
                    }
                    break;
                case TransferState.ReadyToUnload:
                    if (loadPort.E84TransferInProgress)
                    {
                        loadPort.DisableE84();
                    }
                    loadPort.EnableE84();
                    if (loadPort.CarrierPresence == CassettePresence.Correctly)
                    {
                        loadPort.RequestUnload();
                    }
                    break;
            }
        }

        private void DisplayMessage(UserMessage userMessage, LoadPort loadPort)
        {
            if (_loadPortMessage.ContainsKey(loadPort.InstanceId))
            {
                HideMessage(loadPort);
            }

            if (App.ControllerInstance.UserInterface.BusinessPanels.FirstOrDefault(
                    x => x is EquipmentPanel) is not EquipmentPanel mainPanel)
            {
                Logger.Error("Could not main panel to display error recovery message.");
                return;
            }

            _loadPortMessage[loadPort.InstanceId] = userMessage;
            mainPanel.Saliences.Add(SalienceType.Alarm);
            mainPanel.DisplayUserMessageOnLoadPortViewer(userMessage, loadPort.InstanceId);
        }

        private void HideMessage(LoadPort loadPort)
        {
            if (!_loadPortMessage.ContainsKey(loadPort.InstanceId))
            {
                return;
            }

            var userMessage = _loadPortMessage[loadPort.InstanceId];

            if (App.ControllerInstance.UserInterface.BusinessPanels.FirstOrDefault(
                    x => x is EquipmentPanel) is not EquipmentPanel mainPanel)
            {
                Logger.Error("Could not main panel to display error recovery message.");
                return;
            }

            mainPanel.Saliences.Remove(SalienceType.Alarm);
            mainPanel.HideUserMessageOnLoadPortViewer(userMessage, loadPort.InstanceId);
            _loadPortMessage.Remove(loadPort.InstanceId);
        }

        #endregion
    }
}
