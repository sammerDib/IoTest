using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary
{
    public class E30VariablesPanelConfigurationPopupViewModel : NotifyDataError
    {
        private double _delay;

        public double Delay
        {
            get { return _delay; }
            set { SetAndRaiseIfChanged(ref _delay, value); }
        }

        public E30VariablesPanelConfigurationPopupViewModel(double currentDelay)
        {
            Delay = currentDelay;
        }
    }
}
