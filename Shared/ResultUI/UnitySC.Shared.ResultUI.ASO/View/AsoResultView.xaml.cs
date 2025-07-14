using System.Windows;
using System.Windows.Controls;

using LightningChartLib.WPF.ChartingMVVM;
using LightningChartLib.WPF.ChartingMVVM.Axes;

using UnitySC.Shared.ResultUI.ASO.ViewModel;

namespace UnitySC.Shared.ResultUI.ASO.View
{
    /// <summary>
    /// Interaction logic for AsoResultView.xaml
    /// </summary>
    public partial class AsoResultView : UserControl
    {
        private readonly double _maxYpctmargin = 1.1;
        private readonly double _maxXpctmargin = 10.0;

        public AsoResultView()
        {
            InitializeComponent();
            HistoChart.ColorTheme = ColorTheme.LightGray;
            HistoChart.ViewXY.ZoomPanOptions.ViewFitYMarginPixels = 10;
        }

        public AsoResultVM ViewModel { get => (AsoResultVM)DataContext; set => DataContext = value; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel = DataContext as AsoResultVM;
        }

        private void axisY_RangeChanged(object sender, RangeChangedEventArgs e)
        {
            if (HistomaxY.Value == null)
                return;

            double curMinY = 0;
            double curMaxY = _maxYpctmargin * (double)HistomaxY.Value;//MaxYpctmargin * histochart.ViewXY.YAxes[0].Maximum;
            if (e.NewMin < curMinY)
            {
                double newRange = e.NewMax - e.NewMin;
                if (newRange <= (curMaxY - curMinY))
                {
                    HistoChart.ViewXY.YAxes[0].SetRange(curMinY, newRange);
                }
                else
                {
                    HistoChart.ViewXY.YAxes[0].SetRange(curMinY, curMaxY);
                }
            }
            else if (e.NewMax > curMaxY)
            {
                double newRange = e.NewMax - e.NewMin;
                if (newRange <= (curMaxY - curMinY))
                {
                    HistoChart.ViewXY.YAxes[0].SetRange(curMaxY - newRange, curMaxY);
                }
                else
                {
                    HistoChart.ViewXY.YAxes[0].SetRange(curMinY, curMaxY);
                }
            }
        }

        private void axisX_RangeChanged(object sender, RangeChangedEventArgs e)
        {
            if (HistomaxX.Value == null)
                return;

            double curMinX = 0;
            double curMaxX = _maxXpctmargin * (double)HistomaxX.Value;//MaxYpctmargin * histochart.ViewXY.YAxes[0].Maximum;
            if (e.NewMin < curMinX)
            {
                double newRange = e.NewMax - e.NewMin;
                if (newRange <= (curMaxX - curMinX))
                {
                    HistoChart.ViewXY.XAxes[0].SetRange(curMinX, newRange);
                }
                else
                {
                    HistoChart.ViewXY.XAxes[0].SetRange(curMinX, curMaxX);
                }
            }
            else if (e.NewMax > curMaxX)
            {
                double newRange = e.NewMax - e.NewMin;
                if (newRange <= (curMaxX - curMinX))
                {
                    HistoChart.ViewXY.XAxes[0].SetRange(curMaxX - newRange, curMaxX);
                }
                else
                {
                    HistoChart.ViewXY.XAxes[0].SetRange(curMinX, curMaxX);
                }
            }
        }
               
        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
            HistoChart.DataContext = null;
            if (HistoChart.ViewXY != null)
            {
                foreach (var yAxis in HistoChart.ViewXY.YAxes)
                    yAxis.Dispose();
                HistoChart.ViewXY.YAxes.Clear();

                foreach (var xAxis in HistoChart.ViewXY.XAxes)
                    xAxis.Dispose();
                HistoChart.ViewXY.XAxes.Clear();

                if (HistoChart.ViewXY.BarSeries != null)
                {
                    foreach (var bar in HistoChart.ViewXY.BarSeries)
                        bar.Dispose();
                    HistoChart.ViewXY.BarSeries.Clear();
                }

                HistoChart.ViewXY.Dispose();
            }

            HistoChart.Dispose();

            // Stop current thumbnails loading thread
            // ViewModel property used to keep DataContext before GC cleaned it.
            ViewModel.Clean();
        }
    }
}
