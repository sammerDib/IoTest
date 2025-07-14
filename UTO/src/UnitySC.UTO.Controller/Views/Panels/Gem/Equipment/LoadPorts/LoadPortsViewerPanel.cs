using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E87;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts
{
    public class LoadPortsViewerPanel : BusinessPanel
    {
        static LoadPortsViewerPanel()
        {
            DataTemplateGenerator.Create(typeof(LoadPortsViewerPanel), typeof(LoadPortsViewerPanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(EquipmentResources)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a design time instance of the <see cref="T:AgilController.UI.Maintenance.Equipment.LoadPortsViewer.LoadPortsViewerPanel" /> class.
        /// </summary>
        public LoadPortsViewerPanel()
            : this("DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        /// <inheritdoc />
        public LoadPortsViewerPanel(string id, IIcon icon = null)
            : base(id, icon)
        {
            var proceedWithCarrier = new DelegateCommand(ProceedWithCarrierExecute, ProceedCommandCanExecute);
            var cancelCarrier = new DelegateCommand(CancelCarrierExecute, CancelCarrierCommandCanExecute);
            var reserveAtPort = new DelegateCommand(ReserveAtPortExecute, ReserveAtPortCommandCanExecute);
            var cancelReservation = new DelegateCommand(CancelReservationExecute, CancelReservationCommandCanExecute);

            Commands.Add(new BusinessPanelCommand(nameof(GemGeneralRessources.GEM_REFRESH), new DelegateCommand(() => UpdateLoadPortsCollection()), PathIcon.Refresh));

            Commands.Add(
                new BusinessPanelCommand(
                    nameof(EquipmentResources.CARRIERS_PROCEED_WITH_CARRIER),
                    proceedWithCarrier,
                    PathIcon.Play));
            Commands.Add(
                new BusinessPanelCommand(
                    nameof(EquipmentResources.CARRIERS_CANCEL_CARRIER),
                    cancelCarrier,
                    PathIcon.Cancel));
            Commands.Add(
                new BusinessPanelCommand(
                    nameof(EquipmentResources.CARRIERS_RESERVE_AT_PORT),
                    reserveAtPort,
                    PathIcon.LoadPort));
            Commands.Add(
                new BusinessPanelCommand(
                    nameof(EquipmentResources.CARRIERS_CANCEL_RESERVATION),
                    cancelReservation,
                    PathIcon.Cancel));
        }

        #region ReserveAtPort

        private bool ReserveAtPortCommandCanExecute()
            => !App.ControllerInstance.GemController.IsControlledByHost
               && SelectedE87LoadPort != null
               && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();

        private void ReserveAtPortExecute()
        {
            var res = App.ControllerInstance.GemController.E87Std.StandardServices.ReserveAtPort(
                SelectedE87LoadPort.E87LoadPort.PortID);

            DisplayMessageUser(res, nameof(IE87StandardServices.ReserveAtPort));
        }

        #endregion

        #region CancelReservation

        private bool CancelReservationCommandCanExecute()
            => !App.ControllerInstance.GemController.IsControlledByHost
               && SelectedE87LoadPort != null
               && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();

        private void CancelReservationExecute()
        {
            var res = App.ControllerInstance.GemController.E87Std.StandardServices.CancelReservationAtPort(
                SelectedE87LoadPort.E87LoadPort.PortID);

            DisplayMessageUser(res, nameof(IE87StandardServices.CancelReservationAtPort));
        }

        #endregion

        #region CancelCarrier

        private bool CancelCarrierCommandCanExecute()
            => !App.ControllerInstance.GemController.IsControlledByHost
               && SelectedE87LoadPort?.E87LoadPort.AssociatedCarrier != null
               && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();

        private void CancelCarrierExecute()
        {
            var res = App.ControllerInstance.GemController.E87Std.StandardServices.CancelCarrier(
                SelectedE87LoadPort.E87LoadPort.AssociatedCarrier.ObjID,
                SelectedE87LoadPort.LoadPortId);

            DisplayMessageUser(res, nameof(IE87StandardServices.CancelCarrier));
        }
        #endregion

        #region ProceedWithCarrier

        private bool ProceedCommandCanExecute()
            => !App.ControllerInstance.GemController.IsControlledByHost
               && SelectedE87LoadPort?.E87LoadPort.AssociatedCarrier != null
               && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();

        private void ProceedWithCarrierExecute()
        {
            var res = App.ControllerInstance.GemController.E87Std.StandardServices.ProceedWithCarrier(
                SelectedE87LoadPort.E87LoadPort.PortID,
                SelectedE87LoadPort.E87LoadPort.AssociatedCarrier.ObjID,
                new E87PropertiesList());

            DisplayMessageUser(res, nameof(IE87StandardServices.ProceedWithCarrier));
        }

        #endregion

        #region Properties

        public List<LoadPortViewer> LoadPortViewers { get; } = new();

        private LoadPortViewer _selectedE87LoadPort;

        public LoadPortViewer SelectedE87LoadPort
        {
            get => _selectedE87LoadPort;
            set => SetAndRaiseIfChanged(ref _selectedE87LoadPort, value);
        }

        #endregion

        #region Overrides of IdentifiableElement

        /// <inheritdoc />
        public override void OnSetup()
        {
            var loadPorts = GUI.Common.App.Instance.EquipmentManager.Equipment.AllOfType<LoadPort>();
            if (loadPorts == null)
            {
                return;
            }

            foreach (var loadPort in loadPorts)
            {
                var e87lp = App.ControllerInstance.GemController.E87Std.GetLoadPort(loadPort.InstanceId);
                var loadPortViewer = new LoadPortViewer(e87lp.PortID, e87lp, loadPort);
                LoadPortViewers.Add(loadPortViewer);
            }

            App.ControllerInstance.GemController.E87Std.AccessModeStateChanged += LoadPortStateChanged;
            App.ControllerInstance.GemController.E87Std.AssociationStateChanged += LoadPortStateChanged;
            App.ControllerInstance.GemController.E87Std.ReservationStateChanged += LoadPortStateChanged;
            App.ControllerInstance.GemController.E87Std.TransferStateChanged += LoadPortStateChanged;
            App.ControllerInstance.GemController.E87Std.AccessingStateChanged += CarrierStateChanged;
            App.ControllerInstance.GemController.E87Std.CarrierIdStateChanged += CarrierStateChanged;
            App.ControllerInstance.GemController.E87Std.SlotMapStateChanged += CarrierStateChanged;
            App.ControllerInstance.GemController.E87Std.CarrierDisposed += CarrierChanged;
            App.ControllerInstance.GemController.E87Std.CarrierInstantiated += CarrierChanged;
            App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices.EquipmentConstantChanged +=
                EquipmentConstantChanged;
        }

        public override void OnShow()
        {
            base.OnShow();
            foreach (var loadPortViewer in LoadPortViewers)
            {
                loadPortViewer.RefreshAll();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.ControllerInstance.GemController.E87Std.AccessModeStateChanged -= LoadPortStateChanged;
                App.ControllerInstance.GemController.E87Std.AssociationStateChanged -= LoadPortStateChanged;
                App.ControllerInstance.GemController.E87Std.ReservationStateChanged -= LoadPortStateChanged;
                App.ControllerInstance.GemController.E87Std.TransferStateChanged -= LoadPortStateChanged;
                App.ControllerInstance.GemController.E87Std.AccessingStateChanged -= CarrierStateChanged;
                App.ControllerInstance.GemController.E87Std.CarrierIdStateChanged -= CarrierStateChanged;
                App.ControllerInstance.GemController.E87Std.SlotMapStateChanged -= CarrierStateChanged;
                App.ControllerInstance.GemController.E87Std.CarrierDisposed -= CarrierChanged;
                App.ControllerInstance.GemController.E87Std.CarrierInstantiated -= CarrierChanged;
                App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices.EquipmentConstantChanged -=
                    EquipmentConstantChanged;
            }

            base.Dispose(disposing);
        }

        #endregion

        private void UpdateLoadPortsCollection()
        {
            foreach (var loadPortViewer in LoadPortViewers)
            {
                loadPortViewer.RefreshAll();
            }
        }

        private void LoadPortStateChanged(object sender, LoadPortStateChangedArgs e)
        {
            var loadPortViewer =
                LoadPortViewers.SingleOrDefault(viewer => viewer.E87LoadPort?.ObjID == e.LoadPort.ObjID);
            loadPortViewer?.RefreshAll();

        }

        private void CarrierChanged(object sender, CarrierEventArgs e)
        {
            UpdateLoadPortsCollection();
        }

        private void EquipmentConstantChanged(object sender, VariableEventArgs e)
        {
            foreach (var loadPortViewer in LoadPortViewers)
            {
                loadPortViewer.EquipmentConstantChanged();
            }
        }

        private void CarrierStateChanged(object sender, CarrierStateChangedArgs e)
        {
            var loadPortViewer = LoadPortViewers.SingleOrDefault(
                viewer => viewer.E87LoadPort?.AssociatedCarrier?.ObjID == e.Carrier.ObjID);
            loadPortViewer?.RefreshAll();
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
    }
}
