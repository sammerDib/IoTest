using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.EventMarkers;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts.Profile
{
    public enum MarkerType
    {
        Line,
        Horizontal,
        Vertical
    }

    public class MarkerPositionChangedEventArgs : EventArgs
    {
        public MarkerType MarkerType { get; set; }

        public int Index { get; set; }

        public double Value { get; set; }
    }

    public class ProfileChart : BaseLineChart
    {
        private readonly LegendBoxXY _legendBox;
        private readonly AxisX _xAxis;
        private readonly AxisY _yAxis;

        private readonly PointLineSeries _lineSeries1;
        private readonly PointLineSeries _lineSeries2;
        private readonly SeriesEventMarker _marker;
        private readonly SeriesEventMarker _horizontalMarker;
        private readonly SeriesEventMarker _verticalMarker;

        public ProfileChart(string yAxis)
        {
            _legendBox = new LegendBoxXY
            {
                Position = LegendBoxPositionXY.TopRight,
                Offset = new PointIntXY(-17, 13)
            };

            _xAxis = new AxisX
            {
                ScrollMode = XAxisScrollMode.None,
                ValueType = AxisValueType.Number,
                Title = CreateAxisXTitle("Measurement")
            };

            _yAxis = new AxisY
            {
                AllowAutoYFit = false,
                Title = CreateAxisYTitle(yAxis)
            };

            #region Chart

            Chart = new LightningChart
            {
                ViewXY =
                {
                    DropOldSeriesData = false,
                    XAxes = new AxisXList { _xAxis },
                    YAxes = new AxisYList { _yAxis },
                    LegendBoxes = new LegendBoxXYList
                    {
                        _legendBox
                    },
                    AxisLayout = new AxisLayout
                    {
                        AxisGridStrips = XYAxisGridStrips.X,
                        XAxisAutoPlacement = XAxisAutoPlacement.BottomThenTop,
                        YAxisAutoPlacement = YAxisAutoPlacement.LeftThenRight
                    },
                    ZoomPanOptions = new ZoomPanOptions
                    {
                        ViewFitYMarginPixels = 10,
                        AutoYFit = new AutoYFit
                        {
                            Enabled = true
                        }
                    }
                },
                Title =
                {
                    Text = "Profile",
                    Visible = false
                }
            };

            SetupChartTheme();

            #endregion

            #region Markers

            _marker = new SeriesEventMarker
            {
                VerticalPosition = SeriesEventMarkerVerticalPosition.TrackSeries,
                Label = new EventMarkerTitle { Distance = 0, Color = Colors.OrangeRed },
                Symbol = new SeriesMarkerPointShapeStyle
                {
                    Width = 9,
                    Height = 9,
                    BorderColor = Colors.OrangeRed,
                    BorderWidth = 2,
                    Color1 = Colors.Transparent,
                    GradientFill = GradientFillPoint.Solid,
                    Shape = SeriesMarkerPointShape.Circle
                }
            };

            _horizontalMarker = new SeriesEventMarker
            {
                VerticalPosition = SeriesEventMarkerVerticalPosition.TrackSeries,
                Label = new EventMarkerTitle { Distance = 0, Color = Colors.Red },
                Symbol = new SeriesMarkerPointShapeStyle
                {
                    Width = 9,
                    Height = 9,
                    BorderColor = Colors.Red,
                    BorderWidth = 2,
                    Color1 = Colors.Transparent,
                    GradientFill = GradientFillPoint.Solid,
                    Shape = SeriesMarkerPointShape.Circle
                }
            };

            _verticalMarker = new SeriesEventMarker
            {
                VerticalPosition = SeriesEventMarkerVerticalPosition.TrackSeries,
                Label = new EventMarkerTitle { Distance = 0, Color = Colors.Green },
                Symbol = new SeriesMarkerPointShapeStyle
                {
                    Width = 9,
                    Height = 9,
                    BorderColor = Colors.Green,
                    BorderWidth = 2,
                    Color1 = Colors.Transparent,
                    GradientFill = GradientFillPoint.Solid,
                    Shape = SeriesMarkerPointShape.Circle
                }
            };

            _marker.PositionChanged += OnMarkerPositionChanged;
            _horizontalMarker.PositionChanged += OnMarkerPositionChanged;
            _verticalMarker.PositionChanged += OnMarkerPositionChanged;

            #endregion

            #region Series

            _lineSeries1 = new PointLineSeries(Chart.ViewXY, _xAxis, _yAxis)
            {
                Title = new SeriesTitle
                {
                    Text = "H"
                },
                LineStyle = new LineStyle
                {
                    Width = 2,
                    Color = Colors.DarkRed
                },
                SeriesEventMarkers = new SeriesEventMarkerList
                {
                    _marker,
                    _horizontalMarker
                },
                Highlight = Highlight.None

            };

            _lineSeries2 = new PointLineSeries(Chart.ViewXY, _xAxis, _yAxis)
            {
                Title = new SeriesTitle
                {
                    Text = "V"
                },
                LineStyle = new LineStyle
                {
                    Width = 2,
                    Color = Colors.DarkGreen
                },
                SeriesEventMarkers = new SeriesEventMarkerList
                {
                    _verticalMarker
                },
                Highlight = Highlight.None
            };

            Chart.ViewXY.PointLineSeries.Add(_lineSeries1);
            Chart.ViewXY.PointLineSeries.Add(_lineSeries2);

            #endregion

            UpdateVisibilities();
        }

        private void UpdateVisibilities()
        {
            UpdateChart(() =>
            {
                _legendBox.Visible = IsCrossProfile;
                _marker.Visible = !IsCrossProfile;
                _horizontalMarker.Visible = IsCrossProfile;
                _verticalMarker.Visible = IsCrossProfile;
            });
        }

        #region Events

        public event EventHandler<MarkerPositionChangedEventArgs> MarkerPositionChanged;

        #endregion

        #region Event Handlers

        private bool _preventPositionChanged;

        private void OnMarkerPositionChanged(object sender, PositionChangedSeriesEventMarkerEventArgs e)
        {
            if (_preventPositionChanged) return;

            //Update method refreshes the chart already 
            if (e != null) e.CancelRendering = true;

            if (Chart == null || !(sender is SeriesEventMarker marker)) return;

            MarkerPositionChangedEventArgs eventArgs = null;

            UpdateChart(() =>
            {
                double x = marker.XValue;
                double y = marker.YValue;
                var series = (PointLineSeries)marker.GetOwnerSeries();

                int newMarkerIndex = -1;
                if (series.Points != null && series.Points.Length > 0)
                {
                    newMarkerIndex = series.SolveYValueAtXValue(x).NearestDataPointIndex;
                    if (newMarkerIndex >= 0) y = series.Points[newMarkerIndex].Y;
                }

                // Line profile marker
                if (marker == _marker)
                {
                    MarkerIndex = newMarkerIndex;
                    eventArgs = new MarkerPositionChangedEventArgs
                    {
                        Index = newMarkerIndex,
                        MarkerType = MarkerType.Line,
                        Value = y
                    };
                }
                // Horizontal Cross profile marker
                else if (marker == _horizontalMarker)
                {
                    HorizontalMarkerIndex = newMarkerIndex;
                    eventArgs = new MarkerPositionChangedEventArgs
                    {
                        Index = newMarkerIndex,
                        MarkerType = MarkerType.Horizontal,
                        Value = y
                    };
                }
                // Vertical Cross profile marker
                else if (marker == _verticalMarker)
                {
                    VerticalMarkerIndex = newMarkerIndex;
                    eventArgs = new MarkerPositionChangedEventArgs
                    {
                        Index = newMarkerIndex,
                        MarkerType = MarkerType.Vertical,
                        Value = y
                    };
                }

                marker.Label.Text = "(" + x.ToString("0.0") + "; " + y.ToString("0.0") + ")";
            });

            if (eventArgs != null)
            {
                MarkerPositionChanged?.Invoke(this, eventArgs);
            }
        }

        #endregion

        public void UpdateMarkerLabel(MarkerType type, string label)
        {
            switch (type)
            {
                case MarkerType.Line:
                    _marker.Label.Text = label;
                    break;
                case MarkerType.Horizontal:
                    _horizontalMarker.Label.Text = label;
                    break;
                case MarkerType.Vertical:
                    _verticalMarker.Label.Text = label;
                    break;
            }
        }

        private int _markerIndex = -1;

        public int MarkerIndex
        {
            get { return _markerIndex; }
            set { SetProperty(ref _markerIndex, value); }
        }

        private int _horizontalMarkerIndex = -1;

        public int HorizontalMarkerIndex
        {
            get { return _horizontalMarkerIndex; }
            set { SetProperty(ref _horizontalMarkerIndex, value); }
        }

        private int _verticalMarkerIndex = -1;

        public int VerticalMarkerIndex
        {
            get { return _verticalMarkerIndex; }
            set { SetProperty(ref _verticalMarkerIndex, value); }
        }

        private bool _isCrossProfile;

        public bool IsCrossProfile
        {
            get { return _isCrossProfile; }
            set { SetProperty(ref _isCrossProfile, value); }
        }

        public SeriesPoint[] Points => _lineSeries1?.Points;
        public SeriesPoint[] Points2 => _lineSeries2?.Points;

        public void Clear()
        {
            UpdateChart(() =>
            {
                _lineSeries1.Points = Array.Empty<SeriesPoint>();
                _lineSeries2.Points = Array.Empty<SeriesPoint>();

                IsCrossProfile = false;
                UpdateVisibilities();
                UpdateView();
            });
        }

        public void ResetChart(SeriesPoint[] srcPoints)
        {
            UpdateChart(() =>
            {
                _lineSeries1.Points = srcPoints;
                _lineSeries2.Points = Array.Empty<SeriesPoint>();

                IsCrossProfile = false;
                UpdateVisibilities();
                UpdateView();
            });
        }

        public void ResetChart(SeriesPoint[] srcPoints, SeriesPoint[] srcPoints2)
        {
            UpdateChart(() =>
            {
                _lineSeries1.Points = srcPoints;
                _lineSeries2.Points = srcPoints2;

                IsCrossProfile = true;
                UpdateVisibilities();
                UpdateView();
            });
        }

        private void UpdateView()
        {
            Chart.ViewXY.ZoomToFit();

            MoveMarkerToIndex(_marker, MarkerIndex);
            MoveMarkerToIndex(_horizontalMarker, HorizontalMarkerIndex);
            MoveMarkerToIndex(_verticalMarker, VerticalMarkerIndex);
        }

        private void MoveMarkerToIndex(SeriesEventMarker marker, int markerIndex)
        {
            if (marker == null) return;

            var series = (PointLineSeries)marker.GetOwnerSeries();

            if (markerIndex > -1)
            {
                _preventPositionChanged = true;

                if (markerIndex < series.Points.Length)
                {
                    var currentPoint = series.Points[markerIndex];
                    marker.XValue = currentPoint.X;
                    marker.YValue = currentPoint.Y;
                }
                else if (series.Points.Any())
                {
                    marker.XValue = series.Points.Last().X;
                    marker.YValue = series.Points.Last().Y;
                }
                else
                {
                    marker.XValue = 0;
                    marker.YValue = 0;
                }

                _preventPositionChanged = false;
            }

            OnMarkerPositionChanged(marker, null);
        }

        public void UpdateYAxisTitle(string yAxis)
        {
            UpdateChart(() =>
            {
                _yAxis.Title.Text = yAxis;
            });
        }
    }
}
