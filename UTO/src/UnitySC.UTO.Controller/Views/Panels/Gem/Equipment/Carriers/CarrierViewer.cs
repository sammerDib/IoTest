using System.Linq;
using System.Windows.Input;

using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.Semi.Gem300.Abstractions.E87;

using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.Carriers
{
    public class CarrierViewer : Notifier
    {
        #region Properties

        public Agileo.Semi.Gem300.Abstractions.E87.Carrier Carrier { get; }

        public SlotMapViewModel SlotMap { get; } = new();

        public CarrierViewModel CarrierViewModel { get; }

        private LoadPort _associatedE87LoadPort;

        public LoadPort AssociatedE87LoadPort
        {
            get => _associatedE87LoadPort;
            set => SetAndRaiseIfChanged(ref _associatedE87LoadPort, value);
        }

        private UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort _associatedLoadPort;

        public UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort AssociatedLoadPort
        {
            get => _associatedLoadPort;
            set => SetAndRaiseIfChanged(ref _associatedLoadPort, value);
        }

        #endregion

        public CarrierViewer(Agileo.Semi.Gem300.Abstractions.E87.Carrier carrier)
        {
            Carrier = carrier;
            CarrierViewModel = new CarrierViewModel(carrier);
            Update();
        }

        public void Update()
        {
            SlotMap.UpdateSlotMap(Carrier);

            if (Carrier != null)
            {
                var associatedLoadPort = Carrier.PortID == null
                    ? null
                    : App.ControllerInstance.GemController.E87Std.GetLoadPort((int)Carrier.PortID);
                AssociatedE87LoadPort = associatedLoadPort;

                AssociatedLoadPort = associatedLoadPort == null
                    ? null
                    : App.Instance.EquipmentManager.Equipment
                        .AllOfType<UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort>()
                        .SingleOrDefault(lp => lp.InstanceId == associatedLoadPort.PortID);
            }

            OnPropertyChanged(null);
        }

        #region Commands

        private ICommand _goToBusinessPanelLink;

        public ICommand GoToBusinessPanelLinkCommand => _goToBusinessPanelLink ??= new DelegateCommand(GoToBusinessPanelLinkExecute, GoToBusinessPanelLinkCanExecute);

        private void GoToBusinessPanelLinkExecute()
        {
            var selectedCarrierLocationId = Carrier.LocationId;
            var businessPanel = GUI.Common.App.Instance.UserInterface.BusinessPanels.OfType<LoadPortDetailPanel>()
                .SingleOrDefault(x => x.LoadPortId == selectedCarrierLocationId);

            if (businessPanel != null) GUI.Common.App.Instance.UserInterface.Navigation.TryNavigateTo(businessPanel);
        }

        private bool GoToBusinessPanelLinkCanExecute() => Carrier?.LocationId != null;

        #endregion
    }
}
