using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts.StackedArea;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness.CrossSection;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.Helper;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness
{
    public enum ThicknessCrossSectionMode
    {
        Horizontal,
        Vertical,
        Radial,
        Manual
    }

    public class StackedData
    {
        public int Index { get; set; }
        
        public Length Length { get; set; }

        public Length Target { get; set; }
    }

    public class ThicknessCrossSectionVM : ObservableObject, IDisposable
    {
        #region Fields

        private Dictionary<string, InterpolationEngine<ThicknessPointResult>> _interpolationData;

        private readonly Dictionary<Point, Dictionary<string, StackedData>> _crossSectionData = new Dictionary<Point, Dictionary<string, StackedData>>();

        private readonly Dictionary<string, Length> _layerTargets = new Dictionary<string, Length>();

        private ThicknessResult _thicknessResult;

        private readonly Func<string, Color> _getLayerColor;

        private LengthUnit _totalLayerUnit = LengthUnit.Micrometer;

        private List<Point> _bresenhamLine;

        private readonly List<LayerDetailInfoVM> _layersDefails = new List<LayerDetailInfoVM>();

        #endregion

        public Func<double, double> GetChartWidthFunc { get; } = height => height / 2;

        public Func<double, double, double, int> GetChartColumnSpanFunc { get; } = (totalHeight, toolsHeight, expanderHeight) => totalHeight - toolsHeight > expanderHeight ? 2 : 1;

        private int _digits = 8;

        public int Digits
        {
            get => _digits;
            set
            {
                SetProperty(ref _digits, value);
                OnPropertyChanged(nameof(Total));
                // TODO Verify this
                foreach (var layerInfo in ThicknessLayersInfo.LayersDetails)
                {
                    layerInfo.UpdateDigits(value);
                }
            }
        }

        private double _totalValue;

        public string Total => LengthToStringConverter.ConvertToString(_totalValue, Digits, true, "-", _totalLayerUnit);

        public ThicknessLayersInfoVM ThicknessLayersInfo { get; } = new ThicknessLayersInfoVM();

        private ThicknessCrossSectionMode _currentMode;

        public ThicknessCrossSectionMode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                SetProperty(ref _currentMode, value);
                CrossSectionHeatMap.SetProfileMode(value);
            }
        }

        private bool _normalizedModeEnabled;

        public bool NormalizedModeEnabled
        {
            get { return _normalizedModeEnabled; }
            set
            {
                SetProperty(ref _normalizedModeEnabled, value);
                CrossSectionHeatMap.RaiseProfileChanged();
            }
        }

        private Point? _radialDraggingFlag;

        /// <summary>
        /// This property is set when the radial profile line is pressed by the user.
        /// It then notifies the view to start an angle calculation on cursor movement.
        /// </summary>
        public Point? RadialDraggingFlag
        {
            get { return _radialDraggingFlag; }
            set { SetProperty(ref _radialDraggingFlag, value); }
        }

        public ThicknessCrossSectionHeatMapVM CrossSectionHeatMap { get; }

        public StackedAreaChart StackedAreaChart { get; }

        public ObservableCollection<string> LayerMode { get; } = new ObservableCollection<string>();

        private string _currentLayer;

        public string CurrentLayer
        {
            get { return _currentLayer; }
            set
            {
                if (SetProperty(ref _currentLayer, value))
                {
                    OnCurrentLayerChanged();
                }
            }
        }
        
        public ThicknessCrossSectionVM(ThicknessPointSelector pointSelector, int heatmapside, Func<string, Color> getLayerColor)
        {
            _getLayerColor = getLayerColor;

            CrossSectionHeatMap = new ThicknessCrossSectionHeatMapVM(pointSelector, heatmapside);
            CrossSectionHeatMap.ProfileChanged += OnCrossSectionHeatMapProfileChanged;
            CrossSectionHeatMap.RadialDragging += OnRadialDraggingRequested;

            StackedAreaChart = new StackedAreaChart("Cross-Section", "Thickness");
            StackedAreaChart.TrackerPositionChanged += OnTrackerPositionPositionChanged;

            CurrentMode = ThicknessCrossSectionMode.Horizontal;
        }

        #region Event Handlers

        private void OnCrossSectionHeatMapProfileChanged(object sender, ProfileChangedEventArgs args)
        {
            if (_thicknessResult == null || _interpolationData.IsNullOrEmpty()) return;

            lock (_crossSectionData)
            {
                _crossSectionData.Clear();

                // Convert args coords to wafer interpolated coords
                double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(_thicknessResult);
                double waferRadius = waferDiameter / 2;
                // we assume here that all Interpolation engine have the same heatmapSize
                double heatMapToWaferRatio = waferDiameter / _interpolationData.Last().Value.HeatMapSide;

                int startX = (int)((args.StartX + waferRadius) / heatMapToWaferRatio);
                int startY = (int)((args.StartY + waferRadius) / heatMapToWaferRatio);
                int endX = (int)((args.EndX + waferRadius) / heatMapToWaferRatio);
                int endY = (int)((args.EndY + waferRadius) / heatMapToWaferRatio);

                _bresenhamLine = MetroHelper.InterBresenhamLine(startX, startY, endX, endY);

                CalculateCrossSectionData();
                DrawChart();
            }
        }

        private void OnRadialDraggingRequested(object sender, Point point)
        {
            RadialDraggingFlag = point;
        }

        private void OnTrackerPositionPositionChanged(object sender, double e)
        {
            if (_thicknessResult == null) return;

            // Convert args coords to wafer interpolated coords
            double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(_thicknessResult);
            double waferRadius = waferDiameter / 2;
            // we assume here that all Interpolation engine have the same heatmapSize
            double heatMapToWaferRatio = waferDiameter / _interpolationData.Last().Value.HeatMapSide;

            Length waferThickness = null;

            int trackerPosition = (int)StackedAreaChart.TrackerPosition;
            if (trackerPosition >= 0 && trackerPosition < _crossSectionData.Count)
            {
                var data = _crossSectionData.ElementAt(trackerPosition);

                foreach (var layer in _layersDefails)
                {
                    var layerDataLength = data.Value.TryGetValue(layer.LayerName, out var layerData) ? layerData.Length : null;
                    layer.SetValue(layerDataLength);
                }

                waferThickness = data.Value.TryGetValue(ThicknessResultVM.WaferThickness, out var thicknessData) ? thicknessData.Length : null;

                double waferX = (data.Key.X + 0.5) * heatMapToWaferRatio - waferRadius;
                double waferY = (data.Key.Y + 0.5) * heatMapToWaferRatio - waferRadius;

                CrossSectionHeatMap.SetTrackerPosition(waferX, waferY, true);
            }
            else
            {
                foreach (var layer in _layersDefails)
                {
                    layer.SetValue(null);
                }

                CrossSectionHeatMap.SetTrackerPosition(0, 0, false);
            }

            _totalValue = _layersDefails.Sum(vm => vm.LengthValue?.GetValueAs(_totalLayerUnit) ?? 0);
            OnPropertyChanged(nameof(Total));

            if (CurrentLayer == ThicknessResultVM.TotalLayerName)
            {
                // Update the Total value & deviation at realtime
                string stringTotalValue = LengthToStringConverter.ConvertToString(_totalValue, Digits, true, "-", _totalLayerUnit);
                double targetAsTotalUnit = _thicknessResult?.Settings?.TotalTarget != null ? _thicknessResult.Settings.TotalTarget.GetValueAs(_totalLayerUnit) : 0.0;
                string stringTotalDelta = LengthToStringConverter.ConvertToString(_totalValue - targetAsTotalUnit, Digits, true, "-", _totalLayerUnit);
                ThicknessLayersInfo.Value = $"Value = {Environment.NewLine}{stringTotalValue}{Environment.NewLine}{Environment.NewLine}Deviation = {Environment.NewLine}{stringTotalDelta}";

                // Wafer Thickness
                if (_thicknessResult?.Settings?.HasWaferThicknesss == true)
                {
                    string stringWaferThickness = LengthToStringConverter.ConvertToString(waferThickness, Digits, true, "-", _totalLayerUnit);
                    var waferThicknessDelta = waferThickness == null || _thicknessResult?.Settings?.TotalTarget == null ? null : waferThickness - _thicknessResult.Settings.TotalTarget;
                    string stringWaferThicknessDelta = LengthToStringConverter.ConvertToString(waferThicknessDelta, Digits, true, "-", _totalLayerUnit);
                    ThicknessLayersInfo.WaferThickness = $"Value = {Environment.NewLine}{stringWaferThickness}{Environment.NewLine}{Environment.NewLine}Deviation = {Environment.NewLine}{stringWaferThicknessDelta}";
                }
                else
                {
                    ThicknessLayersInfo.WaferThickness = string.Empty;
                }
            }
        }

        #endregion
        
        public void Update(ThicknessResult thicknessResult)
        {
            CrossSectionHeatMap.UpdateAndDraw(thicknessResult);
            _thicknessResult = thicknessResult;

            _layersDefails.Clear();
            LayerMode.Clear();
            _layerTargets.Clear();

            LayerMode.Add(ThicknessResultVM.TotalLayerName);
            
            if (_thicknessResult.Settings.HasWaferThicknesss)
            {
                var target = _thicknessResult.Settings.TotalTarget ?? new Length(0, LengthUnit.Micrometer);
                _layerTargets.Add(ThicknessResultVM.WaferThickness, target);
                // TODO Thickness info vm
            }

            foreach (var layerSetting in _thicknessResult.Settings.ThicknessLayers)
            {
                // Target for calculation
                var target = layerSetting.Target ?? new Length(0, LengthUnit.Micrometer);
                _layerTargets.Add(layerSetting.Name, target);

                // Layer detail for schema display
                var unit = layerSetting.Target?.Unit ?? LengthUnit.Micrometer;
                var layerDetail = new LayerDetailInfoVM(layerSetting.Name, layerSetting, _digits, _getLayerColor(layerSetting.Name), unit);
                _layersDefails.Add(layerDetail);

                // Layer name for single layer selection
                if (layerSetting.IsMeasured)
                {
                    LayerMode.Add(layerSetting.Name);
                }
            }

            _totalLayerUnit = _thicknessResult.Settings.TotalTarget?.Unit ?? LengthUnit.Undefined;

            SetProperty(ref _currentLayer, LayerMode.First(), nameof(CurrentLayer));
            OnCurrentLayerChanged();
        }
        
        public void SetInterpolationData(Dictionary<string, InterpolationEngine<ThicknessPointResult>> interpolationData)
        {
            _interpolationData = interpolationData;
            CrossSectionHeatMap.RaiseProfileChanged();
            OnCurrentLayerChanged();
        }

        private void OnCurrentLayerChanged()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var layersInfo = new List<LayerDetailInfoVM>();

                if (CurrentLayer == ThicknessResultVM.TotalLayerName)
                {
                    // All layers are displayed on Total selected
                    layersInfo.AddRange(_layersDefails);

                    ThicknessLayersInfo.Title = ThicknessResultVM.TotalLayerName;

                    // Total value & deviation is updated at realtime in OnTrackerPositionPositionChanged method
                    ThicknessLayersInfo.ShowTotalArrow = true;

                    // Total target & tolerance
                    string formattedTarget = LengthToStringConverter.ConvertToString(_thicknessResult?.Settings?.TotalTarget, Digits, true, "-", _totalLayerUnit);
                    string formattedTolerance = LengthToleranceToStringConverter.ConvertToString(_thicknessResult?.Settings?.TotalTolerance, Digits, true, "-", _totalLayerUnit);
                    ThicknessLayersInfo.LeftSideTooltip = $"{ThicknessResultVM.TotalLayerName}{Environment.NewLine}Target = {formattedTarget}{Environment.NewLine}Tolerance = +/-{formattedTolerance}";
                }
                else
                {
                    NormalizedModeEnabled = false;

                    var layerInfo = _layersDefails.SingleOrDefault(vm => vm.LayerName == CurrentLayer);
                    if (layerInfo != null)
                    {
                        // Add only the required layer info to the collection
                        layersInfo.Add(layerInfo);
                        
                        ThicknessLayersInfo.Title = "Target";

                        if (CurrentLayer == ThicknessResultVM.WaferThickness)
                        {
                            // Total target & tolerance
                            string formattedTarget = LengthToStringConverter.ConvertToString(_thicknessResult?.Settings?.TotalTarget, Digits, true, "-", layerInfo.Unit);
                            string formattedTolerance = LengthToleranceToStringConverter.ConvertToString(_thicknessResult?.Settings?.TotalTolerance, Digits, true, "-", layerInfo.Unit);
                            ThicknessLayersInfo.Value = $"{formattedTarget}{Environment.NewLine}+/-{formattedTolerance}";
                        }
                        else
                        {
                            var setting = _thicknessResult?.Settings?.ThicknessLayers.SingleOrDefault(output => output.Name.Equals(CurrentLayer));

                            // Layer target & tolerance
                            string formattedTarget = LengthToStringConverter.ConvertToString(setting?.Target, Digits, true, "-", layerInfo.Unit);
                            string formattedTolerance = LengthToleranceToStringConverter.ConvertToString(setting?.Tolerance, Digits, true, "-", layerInfo.Unit);
                            ThicknessLayersInfo.Value = $"{formattedTarget}{Environment.NewLine}+/-{formattedTolerance}";
                        }

                        ThicknessLayersInfo.WaferThickness = string.Empty;
                        ThicknessLayersInfo.ShowTotalArrow = false;
                        ThicknessLayersInfo.LeftSideTooltip = null;
                    }
                }

                ThicknessLayersInfo.LayersDetails = layersInfo;
            });

            UpdateHeatMap();
            DrawChart();
        }
        
        private void UpdateHeatMap()
        {
            if (_interpolationData == null || _currentLayer == null) return;
            if (_interpolationData.TryGetValue(_currentLayer, out var interpolationEngine))
            {
                var unit = interpolationEngine.Unit;
                double? target;
                double? tolerance;
                HeatMapPaletteType paletteType;

                switch (_currentLayer)
                {
                    case ThicknessResultVM.TotalLayerName:
                    case ThicknessResultVM.WaferThickness:
                        {
                            var settings = _thicknessResult?.Settings;
                            paletteType = settings?.TotalTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                            target = settings?.TotalTarget?.GetValueAs(unit);
                            tolerance = settings?.TotalTarget != null ? settings?.TotalTolerance?.GetAbsoluteTolerance(settings.TotalTarget).GetValueAs(unit) : null;
                            break;
                        }
                    default:
                        {
                            var layerSettings = _thicknessResult?.Settings?.ThicknessLayers?.FirstOrDefault(output => output.Name == _currentLayer);
                            paletteType = layerSettings?.Target != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                            target = layerSettings?.Target?.GetValueAs(unit);
                            tolerance = layerSettings?.Target != null ? layerSettings.Tolerance?.GetAbsoluteTolerance(layerSettings.Target).GetValueAs(unit) : null;
                            break;
                        }
                }

                CrossSectionHeatMap.SetUnit(Length.GetUnitSymbol(unit));
                CrossSectionHeatMap.SetTargetTolerance(target, tolerance);
                CrossSectionHeatMap.SetPaletteType(paletteType);

                CrossSectionHeatMap.SetInterpolationResult(interpolationEngine.InterpolationResults);
                CrossSectionHeatMap.SetMinMax(interpolationEngine.CurrentMinValue, interpolationEngine.CurrentMaxValue);
            }
            else
            {
                CrossSectionHeatMap.SetInterpolationResult(null);
                CrossSectionHeatMap.SetMinMax(0, 0);
            }
        }

        private void CalculateCrossSectionData()
        {
            if (_bresenhamLine == null) return;

            foreach (var interpolationEngine in _interpolationData.Reverse())
            {
                if (interpolationEngine.Key == ThicknessResultVM.TotalLayerName)
                {
                    // Display only Layers
                    continue;
                }

                var interpolationUnit = interpolationEngine.Value.Unit;

                for (int index = 0; index < _bresenhamLine.Count; index++)
                {
                    var point = _bresenhamLine[index];

                    var data = new StackedData { Index = index };

                    // Set Target
                    if (_layerTargets.TryGetValue(interpolationEngine.Key, out var target))
                    {
                        data.Target = target;
                    }

                    // Set Value
                    if (interpolationEngine.Value.State == InterpolatorState.InterpolationSuccess)
                    {
                        if (interpolationEngine.Value.InterpolationResults.TryGetValue(new IntPoint((int)point.X, (int)point.Y), out var interpolationResult))
                        {
                            data.Length = new Length(double.IsNaN(interpolationResult.Value) ? 0 : interpolationResult.Value, interpolationUnit);
                        }
                    }
                    else
                    {
                        // Case not measured
                        // TODO Add information that is not measured
                        data.Length = data.Target;
                    }

                    if (_crossSectionData.ContainsKey(point))
                    {
                        _crossSectionData[point].Add(interpolationEngine.Key, data);
                    }
                    else
                    {
                        _crossSectionData.Add(point, new Dictionary<string, StackedData> { { interpolationEngine.Key, data } });
                    }
                }
            }
        }

        private void DrawChart()
        {
            StackedAreaChart.ClearValues();

            if (_interpolationData == null || _crossSectionData == null) return;

            bool isSingleLayer = _currentLayer != ThicknessResultVM.TotalLayerName;

            if (!isSingleLayer)
            {
                StackedAreaChart.SetYAxisTitle($"Thickness ({Length.GetUnitSymbol(_totalLayerUnit)})");
            }

            foreach (var interpolationEngine in _interpolationData.Reverse())
            {
                // Single mode layer
                if (isSingleLayer && interpolationEngine.Key != _currentLayer)
                {
                    continue;
                }
                
                var chartLayerData = new List<StackedData>();

                foreach (var data in _crossSectionData)
                {
                    if (data.Value.TryGetValue(interpolationEngine.Key, out var stackedData))
                    {
                        chartLayerData.Add(stackedData);
                    }
                }

                if (interpolationEngine.Key == ThicknessResultVM.WaferThickness)
                {
                    StackedAreaChart.SetLine(ThicknessResultVM.WaferThickness, chartLayerData, data => data.Index, data =>
                    {
                        double valueAsTotalUnit;

                        if (data.Length != null)
                        {
                            valueAsTotalUnit = data.Length.GetValueAs(_totalLayerUnit);
                        }
                        else
                        {
                            valueAsTotalUnit = 0;
                        }


                        if (!NormalizedModeEnabled)
                        {
                            return valueAsTotalUnit;
                        }

                        if (data.Target == null) return 0;

                        double targetAsTotalUnit = data.Target.GetValueAs(_totalLayerUnit);
                        // Multiply by layer count (-1 : exclude wafer thickness)
                        return valueAsTotalUnit / targetAsTotalUnit * (_layerTargets.Count - 1);
                    }, ThicknessResultVM.WaferThicknessColor);
                }
                else
                {
                    if (isSingleLayer)
                    {
                        StackedAreaChart.SetYAxisTitle($"Thickness ({Length.GetUnitSymbol(interpolationEngine.Value.Unit)})");
                    }

                    StackedAreaChart.SetData(interpolationEngine.Key, chartLayerData, data => data.Index, data =>
                    {
                        if (isSingleLayer)
                        {
                            return data.Length != null ? data.Length.GetValueAs(interpolationEngine.Value.Unit) : 0;
                        }

                        double valueAsTotalUnit = data.Length != null ? data.Length.GetValueAs(_totalLayerUnit) : 0;

                        if (!NormalizedModeEnabled)
                        {
                            return valueAsTotalUnit;
                        }

                        if (data.Target == null) return 0;

                        double targetAsTotalUnit = data.Target.GetValueAs(_totalLayerUnit);
                        return valueAsTotalUnit / targetAsTotalUnit;
                    }, _getLayerColor(interpolationEngine.Key));
                }
            }

            StackedAreaChart.RaiseDataUpdated();
        }

        #region IDisposable

        public void Dispose()
        {
            CrossSectionHeatMap.ProfileChanged -= OnCrossSectionHeatMapProfileChanged;
            CrossSectionHeatMap.RadialDragging -= OnRadialDraggingRequested;
            CrossSectionHeatMap.Dispose();

            StackedAreaChart.TrackerPositionChanged -= OnTrackerPositionPositionChanged;
            StackedAreaChart.Dispose();
        }

        #endregion
    }
}
