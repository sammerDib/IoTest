using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts.StackedArea
{
    public class StackedAreaChart : BaseLineChart
    {
        #region Fields

        private readonly AxisX _xAxis;
        private readonly AxisY _yAxis;

        private readonly LineSeriesCursor _trackerLineSeriesCursor;

        public event EventHandler<double> TrackerPositionChanged;

        #endregion

        #region Properties

        public double TrackerPosition
        {
            get { return _trackerLineSeriesCursor.ValueAtXAxis; }
            set
            {
                _trackerLineSeriesCursor.ValueAtXAxis = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public StackedAreaChart(string xAxisTitle, string yAxisTitle)
        {
            #region Axes

            _xAxis = new AxisX
            {
                ValueType = AxisValueType.Number,
                Title = CreateAxisXTitle(xAxisTitle)
            };

            _yAxis = new AxisY
            {
                ValueType = AxisValueType.Number,
                Title = CreateAxisYTitle(yAxisTitle),
                Minimum = 0
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
                    AxisLayout =
                    {
                        YAxesLayout = YAxesLayout.Stacked,
                        AutoAdjustMargins = true,
                    },
                    ZoomPanOptions =
                    {
                        ViewFitYMarginPixels = 0,
                        PanDirection = PanDirection.Both,
                        WheelZooming = WheelZooming.HorizontalAndVertical
                    }
                },
                Title =
                {
                    Visible = false
                }
            };
            
            Chart.ViewXY.LegendBoxes[0].Visible = false;

            SetupChartTheme();
            
            #endregion

            #region Tracker Line

            _trackerLineSeriesCursor = new LineSeriesCursor(Chart.ViewXY, Chart.ViewXY.XAxes[0])
            {
                AllowDragging = true,
                Highlight = Highlight.Simple,
                LineStyle = new LineStyle { Color = Colors.Red, AntiAliasing = LineAntialias.Normal, Width = 2 }
            };
            _trackerLineSeriesCursor.PositionChanged += OnTrackerPositionChanged;
            Chart.ViewXY.LineSeriesCursors.Add(_trackerLineSeriesCursor);

            #endregion
        }

        #region Event Handlers

        private void OnTrackerPositionChanged(object sender = null, PositionChangedEventArgs e = null)
        {
            OnPropertyChanged(nameof(TrackerPosition));
            TrackerPositionChanged?.Invoke(this, TrackerPosition);
        }

        #endregion

        public void SetYAxisTitle(string title)
        {
            _yAxis.Title.Text = title;
        }

        private HighLowSeriesPoint[] _previousSeriesPoints;

        public void SetData<T>(string seriesName, ICollection<T> items, Func<T, double> getXValue, Func<T, double> getYValue, Color color)
        {
            var series = new HighLowSeries(Chart.ViewXY, _xAxis, _yAxis)
            {
                Fill =
                {
                    Color = color,
                    GradientFill = GradientFill.Solid
                },
                LineStyle = { Width = 2f },
                LineStyleHigh = { Color = Color.FromArgb(50, 0, 0, 0) },
                LineVisibleLow = false,
                Highlight = Highlight.None,
                AllowUserInteraction = false,
                Title = { Text = seriesName }
            };

            var highLowPoints = new HighLowSeriesPoint[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                var item = items.ElementAt(i);

                highLowPoints[i].X = getXValue(item);
                highLowPoints[i].YLow = 0;
                highLowPoints[i].YHigh = getYValue(item);

                if (_previousSeriesPoints != null && _previousSeriesPoints.Length > i)
                {
                    // Stack over previous series high values.
                    highLowPoints[i].YLow += _previousSeriesPoints[i].YHigh;
                    highLowPoints[i].YHigh += _previousSeriesPoints[i].YHigh;
                }
            }

            series.Points = highLowPoints;
            Chart.ViewXY.HighLowSeries.Add(series);

            // Store for next series use. 
            _previousSeriesPoints = highLowPoints;
        }

        public void SetLine<T>(string seriesName, ICollection<T> items, Func<T, double> getXValue, Func<T, double> getYValue, Color color)
        {
            var series = new PointLineSeries(Chart.ViewXY, _xAxis, _yAxis)
            {
                LineStyle = new LineStyle { Color = color, Pattern = LinePattern.Dash, Width = 3 },
                PointsVisible = false,
                Highlight = Highlight.None,
                AllowUserInteraction = false,
                Title = { Text = seriesName }
            };

            var points = new SeriesPoint[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                var item = items.ElementAt(i);
                points[i].X = getXValue(item);
                points[i].Y = getYValue(item);
            }

            series.Points = points;
            Chart.ViewXY.PointLineSeries.Add(series);
        }

        public void ClearValues()
        {
            UpdateChart(() =>
            {
                _previousSeriesPoints = null;
                Chart.ViewXY.HighLowSeries.Clear();
                Chart.ViewXY.PointLineSeries.Clear();
            });
        }
        
        public void RaiseDataUpdated()
        {
            if (Chart.ViewXY.HighLowSeries.Count == 0)
            {
                TrackerPosition = 0;
                return;
            }

            double maxX = Chart.ViewXY.HighLowSeries.Max(series =>
            {
                if (series.GetXMinMax(out double _, out double max))
                {
                    return max;
                }

                return 0;
            });
            if (TrackerPosition < 0)
            {
                TrackerPosition = 0;
            }
            else if (TrackerPosition > maxX)
            {
                TrackerPosition = maxX;
            }
            else
            {
                OnTrackerPositionChanged();
            }

            Application.Current?.Dispatcher?.Invoke(() => Chart.ViewXY.ZoomToFit());
        }
    }
}
