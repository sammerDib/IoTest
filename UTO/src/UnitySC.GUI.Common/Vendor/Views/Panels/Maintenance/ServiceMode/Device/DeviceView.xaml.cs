using System.Windows;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device
{
    /// <summary>
    /// Interaction logic for DeviceViewerView.xaml
    /// </summary>
    public partial class DeviceView
    {
        public static readonly GridLength InitialFirstRowHeight = new(7, GridUnitType.Star);
        public static readonly GridLength InitialSecondRowHeight = new(3, GridUnitType.Star);
        public static readonly double SecondRowMinHeight = 200;

        private GridLength _firstRowLength = InitialFirstRowHeight;
        private GridLength _secondRowLength = InitialSecondRowHeight;

        public DeviceView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsLogExpandedProperty = DependencyProperty.Register(
            nameof(IsLogExpanded),
            typeof(bool),
            typeof(DeviceView),
            new PropertyMetadata(true, OnIsLogExpandedChanged));

        public static readonly DependencyProperty AreCommandsEnabledProperty = DependencyProperty.Register(
            nameof(AreCommandsEnabled),
            typeof(bool),
            typeof(DeviceView),
            new PropertyMetadata(true));

        public bool AreCommandsEnabled
        {
            get { return (bool)GetValue(AreCommandsEnabledProperty); }
            set { SetValue(AreCommandsEnabledProperty, value); }
        }

        private static void OnIsLogExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DeviceView self)
            {
                if (self.IsLogExpanded)
                {
                    self.FirstRow.Height = self._firstRowLength;
                    self.SecondRow.Height = self._secondRowLength;
                    self.SecondRow.MinHeight = SecondRowMinHeight;
                }
                else
                {
                    self._firstRowLength = self.FirstRow.Height;
                    self._secondRowLength = self.SecondRow.Height;

                    self.FirstRow.Height = new GridLength(1, GridUnitType.Star);
                    self.SecondRow.Height = GridLength.Auto;
                    self.SecondRow.MinHeight = 0;
                }
            }
        }

        public bool IsLogExpanded
        {
            get { return (bool)GetValue(IsLogExpandedProperty); }
            set { SetValue(IsLogExpandedProperty, value); }
        }

        private void ExpandDown_Click(object sender, RoutedEventArgs e) => IsLogExpanded = false;

        private void ExpandUp_Click(object sender, RoutedEventArgs e) => IsLogExpanded = true;
    }
}
