using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.EventMarkers;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;

using CommunityToolkit.Mvvm.ComponentModel;


using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts
{
    public class DataInToleranceSeries : ObservableObject
    {
        private static readonly Color s_customTickColor = Color.FromArgb(35, 0, 0, 255);

        private readonly LightningChart _chart;

        private readonly AxisX _xAxis;

        public string Title { get; private set; }

        private readonly SortedDictionary<int, SeriesPoint> _seriesPoints = new SortedDictionary<int, SeriesPoint>();

        private readonly SortedDictionary<int, SeriesErrorPoint> _seriesErrorPoints = new SortedDictionary<int, SeriesErrorPoint>();

        private readonly SeriesEventMarker _selectionMarker;

        private readonly AxisY _yAxis;

        private readonly PointLineSeries _series;

        private ConstantLine _targetLine;

        private ConstantLine _maxToleranceLine;
        
        private ConstantLine _minToleranceLine;

        private readonly HashSet<CustomAxisTick> _customTicks = new HashSet<CustomAxisTick>();

        private IEnumerable<ConstantLine> ConstantLines => new HashSet<ConstantLine> { _targetLine, _maxToleranceLine, _minToleranceLine };

        public DataInToleranceSeries(LightningChart chart, AxisX xAxis, string title, Color lineColor, bool showStdDev, bool showPoints, double fontsize = 14.0, string unit = null, double unitfontsize = 10.0)
        {
            _chart = chart;
            _xAxis = xAxis;
            Title = title;

            #region YAxis

            _yAxis = BaseLineChart.CreateAxisY(_chart.ViewXY, title, fontsize, unit, unitfontsize);
            _chart.ViewXY.YAxes.Add(_yAxis);

            #endregion

            #region Series

            _series = new PointLineSeries(_chart.ViewXY, _xAxis, _yAxis)
            {
                ShowInLegendBox = false,
                PointsVisible = showPoints,
                LineStyle = { Color = lineColor },
                Title =
                {
                    Text = title,
                    Visible = false
                },
                ErrorBars =
                {
                    ShowYError = true,
                    YColor = Colors.Black,
                    YThickness = 1
                },
                AssignYAxisIndex = _chart.ViewXY.YAxes.IndexOf(_yAxis),
                PointsType = showStdDev ? PointsType.ErrorPoints : PointsType.Points,
                DataBreaking = new DataBreakingOptions
                {
                    Value = double.NaN,
                    Enabled = true
                },
                Highlight = Highlight.None
            };

            _chart.ViewXY.PointLineSeries.Add(_series);

            #endregion

            #region Selection Marker

            _selectionMarker = new SeriesEventMarker
            {
                XValue = 0,
                YValue = 0,
                Visible = false,
                AllowUserInteraction = false,
                Symbol = BaseLineChart.GetSelectionSymbol(),
                Label = new EventMarkerTitle
                {
                    Visible = false
                }
            };
            _series.SeriesEventMarkers.Add(_selectionMarker);

            #endregion
        }

        #region Events

        public event EventHandler<int> PointSelected;

        private void MarkerOnMouseClick(object sender, MouseEventArgs e)
        {
            if (sender is SeriesEventMarker marker)
            {
                PointSelected?.Invoke(this, (int)marker.XValue);
            }
        }

        #endregion

        /// <summary>
        /// Remove the series from the chart as well as all its associated elements (AxisY, ConstantsLines & CustomTicks).
        /// </summary>
        public void Delete()
        {
            ClearTargetAndTolerance();
            Clear();

            _chart.ViewXY.PointLineSeries.Remove(_series);
            _chart.ViewXY.YAxes.Remove(_yAxis);
        }

        public void Clear()
        {
            _seriesPoints.Clear();
            _seriesErrorPoints.Clear();

            _series.Clear();

            foreach (var marker in _series.SeriesEventMarkers.Where(marker => marker != _selectionMarker))
            {
                marker.MouseClick -= MarkerOnMouseClick;
            }

            _series.SeriesEventMarkers.Clear();
            _series.SeriesEventMarkers.Add(_selectionMarker);

            _series.InvalidateData();

            foreach (var customTick in _customTicks)
            {
                _xAxis.CustomTicks.Remove(customTick);
            }

            _customTicks.Clear();
        }
        
        public void UpdateTitle(string title)
        {
            Title = title;
            _yAxis.Title.Text = title;
        }

        public void AddErrorPoint(int xIndex, SeriesErrorPoint point)
        {
            _seriesErrorPoints.Add(xIndex, point);
        }

        public void AddPoint(int xIndex, SeriesPoint point)
        {
            _seriesPoints.Add(xIndex, point);
        }

        public void AddMarker(int xIndex, double mean, SeriesMarkerPointShapeStyle markerStyle)
        {
            if (!double.IsNaN(mean) && markerStyle != null)
            {
                var marker = new SeriesEventMarker
                {
                    AllowDragging = false,
                    XValue = xIndex,
                    YValue = mean,
                    Symbol = markerStyle,
                    Label = new EventMarkerTitle
                    {
                        Visible = false
                    }
                };
                marker.MouseClick += MarkerOnMouseClick;
                
                _series.SeriesEventMarkers.Insert(_series.SeriesEventMarkers.Count - 1, marker);
            }
        }

        public void AddCustomTick(int xIndex)
        {
            var customTick = new CustomAxisTick(_xAxis, xIndex, xIndex.ToString(CultureInfo.InvariantCulture), 10, true, s_customTickColor, CustomTickStyle.TickAndGrid);

            _customTicks.Add(customTick);
        }

        public void AddCustomTicks()
        {
            _xAxis.CustomTicks = new CustomAxisTickList();
            _xAxis.CustomTicks.AddRange(_customTicks);
      
        }

        public void FlushDataPoints()
        {
            if(_seriesPoints.Values.Count > 0)
                _series.AddPoints(_seriesPoints.Values.ToArray(), true);
            if (_seriesErrorPoints.Values.Count > 0)
                _series.AddPoints(_seriesErrorPoints.Values.ToArray(), true);

            _seriesPoints.Clear();
            _seriesErrorPoints.Clear();
        }

        #region AutoScale

        /// <summary>
        /// Scale the YAxis and returns the Min/Max values for XAxis
        /// </summary>
        public Tuple<double, double> AutoScale()
        {
            _yAxis.Minimum = 0;
            _yAxis.Maximum = 0;

            double xAxisMin = double.NaN;
            double xAxisMax = double.NaN;
            double yAxisMin = double.NaN;
            double yAxisMax = double.NaN;

            UpdateMax(ref yAxisMax, ConstantLines, line => line?.Value ?? double.NaN);
            UpdateMin(ref yAxisMin, ConstantLines, line => line?.Value ?? double.NaN);

            var errorPoints = _series.PointsWithErrors
                .Where(point => point.X != 0 || point.Y != 0 || point.ErrorYMinus != 0 || point.ErrorYPlus != 0).ToList();

            if (errorPoints.Count > 0)
            {
                UpdateMax(ref xAxisMax, errorPoints, point => point.X);
                UpdateMin(ref xAxisMin, errorPoints, point => point.X);
                UpdateMax(ref yAxisMax, errorPoints, point => point.Y + point.ErrorYPlus);
                UpdateMin(ref yAxisMin, errorPoints, point => point.Y - point.ErrorYMinus);
            }

            var points = _series.Points.Take(_series.PointCount).ToList();

            if (points.Count > 0)
            {
                UpdateMax(ref xAxisMax, points, point => point.X);
                UpdateMin(ref xAxisMin, points, point => point.X);
                UpdateMax(ref yAxisMax, points, point => point.Y);
                UpdateMin(ref yAxisMin, points, point => point.Y);
            }
            
            double range = Math.Abs(yAxisMax - yAxisMin);
            _yAxis.SetRange(yAxisMin - range * 0.05, yAxisMax + range * 0.05);

            return new Tuple<double, double>(xAxisMin, xAxisMax);
        }

        private static void UpdateMin<TSource>(ref double currentMin, IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            var filterNaN = source.Select(selector).Where(value => !double.IsNaN(value)).ToList();
            if (filterNaN.Count <= 0) return;
            double min = filterNaN.Min();

            if (double.IsNaN(currentMin)) currentMin = min;

            currentMin = min < currentMin ? min : currentMin;
        }

        private static void UpdateMax<TSource>(ref double currentMax, IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            var filterNaN = source.Select(selector).Where(value => !double.IsNaN(value)).ToList();
            if (filterNaN.Count <= 0) return;
            double max = filterNaN.Max();

            if (double.IsNaN(currentMax)) currentMax = max;

            currentMax = max > currentMax ? max : currentMax;
        }

        #endregion

        #region Constant Lines

        public void ClearTargetAndTolerance()
        {
            if (_targetLine != null)
            {
                _chart.ViewXY.ConstantLines.Remove(_targetLine);
                _targetLine = null;
            }

            if (_minToleranceLine != null)
            {
                _chart.ViewXY.ConstantLines.Remove(_minToleranceLine);
                _minToleranceLine = null;
            }

            if (_maxToleranceLine != null)
            {
                _chart.ViewXY.ConstantLines.Remove(_maxToleranceLine);
                _maxToleranceLine = null;
            }
        }

        public void DrawTargetLine(double target)
        {
            _targetLine = new ConstantLine(_chart.ViewXY, _xAxis, _yAxis)
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
                Value = target
            };

            _chart.ViewXY.ConstantLines.Add(_targetLine);
        }

        public void DrawToleranceLines(double minAllowed, double maxAllowed)
        {
            _minToleranceLine = new ConstantLine(_chart.ViewXY, _xAxis, _yAxis)
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
                Value = minAllowed
            };

            _chart.ViewXY.ConstantLines.Add(_minToleranceLine);

            _maxToleranceLine = new ConstantLine(_chart.ViewXY, _xAxis, _yAxis)
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
                Value = maxAllowed
            };

            _chart.ViewXY.ConstantLines.Add(_maxToleranceLine);
        }

        #endregion

        public void UpdateSelectedPoint(double? x, double? y)
        {
            if (x.HasValue && y.HasValue)
            {
                _selectionMarker.XValue = x.Value;
                _selectionMarker.YValue = y.Value;
                _selectionMarker.Visible = true;
            }
            else
            {
                _selectionMarker.Visible = false;
            }
        }
    }
}
