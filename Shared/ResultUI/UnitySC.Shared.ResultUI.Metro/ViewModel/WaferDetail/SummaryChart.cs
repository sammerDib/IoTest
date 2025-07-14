using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail
{
    public class SummaryChart : BaseLineChart
    {
        private readonly Dictionary<Enum, Color> _stateToColorDict;

        #region Field

        private readonly AxisX _xAxis;

        private readonly AxisY _yAxis;

        #endregion

        public SummaryChart(string yAxisTitle, Dictionary<Enum, Color> stateToColorDict)
        {
            _stateToColorDict = stateToColorDict;

            #region Axis

            _xAxis = new AxisX
            {
                ScrollMode = XAxisScrollMode.None,
                Visible = false
            };

            _yAxis = new AxisY
            {
                AutoFormatLabels = false,
                ValueType = AxisValueType.Number,
                Visible = true,
                Title =
                {
                    Text = yAxisTitle,
                    Visible = !string.IsNullOrEmpty(yAxisTitle)
                }
            };

            #endregion

            #region Chart

            Chart = new LightningChart
            {
                ViewXY =
                {
                    DropOldSeriesData = false, 
                    XAxes = new AxisXList { _xAxis }, 
                    YAxes = new AxisYList { _yAxis }, 
                    ZoomPanOptions =
                    {
                        ViewFitYMarginPixels = 50
                    }
                },
                Title =
                {
                    Visible = false
                }
            };

            SetupChartTheme();

            //Arrange bars side-by-side and fit to width of the chart
            Chart.ViewXY.BarViewOptions.Grouping = BarsGrouping.ByIndexFitWidth;
            Chart.ViewXY.BarViewOptions.BarSpacing = 50;
            Chart.ViewXY.BarViewOptions.Orientation = BarsOrientation.Vertical;

            //Set the legend box
            Chart.ViewXY.LegendBoxes[0].Visible = false;
            Chart.ViewXY.LegendBoxes[0].Position = LegendBoxPositionXY.TopRight;
            Chart.ViewXY.LegendBoxes[0].Offset.SetValues(0, 30);
            Chart.ViewXY.LegendBoxes[0].Layout = LegendBoxLayout.VerticalColumnSpan;
            Chart.ViewXY.LegendBoxes[0].HighlightSeriesTitleColor = Colors.MediumPurple;

            #endregion
        }

        public void UpdateBars(List<MeasurePointResult> points)
        {
            Chart.BeginUpdate();

            Chart.ViewXY.BarSeries.Clear();

            CreateBar(points.Where(result => result.State == MeasureState.Success), MeasureState.Success);
            CreateBar(points.Where(result => result.State == MeasureState.Partial), MeasureState.Partial);
            CreateBar(points.Where(result => result.State == MeasureState.Error), MeasureState.Error);
            CreateBar(points.Where(result => result.State == MeasureState.NotMeasured), MeasureState.NotMeasured);
            
            Chart.ViewXY.ZoomToFit();
            _yAxis.Minimum = 0.0;
            _yAxis.Maximum = (int)(_yAxis.Maximum + 1);

            Chart.EndUpdate();
        }

        private void CreateBar(IEnumerable<MeasurePointResult> points, Enum result)
        {
            var data = new BarSeriesValue[1];
            
            data[0].Value = points.Count();
            data[0].Text = data[0].Value.ToString(CultureInfo.InvariantCulture);
            
            switch (result)
            {
                case MeasureState.Success:
                    data[0].Location = 0;
                    break;
                case MeasureState.Partial:
                    data[0].Location = 1;
                    break;
                case MeasureState.Error:
                    data[0].Location = 2;
                    break;
                case MeasureState.NotMeasured:
                    data[0].Location = 3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
            
            var barSeries = new BarSeries(Chart.ViewXY, _xAxis, _yAxis)
            {
                Fill = new Fill
                {
                    Color = _stateToColorDict[result],
                    GradientFill = GradientFill.Solid
                },
                LabelStyle = new BarLabelsStyle
                {
                    Angle = 0,
                    VerticalAlign = BarsTitleVerticalAlign.BarTop
                },
                Shadow = new Shadow
                {
                    Visible = false
                },
                Highlight = Highlight.None,
                Title = new SeriesTitle
                {
                    Text = (result as MeasureState?).ToHumanizedString()
                },
                Values = data
            };

            Chart.ViewXY.BarSeries.Add(barSeries);
        }
    }
}
