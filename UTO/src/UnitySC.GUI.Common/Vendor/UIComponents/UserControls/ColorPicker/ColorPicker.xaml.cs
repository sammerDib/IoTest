using System.Windows;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker
{
    public partial class ColorPicker
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(double), typeof(ColorPicker),
                new PropertyMetadata(1.0));

        public double SmallChange
        {
            get => (double)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly DependencyProperty ShowAlphaProperty =
            DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(ColorPicker),
                new PropertyMetadata(true));

        public bool ShowAlpha
        {
            get => (bool)GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }

        public static readonly DependencyProperty ShowColorDisplayProperty = DependencyProperty.Register(
            nameof(ShowColorDisplay),
            typeof(bool),
            typeof(ColorPicker),
            new PropertyMetadata(true));

        public bool ShowColorDisplay
        {
            get { return (bool)GetValue(ShowColorDisplayProperty); }
            set { SetValue(ShowColorDisplayProperty, value); }
        }
    }
}
