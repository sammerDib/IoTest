using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap
{
    public abstract class BaseDieMapVM : BaseHeatMapChartVM
    {
        protected BaseDieMapVM(int heatmapside) : base(heatmapside)
        {
                
        }

        #region Overrides of BaseHeatMapChartVM

        protected override void CustomizeChart(ViewXY view, AxisX xAxis, AxisY yAxis)
        {
            base.CustomizeChart(view, xAxis, yAxis);

            LegendBox.IntensityScales.ScaleSizeDim1 = 150;
        }

        #endregion

        public void UpdateLegendHeight(double height)
        {
            UpdateChart(() =>
            {
                LegendBox.IntensityScales.ScaleSizeDim1 = (int)height - 90;
                LegendBox.Height = (int)(height - 10);
            });
        }
    }
}
