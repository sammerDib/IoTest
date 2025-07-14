using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Semi.Gem300.Abstractions.E87;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;
using TextBox = System.Windows.Controls.TextBox;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.Carriers
{
    public class CarriersViewerPanel : BusinessPanel
    {
        static CarriersViewerPanel()
        {
            DataTemplateGenerator.Create(typeof(CarriersViewerPanel), typeof(CarriersViewerPanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(EquipmentResources)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a design time instance of the <see cref="T:CarriersViewerPanel" /> class.
        /// </summary>
        public CarriersViewerPanel() : this("DesignTime Constructor")
        {
            if (!IsInDesignMode) { throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters."); }
        }

        /// <inheritdoc />
        public CarriersViewerPanel(string id, IIcon icon = null) : base(id, icon)
        {
            var proceedCommand = new DelegateCommand(ProceedCommandExecute, CarrierCommandCanExecute);
            var cancelCommand = new DelegateCommand(CancelCommandExecute, CarrierCommandCanExecute);
            var cancelBind = new DelegateCommand(CancelBindCommandExecute, CancelBindCommandCanExecute);
            var releaseCommand = new DelegateCommand(ReleaseCommandExecute, CarrierCommandCanExecute);
            var readTagCommand = new DelegateCommand(ReadTagCommandExecute, CarrierCommandCanExecute);
            var writeTagCommand = new DelegateCommand(WriteTagCommandExecute, CarrierCommandCanExecute);
            var recreateTagCommand = new DelegateCommand(RecreateCarrierCommandExecute, CarrierCommandCanExecute);
            var createCommand = new DelegateCommand(CreateCarrierNotificationExecute, CreateCarrierNotificationCanExecute);
            var deleteCommand = new DelegateCommand(CancelCarrierNotificationExecute, CarrierCommandCanExecute);
            var editCommand = new DelegateCommand(EditCarrierNotificationExecute, CarrierCommandCanExecute);
            var duplicateCommand = new DelegateCommand(DuplicateNotificationExecute, CarrierCommandCanExecute);

            CreateCarrier = new InvisibleBusinessPanelCommand(nameof(EquipmentResources.CARRIERS_ADD), createCommand);
            DeleteCarrier = new InvisibleBusinessPanelCommand(nameof(EquipmentResources.CARRIERS_DROP), deleteCommand);
            EditCarrier = new InvisibleBusinessPanelCommand(nameof(EquipmentResources.CARRIERS_EDIT), editCommand);
            DuplicateCarrier = new InvisibleBusinessPanelCommand(nameof(EquipmentResources.CARRIERS_DUPLICATE), duplicateCommand);

            Commands.Add(new BusinessPanelCommand(nameof(EquipmentResources.CARRIERS_PROCEED), proceedCommand, PathIcon.Play));
            Commands.Add(new BusinessPanelCommand(nameof(EquipmentResources.CARRIERS_CANCEL), cancelCommand, PathIcon.Cancel));
            Commands.Add(new BusinessPanelCommand(nameof(EquipmentResources.CARRIERS_CANCEL_BIND), cancelBind, PathIcon.DeleteLink));
            Commands.Add(new BusinessPanelCommand(nameof(EquipmentResources.CARRIERS_RELEASE), releaseCommand, PathIcon.RoundedDown));
            Commands.Add(new BusinessPanelCommand(nameof(EquipmentResources.CARRIERS_READ_TAG), readTagCommand, PathIcon.ReadTag));
            Commands.Add(new BusinessPanelCommand(nameof(EquipmentResources.CARRIERS_WRITE_TAG), writeTagCommand, PathIcon.EditTag));
            Commands.Add(new BusinessPanelCommand(nameof(EquipmentResources.CARRIERS_RECREATE), recreateTagCommand, PathIcon.Refresh));
            Commands.Add(CreateCarrier);
            Commands.Add(DeleteCarrier);
            Commands.Add(EditCarrier);
            Commands.Add(DuplicateCarrier);
        }

        #region Field

        private Agileo.Semi.Gem300.Abstractions.E87.Carrier _selectedCarrier;

        #endregion

        #region Properties

        public InvisibleBusinessPanelCommand CreateCarrier { get; }

        public InvisibleBusinessPanelCommand DeleteCarrier { get; }

        public InvisibleBusinessPanelCommand EditCarrier { get; }

        public InvisibleBusinessPanelCommand DuplicateCarrier { get; }

        public DataTableSource<Carrier> Carriers { get; } =
            new();

        public Agileo.Semi.Gem300.Abstractions.E87.Carrier SelectedCarrier
        {
            get => _selectedCarrier;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedCarrier, value))
                {
                    CarrierViewer = value != null ? new CarrierViewer(value) : null;
                }
            }
        }

        private CarrierViewer _carrierViewer;

        public CarrierViewer CarrierViewer
        {
            get => _carrierViewer;
            private set => SetAndRaiseIfChanged(ref _carrierViewer, value);
        }

        private bool _detailsOpen = true;

        public bool DetailsOpen
        {
            get => _detailsOpen;
            set => SetAndRaiseIfChanged(ref _detailsOpen, value);
        }

        #endregion

        #region Commands

        private bool CarrierCommandCanExecute() => SelectedCarrier != null && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();

        private bool CancelBindCommandCanExecute() => SelectedCarrier?.LocationId != null && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();

        private bool CreateCarrierNotificationCanExecute() => App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();

        private void CancelBindCommandExecute()
        {
            void CancelBindExecute()
            {
                var objId = SelectedCarrier.ObjID;

                var returnValueCancelBind = App.ControllerInstance.GemController.E87Std.StandardServices.CancelBind(null, objId);
                DisplayMessageUser(returnValueCancelBind, nameof(IE87StandardServices.CancelBind));
            }

            var cancelCarrier = new Popup(nameof(EquipmentResources.CARRIERS_CANCEL_BIND_NOTIFICATION_TITLE))
            {
                SeverityLevel = MessageLevel.Error,
                Content = EquipmentResources.CARRIERS_CONFIRMATION_DELETE_BIND + $" '{SelectedCarrier.ObjID}'.",
                Commands =
                {
                    new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                    new PopupCommand(nameof(EquipmentResources.CARRIERS_CANCEL_BIND), new DelegateCommand(CancelBindExecute)),
                }
            };
            Popups.Show(cancelCarrier);
        }

        private void ProceedCommandExecute()
        {
            var propertiesList = BuildPropertiesList(SelectedCarrier);
            var returnProceedProceedWithCarrierValue = App.ControllerInstance.GemController.E87Std.StandardServices.ProceedWithCarrier(
                SelectedCarrier.PortID, SelectedCarrier.ObjID, propertiesList);
            DisplayMessageUser(returnProceedProceedWithCarrierValue, nameof(IE87StandardServices.ProceedWithCarrier));
        }

        private void CancelCommandExecute()
        {
            var returnCancelCarrierValue = App.ControllerInstance.GemController.E87Std.StandardServices.CancelCarrier(SelectedCarrier.ObjID, null);
            DisplayMessageUser(returnCancelCarrierValue, nameof(IE87StandardServices.CancelCarrier));
        }

        private void ReleaseCommandExecute()
        {
            var returnCarrierReleaseValue = App.ControllerInstance.GemController.E87Std.StandardServices.CarrierRelease(null, SelectedCarrier.ObjID);
            DisplayMessageUser(returnCarrierReleaseValue, nameof(IE87StandardServices.CarrierRelease));
        }

        private void ReadTagCommandExecute()
        {
            var returnCarrierTagReadDataValue =  App.ControllerInstance.GemController.E87Std.StandardServices.CarrierTagReadData(
                SelectedCarrier.LocationId,
                SelectedCarrier.ObjID, string.Empty,
                null, out _);
            DisplayMessageUser(returnCarrierTagReadDataValue, nameof(IE87StandardServices.CarrierTagReadData));
        }

        private void WriteTagCommandExecute()
        {
            var tagTextBox = new TextBox();
            var writeTag = new Popup(nameof(EquipmentResources.CARRIERS_WRITE_TAG_NOTIFICATION_TITLE))
            {
                Content = tagTextBox,
                Commands =
                {
                    new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                    new PopupCommand(nameof(EquipmentResources.CARRIERS_WRITE_TAG_NOTIFICATION_WRITE), new DelegateCommand(() =>
                    {
                        var returnCarrierTagWriteValue = App.ControllerInstance.GemController.E87Std.StandardServices.CarrierTagWriteData(
                            SelectedCarrier.LocationId,
                            SelectedCarrier.ObjID, string.Empty, null, tagTextBox.Text);
                        DisplayMessageUser(returnCarrierTagWriteValue, nameof(IE87StandardServices.CarrierTagWriteData));
                    }))
                }
            };

            Popups.Show(writeTag);
        }

        private void RecreateCarrierCommandExecute()
        {
            var propertiesList = BuildPropertiesList(SelectedCarrier);
            var returnCarrierRecreateValue = App.ControllerInstance.GemController.E87Std.StandardServices.CarrierReCreate(SelectedCarrier.ObjID, propertiesList);
            DisplayMessageUser(returnCarrierRecreateValue, nameof(IE87StandardServices.CarrierReCreate));
        }

        private void CreateCarrierNotificationExecute()
        {
            var createCarrierNotificationPopup = new CarrierEditionPopup();

            void CreateCarrierExecute()
            {
                AddNewCustomCarrier(createCarrierNotificationPopup.CarrierId, createCarrierNotificationPopup.SelectedLoadPort.LoadPort, createCarrierNotificationPopup.GetE87PropertiesList());
            }

            bool CreateCarrierCanExecute()
            {
                return IsCarrierIdValid(createCarrierNotificationPopup.CarrierId);
            }

            ShowCarrierEditionPopup(nameof(EquipmentResources.CARRIERS_CREATE_POPUP_TITLES),
                new PopupCommand(nameof(EquipmentResources.CARRIERS_CREATE_POPUP_CREATE),
                    new DelegateCommand(CreateCarrierExecute, CreateCarrierCanExecute)), createCarrierNotificationPopup);
        }

        private void CancelCarrierNotificationExecute()
        {
            void ValidateExecute()
            {
                if (SelectedCarrier.PortID != null)
                {
                    CancelBind(SelectedCarrier.ObjID, SelectedCarrier.PortID);
                }
                else
                {
                    CancelCarrierNotification(SelectedCarrier.ObjID);
                }
            }

            var cancelCarrier = new Popup(nameof(EquipmentResources.CARRIERS_CANCEL_CARRIER_NOTIFICATION_TITLE))
            {
                SeverityLevel = MessageLevel.Error,
                Content = SelectedCarrier.LocationId != null ? EquipmentResources.CARRIERS_CONFIRMATION_DELETE_BIND_AND_CARRIER + $" '{SelectedCarrier.ObjID}'." : EquipmentResources.CARRIERS_CONFIRMATION_DELETE_CARRIER + $" '{SelectedCarrier.ObjID}'.",
                Commands =
                {
                    new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                    new PopupCommand(SelectedCarrier.PortID != null
                        ? nameof(EquipmentResources.CARRIERS_CANCEL_NOTIFICATION_CANCEL_BIND_AND_CARRIER)
                        : nameof(EquipmentResources.CARRIERS_CANCEL_NOTIFICATION_CANCEL_CARRIER), new DelegateCommand(ValidateExecute)),
                }
            };

            Popups.Show(cancelCarrier);
        }

        private void EditCarrierNotificationExecute()
        {
            var editCarrierPopup = new CarrierEditionPopup(SelectedCarrier)
            {
                EnableEdition = false
            };

            var selectedLocationId = string.IsNullOrWhiteSpace(SelectedCarrier.LocationId) ? null : App.ControllerInstance.GemController.E87Std.GetLoadPortByLocationID(SelectedCarrier.LocationId).LocationID;
            var substrate = new List<string>();
            editCarrierPopup.Slots.ToList().ForEach(x => substrate.Add(x.SubstrateId));

            void EditCarrierExecute()
            {
                AddNewCustomCarrier(editCarrierPopup.CarrierId, editCarrierPopup.SelectedLoadPort.LoadPort, editCarrierPopup.GetE87PropertiesList());
            }

            bool EditCarrierCanExecute()
            {
                for (var i = 0; i < substrate.Count; i++)
                {
                    if (editCarrierPopup.Slots[i].SubstrateId != substrate[i]) return true;
                }

                return (editCarrierPopup.SelectedLoadPort.LoadPort?.Name) != selectedLocationId;
            }

            ShowCarrierEditionPopup(nameof(EquipmentResources.CARRIERS_EDITION_POPUP_TITLES),
                new PopupCommand(nameof(EquipmentResources.CARRIERS_EDITION_POPUP_APPLY),
                    new DelegateCommand(EditCarrierExecute, EditCarrierCanExecute)), editCarrierPopup);
        }

        private void DuplicateNotificationExecute()
        {
            var duplicateCarrierPopup = new CarrierEditionPopup(SelectedCarrier)
            {
                CarrierId = string.Empty
            };

            var selectedLocationId = string.IsNullOrWhiteSpace(SelectedCarrier.LocationId) ? null : App.ControllerInstance.GemController.E87Std.GetLoadPortByLocationID(SelectedCarrier.LocationId).LocationID;

            void DuplicateCarrierExecute()
            {
                AddNewCustomCarrier(duplicateCarrierPopup.CarrierId, duplicateCarrierPopup.SelectedLoadPort.LoadPort, duplicateCarrierPopup.GetE87PropertiesList());
            }

            bool DuplicateCarrierCanExecute()
            {
                return IsCarrierIdValid(duplicateCarrierPopup.CarrierId) && ((duplicateCarrierPopup.SelectedLoadPort.LoadPort?.Name) != selectedLocationId || selectedLocationId == null);
            }

            ShowCarrierEditionPopup(nameof(EquipmentResources.CARRIERS_CREATE_POPUP_TITLES),
                new PopupCommand(nameof(EquipmentResources.CARRIERS_CREATE_POPUP_CREATE),
                    new DelegateCommand(DuplicateCarrierExecute, DuplicateCarrierCanExecute)), duplicateCarrierPopup);
        }

        #endregion

        #region Methods

        private void UpdateCarrierCollection()
        {
            Carriers.Reset(App.ControllerInstance.GemController.E87Std.Carriers);
            if (SelectedCarrier != null)
            {
                CarrierViewer.Update();
            }
        }

        public void Navigate(string carrierId)
        {
            if (TryNavigate())
            {
                var selectedCarrier = Carriers.Find(x => x.ObjID.Equals(carrierId));
                if (selectedCarrier != null)
                {
                    SelectedCarrier = selectedCarrier;
                    DetailsOpen = true;
                }
            }
        }


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

            Messages.Show(message);
        }

        private static E87PropertiesList BuildPropertiesList(Agileo.Semi.Gem300.Abstractions.E87.Carrier carrier)
        {
            var slotStates = carrier.SlotMap.ToList();
            var contentMapItems = carrier.ContentMap.ToList();

            return new E87PropertiesList(slotStates, contentMapItems);
        }

        private void ShowCarrierEditionPopup(LocalizableText popupTitle, PopupCommand validateCommand, CarrierEditionPopup carrierEditionPopup)
        {
            var carrierNotification = new Popup(popupTitle)
            {
                Content = carrierEditionPopup,
                Commands =
                {
                    new PopupCommand(nameof(EquipmentResources.CARRIERS_CANCEL)),
                    validateCommand
                }
            };
            Popups.Show(carrierNotification);
        }

        private void AddNewCustomCarrier(string carrierId, LoadPort loadPort, E87PropertiesList propertiesList)
        {
            var e87Carrier = App.ControllerInstance.GemController.E87Std.GetCarrierById(carrierId);
            if (e87Carrier != null)
            {
                if (e87Carrier.PortID == null)
                {
                    CancelCarrierNotification(carrierId);
                }
                else
                {
                    CancelBind(carrierId, e87Carrier.PortID);
                }
            }

            if (loadPort == null)
            {
                CarrierNotification(carrierId, propertiesList);
            }
            else
            {
                Bind(carrierId, loadPort, propertiesList);
            }
        }

        private void Bind(string carrierId, LoadPort loadPort, E87PropertiesList propertiesList)
        {
            var returnBindValue = App.ControllerInstance.GemController.E87Std.StandardServices.Bind(
                loadPort.InstanceId,
                carrierId,
                propertiesList);
            DisplayMessageUser(returnBindValue, nameof(IE87StandardServices.Bind));
        }

        private void CancelBind(string carrierId, int? portId)
        {
            var returnCancelBindValue = App.ControllerInstance.GemController.E87Std.StandardServices.CancelBind(portId, carrierId);
            DisplayMessageUser(returnCancelBindValue, nameof(IE87StandardServices.CancelBind));
        }

        private void CarrierNotification(string carrierId, E87PropertiesList propertiesList)
        {
            var returnCarrierNotificationValue = App.ControllerInstance.GemController.E87Std.StandardServices.CarrierNotification(
                carrierId,
                propertiesList);
            DisplayMessageUser(returnCarrierNotificationValue, nameof(IE87StandardServices.CarrierNotification));
        }

        private void CancelCarrierNotification(string carrierId)
        {
            var returnCancelCarrierValue = App.ControllerInstance.GemController.E87Std.StandardServices.CancelCarrierNotification(carrierId);
            DisplayMessageUser(returnCancelCarrierValue, nameof(IE87StandardServices.CancelCarrierNotification));
        }

        private bool IsCarrierIdValid(string carrierId)
        {
            return !string.IsNullOrEmpty(carrierId)
                   && Carriers.All(carrier => carrier.ObjID != carrierId);
        }

        #endregion

        #region Handlers

        private void AccessModeStateChanged(object sender, AccessModeStateChangedArgs e)
            => UpdateCarrierCollection();

        private void AccessingStateChanged(object sender, AccessingStateChangedArgs e)
        {
            UpdateCarrierCollection();
            CarrierViewer?.CarrierViewModel.RaisePropertyChanged(nameof(CarrierViewModel.AccessingStatus));
        }

        private void ReservationStateChanged(object sender, ReservationStateChangedArgs e)
            => UpdateCarrierCollection();

        private void SlotMapStateChanged(object sender, SlotMapStateChangedArgs e)
            => UpdateCarrierCollection();

        private void TransferStateChanged(object sender, TransferStateChangedArgs e)
            => UpdateCarrierCollection();

        private void AssociationStateChanged(object sender, AssociationModeStateChangedArgs e)
            =>  UpdateCarrierCollection();

        private void CarrierIdStateChanged(object sender, CarrierIdStateChangedArgs e)
        {
            UpdateCarrierCollection();
            CarrierViewer?.CarrierViewModel.RaisePropertyChanged(nameof(CarrierViewModel.CarrierIdStatus));
        }

        private void CarrierDisposed(object sender, CarrierEventArgs e)
            => UpdateCarrierCollection();

        private void CarrierInstantiated(object sender, CarrierEventArgs e)
            => UpdateCarrierCollection();

        #endregion

        #region Overrides of IdentifiableElement

        /// <inheritdoc />
        public override void OnSetup()
        {
            App.ControllerInstance.GemController.E87Std.CarrierInstantiated += CarrierInstantiated;
            App.ControllerInstance.GemController.E87Std.CarrierDisposed += CarrierDisposed;
            App.ControllerInstance.GemController.E87Std.CarrierIdStateChanged += CarrierIdStateChanged;
            App.ControllerInstance.GemController.E87Std.AssociationStateChanged += AssociationStateChanged;
            App.ControllerInstance.GemController.E87Std.TransferStateChanged += TransferStateChanged;
            App.ControllerInstance.GemController.E87Std.SlotMapStateChanged += SlotMapStateChanged;
            App.ControllerInstance.GemController.E87Std.ReservationStateChanged += ReservationStateChanged;
            App.ControllerInstance.GemController.E87Std.AccessingStateChanged += AccessingStateChanged;
            App.ControllerInstance.GemController.E87Std.AccessModeStateChanged += AccessModeStateChanged;

            Carriers.AddRange(App.ControllerInstance.GemController.E87Std.Carriers);
        }

        protected override void Dispose(bool disposing)
        {
            App.ControllerInstance.GemController.E87Std.CarrierInstantiated -= CarrierInstantiated;
            App.ControllerInstance.GemController.E87Std.CarrierDisposed -= CarrierDisposed;
            App.ControllerInstance.GemController.E87Std.CarrierIdStateChanged -= CarrierIdStateChanged;
            App.ControllerInstance.GemController.E87Std.AssociationStateChanged -= AssociationStateChanged;
            App.ControllerInstance.GemController.E87Std.TransferStateChanged -= TransferStateChanged;
            App.ControllerInstance.GemController.E87Std.SlotMapStateChanged -= SlotMapStateChanged;
            App.ControllerInstance.GemController.E87Std.ReservationStateChanged -= ReservationStateChanged;
            App.ControllerInstance.GemController.E87Std.AccessingStateChanged -= AccessingStateChanged;
            App.ControllerInstance.GemController.E87Std.AccessModeStateChanged -= AccessModeStateChanged;

            base.Dispose(disposing);
        }

        #endregion
    }
}

