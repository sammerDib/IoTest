using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts
{
    public class DataInToleranceLineChart : BaseLineChart
    {
        #region Fields

        private readonly AxisX _xAxis;

        private readonly bool _showStdDev;

        private readonly bool _showPoints;
        
        private readonly Color _mainLineColor;

        private readonly List<DataInToleranceSeries> _series = new List<DataInToleranceSeries>();

        #endregion Fields

        #region Events

        public event EventHandler<int> PointSelected;

        #endregion

        #region Constructor
        
        public DataInToleranceLineChart(string xAxisTitle, bool showStdDev, Color mainLineColor, bool showPoints)
        {
            #region Fields and Properties

            _showStdDev = showStdDev;
            _mainLineColor = mainLineColor;
            _showPoints = showPoints;

            #endregion Fields and Properties

            #region Axes

            _xAxis = new AxisX
            {
                AutoFormatLabels = false,
                ValueType = AxisValueType.Number,
                CustomTicksEnabled = true,
                Visible = true,
                LabelsNumberFormat = "0",
                Title = CreateAxisXTitle(xAxisTitle)
            };

            #endregion Axes

            #region Chart

            Chart = new LightningChart
            {
                ViewXY =
                {
                    DropOldSeriesData = false,
                    XAxes = new AxisXList { _xAxis },
                    AxisLayout =
                    {
                        YAxesLayout = YAxesLayout.Stacked,
                        AutoAdjustMargins = true,
                    },
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

            Chart.ViewXY.YAxes.Clear();
            Chart.ViewXY.LegendBoxes[0].Visible = false;

            SetupChartTheme();

            #endregion Chart
        }

        public DataInToleranceLineChart(string xAxisTitle, string yAxisTitle, bool showStdDev, Color mainLineColor, bool showPoints, double fontsize = 14.0, string unit = null, double unitfontsize = 10.0) : this(xAxisTitle, showStdDev, mainLineColor, showPoints)
        {
            AddSerie(yAxisTitle, null, fontsize, unit, unitfontsize);
        }

        #endregion Constructor

        #region Event Handlers

        private void LineSeries_PointSelected(object sender, int xValue)
        {
            PointSelected?.Invoke(this, xValue);
        }

        #endregion

        #region Private Methods

        private void CreateDataPoint(DataInToleranceSeries series, int xIndex, double mean, double negativeErrorGap, double positiveErrorGap, SeriesMarkerPointShapeStyle markerStyle)
        {
            if (_showStdDev)
            {
                var newPoint = new SeriesErrorPoint(xIndex, mean, 0, 0, negativeErrorGap, positiveErrorGap);
                series.AddErrorPoint(xIndex, newPoint);
            }
            else
            {
                var newPoint = new SeriesPoint(xIndex, mean);
                series.AddPoint(xIndex, newPoint);
            }

            if (!double.IsNaN(mean) && markerStyle != null)
            {
                series.AddMarker(xIndex, mean, markerStyle);
            }

            series.AddCustomTick(xIndex);
        }

        private void AutoScale()
        {
            Chart.ViewXY.ZoomToFit();

            _xAxis.Minimum = 0;
            _xAxis.Maximum = 0;

            double xAxisMin = double.NaN;
            double xAxisMax = double.NaN;

            foreach (var series in _series)
            {
                (double minX, double maxX) = series.AutoScale();
                if (double.IsNaN(xAxisMin) || minX < xAxisMin)
                {
                    xAxisMin = minX;
                }
                if (double.IsNaN(xAxisMax) || maxX > xAxisMax)
                {
                    xAxisMax = maxX;
                }
            }
            
            _xAxis.SetRange(xAxisMin - 1, xAxisMax + 1);
        }
        
        private DataInToleranceSeries AddSerie(string title, Color? color = null, double fontsize = 14.0, string unit = null, double unitfontsize = 10.0)
        {
            // Add the series only if it doesn't exist. 
            var lineSeries = _series.FirstOrDefault(series => series.Title.Equals(title));
            if (lineSeries != null) return lineSeries;

            UpdateChart(() =>
            {
                lineSeries = new DataInToleranceSeries(Chart, _xAxis, title, color ?? _mainLineColor, _showStdDev, _showPoints, fontsize, unit, unitfontsize);
                lineSeries.PointSelected += LineSeries_PointSelected;
                
                _series.Add(lineSeries);

                if (_series.Count > 1)
                {
                    Chart.ViewXY.ZoomPanOptions = new ZoomPanOptions
                    {
                        ViewFitYMarginPixels = 10, 
                        PanDirection = PanDirection.Horizontal, 
                        WheelZooming = WheelZooming.Horizontal, 
                        RectangleZoomMode = RectangleZoomMode.Horizontal,
                    };
                }
                else
                {
                    Chart.ViewXY.ZoomPanOptions = new ZoomPanOptions
                    {
                        ViewFitYMarginPixels = 50
                    };
                }
            });

            return lineSeries;
        }
        
        #endregion

        #region Public Methods

        public void UpdateYAxisTitle(string title)
        {
            UpdateChart(() =>
            {
                foreach (var series in _series)
                {
                    series.UpdateTitle(title);
                }
            });
        }

        public void SetTargetAndTolerance(double target, double minTolerance, double maxTolerance, string titleSeries = null)
        {
            UpdateChart(() =>
            {
                if (string.IsNullOrEmpty(titleSeries))
                {
                    ClearTargetAndTolerance();

                    foreach (var series in _series)
                    {
                        series.DrawTargetLine(target);
                        series.DrawToleranceLines(minTolerance, maxTolerance);
                    }
                }
                else
                {
                    var series = _series.First(toleranceSeries => toleranceSeries.Title.Equals(titleSeries));
                    series.DrawTargetLine(target);
                    series.DrawToleranceLines(minTolerance, maxTolerance);
                }

                AutoScale();
            });
        }

        public void SetTargetMinAndMax(double minTolerance, double maxTolerance, string titleSeries = null)
        {
            UpdateChart(() =>
            {
                if (string.IsNullOrEmpty(titleSeries))
                {
                    ClearTargetAndTolerance();

                    foreach (var series in _series)
                    {    
                        series.DrawToleranceLines(minTolerance, maxTolerance);
                    }
                }
                else
                {
                    var series = _series.First(toleranceSeries => toleranceSeries.Title.Equals(titleSeries));
                    series.DrawToleranceLines(minTolerance, maxTolerance);
                }

                AutoScale();
            });
        }

        public void ClearAll()
        {
            UpdateChart(() =>
            {
                foreach (var series in _series)
                {
                    series.PointSelected -= LineSeries_PointSelected;
                    series.Delete();
                }

                _series.Clear();
            });
        }

        public void ClearTargetAndTolerance()
        {
            UpdateChart(() =>
            {
                foreach (var series in _series)
                {
                    series.ClearTargetAndTolerance();
                }
            });
        }

        public void SetData<T>(IEnumerable<T> collection, Func<T, int> getIndex, Func<T, double?> getValue, Func<T, SeriesMarkerPointShapeStyle> getMarker, string seriesTitle = null)
        {
            SetData(collection, getIndex, getValue, null, null, getMarker, seriesTitle);
        }

        public void SetData<T>(IEnumerable<T> collection, Func<T, int> getIndex, Func<T, double?> getValue, Func<T, double?> getNegativeError, Func<T, double?> getPositiveError, Func<T, SeriesMarkerPointShapeStyle> getMarker,
            string seriesTitle = null, Color? seriesColor = null, double fontsize = 14.0, string unit = null, double unitfontsize = 10.0)
        {
            UpdateChart(() =>
            {
                var lineSeries = string.IsNullOrEmpty(seriesTitle) ? _series.Single() : AddSerie(seriesTitle, seriesColor, fontsize, unit, unitfontsize);

                lineSeries.Clear();

                foreach (var tsvPoint in collection)
                {
                    int index = getIndex(tsvPoint);

                    double? value = getValue(tsvPoint);
                    double? negativeError = getNegativeError?.Invoke(tsvPoint);
                    double? positiveError = getPositiveError?.Invoke(tsvPoint);

                    if (value.HasValue && !double.IsNaN(value.Value))
                    {
                        if (!negativeError.HasValue || double.IsNaN(negativeError.Value))
                        {
                            negativeError = value.Value;
                        }

                        if (!positiveError.HasValue || double.IsNaN(positiveError.Value))
                        {
                            positiveError = value.Value;
                        }

                        negativeError = value.Value - negativeError.Value;
                        positiveError = positiveError.Value - value.Value;

                        var marker = getMarker.Invoke(tsvPoint);

                        CreateDataPoint(lineSeries, index, value.Value, negativeError.Value, positiveError.Value, marker);
                    }
                    else
                    {
                        CreateDataPoint(lineSeries, index, double.NaN, double.NaN, double.NaN, null);
                    }
                }

                lineSeries.AddCustomTicks();
                lineSeries.FlushDataPoints();

                AutoScale();
            });
        }

        public void UpdateSelectedPoint(double? x, double? y)
        {
            UpdateChart(() =>
            {
                foreach (var series in _series)
                {
                    series.UpdateSelectedPoint(x, y);
                }
            });
        }

        #endregion Public Methods
    }
}
