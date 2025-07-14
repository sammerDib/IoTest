using System.Windows;
using System.Windows.Documents;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <summary>
    /// Class allowing the use of <see cref="MarkupExtensions"/> in a run. Use the Value property instead of the Text property.
    /// </summary>
    public class OneWayRun : Run
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(string), typeof(OneWayRun), new PropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OneWayRun)d).Text = e.NewValue as string;
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
