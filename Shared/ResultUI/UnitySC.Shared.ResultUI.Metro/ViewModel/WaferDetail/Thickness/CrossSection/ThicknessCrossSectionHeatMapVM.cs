using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.EventMarkers;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness.CrossSection
{
    public class ThicknessCrossSectionHeatMapVM : MetroHeatMapVM
    {
        private ThicknessResultSettings Settings => MeasureData is ThicknessResult result ? result.Settings : null;

        private readonly SeriesEventMarker _startMarker;
        private readonly SeriesEventMarker _endMarker;
        private readonly SeriesEventMarker _trackerMarker;
        private readonly LineCollection _manualLineCollection;

        private readonly LineCollection _radialLineCollection;
        private readonly LineSeriesCursor _verticalLineSeriesCursor;
        private readonly ConstantLine _horizontalConstantLine;

        public event EventHandler<ProfileChangedEventArgs> ProfileChanged;
        public event EventHandler<Point> RadialDragging;

        private ThicknessCrossSectionMode _currentMode;

        private double _radius = 100;

        #region Properties

        public double VerticalProfileValue
        {
            get { return _verticalLineSeriesCursor.ValueAtXAxis; }
            set
            {
                _verticalLineSeriesCursor.ValueAtXAxis = value;
                OnPropertyChanged();
            }
        }

        public double HorizontalProfileValue
        {
            get { return _horizontalConstantLine.Value; }
            set
            {
                _horizontalConstantLine.Value = value;
                OnPropertyChanged();
            }
        }

        private double _radialeProfileAngle;

        public double RadialeProfileAngle
        {
            get { return _radialeProfileAngle; }
            set
            {
                SetProperty(ref _radialeProfileAngle, value);
                OnRadialProfilChanged();
            }
        }

        #region Manual Profile

        public double StartManualX
        {
            get { return _startMarker.XValue; }
            set
            {
                _startMarker.XValue = value;
                OnPropertyChanged();
            }
        }

        public double StartManualY
        {
            get { return _startMarker.YValue; }
            set
            {
                _startMarker.YValue = value;
                OnPropertyChanged();
            }
        }

        public double EndManualX
        {
            get { return _endMarker.XValue; }
            set
            {
                _endMarker.XValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs(null));
            }
        }

        public double EndManualY
        {
            get { return _endMarker.YValue; }
            set
            {
                _endMarker.YValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs(null));
            }
        }

        #endregion

        #region Tracker Position

        public double TrackerX => _trackerMarker.Visible ? _trackerMarker.XValue : double.NaN;

        public double TrackerY => _trackerMarker.Visible ? _trackerMarker.YValue : double.NaN;

        #endregion

        #endregion

        public ThicknessCrossSectionHeatMapVM(PointSelectorBase pointSelector, int heatmapside) : base(pointSelector, heatmapside)
        {
            var viewXy = Chart.ViewXY;

            #region Marker

            var markerSeries = new PointLineSeries(viewXy, viewXy.XAxes[0], viewXy.YAxes[0]) { ShowInLegendBox = false };
            viewXy.PointLineSeries.Add(markerSeries);

            _startMarker = new SeriesEventMarker(markerSeries)
            {
                HorizontalPosition = SeriesEventMarkerHorizontalPosition.AtXValue,
                VerticalPosition = SeriesEventMarkerVerticalPosition.AtYValue,
                XValue = -50,
                YValue = -50,
                Symbol = new SeriesMarkerPointShapeStyle
                {
                    Color1 = Colors.Transparent,
                    Color2 = Colors.Transparent,
                    Color3 = Colors.Transparent,
                    Shape = SeriesMarkerPointShape.Circle,
                    Width = 16,
                    Height = 16,
                    BorderWidth = 2,
                    BorderColor = Colors.Magenta
                },
                Label = { Visible = false },
                AllowDragging = true
            };
            markerSeries.SeriesEventMarkers.Add(_startMarker);
            _startMarker.PositionChanged += OnManualMarkerMoved;

            _endMarker = new SeriesEventMarker(markerSeries)
            {
                HorizontalPosition = SeriesEventMarkerHorizontalPosition.AtXValue,
                VerticalPosition = SeriesEventMarkerVerticalPosition.AtYValue,
                XValue = 50,
                YValue = 50,
                Symbol = new SeriesMarkerPointShapeStyle
                {
                    Color1 = Colors.Transparent,
                    Color2 = Colors.Transparent,
                    Color3 = Colors.Transparent,
                    Shape = SeriesMarkerPointShape.Circle,
                    Width = 16,
                    Height = 16,
                    BorderWidth = 2,
                    BorderColor = Colors.DeepSkyBlue
                },
                Label = { Visible = false },
                AllowDragging = true
            };
            markerSeries.SeriesEventMarkers.Add(_endMarker);
            _endMarker.PositionChanged += OnManualMarkerMoved;

            _trackerMarker = new SeriesEventMarker(markerSeries)
            {
                HorizontalPosition = SeriesEventMarkerHorizontalPosition.AtXValue,
                VerticalPosition = SeriesEventMarkerVerticalPosition.AtYValue,
                XValue = 50,
                YValue = 50,
                Symbol = new SeriesMarkerPointShapeStyle
                {
                    Color1 = Colors.Red,
                    Color2 = Colors.Red,
                    Color3 = Colors.Red,
                    Shape = SeriesMarkerPointShape.Circle,
                    Width = 9,
                    Height = 9,
                    BorderWidth = 1,
                    BorderColor = Colors.Black
                },
                Label = { Visible = false },
                AllowUserInteraction = false,
            };
            markerSeries.SeriesEventMarkers.Add(_trackerMarker);

            _manualLineCollection = new LineCollection(viewXy, viewXy.XAxes[0], viewXy.YAxes[0])
            {
                AllowUserInteraction = false,
                ShowInLegendBox = false,
                LineStyle = new LineStyle { Color = Colors.Red, AntiAliasing = LineAntialias.Normal, Width = 2 }
            };

            viewXy.LineCollections.Add(_manualLineCollection);

            #endregion

            #region Profile Line

            // Radial Profile Line
            _radialLineCollection = new LineCollection(viewXy, viewXy.XAxes[0], viewXy.YAxes[0])
            {
                AllowUserInteraction = true,
                ShowInLegendBox = false,
                LineStyle = new LineStyle { Color = Colors.Red, AntiAliasing = LineAntialias.Normal, Width = 2 },
                Highlight = Highlight.Simple
            };
            _radialLineCollection.MouseDown += OnRadialLineCollectionMouseDown;
            viewXy.LineCollections.Add(_radialLineCollection);

            // Vertical Profile Line
            _verticalLineSeriesCursor = new LineSeriesCursor(viewXy, viewXy.XAxes[0])
            {
                AllowDragging = true,
                Highlight = Highlight.Simple,
                LineStyle = new LineStyle { Color = Colors.Red, AntiAliasing = LineAntialias.Normal, Width = 2 }
            };
            _verticalLineSeriesCursor.PositionChanged += OnVerticalLineMoved;
            viewXy.LineSeriesCursors.Add(_verticalLineSeriesCursor);

            // Horizontal Profile Line
            _horizontalConstantLine = new ConstantLine(viewXy, viewXy.XAxes[0], viewXy.YAxes[0])
            {
                ShowInLegendBox = false,
                LineStyle = new LineStyle { Color = Colors.Red, AntiAliasing = LineAntialias.Normal, Width = 2 },
                Highlight = Highlight.Simple,
                AllowMoveByUser = true,
            };
            _horizontalConstantLine.ValueChanged += OnHorizontalLineMoved;
            viewXy.ConstantLines.Add(_horizontalConstantLine);

            #endregion

            HeatMap.Title.Text = "Thickness";
            HeatMap.LegendBoxUnits = Length.GetUnitSymbol(LengthUnit.Micrometer);
        }

        #region Event Handlers

        #region Rotate Radial Line By Mouse

        private void OnRadialLineCollectionMouseDown(object sender, MouseEventArgs e)
        {
            // Screen coordinates from chart center
            float yPos = Chart.ViewXY.YAxes[0].ValueToCoord(0);
            float xPos = Chart.ViewXY.XAxes[0].ValueToCoord(0);

            RadialDragging?.Invoke(this, new Point(xPos, yPos));
        }

        #endregion

        #region Manual Profile Changed

        private void OnManualMarkerMoved(object sender = null, PositionChangedSeriesEventMarkerEventArgs e = null)
        {
            OnPropertyChanged(nameof(StartManualX));
            OnPropertyChanged(nameof(StartManualY));
            OnPropertyChanged(nameof(EndManualX));
            OnPropertyChanged(nameof(EndManualY));

            _manualLineCollection.Clear();
            _manualLineCollection.Lines = new[]
            {
                new SegmentLine(_startMarker.XValue, _startMarker.YValue, _endMarker.XValue, _endMarker.YValue)
            };

            ProfileChanged?.Invoke(this, new ProfileChangedEventArgs
            {
                StartX = StartManualX,
                StartY = StartManualY,
                EndX = EndManualX,
                EndY = EndManualY
            });
        }

        #endregion

        #region Vertical Profile Changed

        private void OnVerticalLineMoved(object sender = null, PositionChangedEventArgs e = null)
        {
            OnPropertyChanged(nameof(VerticalProfileValue));

            ProfileChanged?.Invoke(this, new ProfileChangedEventArgs
            {
                StartX = VerticalProfileValue,
                StartY = -_radius,
                EndX = VerticalProfileValue,
                EndY = _radius
            });
        }

        #endregion

        #region Horizontal Profile Changed

        private void OnHorizontalLineMoved(object sender = null, ValueChangedEventArgs e = null)
        {
            OnPropertyChanged(nameof(HorizontalProfileValue));

            ProfileChanged?.Invoke(this, new ProfileChangedEventArgs
            {
                StartX = -_radius,
                StartY = HorizontalProfileValue,
                EndX = _radius,
                EndY = HorizontalProfileValue
            });
        }

        #endregion

        private void OnRadialProfilChanged()
        {
            double x = _radius * Math.Sin(Math.PI * 2 * _radialeProfileAngle / 360);
            double y = _radius * Math.Cos(Math.PI * 2 * _radialeProfileAngle / 360);

            _radialLineCollection.Clear();
            _radialLineCollection.Lines = new[]
            {
                new SegmentLine(x, y, -x, -y)
            };

            ProfileChanged?.Invoke(this, new ProfileChangedEventArgs
            {
                StartX = x,
                StartY = y,
                EndX = -x,
                EndY = -y
            });
        }

        #endregion

        #region Overrides of ObservableObject

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(MeasureData))
            {
                double diameter = MetroHelper.GetWaferDiameterMillimeters(MeasureData);
                _radius = diameter / 2;

                // Force recalculation of profile.
                RaiseProfileChanged();
            }
        }

        #endregion

        #region Overrides of MetroHeatMapVM<ThicknessPointResult>

        public void UpdateAndDraw(MeasureResultBase measureResultBase)
        {
            Update(measureResultBase);

            var settings = Settings;
            if (settings == null) return;

            double? target = settings.TotalTarget?.Micrometers;
            double? tolerance = settings.TotalTolerance?.GetAbsoluteTolerance(settings.TotalTarget).Micrometers;

            SetPaletteType(HeatMapPaletteType.TargetTolerance);
            SetTargetTolerance(target, tolerance);
        }

        #endregion

        public void SetProfileMode(ThicknessCrossSectionMode mode)
        {
            _currentMode = mode;

            _verticalLineSeriesCursor.Visible = false;
            _radialLineCollection.Visible = false;
            _horizontalConstantLine.Visible = false;

            _startMarker.Visible = false;
            _endMarker.Visible = false;
            _manualLineCollection.Visible = false;

            switch (mode)
            {
                case ThicknessCrossSectionMode.Horizontal:
                    _horizontalConstantLine.Visible = true;
                    OnHorizontalLineMoved();
                    break;
                case ThicknessCrossSectionMode.Vertical:
                    _verticalLineSeriesCursor.Visible = true;
                    OnVerticalLineMoved();
                    break;
                case ThicknessCrossSectionMode.Radial:
                    _radialLineCollection.Visible = true;
                    OnRadialProfilChanged();
                    break;
                case ThicknessCrossSectionMode.Manual:
                    _startMarker.Visible = true;
                    _endMarker.Visible = true;
                    _manualLineCollection.Visible = true;
                    OnManualMarkerMoved();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public void RaiseProfileChanged()
        {
            SetProfileMode(_currentMode);
        }

        public void SetTrackerPosition(double x, double y, bool isVisible)
        {
            _trackerMarker.XValue = x;
            _trackerMarker.YValue = y;
            _trackerMarker.Visible = isVisible;

            OnPropertyChanged(nameof(TrackerX));
            OnPropertyChanged(nameof(TrackerY));
        }

        #region Overrides of MetroHeatMapVM

        public override void Dispose()
        {
            _startMarker.PositionChanged -= OnManualMarkerMoved;
            _endMarker.PositionChanged -= OnManualMarkerMoved;
            _radialLineCollection.MouseDown -= OnRadialLineCollectionMouseDown;
            _verticalLineSeriesCursor.PositionChanged -= OnVerticalLineMoved;
            _horizontalConstantLine.ValueChanged -= OnHorizontalLineMoved;

            base.Dispose();
        }

        #endregion
    }
}
