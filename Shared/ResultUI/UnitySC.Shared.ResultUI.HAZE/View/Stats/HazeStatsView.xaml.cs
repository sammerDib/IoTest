using System.Windows;
using System.Windows.Data;

using LightningChartLib.WPF.ChartingMVVM;

using UnitySC.Shared.ResultUI.HAZE.ViewModel.Stats;

namespace UnitySC.Shared.ResultUI.HAZE.View.Stats
{
    /// <summary>
    /// Interaction logic for HazeStatsView.xaml
    /// </summary>
    public partial class HazeStatsView
    {
        private bool _firstRendering = true;

        public HazeStatsView()
        {
            InitializeComponent();

            SetBinding(AutoFitFlagProperty, new Binding(nameof(HazeStatsVM.ResetChartFlag)));
            Chart.ColorTheme = ColorTheme.LightGray;
        }

        private void Chart_AfterRendering(object sender, AfterRenderingEventArgs e)
        {
            if (!_firstRendering) return;
            _firstRendering = false;
            ResetZoomFit();
        }
        private void ResetZoomFit()
        {
            Chart.ViewXY.ZoomToFit();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is HazeStatsVM hazeStatsVm)
            {
                hazeStatsVm.Chart = Chart;
            }
            ResetZoomFit();
        }

        public static readonly DependencyProperty AutoFitFlagProperty = DependencyProperty.Register(
            nameof(AutoFitFlag), typeof(bool), typeof(HazeStatsView), new PropertyMetadata(default(bool), AutoFitFlagChangedCallback));

        public bool AutoFitFlag
        {
            get { return (bool)GetValue(AutoFitFlagProperty); }
            set { SetValue(AutoFitFlagProperty, value); }
        }

        private static void AutoFitFlagChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HazeStatsView self)
            {
                self.Chart.ViewXY.ZoomToFit();
            }
        }
    }
}
