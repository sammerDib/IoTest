using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class Chips : Button
    {
        static Chips()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chips), new FrameworkPropertyMetadata(typeof(Chips)));
        }

        public static readonly DependencyProperty IsActivatedProperty = DependencyProperty.Register(
            nameof(IsActivated), typeof(bool), typeof(Chips), new PropertyMetadata(default(bool)));

        [Category("Main")]
        public bool IsActivated
        {
            get { return (bool)GetValue(IsActivatedProperty); }
            set { SetValue(IsActivatedProperty, value); }
        }

        public static readonly DependencyProperty ShowRemoveIconProperty = DependencyProperty.Register(
            nameof(ShowRemoveIcon), typeof(bool), typeof(Chips), new PropertyMetadata(default(bool)));

        [Category("Main")]
        public bool ShowRemoveIcon
        {
            get { return (bool)GetValue(ShowRemoveIconProperty); }
            set { SetValue(ShowRemoveIconProperty, value); }
        }

        public static readonly DependencyProperty IsInProgressProperty = DependencyProperty.Register(
            nameof(IsInProgress), typeof(bool), typeof(Chips), new PropertyMetadata(default(bool)));

        [Category("Main")]
        public bool IsInProgress
        {
            get { return (bool)GetValue(IsInProgressProperty); }
            set { SetValue(IsInProgressProperty, value); }
        }
    }
}
