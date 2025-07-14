using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public enum DisplayerColor
    {
        None,
        Green,
        Orange,
        Red,
        Blue
    }

    public class Displayer : ButtonBase
    {
        static Displayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Displayer), new FrameworkPropertyMetadata(typeof(Displayer)));
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(DisplayerColor), typeof(Displayer), new PropertyMetadata(default(DisplayerColor)));

        public DisplayerColor Color
        {
            get { return (DisplayerColor)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(string), typeof(Displayer), new PropertyMetadata(default(string)));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon), typeof(Geometry), typeof(Displayer), new PropertyMetadata(default(Geometry)));

        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty CommandIconProperty = DependencyProperty.Register(nameof(CommandIcon), typeof(Geometry), typeof(Displayer), new PropertyMetadata(default(Geometry)));

        public Geometry CommandIcon
        {
            get { return (Geometry)GetValue(CommandIconProperty); }
            set { SetValue(CommandIconProperty, value); }
        }
    }
}
