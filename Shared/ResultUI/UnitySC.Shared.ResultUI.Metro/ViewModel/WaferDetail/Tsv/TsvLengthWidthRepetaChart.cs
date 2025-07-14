using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Annotations;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.EventMarkers;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv
{
    public class TsvLengthWidthRepetaChart : BaseLineChart
    {
        #region Fields

        private readonly AxisX _xAxis;

        private readonly AxisY _yAxis;

        private readonly FreeformPointLineSeries _pointsSeries;

        private readonly AnnotationXY _toleranceAnnotation;

        #endregion Fields

        #region Constructor

        public TsvLengthWidthRepetaChart(string xAxisTitle, string yAxisTitle)
        {
            #region Axes

            _xAxis = new AxisX
            {
                AutoFormatLabels = false,
                ValueType = AxisValueType.Number,
                Visible = true,
                Title = CreateAxisXTitle(xAxisTitle)
            };

            _yAxis = new AxisY
            {
                AutoFormatLabels = false,
                ValueType = AxisValueType.Number,
                Visible = true,
                Title = CreateAxisYTitle(yAxisTitle)
            };

            #endregion Axes

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

            Chart.ViewXY.LegendBoxes[0].Visible = false;

            SetupChartTheme();

            #endregion Chart

            #region Series

            _pointsSeries = new FreeformPointLineSeries(Chart.ViewXY, _xAxis, _yAxis)
            {
                ShowInLegendBox = false,
                PointsVisible = false,
                Title =
                {
                    Visible = false
                },
                LineVisible = false
            };

            Chart.ViewXY.FreeformPointLineSeries.Add(_pointsSeries);

            #endregion Series

            #region Annotations

            _toleranceAnnotation = new AnnotationXY(Chart.ViewXY, _xAxis, _yAxis)
            {
                Behind = true,
                AllowUserInteraction = false,
                Style = AnnotationStyle.Rectangle,
                Sizing = AnnotationXYSizing.AxisValuesBoundaries,
                AxisValuesBoundaries =
                {
                    XMin = 0,
                    XMax = 0,
                    YMin = 0,
                    YMax = 0
                },
                Fill =
                {
                    Color = Color.FromArgb(0x55, 0x08, 0xB4, 0x08),
                    GradientFill = GradientFill.Solid
                },
                BorderVisible = false,
                Shadow =
                {
                    Visible = false
                },
                TextStyle =
                {
                    Visible = false
                }
            };

            Chart.ViewXY.Annotations.Add(_toleranceAnnotation);

            #endregion Annotations
        }

        #endregion Constructor

        #region Private Methods

        private void Clear()
        {
            _pointsSeries.Clear();
            _pointsSeries.SeriesEventMarkers.Clear();
        }

        private void AutoScale()
        {
            Chart.ViewXY.ZoomToFit();

            _xAxis.Minimum = _toleranceAnnotation.AxisValuesBoundaries.XMin;
            _xAxis.Maximum = _toleranceAnnotation.AxisValuesBoundaries.XMax;
            _yAxis.Minimum = _toleranceAnnotation.AxisValuesBoundaries.YMin;
            _yAxis.Maximum = _toleranceAnnotation.AxisValuesBoundaries.YMax;

            foreach (var point in _pointsSeries.Points.Take(_pointsSeries.PointCount))
            {
                if (_xAxis.Minimum > point.X) _xAxis.Minimum = point.X;
                if (_xAxis.Maximum < point.X) _xAxis.Maximum = point.X;
                if (_yAxis.Minimum > point.Y) _yAxis.Minimum = point.Y;
                if (_yAxis.Maximum < point.Y) _yAxis.Maximum = point.Y;
            }

            _xAxis.Minimum -= 10;
            _xAxis.Maximum += 10;
            _yAxis.Minimum -= 10;
            _yAxis.Maximum += 10;
        }

        private void DrawTargetLines(double widthTarget, double lengthTarget)
        {
            Chart.ViewXY.ConstantLines.Clear();
            Chart.ViewXY.LineSeriesCursors.Clear();

            var widthLine = new ConstantLine(Chart.ViewXY, _xAxis, _yAxis)
            {
                Visible = true,
                Behind = true,
                LineStyle =
                {
                    Color = Color.FromArgb(0xFF, 0xD8, 0x12, 0x12),
                    Pattern = LinePattern.Dot,
                    Width = 3
                },
                AllowUserInteraction = false,
                Value = widthTarget
            };

            Chart.ViewXY.ConstantLines.Add(widthLine);

            var lengthLine = new LineSeriesCursor(Chart.ViewXY, _xAxis)
            {
                Visible = true,
                Behind = true,
                LineStyle =
                {
                    Color = Color.FromArgb(0xFF, 0xD8, 0x12, 0x12),
                    Pattern = LinePattern.Dot,
                    Width = 3
                },
                AllowUserInteraction = false,
                ValueAtXAxis = lengthTarget
            };
            Chart.ViewXY.LineSeriesCursors.Add(lengthLine);
        }

        #endregion

        #region Public Methods

        public void SetData<T>(IEnumerable<T> collection, Func<T, double?> getX, Func<T, double?> getY, Func<T, SeriesMarkerPointShapeStyle> getMarker)
        {
            UpdateChart(() =>
            {
                Clear();

                foreach (var data in collection)
                {
                    double? x = getX(data);
                    double? y = getY(data);

                    if (!x.HasValue || double.IsNaN(x.Value)) continue;
                    if (!y.HasValue || double.IsNaN(y.Value)) continue;
                    
                    _pointsSeries.AddPoints(new[] { new SeriesPoint(x.Value, y.Value) }, true);

                    var marker = new SeriesEventMarker
                    {
                        AllowDragging = false,
                        XValue = x.Value,
                        YValue = y.Value,
                        Symbol = getMarker.Invoke(data),
                        Label = new EventMarkerTitle
                        {
                            Visible = false
                        }
                    };

                    _pointsSeries.SeriesEventMarkers.Add(marker);
                }

                AutoScale();
            });
        }

        public void SetTargetAndTolerance(double xTarget, double xTolerance, double yTarget, double yTolerance)
        {
            UpdateChart(() =>
            {
                _toleranceAnnotation.AxisValuesBoundaries.XMin = xTarget - xTolerance;
                _toleranceAnnotation.AxisValuesBoundaries.XMax = xTarget + xTolerance;
                _toleranceAnnotation.AxisValuesBoundaries.YMin = yTarget - yTolerance;
                _toleranceAnnotation.AxisValuesBoundaries.YMax = yTarget + yTolerance;

                if (_xAxis.Minimum > _toleranceAnnotation.AxisValuesBoundaries.XMin) _xAxis.Minimum = _toleranceAnnotation.AxisValuesBoundaries.XMin;
                if (_xAxis.Maximum < _toleranceAnnotation.AxisValuesBoundaries.XMax) _xAxis.Maximum = _toleranceAnnotation.AxisValuesBoundaries.XMax;
                if (_yAxis.Minimum > _toleranceAnnotation.AxisValuesBoundaries.YMin) _yAxis.Minimum = _toleranceAnnotation.AxisValuesBoundaries.YMin;
                if (_yAxis.Maximum < _toleranceAnnotation.AxisValuesBoundaries.YMax) _yAxis.Maximum = _toleranceAnnotation.AxisValuesBoundaries.YMax;

                DrawTargetLines( yTarget, xTarget);
                AutoScale();
            });
        }

        public void ClearTargetAndTolerance()
        {
            UpdateChart(() =>
            {
                Chart.ViewXY.ConstantLines.Clear();
                Chart.ViewXY.LineSeriesCursors.Clear();
            });
        }
        
        #endregion Public Methods
    }
}
