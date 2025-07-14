using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class Led : Control
    {
        static Led()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Led), new FrameworkPropertyMetadata(typeof(Led)));
        }

        public static readonly DependencyProperty IsActivatedProperty = DependencyProperty.Register(
            nameof(IsActivated), typeof(bool), typeof(Led), new PropertyMetadata(default(bool)));

        public bool IsActivated
        {
            get { return (bool)GetValue(IsActivatedProperty); }
            set { SetValue(IsActivatedProperty, value); }
        }
    }
}
