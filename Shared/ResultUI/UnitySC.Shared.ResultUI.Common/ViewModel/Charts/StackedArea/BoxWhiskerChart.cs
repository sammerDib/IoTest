using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.EventMarkers;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;

using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts.StackedArea
{
    public class BoxWhiskerChart : BaseLineChart
    {
        private const double BoxWidth = 0.5;
        private const double BoxHeight = 0.2;

        #region Fields

        private readonly AxisX _xAxis;
        private readonly AxisY _yAxis;

        private readonly PointLineSeries _markerSeries;

        #endregion

        public BoxWhiskerChart(string xAxisTitle, string yAxisTitle)
        {
            #region Axes

            _xAxis = new AxisX
            {
                ScrollMode = XAxisScrollMode.None,
                ValueType = AxisValueType.Number,
                Title = CreateAxisXTitle(xAxisTitle),
                CustomTicksEnabled = true,
                AutoFormatLabels = false
            };

            _yAxis = new AxisY
            {
                ValueType = AxisValueType.Number,
                Title = CreateAxisYTitle(yAxisTitle)
            };

            #endregion

            #region Chart

            Chart = new LightningChart
            {
                ViewXY =
                {
                    XAxes = new AxisXList { _xAxis },
                    YAxes = new AxisYList { _yAxis },
                },
                Title =
                {
                    Visible = false
                }
            };
            
            Chart.ViewXY.LegendBoxes[0].Visible = false;

            SetupChartTheme();

            #endregion

            #region Series

            _markerSeries = new PointLineSeries(Chart.ViewXY, _xAxis, _yAxis)
            {
                ShowInLegendBox = false,
                PointsVisible = true,
                LineVisible = true,
                Title =
                {
                    Visible = false
                }
            };

            Chart.ViewXY.PointLineSeries.Add(_markerSeries);

            #endregion

            #region Target & Tolerance

            var targetLine = new ConstantLine(Chart.ViewXY, _xAxis, _yAxis)
            {
                Visible = true,
                Behind = true,
                LineStyle =
                {
                    Color = Color.FromArgb(0xFF, 0x08, 0xB4, 0x08),
                    Pattern = LinePattern.Dash,
                    Width = 2
                },
                AllowUserInteraction = false,
                Value = 0
            };

            Chart.ViewXY.ConstantLines.Add(targetLine);

            var minToleranceLine = new ConstantLine(Chart.ViewXY, _xAxis, _yAxis)
            {
                Visible = true,
                Behind = true,
                LineStyle =
                {
                    Color = Color.FromArgb(0xFF, 0xD8, 0x12, 0x12),
                    Pattern = LinePattern.Dot,
                    Width = 2
                },
                AllowUserInteraction = false,
                Value = -1
            };

            Chart.ViewXY.ConstantLines.Add(minToleranceLine);

            var maxToleranceLine = new ConstantLine(Chart.ViewXY, _xAxis, _yAxis)
            {
                Visible = true,
                Behind = true,
                LineStyle =
                {
                    Color = Color.FromArgb(0xFF, 0xD8, 0x12, 0x12),
                    Pattern = LinePattern.Dot,
                    Width = 2
                },
                AllowUserInteraction = false,
                Value = 1
            };

            Chart.ViewXY.ConstantLines.Add(maxToleranceLine);

            #endregion
        }

        public void AddEmptyBox(double x, string caption)
        {
            _xAxis.CustomTicks.Add(new CustomAxisTick(_xAxis, x, caption, 10, true, Colors.Black, CustomTickStyle.TickAndGrid));
        }

        /// <summary>
        /// Add box with whiskers. Box shows the range from upper quartile to lower quartile. 
        /// Whiskers show the range from minimum to lower quartile and upper quartile to maximum. 
        /// Horizontal line inside the box shows the median.
        /// </summary>
        /// <param name="minimum">Minimum</param>
        /// <param name="lowerQuartile">Lower quartile</param>
        /// <param name="median">Median</param>
        /// <param name="upperQuartile">Upper quartile</param>
        /// <param name="maximum">Maximum</param>
        /// <param name="x">X value the box is centered to</param>
        /// <param name="caption">Box caption</param>
        /// <param name="hasRepeta">Has repeta</param>
        /// <param name="fillColor">Fill color</param>
        public void AddBoxAndWhiskers(double minimum, double lowerQuartile, double median, double upperQuartile, double maximum, double x, string caption, bool hasRepeta, Color fillColor, SeriesMarkerPointShapeStyle markerStyle)
        {
            _xAxis.CustomTicks.Add(new CustomAxisTick(_xAxis, x, caption, 10, true, Colors.Black, CustomTickStyle.TickAndGrid));
            _xAxis.InvalidateCustomTicks();

            //Add polygon for box 
            var polygon = new PolygonSeries(Chart.ViewXY, _xAxis, _yAxis)
            {
                Fill =
                {
                    Color = fillColor, 
                    GradientFill = GradientFill.Solid,
                },
                Behind = true,
                Border =
                {
                    Color = Colors.DarkGray,
                    Width = 1
                },
                AllowUserInteraction = false,
                Title =
                {
                    Visible = false, 
                    Text = caption, 
                    Color = ChartTools.CalcGradient(fillColor, Colors.White, 70)
                }
            };

            var polygonPath = new PointDouble2D[4];
            double lowerHeight = lowerQuartile;
            double upperHeight = upperQuartile;
            if (!hasRepeta)
            {
                lowerHeight = lowerQuartile - BoxHeight;
                upperHeight = upperQuartile + BoxHeight;
            }
            polygonPath[0].X = x - BoxWidth / 2.0;
            polygonPath[0].Y = lowerHeight;
            polygonPath[1].X = x + BoxWidth / 2.0;
            polygonPath[1].Y = lowerHeight;
            polygonPath[2].X = x + BoxWidth / 2.0;
            polygonPath[2].Y = upperHeight;
            polygonPath[3].X = x - BoxWidth / 2.0;
            polygonPath[3].Y = upperHeight;
            polygon.Points = polygonPath;

            Chart.ViewXY.PolygonSeries.Add(polygon);

            //Add line collection for median and whiskers
            var lineCollectionMedian = new LineCollection(Chart.ViewXY, _xAxis, _yAxis)
            {
                LineStyle =
                {
                    Color = Colors.Black,
                    Width = 1,
                    AntiAliasing = LineAntialias.None
                },
                AllowUserInteraction = false
            };
            lineCollectionMedian.Lines = new[]
            {
                new SegmentLine(x - BoxWidth / 2.0, median, x + BoxWidth / 2.0, median), //median line 
                new SegmentLine(x, maximum, x, upperQuartile), //upper whisker 
                new SegmentLine(x - BoxWidth / 8.0, maximum, x + BoxWidth / 8.0, maximum), //upper whisker end 
                new SegmentLine(x, minimum, x, lowerQuartile), //lower whisker
                new SegmentLine(x - BoxWidth / 8.0, minimum, x + BoxWidth / 8.0, minimum) //lower whisker end 
            };

            Chart.ViewXY.LineCollections.Add(lineCollectionMedian);

            if (markerStyle != null)
            {
                var marker = new SeriesEventMarker
                {
                    AllowDragging = false,
                    XValue = x,
                    YValue = median,
                    Visible = true,
                    Symbol = markerStyle,
                    Label = new EventMarkerTitle
                    {
                        Visible = false
                    }
                };
                _markerSeries.SeriesEventMarkers.Add(marker);
            }
        }

        public void AutoScale()
        {
            Chart.ViewXY.ZoomToFit();

            // Show custom ticks even if there is no data
            double maxCustomTick = _xAxis.CustomTicks.Max(tick => tick.AxisValue);
            if (_xAxis.Maximum < maxCustomTick) _xAxis.Maximum = maxCustomTick;

            // 5% margin for X & Y axis
            double xRange = _xAxis.Maximum - _xAxis.Minimum;
            _xAxis.Minimum -= xRange * 0.05;
            _xAxis.Maximum += xRange * 0.05;

            double yRange = _yAxis.Maximum - _yAxis.Minimum;
            _yAxis.Minimum -= yRange * 0.05;
            _yAxis.Maximum += yRange * 0.05;
        }

        public void Clear()
        {
            UpdateChart(() =>
            {
                Chart.ViewXY.PolygonSeries.Clear();
                Chart.ViewXY.LineCollections.Clear();
                _xAxis.CustomTicks.Clear();
                _markerSeries.SeriesEventMarkers.Clear();
            });
        }
    }
}
