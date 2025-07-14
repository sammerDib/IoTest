using System.Windows;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker
{
    public enum ColorSlidersType
    {
        Hsl,
        Hsv,
        Rgb
    }

    public partial class ColorSliders
    {
        public ColorSliders()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(double), typeof(ColorSliders),
                new PropertyMetadata(1.0));

        public double SmallChange
        {
            get => (double)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly DependencyProperty ShowAlphaProperty =
            DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(ColorSliders),
                new PropertyMetadata(true));
        
        public bool ShowAlpha
        {
            get => (bool)GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }

        public static readonly DependencyProperty SlidersTypeProperty = DependencyProperty.Register(
            nameof(SlidersType),
            typeof(ColorSlidersType),
            typeof(ColorSliders),
            new PropertyMetadata(default(ColorSlidersType)));

        public ColorSlidersType SlidersType
        {
            get { return (ColorSlidersType)GetValue(SlidersTypeProperty); }
            set { SetValue(SlidersTypeProperty, value); }
        }
    }
}
