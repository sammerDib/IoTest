using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Common.Configuration;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E94;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.JobDefinition;
using UnitySC.Equipment.Devices.Controller;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.Shared.Tools.Collection;
using UnitySC.GUI.Common.UIComponents.Popup;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.Services;
using UnitySC.UTO.Controller.Views.Panels.Gem;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts.Popups;

using EquipmentResources = UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.EquipmentResources;
using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;
using Status = Agileo.Semi.Gem300.Abstractions.E87.Status;
using Popup = Agileo.GUI.Services.Popups.Popup;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class E87Observer : E30StandardSupport
    {
        #region Fields

        private readonly IE87Standard _e87Standard;
        private Equipment.Devices.Controller.Controller _controller;
        private bool[] _doNotProceedByLoadPort;
        private Dictionary<string, JobStatus> _previousJobStatusMap;
        private IE94Standard _e94Standard => App.ControllerInstance.GemController.E94Std;
        private readonly Dictionary<int, TransferState?> _previousTransferStates;
        private int _ceUnknownCarrierId;
        private int _dvPortId;
        #endregion

        #region Constructor

        public E87Observer(IE87Standard e87Standard, IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
            _e87Standard = e87Standard;
            _previousTransferStates = new Dictionary<int, TransferState?>();
        }

        #endregion

        #region Overrides

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            _previousJobStatusMap = new Dictionary<string, JobStatus>();

            _controller = Equipment.AllOfType<Equipment.Devices.Controller.Controller>().First();
            _controller.StatusValueChanged += Controller_StatusValueChanged;
            _controller.JobStatusChanged += Controller_JobStatusChanged;
            var numberOfLoadPort = 0;
            foreach (var loadPort in Equipment.AllOfType<LoadPort>())
            {
                loadPort.CommandExecutionStateChanged += LoadPort_CommandExecutionStateChanged;
                loadPort.ConfigManager.CurrentChanged += LoadPort_ConfigManager_CurrentChanged;
                loadPort.CarrierIdRequestedFromOperator += LoadPort_CarrierIdRequestedFromOperator;
                numberOfLoadPort++;
            }

            _doNotProceedByLoadPort = new bool[numberOfLoadPort];

            _e87Standard.CarrierInstantiated += E87Standard_CarrierInstantiated;
            _e87Standard.CarrierIdStateChanged += E87Standard_CarrierIdStateChanged;
            _e87Standard.TransferStateChanged += E87Standard_TransferStateChanged;
            _e87Standard.AccessModeStateChanged += E87Standard_AccessModeStateChanged;
            _e87Standard.SlotMapStateChanged += E87Standard_SlotMapStateChanged;
            _e87Standard.E30Standard.DataServices.EventSent += DataServices_EventSent;

            _ceUnknownCarrierId = _e87Standard.E30Standard.DataServices.GetEventID(E87WellknownNames.CEIDs.UnknownCarrierID);
            _dvPortId = _e87Standard.E30Standard.DataServices.GetVariableIDByName(E87WellknownNames.DVs.PortID);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _controller.StatusValueChanged -= Controller_StatusValueChanged;
                _controller.JobStatusChanged -= Controller_JobStatusChanged;

                foreach (var loadPort in Equipment.AllOfType<LoadPort>())
                {
                    loadPort.StatusValueChanged -= LoadPort_StatusValueChanged;
                    loadPort.CommandExecutionStateChanged -= LoadPort_CommandExecutionStateChanged;
                    loadPort.ConfigManager.CurrentChanged -= LoadPort_ConfigManager_CurrentChanged;
                    loadPort.CarrierIdRequestedFromOperator -=
                        LoadPort_CarrierIdRequestedFromOperator;
                }

                _e87Standard.CarrierInstantiated -= E87Standard_CarrierInstantiated;
                _e87Standard.CarrierIdStateChanged -= E87Standard_CarrierIdStateChanged;
                _e87Standard.TransferStateChanged -= E87Standard_TransferStateChanged;
                _e87Standard.AccessModeStateChanged -= E87Standard_AccessModeStateChanged;
                _e87Standard.SlotMapStateChanged -= E87Standard_SlotMapStateChanged;
                _e87Standard.E30Standard.DataServices.EventSent -= DataServices_EventSent;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers

        private void E87Standard_CarrierInstantiated(object sender, CarrierEventArgs e)
        {
            if (e?.Carrier != null
                && e.Carrier.CarrierIdStatus is CarrierIdStatus.VerificationOk
                    or CarrierIdStatus.VerificationFailed)
            {
                var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                    .First(lp => lp.Value.InstanceId == e.Carrier.PortID)
                    .Value;

                if (e.Carrier.ObjID != deviceLoadPort.Carrier.Id)
                {
                    deviceLoadPort.SetCarrierId(e.Carrier.ObjID);
                }
            }
        }

        private void E87Standard_TransferStateChanged(object sender, TransferStateChangedArgs e)
        {
            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                .First(lp => lp.Value.InstanceId == e.LoadPort.PortID)
                .Value;

            if (deviceLoadPort == null || !deviceLoadPort.IsCommunicating)
            {
                return;
            }

            _previousTransferStates[deviceLoadPort.InstanceId] = e.PreviousSate;

            Task.Factory.StartNew(
                () =>
                {
                    switch (e.CurrentState)
                    {
                        case TransferState.OutOfService:
                            deviceLoadPort.SetLightAsync(
                                LoadPortLightRoleType.UnloadReady,
                                LightState.Off);
                            deviceLoadPort.SetLightAsync(
                                LoadPortLightRoleType.LoadReady,
                                LightState.Off);
                            deviceLoadPort.SetLightAsync(
                                LoadPortLightRoleType.AccessModeManual,
                                LightState.Off);
                            break;
                        case TransferState.TransferBlocked:
                            deviceLoadPort.SetLightAsync(
                                LoadPortLightRoleType.UnloadReady,
                                LightState.Off);
                            deviceLoadPort.SetLightAsync(
                                LoadPortLightRoleType.LoadReady,
                                LightState.Off);
                            break;
                        case TransferState.ReadyToLoad:
                            if (deviceLoadPort.CarrierPresence == CassettePresence.Absent)
                            {
                                deviceLoadPort.SetLightAsync(
                                    LoadPortLightRoleType.UnloadReady,
                                    LightState.Off);
                                deviceLoadPort.SetLightAsync(
                                    LoadPortLightRoleType.LoadReady,
                                    LightState.On);
                            }

                            break;
                        case TransferState.ReadyToUnload:
                            E30Standard.AlarmServices.ClearAlarm(
                                E87WellknownNames.Alarms.DuplicateCarrierID);
                            if (deviceLoadPort.CarrierPresence == CassettePresence.Correctly)
                            {
                                deviceLoadPort.SetLightAsync(
                                    LoadPortLightRoleType.UnloadReady,
                                    LightState.On);
                                deviceLoadPort.SetLightAsync(
                                    LoadPortLightRoleType.LoadReady,
                                    LightState.Off);
                            }

                            if (App.UtoInstance.JobQueueManager.IsCarrierStillNeeded(e.LoadPort.AssociatedCarrier?.ObjID)
                                && App.ControllerInstance.ControllerConfig.UnloadCarrierBetweenJobs
                                && IsLocalMode())
                            {
                                _e87Standard.StandardServices.CarrierReCreate(
                                    e.LoadPort.AssociatedCarrier.ObjID,
                                    new E87PropertiesList(e.LoadPort.AssociatedCarrier.SlotMap,
                                                            e.LoadPort.AssociatedCarrier.ContentMap));
                            }
                            break;
                    }

                    if (e.CurrentState is TransferState.OutOfService
                        || e.PreviousSate is TransferState.OutOfService)
                    {
                        deviceLoadPort.SetIsInService(e.CurrentState != TransferState.OutOfService);
                    }
                });
        }

        private void E87Standard_CarrierIdStateChanged(object sender, CarrierIdStateChangedArgs e)
        {
            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                .First(lp => lp.Value.InstanceId == e.Carrier.PortID)
                .Value;

            if (e.CurrentState == CarrierIdStatus.WaitingForHost && IsLocalMode())
            {
                var loadPort = _e87Standard.LoadPorts.FirstOrDefault(lp => lp.PortID == e.Carrier.PortID);
                if (loadPort == null)
                {
                    return;

                }

                //Special use case
                //Local mode: Carrier recreate without carrierIdReader and ByPassReadId at false
                if (loadPort.AssociationState == AssociationState.Associated
                    && !_e87Standard.GetBypassReadID(loadPort.PortID)
                    && !deviceLoadPort.Configuration.IsCarrierIdSupported)
                {
                    Task.Run(
                        () =>
                        {
                            _e87Standard.StandardServices.ProceedWithCarrier(
                                e.Carrier.PortID,
                                e.Carrier.ObjID,
                                new E87PropertiesList());
                        });
                }
            }

            if (e.CurrentState is CarrierIdStatus.VerificationOk or CarrierIdStatus.VerificationFailed
                && e.Carrier.ObjID != deviceLoadPort.Carrier.Id)
            {
                deviceLoadPort.SetCarrierId(e.Carrier.ObjID);
            }
        }

        private void LoadPort_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            //Do not raise event when controller is in maintenance state
            if (sender is not LoadPort loadPort
                || App.ControllerInstance.ControllerEquipmentManager.Controller.State
                is OperatingModes.Maintenance or OperatingModes.Engineering)
            {
                return;
            }

            switch (e.NewState)
            {
                case ExecutionState.Running:
                    switch (e.Execution.Context.Command.Name)
                    {
                        case nameof(LoadPort.Initialize):
                            try
                            {
                                _e87Standard.IntegrationServices.NotifyLoadPortTransferBlocked(
                                    loadPort.InstanceId);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            break;
                    }

                    break;

                case ExecutionState.Success:
                    switch (e.Execution.Context.Command.Name)
                    {
                        case nameof(LoadPort.Initialize):
                            loadPort.StatusValueChanged += LoadPort_StatusValueChanged;

                            if (loadPort.CarrierPresence == CassettePresence.Absent
                                && App.ControllerInstance.GemController.E84Observer.AccessViolationLoadPorts.Contains(loadPort.InstanceId))
                            {
                                App.ControllerInstance.GemController.E84Observer
                                    .AccessViolationLoadPorts.Remove(loadPort.InstanceId);
                            }

                            Task.Run(
                                () =>
                                {
                                    if (_controller.State != OperatingModes.Initialization)
                                    {
                                        UpdateLoadPortState(loadPort);
                                    }

                                    E30Standard.AlarmServices.ClearAlarm(
                                        $"E84:TP1:LoadPort:{loadPort.InstanceId}");
                                    E30Standard.AlarmServices.ClearAlarm(
                                        $"E84:TP2:LoadPort:{loadPort.InstanceId}");
                                    E30Standard.AlarmServices.ClearAlarm(
                                        $"E84:TP3:LoadPort:{loadPort.InstanceId}");
                                    E30Standard.AlarmServices.ClearAlarm(
                                        $"E84:TP4:LoadPort:{loadPort.InstanceId}");
                                    E30Standard.AlarmServices.ClearAlarm(
                                        $"E84:TP5:LoadPort:{loadPort.InstanceId}");
                                    E30Standard.AlarmServices.ClearAlarm(
                                        $"E84:HandoffError:LoadPort:{loadPort.InstanceId}");
                                    E30Standard.AlarmServices.ClearAlarm(
                                        E87WellknownNames.Alarms.PIOFailure
                                        + $"LP{loadPort.InstanceId}");

                                    if (!App.ControllerInstance.GemController.E84Observer
                                        .AccessViolationLoadPorts.Contains(loadPort.InstanceId))
                                    {
                                        E30Standard.AlarmServices.ClearAlarm(
                                            E87WellknownNames.Alarms.AccessModeViolation_Load
                                            + loadPort.InstanceId);
                                    }
                                    E30Standard.AlarmServices.ClearAlarm(
                                        E87WellknownNames.Alarms.AccessModeViolation_Unload
                                        + loadPort.InstanceId);
                                });
                            break;

                        case nameof(LoadPort.Clamp):
                            try
                            {
                                _e87Standard.AdditionalServices.NotifyCarrierHasBeenClamped(
                                    loadPort.InstanceId);

                                _e87Standard.IntegrationServices.NotifyCarrierLoadingFinished(
                                    loadPort.InstanceId);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            break;

                        case nameof(LoadPort.Unclamp):
                            try
                            {
                                _e87Standard.AdditionalServices.NotifyCarrierHasBeenUnclamped(
                                    loadPort.InstanceId);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            break;

                        case nameof(LoadPort.Close):
                            try
                            {
                                _e87Standard.AdditionalServices.NotifyCarrierHasBeenClosed(
                                    loadPort.InstanceId);

                                _e87Standard.IntegrationServices.NotifyCarrierHasBeenUnDocked(
                                    loadPort.InstanceId);

                                E30Standard.DataServices.SendEvent(
                                    CEIDs.CustomEvents.CarrierUndocked,
                                    new List<VariableUpdate>()
                                    {
                                        new(
                                            E87WellknownNames.DVs.PortID,
                                            (byte)loadPort.InstanceId)
                                    });
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            break;

                        case nameof(LoadPort.ReleaseCarrier):
                            try
                            {
                                _e87Standard.AdditionalServices.NotifyCarrierHasBeenClosed(
                                    loadPort.InstanceId);

                                _e87Standard.IntegrationServices.NotifyCarrierHasBeenUnDocked(
                                    loadPort.InstanceId);

                                E30Standard.DataServices.SendEvent(
                                    CEIDs.CustomEvents.CarrierUndocked,
                                    new List<VariableUpdate>()
                                    {
                                        new(
                                            E87WellknownNames.DVs.PortID,
                                            (byte)loadPort.InstanceId)
                                    });

                                _e87Standard.AdditionalServices.NotifyCarrierHasBeenUnclamped(
                                    loadPort.InstanceId);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            break;

                        case nameof(LoadPort.Open):

                            var slotStates =
                                ConvertMappingTableToE87SlotStates(loadPort.Carrier.MappingTable);
                            var e87Carrier = _e87Standard.GetCarrierById(loadPort.Carrier.Id);
                            if (e87Carrier == null)
                            {
                                return;
                            }

                            try
                            {
                                _e87Standard.IntegrationServices.NotifyCarrierHasBeenDocked(
                                    loadPort.InstanceId);

                                E30Standard.DataServices.SendEvent(
                                    CEIDs.CustomEvents.CarrierDocked,
                                    new List<VariableUpdate>()
                                    {
                                        new(
                                            E87WellknownNames.DVs.PortID,
                                            (byte)loadPort.InstanceId)
                                    });

                                _e87Standard.AdditionalServices.NotifyCarrierHasBeenOpened(
                                    loadPort.InstanceId);

                                if (e87Carrier.SlotMapStatus == SlotMapStatus.NotRead)
                                {
                                    _e87Standard.IntegrationServices
                                        .NotifyCarrierSlotMapReadingFinished(
                                            loadPort.InstanceId,
                                            loadPort.Carrier.Id,
                                            new CarrierSlotMapReadResult(
                                                slotStates,
                                                Status.Performed()));
                                }
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            if (!IsLocalMode()
                                || e87Carrier.SlotMapStatus != SlotMapStatus.WaitingForHost)
                            {
                                return;
                            }

                            if (slotStates.Any(
                                    s => s is Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                            .DoubleSlotted
                                        or Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                            .CrossSlotted))
                            {
                                ShowDoubleSlotDetectedPopup(loadPort);

                                if (e87Carrier.SlotMapStatus is SlotMapStatus.VerificationFailed)
                                {
                                    return;
                                }
                            }

                            var contentMap = new List<ContentMapItem>();
                            if (e87Carrier.ContentMap.Count > 0)
                            {
                                //Carrier recreate use case
                                contentMap = e87Carrier.ContentMap.ToList();
                            }
                            else
                            {
                                if (slotStates.Any(
                                        s => s is Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                                .NotEmpty
                                            or Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                                .CorrectlyOccupied))
                                {
                                    ShowLotIdPopup(loadPort, contentMap, slotStates);
                                }
                                else
                                {
                                    contentMap.AddRange(
                                        slotStates.Select(_ => new ContentMapItem("", "")));
                                }
                            }

                            _e87Standard.StandardServices.ProceedWithCarrier(
                                loadPort.InstanceId,
                                loadPort.Carrier.Id,
                                new E87PropertiesList(slotStates, contentMap));

                            break;

                        case nameof(LoadPort.ReadCarrierId):

                            if (_e87Standard.GetCarrierById(loadPort.Carrier.Id) is { } carrier
                                && carrier.PortID == loadPort.InstanceId
                                && carrier.CarrierIdStatus != CarrierIdStatus.IDNotRead)
                            {
                                return;
                            }

                            try
                            {
                                _e87Standard.IntegrationServices.NotifyCarrierIdReadingFinished(
                                    loadPort.InstanceId,
                                    CarrierIDReadResult.Performed(loadPort.Carrier.Id));
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            if (IsLocalMode()
                                && !_doNotProceedByLoadPort[loadPort.InstanceId - 1]
                                && loadPort.AccessMode != LoadingType.Auto
                                && (!loadPort.IsAutoHandOffEnabled
                                    || loadPort.IsAutoHandOffInProgress))
                            {
                                //In local mode, automatically proceed with carrier if carrier id read success
                                _e87Standard.StandardServices.ProceedWithCarrier(
                                    loadPort.InstanceId,
                                    loadPort.Carrier.Id,
                                    new E87PropertiesList());
                            }

                            _doNotProceedByLoadPort[loadPort.InstanceId - 1] = false;

                            break;
                    }

                    break;

                case ExecutionState.Failed:
                    switch (e.Execution.Context.Command.Name)
                    {
                        case nameof(LoadPort.Open):
                            var slotStates =
                                new List<Agileo.Semi.Gem300.Abstractions.E87.SlotState>();
                            if (loadPort.Carrier.MappingTable is not null)
                            {
                                foreach (var slotState in loadPort.Carrier.MappingTable)
                                {
                                    switch (slotState)
                                    {
                                        case SlotState.NoWafer:
                                            slotStates.Add(
                                                Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                                    .Empty);
                                            break;
                                        case SlotState.HasWafer:
                                            slotStates.Add(
                                                Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                                    .CorrectlyOccupied);
                                            break;
                                        case SlotState.DoubleWafer:
                                            slotStates.Add(
                                                Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                                    .DoubleSlotted);
                                            break;
                                        case SlotState.CrossWafer:
                                            slotStates.Add(
                                                Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                                    .CrossSlotted);
                                            break;
                                        default:
                                            slotStates.Add(
                                                Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                                    .Undefined);
                                            break;
                                    }
                                }
                            }

                            try
                            {
                                _e87Standard.IntegrationServices
                                    .NotifyCarrierSlotMapReadingFinished(
                                        loadPort.InstanceId,
                                        loadPort.Carrier.Id,
                                        new CarrierSlotMapReadResult(
                                            slotStates,
                                            Status.PerformedWithErrors()));

                                _e87Standard.IntegrationServices.NotifyCarrierOpenFailureOccurred(
                                    loadPort.InstanceId);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            break;

                        case nameof(LoadPort.ReadCarrierId):
                            var e87Carrier = _e87Standard.GetCarrierById(loadPort.Carrier.Id);
                            if (e87Carrier != null
                                && e87Carrier.PortID == loadPort.InstanceId
                                && e87Carrier.CarrierIdStatus != CarrierIdStatus.IDNotRead)
                            {
                                return;
                            }

                            try
                            {
                                _e87Standard.IntegrationServices.NotifyCarrierIdReadingFinished(
                                    loadPort.InstanceId,
                                    new CarrierIDReadResult(
                                        loadPort.Carrier.Id,
                                        Status.PerformedWithErrors()));

                                if (e87Carrier != null)
                                {
                                    // UC: Read fail on CarrierRecreate
                                    _e87Standard.StandardServices.ProceedWithCarrier(
                                        loadPort.InstanceId,
                                        e87Carrier.ObjID,
                                        new E87PropertiesList());
                                    return;
                                }
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            if (IsLocalMode())
                            {
                                ShowCarrierIdReadFailPopup(loadPort);
                            }
                            _doNotProceedByLoadPort[loadPort.InstanceId - 1] = false;
                            break;

                        case nameof(LoadPort.Dock):
                            try
                            {
                                _e87Standard.IntegrationServices.NotifyCarrierDockFailureOccurred(
                                    loadPort.InstanceId);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }

                            break;
                    }

                    break;
            }
        }

        private void LoadPort_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            var e87LoadPort = _e87Standard.LoadPorts.First(lp => lp.PortID == loadPort.InstanceId);

            switch (e.Status.Name)
            {
                case nameof(LoadPort.CarrierPresence):
                    if (loadPort.CarrierPresence == CassettePresence.Correctly)
                    {
                        e87LoadPort.IsCarrierPresent = true;
                    }
                    else if (loadPort.CarrierPresence == CassettePresence.Absent)
                    {
                        e87LoadPort.IsCarrierPresent = false;
                    }

                    //Handle transfer state only in manual mode
                    if (loadPort.AccessMode == LoadingType.Manual)
                    {
                        if (loadPort.CarrierPresence == CassettePresence.NoPresentPlacement
                            || loadPort.CarrierPresence == CassettePresence.PresentNoPlacement)
                        {
                            if (loadPort is LayingPlanLoadPort layingPlanLoadPort)
                            {
                                HandlingLayingPlanLpResynchronisation(layingPlanLoadPort, e87LoadPort);
                            }
                            
                            if (e87LoadPort.TransferState == TransferState.ReadyToUnload)
                            {
                                try
                                {
                                    _e87Standard.IntegrationServices.NotifyCarrierUnloadingStarted(
                                        loadPort.InstanceId);
                                }
                                catch (Exception exception)
                                {
                                    Logger.Error(exception, "Error occurred in E87 services.");
                                }
                            }
                            else if (e87LoadPort.TransferState == TransferState.ReadyToLoad)
                            {
                                try
                                {
                                    _e87Standard.IntegrationServices.NotifyCarrierLoadingStarted(
                                        e87LoadPort.PortID);
                                }
                                catch (Exception exception)
                                {
                                    Logger.Error(exception, "Error occurred in E87 services.");
                                }
                            }
                        }
                        else if (loadPort.CarrierPresence == CassettePresence.Absent)
                        {
                            try
                            {
                                //If carrier presence switches from CorrectlyPlaced to Absent
                                //We need to switch to transfer blocked before
                                if (e87LoadPort.TransferState == TransferState.ReadyToUnload)
                                {
                                    _e87Standard.IntegrationServices.NotifyCarrierUnloadingStarted(
                                        loadPort.InstanceId);
                                }

                                _e87Standard.IntegrationServices.NotifyCarrierUnloadingFinished(
                                    loadPort.InstanceId);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }
                        }
                        else if (loadPort.CarrierPresence == CassettePresence.Correctly)
                        {
                            try
                            {
                                //If carrier presence switches from Absent to CorrectlyPlaced
                                //We need to switch to transfer blocked before
                                if (e87LoadPort.TransferState == TransferState.ReadyToLoad)
                                {
                                    _e87Standard.IntegrationServices.NotifyCarrierLoadingStarted(
                                        loadPort.InstanceId);
                                }
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }
                        }
                    }

                    break;

                case nameof(LoadPort.IsHandOffButtonPressed):
                    if (loadPort.IsHandOffButtonPressed
                        && loadPort.HandOffLightState == LightState.On
                        && IsLocalMode())
                    {
                        loadPort.ClampAsync();
                    }
                    break;
            }
        }

        private void Controller_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(IController.State))
            {
                switch (_controller.State)
                {
                    case OperatingModes.Engineering:
                    case OperatingModes.Maintenance:
                    case OperatingModes.Initialization:
                        foreach (var loadPort in Equipment.AllOfType<LoadPort>())
                        {
                            loadPort.StatusValueChanged -= LoadPort_StatusValueChanged;
                        }

                        foreach (var loadPort in _e87Standard.LoadPorts)
                        {
                            try
                            {
                                _e87Standard.IntegrationServices.NotifyLoadPortTransferBlocked(
                                    loadPort.PortID);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception, "Error occurred in E87 services.");
                            }
                        }

                        break;
                }
            }
        }

        private void Controller_JobStatusChanged(
            object sender,
            Equipment.Devices.Controller.EventArgs.JobStatusChangedEventArgs e)
        {
            if (e.Job != null)
            {
                if (!_previousJobStatusMap.ContainsKey(e.Job.Name))
                {
                    _previousJobStatusMap.Add(e.Job.Name, JobStatus.Created);
                }

                var sourcePorts = e.Job.Wafers.Select(s => s.SourcePort).Distinct().ToList();

                switch (e.Job.Status)
                {
                    case JobStatus.Executing:
                        if (_previousJobStatusMap[e.Job.Name] != JobStatus.Paused)
                        {
                            foreach (var sourcePort in sourcePorts)
                            {
                                try
                                {
                                    _e87Standard.IntegrationServices.NotifyCarrierAccessingStart(
                                        _e87Standard.GetCarrierByPortId(sourcePort).ObjID);
                                }
                                catch (Exception exception)
                                {
                                    Logger.Error(exception, "Error occurred in E87 services.");
                                }
                            }
                        }

                        break;
                }

                if (e.Job.Status is JobStatus.Completed or JobStatus.Failed or JobStatus.Stopped)
                {
                    _previousJobStatusMap.Remove(e.Job.Name);
                    return;
                }

                _previousJobStatusMap[e.Job.Name] = e.Job.Status;
            }
        }

        private void E87Standard_AccessModeStateChanged(object sender, AccessModeStateChangedArgs e)
        {
            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                .First(lp => lp.Value.InstanceId == e.LoadPort.PortID)
                .Value;

            Task.Run(
                () => { ChangeLoadPortAccessMode(deviceLoadPort, e.LoadPort, e.CurrentState); });
        }

        private void LoadPort_ConfigManager_CurrentChanged(
            object sender,
            ConfigurationChangedEventArgs e)
        {
            var loadPort = Equipment.AllDevices<LoadPort>()
                .FirstOrDefault(lp => lp.ConfigManager.Equals(sender));

            if (loadPort == null)
            {
                return;
            }

            if (loadPort.Configuration.IsCarrierIdSupported)
            {
                _e87Standard.AdditionalServices.NotifyIDReaderIsAvailable(loadPort.InstanceId);
            }
            else
            {
                _e87Standard.AdditionalServices.NotifyIDReaderIsUnavailable(loadPort.InstanceId);
            }

            _e87Standard.SetBypassReadID(loadPort.InstanceId, loadPort.Configuration.CarrierIdentificationConfig.ByPassReadId);
        }

        private void LoadPort_CarrierIdRequestedFromOperator(object sender, EventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            if (IsLocalMode())
            {
                ShowCarrierIdPopup(loadPort);
            }
            else
            {
                _e87Standard.AdditionalServices.NotifyUnknownCarrierID(loadPort.InstanceId);
            }
        }

        private void E87Standard_SlotMapStateChanged(object sender, SlotMapStateChangedArgs e)
        {
            if (e.CurrentState != SlotMapStatus.VerificationOk)
            {
                return;
            }

            if (!App.UtoInstance.ControllerConfig.StartHotLot)
            {
                return;
            }

            SearchNextJobToStart();
        }

        private void DataServices_EventSent(object sender, EventSentEventArgs e)
        {
            if (e.Ceid != _ceUnknownCarrierId
                || !e.Variables.ContainsKey(_dvPortId))
            {
                return;
            }

            var portId = e.Variables[_dvPortId].ValueTo<int>();

            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                .First(lp => lp.Value.InstanceId == portId)
                .Value;

            if (deviceLoadPort == null)
            {
                return;
            }

            if (!deviceLoadPort.Configuration.IsCarrierIdSupported && IsLocalMode())
            {
                ShowCarrierIdReadFailPopup(deviceLoadPort);
                _doNotProceedByLoadPort[deviceLoadPort.InstanceId - 1] = false;
            }
        }

        #endregion

        #region Private Methods

        private void ChangeLoadPortAccessMode(
            LoadPort loadPort,
            Agileo.Semi.Gem300.Abstractions.E87.LoadPort e87LoadPort,
            AccessMode accessMode)
        {
            if (!loadPort.Configuration.IsE84Enabled || !loadPort.IsInService)
            {
                //Do nothing because init has already disabled E84
                //And the SetAccessMode command will be refused
                return;
            }

            if (loadPort.E84TransferInProgress)
            {
                loadPort.DisableE84();
            }

            switch (accessMode)
            {
                case AccessMode.Manual:
                    loadPort.SetAccessMode(LoadingType.Manual);
                    break;
                case AccessMode.Auto:
                    if (loadPort.AccessMode != LoadingType.Auto)
                    {
                        loadPort.SetAccessMode(LoadingType.Auto);
                        loadPort.EnableE84();
                        if (e87LoadPort.TransferState == TransferState.ReadyToLoad)
                        {
                            loadPort.RequestLoad();
                        }
                        else if (e87LoadPort.TransferState == TransferState.ReadyToUnload)
                        {
                            loadPort.RequestUnload();
                        }
                    }

                    break;
            }
        }

        private void ShowCarrierIdPopup(LoadPort loadPort)
        {
            var carrierIdPopup = new IdPopupViewModel();
            var popup = new ValidablePopup(new PopupCommand(
                nameof(GemGeneralRessources.GEM_VALIDATE),
                new DelegateCommand(
                    () => loadPort.SetCarrierId(carrierIdPopup.Id),
                    () => !carrierIdPopup.HasErrors)),
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_CANCEL),
                    new DelegateCommand(
                        () =>
                        {
                            if (string.IsNullOrWhiteSpace(loadPort.Carrier.Id))
                            {
                                loadPort.SetCarrierId("UNKNOWN");
                            }

                            App.ControllerInstance.GemController.E87Std.StandardServices
                                .CancelCarrier(loadPort.Carrier.Id, loadPort.InstanceId);
                        })),
                new InvariantText($"{nameof(loadPort)} {loadPort.InstanceId}"),
                new LocalizableText(nameof(EquipmentResources.CARRIERS_WRITE_CARRIER_ID)))
            {
                Content = carrierIdPopup
            };

            PopupHelper.ShowPopupWithSalience(nameof(L10N.ROOT_MAIN_EQUIPMENT), popup);
        }

        private void ShowDoubleSlotDetectedPopup(LoadPort loadPort)
        {
            //error use case => show popup with Cancel or Proceed choices
            var popup = new Popup(
                nameof(EquipmentResources.LOADPORT_DOUBLE_CROSS_DETECTED),
                nameof(EquipmentResources.LOADPORT_PROCEED_QUESTION))
            {
                Priority = PopupPriority.Critic
            };

            popup.Commands.Add(
                new PopupCommand(
                    nameof(EquipmentResources.CARRIERS_CANCEL),
                    new DelegateCommand(
                        () =>
                        {
                            _e87Standard.StandardServices.CancelCarrier(
                                loadPort.Carrier.Id,
                                loadPort.InstanceId);
                        })));
            popup.Commands.Add(new PopupCommand(nameof(EquipmentResources.CARRIERS_PROCEED)));

            PopupHelper.ShowPopupWithSalience(nameof(L10N.ROOT_MAIN_EQUIPMENT), popup);
        }

        private void ShowCarrierIdReadFailPopup(LoadPort loadPort)
        {
            var carrierIdPopup = new IdPopupViewModel();
            var popup = new ValidablePopup(
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_VALIDATE),
                    new DelegateCommand(
                        () =>
                        {
                            loadPort.SetCarrierId(carrierIdPopup.Id);
                            if (!_doNotProceedByLoadPort[loadPort.InstanceId - 1])
                            {
                                _e87Standard.StandardServices.ProceedWithCarrier(
                                    loadPort.InstanceId,
                                    loadPort.Carrier.Id,
                                    new E87PropertiesList());
                            }
                        }, () => !carrierIdPopup.HasErrors)),
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_CANCEL),
                    new DelegateCommand(
                        () =>
                        {
                            _e87Standard.StandardServices.CancelCarrier(
                                "UNKNOWN",
                                loadPort.InstanceId);
                        })),
                new InvariantText($"{nameof(loadPort)} {loadPort.InstanceId}"),
                new LocalizableText(nameof(EquipmentResources.CARRIERS_WRITE_CARRIER_ID)))
            {
                Content = carrierIdPopup
            };

            PopupHelper.ShowPopupWithSalience(nameof(L10N.ROOT_MAIN_EQUIPMENT), popup);
        }

        private void ShowLotIdPopup(
            LoadPort loadPort,
            List<ContentMapItem> contentMap,
            List<Agileo.Semi.Gem300.Abstractions.E87.SlotState> slotStates)
        {
            var lotIdPopup = new IdPopupViewModel();
            var popup = new ValidablePopup(
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_VALIDATE),
                    new DelegateCommand(
                        () =>
                        {
                            var waferIndex = 1;
                            foreach (var slotState in slotStates)
                            {
                                switch (slotState)
                                {
                                    case Agileo.Semi.Gem300.Abstractions.E87.SlotState.Undefined:
                                    case Agileo.Semi.Gem300.Abstractions.E87.SlotState.Empty:
                                        contentMap.Add(new ContentMapItem("", ""));
                                        break;
                                    case Agileo.Semi.Gem300.Abstractions.E87.SlotState.NotEmpty:
                                    case Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                        .CorrectlyOccupied:
                                        contentMap.Add(
                                            new ContentMapItem(
                                                lotIdPopup.Id,
                                                $"{loadPort.Carrier.Id}.{waferIndex:00}"));
                                        break;
                                    case Agileo.Semi.Gem300.Abstractions.E87.SlotState
                                        .DoubleSlotted:
                                    case Agileo.Semi.Gem300.Abstractions.E87.SlotState.CrossSlotted:
                                        contentMap.Add(
                                            new ContentMapItem(
                                                "",
                                                "")); //Keep empty in this error case
                                        break;
                                }

                                waferIndex++;
                            }
                        },
                        () => !lotIdPopup.HasErrors)),
                null,
                new InvariantText($"{nameof(loadPort)} {loadPort.InstanceId}"),
                new LocalizableText(nameof(EquipmentResources.CARRIERS_WRITE_LOT_ID)))
            {
                Priority = PopupPriority.Critic,
                Content = lotIdPopup
            };

            PopupHelper.ShowPopupWithSalience(nameof(L10N.ROOT_MAIN_EQUIPMENT), popup);
        }

        private List<Agileo.Semi.Gem300.Abstractions.E87.SlotState>
            ConvertMappingTableToE87SlotStates(IEnumerable<SlotState> mappingTable)
        {
            var slotStates = new List<Agileo.Semi.Gem300.Abstractions.E87.SlotState>();
            foreach (var slotState in mappingTable)
            {
                switch (slotState)
                {
                    case SlotState.NoWafer:
                        slotStates.Add(Agileo.Semi.Gem300.Abstractions.E87.SlotState.Empty);
                        break;
                    case SlotState.HasWafer:
                        slotStates.Add(
                            Agileo.Semi.Gem300.Abstractions.E87.SlotState.CorrectlyOccupied);
                        break;
                    case SlotState.DoubleWafer:
                        slotStates.Add(Agileo.Semi.Gem300.Abstractions.E87.SlotState.DoubleSlotted);
                        break;
                    case SlotState.CrossWafer:
                        slotStates.Add(Agileo.Semi.Gem300.Abstractions.E87.SlotState.CrossSlotted);
                        break;
                    default:
                        slotStates.Add(Agileo.Semi.Gem300.Abstractions.E87.SlotState.Undefined);
                        break;
                }
            }

            return slotStates;
        }

        private void UpdateLoadPortState(LoadPort loadPort)
        {
            if (!_previousTransferStates.TryGetValue(
                    loadPort.InstanceId,
                    out var previousTransferState))
            {
                previousTransferState = TransferState.TransferBlocked;
            }

            var e87LoadPort = _e87Standard.GetLoadPort(loadPort.InstanceId);

            //Update access mode
            if (loadPort.Configuration.IsE84Enabled)
            {
                ChangeLoadPortAccessMode(loadPort, e87LoadPort, e87LoadPort.AccessMode);
            }
            else
            {
                if (e87LoadPort.AccessMode == AccessMode.Auto)
                {
                    _e87Standard.StandardServices.ChangeAccess(
                        AccessMode.Manual,
                        loadPort.InstanceId);
                }
            }

            //Update carrier presence
            if (loadPort.IsInService)
            {
                _e87Standard.StandardServices.ChangeServiceStatus(
                    loadPort.InstanceId,
                    ServiceStatus.InService);

                if (previousTransferState == TransferState.ReadyToUnload
                    && loadPort.CarrierPresence == CassettePresence.Correctly)
                {
                    e87LoadPort.IsCarrierPresent = true;
                    _e87Standard.IntegrationServices.NotifyLoadPortTransferReady(
                        loadPort.InstanceId);
                    return;
                }

                e87LoadPort.IsCarrierPresent = false;
                _e87Standard.IntegrationServices.NotifyLoadPortTransferReady(loadPort.InstanceId);

                if (loadPort.CarrierPresence == CassettePresence.Correctly &&
                    !App.ControllerInstance.GemController.E84Observer.AccessViolationLoadPorts.Contains(loadPort.InstanceId))
                {
                    e87LoadPort.IsCarrierPresent = true;
                    _doNotProceedByLoadPort[loadPort.InstanceId - 1] = true;
                    _e87Standard.IntegrationServices.NotifyCarrierLoadingStarted(
                        loadPort.InstanceId);
                    loadPort.Clamp();
                }
            }
            else
            {
                _e87Standard.StandardServices.ChangeServiceStatus(
                    loadPort.InstanceId,
                    ServiceStatus.OutOfService);
            }
        }

        private void SearchNextJobToStart()
        {
            if (_e94Standard.ControlJobs.IsEmpty())
            {
                return;
            }

            var selectedCjs = _e94Standard.ControlJobs.Where(cj => cj.State == State.SELECTED).ToList();
            if (selectedCjs.Count != 1)
            {
                return;
            }

            var selectedCj = selectedCjs[0];

            if (selectedCj.CarrierInputSpecifications.All(
                    element => _e87Standard.GetCarrierById(element) is
                    {
                        CarrierIdStatus: CarrierIdStatus.VerificationOk,
                        SlotMapStatus: SlotMapStatus.VerificationOk
                    }))
            {
                return;
            }

            // find other job to start
            var nextCj = _e94Standard.QueuedControlJobs.FirstOrDefault(
                cjId =>
                {
                    var controlJob = _e94Standard.GetControlJob(cjId);
                    return controlJob.CarrierInputSpecifications.All(
                        element => _e87Standard.GetCarrierById(element) is
                        {
                            CarrierIdStatus: CarrierIdStatus.VerificationOk,
                            SlotMapStatus: SlotMapStatus.VerificationOk
                        });
                });

            _e94Standard.StandardServices.Hoq(nextCj);
            _e94Standard.StandardServices.Deselect(selectedCj.ObjID);
        }

        private void HandlingLayingPlanLpResynchronisation(LayingPlanLoadPort loadPort, Agileo.Semi.Gem300.Abstractions.E87.LoadPort e87LoadPort)
        {
            //UC: LayingPlanLoadPort
            //Presence changes require resynchronisation E87
            if (e87LoadPort.TransferState == TransferState.TransferBlocked)
            {
                if (e87LoadPort.AssociatedCarrier == null)
                {
                    _e87Standard.StandardServices.CancelCarrierAtPort(loadPort.InstanceId);
                }
                else
                {
                    switch (e87LoadPort.AssociatedCarrier.CarrierAccessingStatus)
                    {
                        case AccessingStatus.NotAccessed:
                            _e87Standard.StandardServices.CancelCarrierAtPort(loadPort.InstanceId);
                            break;
                        case AccessingStatus.InAccess:
                            _e87Standard.IntegrationServices.NotifyCarrierAccessingHasBeenFinished(loadPort.Carrier.Id, true);
                            break;
                        case AccessingStatus.CarrierComplete:
                        case AccessingStatus.CarrierStopped:
                            _e87Standard.StandardServices.CarrierRelease(
                            loadPort.InstanceId,
                                loadPort.Carrier.Id);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                var startTime = DateTime.Now;
                while (e87LoadPort.TransferState != TransferState.ReadyToUnload || (DateTime.Now - startTime).TotalSeconds < 3)
                {
                    Thread.Sleep(100);
                }
            }
        }
        #endregion

        #region public

        public void RefreshLoadPortState()
        {
            foreach (var loadPort in App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                         .Values)
            {
                UpdateLoadPortState(loadPort);
            }
        }

        #endregion
    }
}
