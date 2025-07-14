using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Agileo.AlarmModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E94;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.Equipment.LoadPort.Popup;
using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.Carriers;

using ErrorCode = Agileo.Semi.Communication.Abstractions.E5.ErrorCode;
using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;
using Status = Agileo.Semi.Gem300.Abstractions.E87.Status;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts
{
    public class LoadPortViewer : LoadPortCardViewModel
    {
        #region Members

        private bool _isAccessModeChanging;
        private bool _isServiceModeChanging;

        #endregion

        #region Properties

        public int LoadPortId { get; }

        public Agileo.Semi.Gem300.Abstractions.E87.LoadPort E87LoadPort { get; }

        public SlotMapViewModel SlotMap { get; } = new();

        public ServiceStatus LoadPortServiceStatus
        {
            get
                => E87LoadPort.TransferState == TransferState.OutOfService
                    ? ServiceStatus.OutOfService
                    : ServiceStatus.InService;
            set
                => App.ControllerInstance.GemController.E87Std.StandardServices.ChangeServiceStatus(
                    E87LoadPort.PortID,
                    value);
        }

        public bool IsServiceModeChanging
        {
            get => _isServiceModeChanging;
            set => SetAndRaiseIfChanged(ref _isServiceModeChanging, value);
        }

        public AccessMode AccessMode
        {
            get => E87LoadPort.AccessMode;
            set
                => App.ControllerInstance.GemController.E87Std.StandardServices.ChangeAccess(
                    value,
                    new List<PortId> {E87LoadPort.PortID});
        }

        public bool IsAccessModeChanging
        {
            get => _isAccessModeChanging;
            set => SetAndRaiseIfChanged(ref _isAccessModeChanging, value);
        }

        public bool ByPassReadId
        {
            get
            {
                if (E87LoadPort == null)
                {
                    return true;
                }

                return App.ControllerInstance.GemController.E87Std.GetBypassReadID(
                    E87LoadPort.PortID);
            }
        }

        public TransferState TransferState => E87LoadPort.TransferState;

        public AccessingStatus CarrierAccessingStatus
        {
            get
            {
                if (E87LoadPort.AssociatedCarrier is null)
                {
                    return AccessingStatus.NotAccessed;
                }
                return E87LoadPort.AssociatedCarrier.CarrierAccessingStatus;
            }
        }

        public ReservationState ReservationState => E87LoadPort.ReservationState;

        public string LotId
        {
            get
            {
                if (E87LoadPort.AssociatedCarrier is null || E87LoadPort.AssociatedCarrier.ContentMap.Count is 0)
                {
                    return null;
                }

                return E87LoadPort.AssociatedCarrier.ContentMap
                    .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.LotID))
                    ?.LotID;
            }
        }

        public bool IsControllerOk
            => App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();

        public UserMessageDisplayer UserMessageDisplayer { get; } = new();

        #endregion

        #region Constructors

        public LoadPortViewer()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }

            LoadPortId = 1;
        }

        public LoadPortViewer(
            int loadPortId,
            Agileo.Semi.Gem300.Abstractions.E87.LoadPort e87LoadPort,
            LoadPort loadPort)
            : base(loadPort)
        {
            LoadPortId = loadPortId;
            E87LoadPort = e87LoadPort;

            App.ControllerInstance.GemController.E87Std.AccessingStateChanged +=
                E87Std_AccessingStateChanged;
            App.ControllerInstance.GemController.E87Std.AssociationStateChanged +=
                E87Std_AssociationStateChanged;
            App.ControllerInstance.GemController.E87Std.ReservationStateChanged +=
                E87Std_ReservationStateChanged;
            App.ControllerInstance.GemController.E87Std.TransferStateChanged +=
                E87Std_TransferStateChanged;
            App.ControllerInstance.GemController.E87Std.AccessModeStateChanged +=
                E87Std_AccessModeStateChanged;
            App.ControllerInstance.GemController.E87Std.CarrierIdStateChanged +=
                E87Std_CarrierIdStateChanged;
            App.ControllerInstance.GemController.E87Std.SlotMapStateChanged +=
                E87Std_SlotMapStateChanged;
            App.ControllerInstance.GemController.E87Std.CarrierDisposed += E87Std_CarrierDisposed;

            App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices
                .EquipmentConstantChanged += E30Std_EquipmentConstantChanged;

            App.ControllerInstance.ControllerEquipmentManager.Controller.PropertyChanged += Controller_PropertyChanged;

            LoadPort.PropertyChanged += LoadPort_PropertyChanged;
        }

        #endregion

        #region Overrides of LoadPortCardViewModel

        protected override void UpdateSlotMap(IEnumerable<SlotState> slotStates = null)
        {
            base.UpdateSlotMap(slotStates);
            if (E87LoadPort == null)
                return;

            SlotMap.UpdateSlotMap(E87LoadPort.AssociatedCarrier);
        }

        #endregion

        #region Methods

        private void DisplayMessageUser(Status executedService, string serviceCalledString = "")
        {
            var message = executedService.IsFailure
                ? new UserMessage(MessageLevel.Error, $"{serviceCalledString} failed. Status : {executedService.ToString().ToLower()}")
                : new UserMessage(MessageLevel.Success, $"{serviceCalledString} {executedService.ToString().ToLower()}");
            if (message.Level == MessageLevel.Success)
            {
                message.SecondsDuration = 5;
            }
            else
            {
                message.CanUserCloseMessage = true;
            }

            if (AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel is
                LoadPortDetailPanel panel)
            {
                panel.Messages.Show(message);
            }
        }

        #endregion

        #region Public Methods

        public void RefreshAll()
        {
            OnPropertyChanged(null);
        }

        public void EquipmentConstantChanged()
        {
            // [TLa] Here we allow ourselves to raise the propertyChanged at each change of EC because this event is rarely raised and will not impact performance.
            OnPropertyChanged(nameof(ByPassReadId));
        }

        #endregion

        #region Event Handlers

        private void Controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IGenericDevice.State)))
            {
                OnPropertyChanged(nameof(IsControllerOk));
            }
        }

        private void E87Std_AccessingStateChanged(object sender, AccessingStateChangedArgs e)
        {
            if (LoadPort.Carrier != null
                && LoadPort.Carrier.Id == e.Carrier.ObjID)
            {
                OnPropertyChanged(nameof(CarrierAccessingStatus));
            }
        }

        private void E87Std_SlotMapStateChanged(object sender, SlotMapStateChangedArgs e)
        {
            if (LoadPort.InstanceId == e.Carrier.PortID)
            {
                UpdateSlotMap(LoadPort.Carrier?.GetMappingTable());
                OnPropertyChanged(nameof(LotId));
            }
        }

        private void E87Std_CarrierIdStateChanged(object sender, CarrierIdStateChangedArgs e)
        {
            if (LoadPort.Carrier != null
                && LoadPort.Carrier.Id == e.Carrier.ObjID)
            {
                RefreshAll();
            }
        }

        private void E87Std_AccessModeStateChanged(object sender, AccessModeStateChangedArgs e)
        {
            if (LoadPort.InstanceId == e.LoadPort.PortID)
            {
                OnPropertyChanged(nameof(AccessMode));
            }
        }

        private void E87Std_TransferStateChanged(object sender, TransferStateChangedArgs e)
        {
            if (LoadPort.InstanceId == e.LoadPort.PortID)
            {
                OnPropertyChanged(nameof(TransferState));
            }
        }

        private void E87Std_ReservationStateChanged(object sender, ReservationStateChangedArgs e)
        {
            if (LoadPort.InstanceId == e.LoadPort.PortID)
            {
                OnPropertyChanged(nameof(ReservationState));
            }
        }

        private void E87Std_AssociationStateChanged(object sender, AssociationModeStateChangedArgs e)
        {
            if (LoadPort.InstanceId == e.LoadPort.PortID)
            {
                RefreshAll();
            }
        }

        private void E87Std_CarrierDisposed(object sender, CarrierEventArgs e)
        {
            if (E87LoadPort.Locations.Any(l => l.LocationId.Equals(e.Carrier.LocationId)))
            {
                UpdateSlotMap();
            }
        }

        private void E30Std_EquipmentConstantChanged(object sender, VariableEventArgs e)
        {
            if (e.Variable.Name.Contains(E87WellknownNames.ECs.BypassReadID))
            {
                EquipmentConstantChanged();
            }
        }

        private void LoadPort_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(nameof(IGenericDevice.State)))
            {
                return;
            }

            if (LoadPort.State == OperatingModes.Maintenance)
            {
                var alarm = LoadPort.Alarms.FirstOrDefault(al => al.State == AlarmState.Set && !al.Acknowledged);
                var message = alarm != null
                    ? alarm.Description
                    : nameof(EquipmentResources.LOADPORT_MAINTENANCE);

                UserMessageDisplayer.HideAll();
                UserMessageDisplayer.Show(new UserMessage(MessageLevel.Error, new LocalizableText(message))
                {
                    Icon = PathIcon.Caution
                });
            }
            else
            {
                UserMessageDisplayer.HideAll();
            }
        }

        #endregion

        #region Commands

        #region Proceed Command

        private SafeDelegateCommandAsync _proceedCommand;

        public SafeDelegateCommandAsync ProceedCommand
            => _proceedCommand ??= new SafeDelegateCommandAsync(
                ProceedCommandExecute,
                ProceedCommandCanExecute);

        private Task ProceedCommandExecute()
        {
            var task = new Task(
                () =>
                {
                    //If carrier is already stopped/Completed
                    var e87Carrier = App.UtoInstance.GemController.E87Std.GetCarrierById(LoadPort.Carrier?.Id);

                    if (e87Carrier != null)
                    {
                        var usedControlJobs =
                            App.UtoInstance.GemController.E94Std.ControlJobs.Where(
                                x => x.CarrierInputSpecifications.Any(c => c == e87Carrier.ObjID)).ToList();

                        if (usedControlJobs.Any(cj => cj.State is State.SELECTED or State.QUEUED)
                            && App.UtoInstance.GemController.E94Std.ControlJobs.All(cj => cj.State != State.EXECUTING))
                        {
                            var popup = new Popup(PopupButtons.YesNo, e87Carrier.ObjID, nameof(EquipmentResources.CARRIER_USED_BY_JOBQUEUE));

                            var resultPopup = GUI.Common.App.Instance.UserInterface.Navigation.SelectedBusinessPanel?.Popups.ShowAndWaitResult(popup);

                            if (resultPopup != PopupResult.Yes)
                            {
                                return;
                            }
                        }

                        if (e87Carrier.SlotMapStatus is SlotMapStatus.VerificationOk or SlotMapStatus.VerificationFailed &&
                            e87Carrier.CarrierAccessingStatus is not AccessingStatus.InAccess)
                        {
                            //If any process job is completed and we are recreating the material, we need to force the signal that the material has departed
                            //In the case there are two carriers used and we need to recreate both
                            foreach (var usedControlJob in usedControlJobs.Where(cj=>cj.State == State.COMPLETED))
                            {
                                foreach (var processJob in usedControlJob.ProcessingControlSpecifications)
                                {
                                    try
                                    {
                                        App.ControllerInstance.GemController.E94Std.IntegrationServices.NotifyMaterialHasDeparted(processJob.PRJobID);
                                    }
                                    catch (Exception exception)
                                    {
                                        App.ControllerInstance.GemController.E94Std.Logger.Error(exception, "Error occurred in E94 services.");
                                    }
                                }
                            }

                            var res = App.ControllerInstance.GemController.E87Std.StandardServices.CarrierReCreate(
                                e87Carrier.ObjID,
                            new E87PropertiesList(e87Carrier.SlotMap, e87Carrier.ContentMap));

                            DisplayMessageUser(res, nameof(IE87StandardServices.CarrierReCreate));

                            return;
                        }

                        //Use case Unknwown Carrier recreate
                        if (e87Carrier.CarrierIdStatus is CarrierIdStatus.VerificationFailed)
                        {
                            App.ControllerInstance.GemController.E87Std.IntegrationServices.NotifyCarrierUnloadingStarted(LoadPortId);
                            E87LoadPort.IsCarrierPresent = false;
                            App.ControllerInstance.GemController.E87Std.IntegrationServices.NotifyCarrierUnloadingFinished(LoadPortId);

                            App.ControllerInstance.GemController.E87Std.IntegrationServices.NotifyCarrierLoadingStarted(LoadPortId);
                            E87LoadPort.IsCarrierPresent = true;

                            if (LoadPort.IsClamped)
                            {
                                App.ControllerInstance.GemController.E87Std.IntegrationServices.NotifyCarrierLoadingFinished(LoadPortId);
                                return;
                            }
                        }
                    }

                    //If carrier is not clamped,
                    //proceed command will clamp the carrier on the load port
                    if (!LoadPort.IsClamped)
                    {
                        LoadPort.ClampAsync();
                        return;
                    }

                    //Because the carrier is already clamped,
                    //it means that the proceed will validate the carrierId
                    var result = App.ControllerInstance.GemController.E87Std.StandardServices.ProceedWithCarrier(
                        LoadPort.InstanceId,
                        LoadPort.Carrier?.Id,
                        E87PropertiesList.Empty);

                    DisplayMessageUser(result, nameof(IE87StandardServices.ProceedWithCarrier));
                });
            task.Start();
            return task;
        }

        private bool ProceedCommandCanExecute()
        {
            if (E87LoadPort == null || App.ControllerInstance.GemController.IsControlledByHost)
            {
                return false;
            }

            if (LoadPort == null)
            {
                return false;
            }

            if (!App.ControllerInstance.ControllerEquipmentManager.IsControllerOk())
            {
                return false;
            }

            //If Open command is available, all other commands are available
            var canDeviceExecute = LoadPort.CanExecute(nameof(LoadPort.Open), out _, true);
            if (!canDeviceExecute)
            {
                return false;
            }

            if (E87LoadPort.AssociatedCarrier == null)
            {
                return LoadPort.CarrierPresence == CassettePresence.Correctly;
            }

            return (E87LoadPort.AssociatedCarrier.CarrierAccessingStatus.Equals(AccessingStatus.NotAccessed)
                    && E87LoadPort.AssociatedCarrier.SlotMapStatus != SlotMapStatus.VerificationOk
                    && E87LoadPort.AssociatedCarrier.SlotMapStatus != SlotMapStatus.VerificationFailed)
                   //Carrier Recreate 
                   || E87LoadPort.TransferState == TransferState.ReadyToUnload;
        }

        #endregion

        #region Release Command

        private SafeDelegateCommandAsync _releaseCommand;

        public SafeDelegateCommandAsync ReleaseCommand
            => _releaseCommand ??= new SafeDelegateCommandAsync(
                ReleaseCommandExecute,
                ReleaseCommandCanExecute);

        private Task ReleaseCommandExecute()
        {
            var task = new Task(
                () =>
                {
                    // CarrierID is verified by Operator and result is failed
                    // CancelCarrier must be called
                    if (E87LoadPort.AssociatedCarrier == null ||
                        E87LoadPort.AssociatedCarrier.CarrierAccessingStatus == AccessingStatus.NotAccessed)
                    {
                        var res = App.ControllerInstance.GemController.E87Std.StandardServices.CancelCarrierAtPort(
                            LoadPort.InstanceId);

                        DisplayMessageUser(res, nameof(IE87StandardServices.CancelCarrier));
                    }
                    else if (E87LoadPort.AssociatedCarrier.CarrierAccessingStatus is AccessingStatus.CarrierComplete or AccessingStatus.CarrierStopped)
                    {
                        try
                        {
                            App.ControllerInstance.GemController.E87Std.StandardServices.CarrierRelease(
                                LoadPortId,
                                LoadPort.Carrier.Id);
                        }
                        catch (Exception e)
                        {

                            DisplayMessageUser(
                                Status.InvalidState()
                                    .WithError(
                                        ErrorCode.ActionFailedDueToErrors,
                                        string.Join(Environment.NewLine, e.Message)),
                                nameof(IE87Standard.StandardServices.CarrierRelease));
                        }

                    }
                    else
                    {
                        try
                        {
                            App.ControllerInstance.GemController.E87Std.IntegrationServices
                                .NotifyCarrierAccessingHasBeenFinished(LoadPort.Carrier.Id, false);
                        }
                        catch (Exception e)
                        {

                            DisplayMessageUser(
                                Status.InvalidState()
                                    .WithError(
                                        ErrorCode.ActionFailedDueToErrors,
                                        string.Join(Environment.NewLine, e.Message)),
                                nameof(IE87IntegrationServices
                                    .NotifyCarrierAccessingHasBeenFinished));
                        }
                    }
                });
            task.Start();
            return task;
        }

        private bool ReleaseCommandCanExecute()
        {
            if (!App.ControllerInstance.ControllerEquipmentManager.IsControllerOk())
            {
                return false;
            }

            if (LoadPort == null)
            {
                return false;
            }

            var context = LoadPort.NewCommandContext(nameof(LoadPort.ReleaseCarrier));
            var canDeviceExecute = LoadPort.CanExecute(context);
            if (!canDeviceExecute)
            {
                return false;
            }

            return !App.ControllerInstance.GemController.IsControlledByHost
                   && E87LoadPort != null
                   && LoadPort.CarrierPresence == CassettePresence.Correctly
                   && LoadPort.IsClamped
                   && !Remote.Helpers.Helpers.IsCarrierUsedByCurrentPj(LoadPort.Carrier?.Id)
                   && !Remote.Helpers.Helpers.IsCarrierUsedByQueuedPj(LoadPort.Carrier?.Id);
        }

        #endregion

        #region AccessModeCommand

        private DelegateCommand _accessModeCommand;

        public DelegateCommand AccessModeCommand
            => _accessModeCommand ??= new DelegateCommand(
                AccessModeCommandExecute,
                AccessModeCommandCanExecute);

        private void AccessModeCommandExecute()
        {
            _ = Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        IsAccessModeChanging = true;
                        var res = App.ControllerInstance.GemController.E87Std.StandardServices
                            .ChangeAccess(
                                E87LoadPort.AccessMode == AccessMode.Auto
                                    ? AccessMode.Manual
                                    : AccessMode.Auto,
                                new List<PortId>() { E87LoadPort.PortID });

                        DisplayMessageUser(res, nameof(IE87StandardServices.ChangeAccess));
                    }
                    catch (Exception e)
                    {
                        DisplayMessageUser(
                            Status.InvalidState()
                                .WithError(
                                    ErrorCode.ActionFailedDueToErrors,
                                    string.Join(Environment.NewLine, e.Message)),
                            nameof(IE87StandardServices.ChangeAccess));
                    }

                    IsAccessModeChanging = false;
                });
        }

        private bool AccessModeCommandCanExecute()
        {
            return !IsAccessModeChanging;
        }

        #endregion AccessModeCommand

        #region ServiceModeCommand

        private ICommand _serviceModeCommand;

        public ICommand ServiceModeCommand
            => _serviceModeCommand ??= new DelegateCommand(
                ServiceModeCommandExecute,
                ServiceModeCommandCanExecute);

        private void ServiceModeCommandExecute()
        {
            _ = Task.Factory.StartNew(
                () =>
                {
                    IsServiceModeChanging = true;
                    try
                    {
                        var res =
                            App.ControllerInstance.GemController.E87Std.StandardServices
                                .ChangeServiceStatus(
                                    E87LoadPort.PortID,
                                    E87LoadPort.TransferState == TransferState.OutOfService
                                        ? ServiceStatus.InService
                                        : ServiceStatus.OutOfService);

                        DisplayMessageUser(res, nameof(IE87StandardServices.ChangeServiceStatus));
                    }
                    catch (Exception e)
                    {
                        DisplayMessageUser(
                            Status.InvalidState()
                                .WithError(
                                    ErrorCode.ActionFailedDueToErrors,
                                    string.Join(Environment.NewLine, e.Message)),
                            nameof(IE87StandardServices.ChangeServiceStatus));
                    }

                    IsServiceModeChanging = false;
                });
        }

        private bool ServiceModeCommandCanExecute()
        {
            return !IsServiceModeChanging;
        }

        #endregion ServiceModeCommand

        #region AccessModePopupCommand

        private SafeDelegateCommand _accessModePopupCommand;

        public SafeDelegateCommand AccessModePopupCommand
            => _accessModePopupCommand ??= new SafeDelegateCommand(
                AccessModePopupCommandExecute,
                AccessModePopupCommandCanExecute);

        private void AccessModePopupCommandExecute()
        {
            var popupContent = new SetAccessModePopup() { AccessMode = E87LoadPort.AccessMode };
            var popup = new Popup(new LocalizableText(nameof(GUI.Common.Resources.EquipmentResources.POPUP_SET_ACCESS_MODE)))
            {
                Content = popupContent
            };

            popup.Commands.Add(new PopupCommand(Agileo.GUI.Properties.Resources.S_OK,
                new DelegateCommand(() =>
                {
                    App.ControllerInstance.GemController.E87Std.StandardServices
                        .ChangeAccess(
                            popupContent.AccessMode,
                            new List<PortId>() { E87LoadPort.PortID });
                }, () => NoE84TransfertInProgress())));
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));

            AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel?.Popups.Show(popup);
        }

        private bool AccessModePopupCommandCanExecute()
        {
            if (E87LoadPort == null)
            {
                return false;
            }

            if (LoadPort == null)
            {
                return false;
            }

            if (!App.ControllerInstance.ControllerEquipmentManager.IsControllerOk())
            {
                return false;
            }

            return LoadPort.CanExecute(nameof(ILoadPort.SetAccessMode), false, out _) && NoE84TransfertInProgress();
        }

        private bool NoE84TransfertInProgress()
        {
            return LoadPort.AccessMode == LoadingType.Manual
                   || (LoadPort.AccessMode == LoadingType.Auto && !LoadPort.I_CS_0 && !LoadPort.I_CS_1);
        }
        #endregion

        #region GoToCarrierView

        protected override void ImplGoToCarrierViewExecute()
        {
            GUI.Common.App.Instance.UserInterface.BusinessPanels.OfType<CarriersViewerPanel>()
                .FirstOrDefault()
                ?.Navigate(LoadPort.Carrier.Id);
        }

        protected override bool ImplGoToCarrierViewCanExecute()
        {
            var gemPanel = GUI.Common.App.Instance.UserInterface.BusinessPanels
                .OfType<CarriersViewerPanel>()
                .FirstOrDefault();
            return !string.IsNullOrWhiteSpace(LoadPort?.Carrier?.Id) && (gemPanel?.IsEnabled ?? false);
        }

        #endregion

        #endregion Commands

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.ControllerInstance.GemController.E87Std.AccessingStateChanged -=
                    E87Std_AccessingStateChanged;
                App.ControllerInstance.GemController.E87Std.AssociationStateChanged -=
                    E87Std_AssociationStateChanged;
                App.ControllerInstance.GemController.E87Std.ReservationStateChanged -=
                    E87Std_ReservationStateChanged;
                App.ControllerInstance.GemController.E87Std.TransferStateChanged -=
                    E87Std_TransferStateChanged;
                App.ControllerInstance.GemController.E87Std.AccessModeStateChanged -=
                    E87Std_AccessModeStateChanged;
                App.ControllerInstance.GemController.E87Std.CarrierIdStateChanged -=
                    E87Std_CarrierIdStateChanged;
                App.ControllerInstance.GemController.E87Std.SlotMapStateChanged -=
                    E87Std_SlotMapStateChanged;
                App.ControllerInstance.GemController.E87Std.CarrierDisposed -=
                    E87Std_CarrierDisposed;
                App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices
                    .EquipmentConstantChanged -= E30Std_EquipmentConstantChanged;

                App.ControllerInstance.ControllerEquipmentManager.Controller.PropertyChanged -= Controller_PropertyChanged;

                LoadPort.PropertyChanged -= LoadPort_PropertyChanged;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
