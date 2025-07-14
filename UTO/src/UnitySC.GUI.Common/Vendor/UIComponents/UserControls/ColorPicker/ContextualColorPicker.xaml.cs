using System.Windows;

using UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Models;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker
{
    public partial class ContextualColorPicker
    {
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(double), typeof(ContextualColorPicker),
                new PropertyMetadata(1.0));

        public static readonly DependencyProperty ShowAlphaProperty =
            DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(ContextualColorPicker),
                new PropertyMetadata(true));

        public static readonly DependencyProperty PickerTypeProperty
            = DependencyProperty.Register(nameof(PickerType), typeof(PickerType), typeof(ContextualColorPicker),
                new PropertyMetadata(PickerType.Hsv));

        public double SmallChange
        {
            get => (double)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public bool ShowAlpha
        {
            get => (bool)GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }
        public PickerType PickerType
        {
            get => (PickerType)GetValue(PickerTypeProperty);
            set => SetValue(PickerTypeProperty, value);
        }

        public ContextualColorPicker()
        {
            InitializeComponent();
        }
    }
}
