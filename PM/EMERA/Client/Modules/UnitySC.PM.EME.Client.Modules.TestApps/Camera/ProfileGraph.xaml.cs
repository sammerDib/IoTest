using System.Windows;

using LightningChartLib.WPF.ChartingMVVM;

namespace UnitySC.PM.EME.Client.Modules.TestApps.Camera
{
    public partial class ProfileGraph
    {
        public ProfileGraph()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(nameof(Points),
            typeof(SeriesPoint[]), typeof(ProfileGraph), new PropertyMetadata(default(SeriesPoint[])));

        public SeriesPoint[] Points
        {
            get => (SeriesPoint[])GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum),
            typeof(int), typeof(ProfileGraph), new PropertyMetadata(255));

        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
    }
}
