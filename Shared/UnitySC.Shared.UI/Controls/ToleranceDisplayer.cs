using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Color = System.Windows.Media.Color;

namespace UnitySC.Shared.UI.Controls
{
    public enum Tolerance
    {
        Good,
        Warning,
        Bad,
        NotMeasured,
        None
    }

    public class ToleranceDisplayer : Control
    {
        public static SolidColorBrush GoodColorBrush { get; }
        public static SolidColorBrush WarningColorBrush { get; }
        public static SolidColorBrush BadColorBrush { get; }
        public static SolidColorBrush NotMeasuredColorBrush { get; }

        static ToleranceDisplayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToleranceDisplayer), new FrameworkPropertyMetadata(typeof(ToleranceDisplayer)));
            GoodColorBrush = new SolidColorBrush(Color.FromRgb(8, 180, 8));
            WarningColorBrush = new SolidColorBrush(Color.FromRgb(249, 129, 2));
            BadColorBrush = new SolidColorBrush(Color.FromRgb(216, 18, 18));
            NotMeasuredColorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 255));
        }

        public static readonly DependencyProperty ToleranceProperty = DependencyProperty.Register(
            nameof(Tolerance), typeof(Tolerance), typeof(ToleranceDisplayer), new PropertyMetadata(default(Tolerance)));

        public Tolerance Tolerance
        {
            get { return (Tolerance)GetValue(ToleranceProperty); }
            set { SetValue(ToleranceProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(string), typeof(ToleranceDisplayer), new PropertyMetadata(default(string)));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
