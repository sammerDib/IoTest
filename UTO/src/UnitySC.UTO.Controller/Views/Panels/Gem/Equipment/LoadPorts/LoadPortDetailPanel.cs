using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.Semi.Gem300.Abstractions.E87;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.Carriers;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts.E84SignalsViewer;

using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts
{
    public class LoadPortDetailPanel : BusinessPanel
    {
        #region Properties

        public LoadPortViewer LoadPortViewer { get; }

        public string LoadPortId { get; }

        public InvisibleBusinessPanelCommand ChangeAccessModeCommand { get; }

        public InvisibleBusinessPanelCommand ChangeServiceStatusCommand { get; }

        public E84Signals E84Signals { get; }

        #endregion
        static LoadPortDetailPanel()
        {
            DataTemplateGenerator.Create(typeof(LoadPortDetailPanel), typeof(LoadPortDetailPanelView));
        }

        public LoadPortDetailPanel()
            : base("DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public LoadPortDetailPanel(LoadPort loadPort, string id, IIcon icon = null)
            : base(id, icon)
        {
            var e87LoadPort = App.ControllerInstance.GemController.E87Std.GetLoadPort(loadPort.InstanceId);

            LoadPortId = e87LoadPort.LocationID;

            LoadPortViewer = new LoadPortViewer(e87LoadPort.PortID, e87LoadPort, loadPort);
            E84Signals = new E84Signals(loadPort);

            var proceedWithCarrier = new DelegateCommand(ProceedWithCarrierExecute, () => App.ControllerInstance.ControllerEquipmentManager.IsControllerOk());
            var cancelCarrier = new DelegateCommand(CancelCarrierExecute, () => App.ControllerInstance.ControllerEquipmentManager.IsControllerOk());
            var reserveAtPort = new DelegateCommand(ReserveAtPortExecute, () => App.ControllerInstance.ControllerEquipmentManager.IsControllerOk());
            var cancelReservation = new DelegateCommand(CancelReservationExecute, () => App.ControllerInstance.ControllerEquipmentManager.IsControllerOk());
            var stopDataCollection = new DelegateCommand(StopDataCollectionExecute, () => false);

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
                    PathIcon.DeleteLink));
            Commands.Add(
                new BusinessPanelCommand(
                    nameof(EquipmentResources.CARRIERS_CANCEL_RESERVATION),
                    cancelReservation,
                    PathIcon.Cancel));
            Commands.Add(
                new BusinessPanelCommand(
                    nameof(EquipmentResources.CARRIERS_STOP_DATA_COLLECTION),
                    stopDataCollection,
                    PathIcon.DeleteLink));

            ChangeAccessModeCommand = new InvisibleBusinessPanelCommand(
                nameof(EquipmentResources.LOADPORT_ACCESS_MODE),
                new DelegateCommand(() => { }, () => false));
            ChangeServiceStatusCommand = new InvisibleBusinessPanelCommand(
                nameof(EquipmentResources.LOADPORT_SERVICE_STATUS),
                new DelegateCommand(() => { }, () => false));
            Commands.Add(ChangeAccessModeCommand);
            Commands.Add(ChangeServiceStatusCommand);

            RefreshAll();
        }

        #region Commands

        private ICommand _goToAssociatedCarrier;

        public ICommand GoToAssociatedCarrier
            => _goToAssociatedCarrier
               ?? (_goToAssociatedCarrier = new DelegateCommand(
                   GoToAssociatedCarrierExecute,
                   GoToAssociatedCarrierCanExecute));

        public void GoToAssociatedCarrierExecute()
        {
            var carrierPanel =
                GUI.Common.App.Instance.UserInterface.BusinessPanels?.OfType<CarriersViewerPanel>().FirstOrDefault();
            if (carrierPanel != null)
            {
                carrierPanel.Navigate(LoadPortViewer.E87LoadPort.AssociatedCarrier.ObjID);
            }
        }

        private bool GoToAssociatedCarrierCanExecute()
            => LoadPortViewer.E87LoadPort?.AssociatedCarrier?.LocationId != null;

        private void StopDataCollectionExecute()
        {
            // Nothing yet
        }

        private void CancelReservationExecute()
            => App.ControllerInstance.GemController.E87Std.StandardServices.CancelReservationAtPort(
                LoadPortViewer.E87LoadPort.PortID);


        private void CancelCarrierExecute()
            => App.ControllerInstance.GemController.E87Std.StandardServices.CancelCarrier(
                LoadPortViewer.E87LoadPort.AssociatedCarrier.ObjID,
                LoadPortViewer.E87LoadPort.PortID);


        private void ReserveAtPortExecute()
            => App.ControllerInstance.GemController.E87Std.StandardServices.ReserveAtPort(LoadPortViewer.E87LoadPort.PortID);


        private void ProceedWithCarrierExecute()
            => App.ControllerInstance.GemController.E87Std.StandardServices.ProceedWithCarrier(
                LoadPortViewer.E87LoadPort.PortID,
                LoadPortViewer.E87LoadPort.AssociatedCarrier.ObjID,
                new E87PropertiesList());


        #endregion

        private void RefreshAll()
        {
            LoadPortViewer.RefreshAll();
            OnPropertyChanged(null);
        }

        public LocalizableText E84SignalChartName
            => new(nameof(EquipmentResources.LOADPORT_E84_SIGNAL_CHART_NAME), "a voir");

        /// <summary>
        /// The top chart view where to data is analyzed.
        /// </summary>
        // Warning disabled since E84 is not yet implemented
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public ProcessDataVisualizationViewModel E84SignalChart { get; private set; }

        // Warning disabled since E84 is not yet implemented
        // ReSharper disable once UnusedMember.Local
        private void E84SignalChartPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ProcessDataVisualizationViewModel.SelectedDcp)))
            {
                OnPropertyChanged(nameof(E84SignalChartName));
            }
        }

    }
}
