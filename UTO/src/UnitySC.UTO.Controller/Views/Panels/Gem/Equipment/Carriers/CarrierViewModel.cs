using Agileo.GUI.Components;
using Agileo.Semi.Gem300.Abstractions.E87;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.Carriers
{
    public class CarrierViewModel : Notifier
    {
        private readonly Carrier _carrier;

        public CarrierViewModel(Carrier carrier)
        {
            _carrier = carrier;
        }

        public CarrierIdStatus CarrierIdStatus => _carrier.CarrierIdStatus;

        public AccessingStatus AccessingStatus => _carrier.CarrierAccessingStatus;

        public string Usage => _carrier.Usage;

        public void RaisePropertyChanged(string propertyName) => OnPropertyChanged(propertyName);
    }
}
