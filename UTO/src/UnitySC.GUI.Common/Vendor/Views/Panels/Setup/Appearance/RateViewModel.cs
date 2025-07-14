using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.UIComponents.Controls;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Appearance
{
    public class RateViewModel : Notifier, ITickItem
    {
        public double Value { get; }

        public string Name { get; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetAndRaiseIfChanged(ref _isSelected, value); }
        }

        public RateViewModel(double value, string name)
        {
            Value = value;
            Name = name;
        }
    }
}
