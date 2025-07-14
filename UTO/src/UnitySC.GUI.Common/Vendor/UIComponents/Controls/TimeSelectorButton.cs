using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class TimeSelectorButton : Button
    {
        static TimeSelectorButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSelectorButton), new FrameworkPropertyMetadata(typeof(TimeSelectorButton)));
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(TimeSelectorButton), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsNowProperty = DependencyProperty.Register(
            "IsNow", typeof(bool), typeof(TimeSelectorButton), new PropertyMetadata(default(bool)));

        public bool IsNow
        {
            get { return (bool)GetValue(IsNowProperty); }
            set { SetValue(IsNowProperty, value); }
        }

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            "IsHighlighted", typeof(bool), typeof(TimeSelectorButton), new PropertyMetadata(default(bool)));

        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }
    }
}
