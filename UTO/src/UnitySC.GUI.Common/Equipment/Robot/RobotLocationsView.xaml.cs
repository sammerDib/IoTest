using System.ComponentModel;
using System.Windows;

namespace UnitySC.GUI.Common.Equipment.Robot
{
    /// <summary>
    /// Interaction logic for RobotLocationsView.xaml
    /// </summary>
    public partial class RobotLocationsView
    {
        public RobotLocationsView()
        {
            InitializeComponent();
        }

        [Category("Main")]
        public static readonly DependencyProperty UpperArmSimplifiedWaferIdProperty = DependencyProperty.Register(
            nameof(UpperArmSimplifiedWaferId),
            typeof(string),
            typeof(RobotLocationsView),
            new PropertyMetadata(default(string)));

        public string UpperArmSimplifiedWaferId
        {
            get => (string)GetValue(UpperArmSimplifiedWaferIdProperty);
            set => SetValue(UpperArmSimplifiedWaferIdProperty, value);
        }

        [Category("Main")]
        public static readonly DependencyProperty LowerArmSimplifiedWaferIdProperty = DependencyProperty.Register(
            nameof(LowerArmSimplifiedWaferId),
            typeof(string),
            typeof(RobotLocationsView),
            new PropertyMetadata(default(string)));

        public string LowerArmSimplifiedWaferId
        {
            get => (string)GetValue(LowerArmSimplifiedWaferIdProperty);
            set => SetValue(LowerArmSimplifiedWaferIdProperty, value);
        }
    }
}
