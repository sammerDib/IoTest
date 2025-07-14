using System.Windows;
using System.Windows.Controls;

namespace UnitySC.PM.DMT.CommonUI.View.ExposureSettings
{
    /// <summary>
    /// Logique d'interaction pour ManualExposureSettingsFullScreenView.xaml
    /// </summary>
    public partial class ManualExposureSettingsFullScreenView : UserControl
    {
        public ManualExposureSettingsFullScreenView()
        {
            InitializeComponent();
        }

        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
            chart.DataContext = null;
            if (chart.ViewXY != null)
            {
                foreach (var yAxis in chart.ViewXY.YAxes)
                    yAxis.Dispose();
                chart.ViewXY.YAxes.Clear();

                foreach (var xAxis in chart.ViewXY.XAxes)
                    xAxis.Dispose();
                chart.ViewXY.XAxes.Clear();

                foreach (var point in chart.ViewXY.PointLineSeries)
                    point.Dispose();
                chart.ViewXY.PointLineSeries.Clear();

                chart.ViewXY.Dispose();
            }

            chart.Dispose();
        }
    }
}
