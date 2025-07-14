using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.EventMarkers;
using LightningChartLib.WPF.Charting.Maps;
using LightningChartLib.WPF.Charting.SeriesXY;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap
{
    public abstract class MetroDieMapVM<T> : BaseDieMapVM where T : MeasurePointResult
    {
        #region Fields

        private readonly PointSelectorBase _pointSelector;

        private MeasureDieResult _currentDie;

        private double _dieWidth;

        private double _dieHeight;

        private readonly Dictionary<SeriesEventMarker, MeasurePointResult> _markerToMeasurePoint = new Dictionary<SeriesEventMarker, MeasurePointResult>();

        private readonly List<T> _displayedPoints = new List<T>();

        private InterpolationEngine<T> _interpolationEngine;

        #endregion

        protected MetroDieMapVM(PointSelectorBase pointSelector, int heatmapside) : base(heatmapside)
        {
            _pointSelector = pointSelector;
            _pointSelector.SelectedPointChanged += PointSelectorSelectedPointChanged;
            _pointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;
        }

        #region Event Handlers

        private void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            UpdateDiePoints();
        }

        private void PointSelectorSelectedPointChanged(object sender, EventArgs e)
        {
            var selectedPoint = _pointSelector.SingleSelectedPoint;

            SetSelectedPoint(selectedPoint?.XPosition, selectedPoint?.YPosition);

            if (selectedPoint == null)
            {
                Clear();
                return;
            }

            var selectedDie = _pointSelector.Dies.SingleOrDefault(die => die.Points.Contains(selectedPoint));
            if (selectedDie == null)
            {
                Clear();
                return;
            }

            if (_currentDie != selectedDie)
            {
                _currentDie = selectedDie;

                UpdateDiePoints();
                ResetInterpolatorEngine();
                InterpolateData();
            }
        }

        #endregion

        #region Private Methods

        private void Clear()
        {
            _currentDie = null;

            ClearPointMarkers();

            // Clear HeatMap
            var viewData = HeatMap.Data;
            for (int i = 0; i < HeatMapSide; ++i)
            {
                for (int j = 0; j < HeatMapSide; ++j)
                {
                    viewData[i, j].Value = double.NaN;
                }
            }
            HeatMap.InvalidateData();

            UpdatePalette();
        }

        private void UpdateDiePoints()
        {
            _displayedPoints.Clear();

            if (_currentDie != null)
            {
                _displayedPoints.AddRange(_currentDie.Points.OfType<T>().Where(point => _pointSelector.CheckedPoints.Contains(point)));
            }

            UpdateChart(DrawPointMarkers);
        }

        private void FilterDieShape()
        {
            var shape = new[]
            {
                new PointDouble2D(0, 0),
                new PointDouble2D(_dieWidth, 0),
                new PointDouble2D(_dieWidth, _dieHeight),
                new PointDouble2D(0, _dieHeight),
            };

            var border = new PolygonSeries(Chart.ViewXY, Chart.ViewXY.XAxes[0], Chart.ViewXY.YAxes[0])
            {
                ShowInLegendBox = false,
                AllowUserInteraction = false,
                Points = shape,
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
            Chart.ViewXY.PolygonSeries.Add(border);

            var stencilArea = new StencilArea(HeatMap.Stencil);
            stencilArea.AddPolygon(shape);
            HeatMap.Stencil.AdditiveAreas.Clear();
        }

        #region Markers

        private void DrawPointMarkers()
        {
            ClearPointMarkers();

            if (_currentDie == null) return;

            foreach (var point in _displayedPoints.OrderBy(result => result.State))
            {
                var marker = DrawPointMarker(point.XPosition, point.YPosition, MetroHelper.GetSymbol(point.State));
                _markerToMeasurePoint.Add(marker, point);
            }

            PointSeries.InvalidateData();
        }

        protected override void OnMarkerClicked(object sender, MouseEventArgs e)
        {
            if (!(sender is SeriesEventMarker marker)) return;

            _pointSelector.SetSelectedPoint(this, _markerToMeasurePoint[marker]);
        }

        #endregion

        #endregion

        #region Overrides of BaseHeatMapChartVM

        protected override void ClearPointMarkers()
        {
            base.ClearPointMarkers();
            _markerToMeasurePoint.Clear();
        }

        #endregion

        #region Interpolation
        
        public void SetInterpolationEngine(InterpolationEngine<T> dieInterpolationEngine)
        {
            var dieSizemm = GetMillimeterDieSize();
            _dieHeight = dieSizemm.Height;
            _dieWidth = dieSizemm.Width;

            if (_interpolationEngine != null)
            {
                _interpolationEngine.InterpolationDone -= OnInterpolationDone;
                _interpolationEngine.Cancel();
                _interpolationEngine = null;
            }

            _interpolationEngine = dieInterpolationEngine;

            if (_interpolationEngine != null)
            {
                _interpolationEngine.InterpolationDone += OnInterpolationDone;
                InterpolateData();
            }
        }

        private void ResetInterpolatorEngine()
        {
            _interpolationEngine?.ResetState();
        }

        private void InterpolateData()
        {
            if (_currentDie == null) return;
            _interpolationEngine?.InterpolateSquare(_currentDie.Points.OfType<T>().ToList(), _dieHeight, _dieWidth);
        }
        
        protected abstract Size GetMillimeterDieSize();

        private void OnInterpolationDone(object sender, EventArgs e)
        {
            Application.Current?.Dispatcher?.BeginInvoke((Action)(() =>
            {
                SetMinMax(_interpolationEngine.CurrentMinValue, _interpolationEngine.CurrentMaxValue);
                SetInterpolationResult(_interpolationEngine.InterpolationResults);
            }));
        }
        
        private void SetInterpolationResult(Dictionary<IntPoint, PointInterpolationResult> interpolationResults)
        {
            UpdateChart(() =>
            {
                HeatMap.SetRangesXY(0, _dieWidth, 0, _dieHeight);

                FilterDieShape();
                UpdatePalette();

                int Xmax = HeatMap.Data.GetLength(0);
                int Ymax = HeatMap.Data.GetLength(1);
                foreach (var result in interpolationResults)
                {
                    if (result.Key.X >= Xmax || result.Key.Y >= Ymax) continue; // quick fix to avoid index exeeced (real issue is to find a way to handle interpolator of wafer and diesquare)

                    HeatMap.Data[result.Key.X, result.Key.Y].X = result.Value.X;
                    HeatMap.Data[result.Key.X, result.Key.Y].Y = result.Value.Y;
                    HeatMap.Data[result.Key.X, result.Key.Y].Value = result.Value.Value;

                    if (24 <= result.Value.X && result.Value.X <= 25 &&
                       24 <= result.Value.Y && result.Value.Y <= 25)
                    {
                        Debug.WriteLine($"\n------------------------------");
                       Debug.WriteLine($"interp XY [{result.Value.X},{result.Value.Y}] = {result.Value.Value}");
                       Debug.WriteLine($"##############################\n");
                    }
                }

                // Notify chart about updated data.
                HeatMap.InvalidateValuesDataOnly();
                HeatMap.InvalidateData();

                Chart.ViewXY.ZoomToFit();
            });

            IsBusy = false;
        }
        
        #endregion

        #region Overrides of BaseHeatMapChartVM

        public override void Dispose()
        {
            _pointSelector.SelectedPointChanged -= PointSelectorSelectedPointChanged;
            _pointSelector.CheckedPointsChanged -= PointSelectorOnCheckedPointsChanged;

            base.Dispose();
        }

        #endregion
    }
}
