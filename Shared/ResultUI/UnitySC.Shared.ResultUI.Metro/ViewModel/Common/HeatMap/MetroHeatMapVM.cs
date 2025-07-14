using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.EventMarkers;
using LightningChartLib.WPF.Charting.Maps;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap
{
    public class MetroHeatMapVM : BaseHeatMapChartVM
    {
        #region Fields
        
        private readonly PointSelectorBase _pointSelector;

        private readonly Dictionary<SeriesEventMarker, MeasurePointResult> _markerToMeasurePoint = new Dictionary<SeriesEventMarker, MeasurePointResult>();
        
        #endregion

        #region Properties

        #region Die Grid

        private LineCollection LineCollection { get; set; }

        public bool IsDieGridVisible
        {
            get
            {
                return LineCollection != null && LineCollection.Visible;
            }
            set
            {
                LineCollection.Visible = value;
                OnPropertyChanged();
            }
        }

        private bool _canChangeDieGridVisible;

        public bool CanChangeDieGridVisible
        {
            get { return _canChangeDieGridVisible; }
            private set { SetProperty(ref _canChangeDieGridVisible, value); }
        }

        #endregion

        private MeasureResultBase _measureData;

        public MeasureResultBase MeasureData
        {
            get => _measureData;
            private set => SetProperty(ref _measureData, value);
        }

        #endregion

        #region Constructor

        public MetroHeatMapVM(PointSelectorBase pointSelector, int heatmapside) : base(heatmapside)
        {
            _pointSelector = pointSelector;

            _pointSelector.SelectedPointChanged += PointSelectorOnSelectedPointChanged;
            _pointSelector.CheckedPointsChanged += PointSelectorCheckedPointsChanged;
        }

        #endregion Constructor

        #region Overrides of BaseHeatMapChartVM

        protected override void CustomizeChart(ViewXY view, AxisX xAxis, AxisY yAxis)
        {
            LineCollection = new LineCollection(view, xAxis, yAxis)
            {
                Highlight = Highlight.None,
                AllowUserInteraction = false,
                Title = new SeriesTitle
                {
                    Text = "Die Grid"
                },
                ShowInLegendBox = false,
                Behind = false,
                LineStyle = new LineStyle
                {
                    Color = Colors.Black,
                    Pattern = LinePattern.Solid,
                    Width = 1
                }
            };
            view.LineCollections.Add(LineCollection);
        }

        protected override void OnMarkerClicked(object sender, MouseEventArgs e)
        {
            if (!(sender is SeriesEventMarker marker)) return;
            _pointSelector.SetSelectedPoint(this, _markerToMeasurePoint[marker]);
        }

        protected override void ClearPointMarkers()
        {
            base.ClearPointMarkers();
            _markerToMeasurePoint.Clear();
        }

        #endregion

        #region Event Handlers

        private void PointSelectorOnSelectedPointChanged(object sender, EventArgs e)
        {
            var selectedPoint = _pointSelector.SingleSelectedPoint;
            SetSelectedPoint(selectedPoint?.WaferRelativeXPosition, selectedPoint?.WaferRelativeYPosition);
        }

        private void PointSelectorCheckedPointsChanged(object sender, EventArgs e)
        {
            UpdateChart(DrawPointMarkers);
        }

        #endregion
        
        #region Private Methods

        private void DrawPointMarkers()
        {
            ClearPointMarkers();

            var pointsToDisplay = _pointSelector.CheckedPoints;
            foreach (var measurePointResult in pointsToDisplay.OrderBy(result => result.State))
            {
                var marker = DrawPointMarker(measurePointResult.WaferRelativeXPosition, measurePointResult.WaferRelativeYPosition, MetroHelper.GetSymbol(measurePointResult.State));
                _markerToMeasurePoint.Add(marker, measurePointResult);
            }

            PointSeries.InvalidateData();
        }

        private void DrawDieGrid()
        {
            LineCollection.Lines = Array.Empty<SegmentLine>();

            var dieMap = MeasureData?.DiesMap;
            if (dieMap == null)
            {
                CanChangeDieGridVisible = false;
                return;
            }

            var lines = new List<SegmentLine>();

            double leftMargin = dieMap.DieGridTopLeftXPosition.Millimeters;
            double topMargin = dieMap.DieGridTopLeftYPosition.Millimeters;
            double pitchWidth = dieMap.DiePitchWidth.Millimeters;
            double pitchHeight = dieMap.DiePitchHeight.Millimeters;
            double waferRadius = MeasureData.Wafer.Diameter.Millimeters / 2;

            double dieWidth = dieMap.DieSizeWidth.Millimeters;
            double dieHeight = dieMap.DieSizeHeight.Millimeters;

            for (int index = 0; index < dieMap.DiesPresence.Count; index++)
            {
                var diesPresence = dieMap.DiesPresence[index];

                for (int i = 0; i < diesPresence.Dies.Length; i++)
                {
                    char die = diesPresence.Dies[i];

                    if (die != '1') continue;

                    double leftX = leftMargin + pitchWidth * i;
                    double rightX = leftX + dieWidth;
                    double topY = topMargin - pitchHeight * index ;
                    double bottomY = topY - dieHeight;

                    // Die Top
                    lines.Add(new SegmentLine(leftX, topY, rightX, topY));
                    // Die Bottom
                    lines.Add(new SegmentLine(leftX, bottomY, rightX, bottomY));
                    // Die Left
                    lines.Add(new SegmentLine(leftX, topY, leftX, bottomY));
                    // Die Right
                    lines.Add(new SegmentLine(rightX, topY, rightX, bottomY));
                }
            }

            LineCollection.Lines = lines.ToArray();
            CanChangeDieGridVisible = lines.Count > 0;
        }
        
        private void ApplyInterpolationResults(Dictionary<IntPoint, PointInterpolationResult> interpolationResults)
        {
            if (interpolationResults == null) return;

            foreach (var result in interpolationResults)
            {
                HeatMap.Data[result.Key.X, result.Key.Y].X = result.Value.X;
                HeatMap.Data[result.Key.X, result.Key.Y].Y = result.Value.Y;
                HeatMap.Data[result.Key.X, result.Key.Y].Value = result.Value.Value;
            }

            // Notify chart about updated data.
            HeatMap.InvalidateValuesDataOnly();
        }

        private void FilterWaferShape(double waferRadius)
        {
            int circlePointCount = 2 * HeatMapSide;
            double angleStep = -Math.PI * 2.0 / circlePointCount;
            var circlePoints = new PointDouble2D[circlePointCount];

            for (int i = 0; i < circlePointCount; ++i)
            {
                circlePoints[i].X = waferRadius * Math.Cos(i * angleStep);
                circlePoints[i].Y = waferRadius * Math.Sin(i * angleStep);
            }

            var circle = new PolygonSeries(Chart.ViewXY, Chart.ViewXY.XAxes[0], Chart.ViewXY.YAxes[0])
            {
                ShowInLegendBox = false,
                AllowUserInteraction = false,
                Points = circlePoints,
                Fill =
                {
                    Style = RectFillStyle.None
                },
                Border =
                {
                    Color = Color.FromArgb(0xff, 0x29, 0x3D, 0x29),
                    Width = 2
                }
            };

            Chart.ViewXY.PolygonSeries.Clear();
            Chart.ViewXY.PolygonSeries.Add(circle);

            // Filter HeatMap with wafer shape
            var stencilArea = new StencilArea(HeatMap.Stencil);
            stencilArea.AddPolygon(circlePoints);
            HeatMap.Stencil.AdditiveAreas.Clear();
            HeatMap.Stencil.AdditiveAreas.Add(stencilArea);
        }

        #endregion

        #region Public Methods

        public void Update(MeasureResultBase result)
        {
            MeasureData = result;
        }

        public void SetInterpolationResult(Dictionary<IntPoint, PointInterpolationResult> interpolationResults)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(MeasureData);
                double waferRadius = waferDiameter / 2.0;

                UpdateChart(() =>
                {
                    HeatMap.SetRangesXY(-waferRadius, waferRadius, -waferRadius, waferRadius);

                    FilterWaferShape(waferRadius);
                    DrawDieGrid();

                    UpdatePalette();

                    ApplyInterpolationResults(interpolationResults);

                    Chart.ViewXY.ZoomToFit();
                });

                IsBusy = false;
            });
        }

        #endregion

        #region Overrides of BaseHeatMapChartVM

        public override void Dispose()
        {
            _pointSelector.SelectedPointChanged -= PointSelectorOnSelectedPointChanged;
            _pointSelector.CheckedPointsChanged -= PointSelectorCheckedPointsChanged;

            base.Dispose();
        }

        #endregion
    }
}
