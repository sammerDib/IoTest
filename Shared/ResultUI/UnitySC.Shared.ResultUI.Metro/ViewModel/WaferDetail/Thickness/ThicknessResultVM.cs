using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts.StackedArea;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness.DieDetails;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness
{
    public class ThicknessResultVM : MetroResultVM
    {
        public const string TotalLayerName = "Total";
        public const string WaferThickness = "Wafer Thickness";
        public const string CrossSectionModeName = "Cross-Section";

        public static Color WaferThicknessColor = Color.FromArgb(90, 0, 0, 0);
        public static Color TotalColor = Color.FromArgb(255, 237, 237, 237);

        #region Fields

        private readonly ThicknessPointsListVM _pointList;

        private readonly Dictionary<string, InterpolationEngine<ThicknessPointResult>> _interpolationEngines = new Dictionary<string, InterpolationEngine<ThicknessPointResult>>();

        private readonly Dictionary<string, LengthUnit> _layersUnit = new Dictionary<string, LengthUnit>();

        #endregion

        #region Properties

        public ThicknessResult ThicknessResult => MetroResult.MeasureResult as ThicknessResult;

        public ThicknessPointSelector PointSelector { get; } = new ThicknessPointSelector();

        public ThicknessGlobalStatsVM GlobalStats { get; }

        public ThicknessDetailMeasureInfoVM DetailMeasureInfo { get; }

        public ThicknessDataRepetaVM DataRepeta { get; }

        public MetroHeatMapVM HeatMapVM { get; }

        public DataInToleranceLineChart GlobalStatsChart { get; }

        public BoxWhiskerChart SelectedPointBoxWhiskerChart { get; }

        public DataInToleranceLineChart SelectedPointsLayerChart { get; }

        public DataInToleranceLineChart SelectedPointLayerRepetaChart { get; }

        public ThicknessDieMapVM DieMap { get; }

        public ThicknessDieStatsVM DieStats { get; }

        public ThicknessCrossSectionVM CrossSection { get; }

        public MeasureTypeCategoriesVM MeasureTypeCategories { get; }

        public PointsLocationVM PointsLocation { get; }

        private List<string> _layersSource;

        public List<string> LayersSource
        {
            get { return _layersSource; }
            private set { SetProperty(ref _layersSource, value); }
        }

        private bool _isCrossSectionMode;

        public bool IsCrossSectionMode
        {
            get { return _isCrossSectionMode; }
            private set { SetProperty(ref _isCrossSectionMode, value); }
        }

        private LengthUnit _currentUnit = LengthUnit.Micrometer;

        public LengthUnit CurrentUnit
        {
            get { return _currentUnit; }
            set
            {
                if (value == LengthUnit.Undefined) return;
                SetProperty(ref _currentUnit, value);
            }
        }

        #endregion

        public ThicknessResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _pointList = new ThicknessPointsListVM(PointSelector) { Digits = Digits };
            GlobalStats = new ThicknessGlobalStatsVM(PointSelector);
            DetailMeasureInfo = new ThicknessDetailMeasureInfoVM(GetLayerColor) { Digits = Digits };
            DataRepeta = new ThicknessDataRepetaVM(PointSelector) { Digits = Digits };
            HeatMapVM = new MetroHeatMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Circle);
            DieMap = new ThicknessDieMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Square);
            DieStats = new ThicknessDieStatsVM(PointSelector);
            CrossSection = new ThicknessCrossSectionVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Circle, GetLayerColor);
            MeasureTypeCategories = new MeasureTypeCategoriesVM(PointSelector);
            PointsLocation = new PointsLocationVM(PointSelector);

            GlobalStatsChart = new DataInToleranceLineChart("N°", true, Colors.MediumPurple, false);
            SelectedPointBoxWhiskerChart = new BoxWhiskerChart("Layer", "Normalized Thickness");
            SelectedPointsLayerChart = new DataInToleranceLineChart("N°", "Various", true, Colors.MediumPurple, false);
            SelectedPointLayerRepetaChart = new DataInToleranceLineChart("N°", "Various", false, Colors.MediumSeaGreen, false);

            PointSelector.SelectedPointChanged += PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;
            PointSelector.SelectedLayerChanged += PointSelectorOnSelectedLayerChanged;
            PointSelector.CurrentRepetaPointsChanged += PointSelectorCurrentRepetaPointsChanged;

            SelectedPointsLayerChart.PointSelected += SelectedPointsChartOnPointSelected;
        }

        #region Event Handlers

        private void PointSelectorOnSelectedPointChanged(object sender, EventArgs e)
        {
            DetailMeasureInfo.Point = PointSelector.SingleSelectedPoint as ThicknessPointResult;
            DetailMeasureInfo.Die = PointSelector.GetDieFromPoint(PointSelector.SingleSelectedPoint);

            OnPropertyChanged(nameof(SelectedMeasurePointResultIndex));
            UpdateSelectedPointsLayerChartSelection();
            UpdateSelectedPointBoxWhiskerChart();
        }

        private void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedMeasurePointResultIndex));
            UpdateSelectedPointsLayerChartData();
        }

        private void PointSelectorOnSelectedLayerChanged(object sender, EventArgs e)
        {
            DetailMeasureInfo.Layer = PointSelector.SelectedLayer;

            // Update Digits number
            switch (PointSelector.SelectedLayer)
            {
                case TotalLayerName:
                case CrossSectionModeName:
                case WaferThickness:
                    {
                        var settings = ThicknessResult?.Settings;
                        CurrentUnit = settings?.TotalTarget?.Unit ?? LengthUnit.Undefined;
                        Digits = MetroHelper.GetDecimalCount(settings?.TotalTarget?.Value, settings?.TotalTolerance?.Value);
                        break;
                    }
                default:
                    {
                        var layerSettings = ThicknessResult?.Settings?.ThicknessLayers?.FirstOrDefault(output => output.Name == PointSelector.SelectedLayer);
                        CurrentUnit = layerSettings?.Target?.Unit ?? LengthUnit.Undefined;
                        Digits = MetroHelper.GetDecimalCount(layerSettings?.Target?.Value, layerSettings?.Tolerance?.Value);
                        break;
                    }
            }

            // Cross section mode
            if (PointSelector.SelectedLayer == CrossSectionModeName)
            {
                IsCrossSectionMode = true;
            }
            else
            {
                IsCrossSectionMode = false;

                UpdateSelectedPointsLayerChartData();
                UpdateSelectedPointsLayerChartSelection();
                UpdateSelectedPointLayerRepetaChartData();

                UpdateChartsTargetAndTolerance();

                // Sync DataRepeta Layer Selection
                DataRepeta.SelectedLayer = PointSelector.SelectedLayer;
            }

            UpdateHeatMapChart();
        }

        /// <summary>
        /// Used only for die interpolation. Full wafer interpolation is performed only once during ResDataChanged
        /// </summary>
        private InterpolationEngine<ThicknessPointResult> CreateInterpolationEngine(bool dieMode)
        {
            var outputSetting = ThicknessResult?.Settings?.ThicknessLayers?.SingleOrDefault(output => output.Name.Equals(PointSelector.SelectedLayer));

            switch (PointSelector.SelectedLayer)
            {
                case TotalLayerName:
                    {
                        return new InterpolationEngine<ThicknessPointResult>(
                            point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                            point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                            point => point.TotalThicknessStat?.Mean?.GetValueAs(CurrentUnit),
                            CurrentUnit,
                            ThicknessResult.Settings?.TotalTarget?.GetValueAs(CurrentUnit),
                            dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);
                    }
                default:
                    {
                        return new InterpolationEngine<ThicknessPointResult>(
                            point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                            point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                            point => point.ThicknessLayerStats.TryGetValue(PointSelector.SelectedLayer, out var container) ? container.Mean?.GetValueAs(CurrentUnit) : null,
                            CurrentUnit,
                            outputSetting?.Target?.GetValueAs(CurrentUnit),
                            dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);
                    }
            }
        }

        private void UpdateHeatMapChart()
        {
            HeatMapPaletteType paletteType;
            double? target;
            double? tolerance;
            string unit = Length.GetUnitSymbol(CurrentUnit);

            switch (PointSelector.SelectedLayer)
            {
                case TotalLayerName:
                case CrossSectionModeName:
                case WaferThickness:
                    {
                        var settings = ThicknessResult?.Settings;
                        paletteType = settings?.TotalTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                        target = settings?.TotalTarget?.GetValueAs(CurrentUnit);
                        tolerance = settings?.TotalTarget != null ? settings?.TotalTolerance?.GetAbsoluteTolerance(settings.TotalTarget).GetValueAs(CurrentUnit) : null;
                        break;
                    }
                default:
                    {
                        var layerSettings = ThicknessResult?.Settings?.ThicknessLayers?.FirstOrDefault(output => output.Name == PointSelector.SelectedLayer);
                        paletteType = layerSettings?.Target != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                        target = layerSettings?.Target?.GetValueAs(CurrentUnit);
                        tolerance = layerSettings?.Target != null ? layerSettings.Tolerance?.GetAbsoluteTolerance(layerSettings.Target).GetValueAs(CurrentUnit) : null;
                        break;
                    }
            }

            // Global HeatMap
            HeatMapVM.SetTitle(PointSelector.SelectedLayer);
            HeatMapVM.SetTargetTolerance(target, tolerance);
            HeatMapVM.SetUnit(unit);
            HeatMapVM.SetPaletteType(paletteType);

            // Cross Section HeatMap
            CrossSection.CrossSectionHeatMap.SetTitle(PointSelector.SelectedLayer);
            CrossSection.CrossSectionHeatMap.SetTargetTolerance(target, tolerance);
            CrossSection.CrossSectionHeatMap.SetUnit(unit);
            CrossSection.CrossSectionHeatMap.SetPaletteType(paletteType);

            // Die HeatMap
            DieMap.SetTargetTolerance(target, tolerance);
            DieMap.SetUnit(unit);
            DieMap.SetPaletteType(paletteType);
            DieMap.SetInterpolationEngine(CreateInterpolationEngine(true));
            // note RTi voir comment alimenter l'engine du die avec l'interpolator de l'engine du wafer circle !
            // note TLa l'interpolator de l'enfine du wafer circle a une résolution plus faible et les points en dehors du die peuvent aussi impacter la heatmap

            if (_interpolationEngines.TryGetValue(PointSelector.SelectedLayer, out var interpolationEngine))
            {
                if (interpolationEngine.State == InterpolatorState.InterpolationSuccess)
                {
                    HeatMapVM.SetInterpolationResult(interpolationEngine.InterpolationResults);
                    HeatMapVM.SetMinMax(interpolationEngine.CurrentMinValue, interpolationEngine.CurrentMaxValue);
                }
                else
                {
                    HeatMapVM.SetInterpolationResult(null);
                    HeatMapVM.SetMinMax(0, 0);
                }
            }
        }

        private void PointSelectorCurrentRepetaPointsChanged(object sender, EventArgs e)
        {
            UpdateSelectedPointLayerRepetaChartData();
        }

        private void SelectedPointsChartOnPointSelected(object sender, int e)
        {
            if (PointSelector.SortedIndexToPoint.TryGetValue(e - 1, out var pointToSelect))
            {
                PointSelector.SetSelectedPoint(this, pointToSelect);
            }
        }

        #endregion

        #region Overrides of ResultWaferVM

        public override string FormatName => "Thickness";

        #endregion

        #region Overrides of MetroResultVM

        public override MetroPointsListVM ResultPointsList => _pointList;

        protected override PointSelectorBase GetPointSelector() => PointSelector;

        protected override void OnDigitsChanged()
        {
            DetailMeasureInfo.Digits = Digits;
            DataRepeta.Digits = Digits;
            ResultPointsList.Digits = Digits;
            CrossSection.Digits = Digits;
        }

        protected override void OnResDataChanged(List<MeasurePointResult> points)
        {
            #region Cancel previous interpolations

            foreach (var interpolationEngine in _interpolationEngines)
            {
                interpolationEngine.Value.InterpolationDone -= OnInterpolationDone;
                interpolationEngine.Value.Cancel();
            }
            _interpolationEngines.Clear();

            #endregion

            _layersUnit.Clear();

            if (ThicknessResult.Settings.TotalTarget == null && ThicknessResult.Settings.TotalTolerance == null &&
                ThicknessResult.Settings.ThicknessLayers.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(ThicknessResult.Settings), "Some settings of thickness result are null.");
            }

            if (ThicknessResult != null)
            {
                var layerSource = new List<string>();

                if (ThicknessResult.Settings.TotalTarget != null)
                {
                    _layersUnit.Add(TotalLayerName, ThicknessResult.Settings.TotalTarget?.Unit ?? LengthUnit.Micrometer);
                    layerSource.Add(TotalLayerName);
                }

                if (ThicknessResult.Settings.HasWaferThicknesss)
                {
                    // Configure the WaferThickness unit with the total unit because there is no unit defined for the WaferThickness
                    var waferThicknessUnit = ThicknessResult.Settings.TotalTarget?.Unit ?? LengthUnit.Micrometer;
                    _layersUnit.Add(WaferThickness, waferThicknessUnit);
                    layerSource.Add(WaferThickness);
                }

                foreach (var layer in ThicknessResult.Settings.ThicknessLayers)
                {
                    _layersUnit.Add(layer.Name, layer.Target?.Unit ?? LengthUnit.Micrometer);
                    if (layer.IsMeasured)
                    {
                        layerSource.Add(layer.Name);
                    }
                }

                _layersUnit.Add(CrossSectionModeName, ThicknessResult.Settings.TotalTarget?.Unit ?? LengthUnit.Micrometer);
                layerSource.Add(CrossSectionModeName);

                LayersSource = layerSource;

                #region Interpolation

                var totalUnit = ThicknessResult.Settings.TotalTarget?.Unit ?? LengthUnit.Micrometer;

                _interpolationEngines.Add(TotalLayerName, new InterpolationEngine<ThicknessPointResult>(
                    point => point.WaferRelativeXPosition,
                    point => point.WaferRelativeYPosition,
                    point => point.TotalThicknessStat?.Mean?.GetValueAs(totalUnit),
                    totalUnit,
                    ThicknessResult.Settings.TotalTarget?.GetValueAs(totalUnit),
                    InterpolationEngine<object>.DefaultHeatMapSide_Circle));

                if (ThicknessResult.Settings.HasWaferThicknesss)
                {
                    _interpolationEngines.Add(WaferThickness, new InterpolationEngine<ThicknessPointResult>(
                        point => point.WaferRelativeXPosition,
                        point => point.WaferRelativeYPosition,
                        point => point.WaferThicknessStat?.Mean?.GetValueAs(totalUnit),
                        totalUnit,
                        ThicknessResult.Settings.TotalTarget?.GetValueAs(totalUnit),
                        InterpolationEngine<object>.DefaultHeatMapSide_Circle));
                }

                foreach (var layer in ThicknessResult.Settings.ThicknessLayers)
                {
                    var outputUnit = layer.Target?.Unit ?? LengthUnit.Micrometer;

                    _interpolationEngines.Add(layer.Name, new InterpolationEngine<ThicknessPointResult>(
                        point => point.WaferRelativeXPosition,
                        point => point.WaferRelativeYPosition,
                        point => point.ThicknessLayerStats.TryGetValue(layer.Name, out var container) ? container.Mean?.GetValueAs(outputUnit) : null,
                        outputUnit,
                        layer.Target?.GetValueAs(outputUnit),
                        InterpolationEngine<object>.DefaultHeatMapSide_Circle));
                }

                #endregion
            }
            else
            {
                LayersSource = new List<string>();
            }

            HasThumbnail = false;

            _pointList.SetLayersUnit(_layersUnit);
            DetailMeasureInfo.SetLayersUnit(_layersUnit);
            DetailMeasureInfo.Settings = ThicknessResult?.Settings;

            if (ThicknessResult.Settings.HasWarpMeasure && ThicknessResult.WarpStat != null && ThicknessResult.WarpStat.Mean != null)
            {
                DetailMeasureInfo.WarpResultLength = ThicknessResult.WarpStat.Mean;
                DetailMeasureInfo.GlobalState = ThicknessResult.WarpStat.State;
                DetailMeasureInfo.Digits = Digits;
            }
            else
            {
                DetailMeasureInfo.WarpResultLength = null;
                DetailMeasureInfo.GlobalState = MeasureState.NotMeasured;
            }

            // NOTE RTI : plus tard, Comprendre pourquoi UpdatePointsSource est appelé ici alors qu'il a été déjà appélé dans la classe de base qui appelle OnResDataChanged
            // cf MetroResultVM.UpdateResData
            ResultPointsList.UpdatePointsSource(points, HasRepeta, IsDieMode, HasQualityScore, HasSiteID);

            // Don't give the possibility to select the cross Section from DataRepeta
            DataRepeta.UpdateLayerSource(LayersSource.GetRange(0, LayersSource.Count - 1), _layersUnit);

            HeatMapVM.Update(ThicknessResult);
            CrossSection.Update(ThicknessResult);

            DieMap.ThicknessResult = ThicknessResult;

            UpdateGlobalChartData();

            OnPropertyChanged(nameof(ThicknessResult));

            PointSelector.SetLayerAndRaiseEvents(LayersSource.FirstOrDefault());

            MeasureTypeCategories.UpdateCategories(points);

            MeasureTypeCategories.UpdateCategories(points);
            PointsLocation.PopulatePointsLocationCollection();

            #region Start Interpolations

            foreach (var interpolationEngine in _interpolationEngines)
            {
                interpolationEngine.Value.InterpolationDone += OnInterpolationDone;
                Debug.WriteLine($"Interpolations {interpolationEngine.Key} started");

                double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(ThicknessResult);
                interpolationEngine.Value.InterpolateCircle(points.OfType<ThicknessPointResult>().ToList(), waferDiameter);
            }

            #endregion
        }

        private void OnInterpolationDone(object sender, EventArgs e)
        {
            foreach (var interpolationEngine in _interpolationEngines)
            {
                if (!ReferenceEquals(sender, interpolationEngine.Value)) continue;

                if (PointSelector.SelectedLayer.Equals(interpolationEngine.Key))
                {
                    // Set HeatMap values
                    HeatMapVM.SetInterpolationResult(interpolationEngine.Value.InterpolationResults);
                    HeatMapVM.SetMinMax(interpolationEngine.Value.CurrentMinValue, interpolationEngine.Value.CurrentMaxValue);
                }
            }

            if (_interpolationEngines.Values.All(engine => engine.State > InterpolatorState.InProgress))
            {
                CrossSection.SetInterpolationData(_interpolationEngines);
            }
        }

        #endregion

        #region Private Methods

        private Color GetLayerColor(string layerName)
        {
            switch (layerName)
            {
                case TotalLayerName:
                    return TotalColor;
                case WaferThickness:
                    return WaferThicknessColor;
                default:
                    {
                        var layerSettings = ThicknessResult?.Settings?.ThicknessLayers?.SingleOrDefault(settings => settings.Name == layerName);
                        return layerSettings == null ? Colors.Magenta : layerSettings.LayerColor;
                    }
            }
        }

        private void UpdateGlobalChartData()
        {
            GlobalStatsChart.ClearAll();

            var settings = ThicknessResult?.Settings;
            if (settings == null)
            {
                return;
            }

            bool hasTotalTarget = settings.TotalTarget != null;

            const int layerToDisplay = 5;
            int currentLayerIndex = 0;

            if (hasTotalTarget)
            {
                _layersUnit.TryGetValue(TotalLayerName, out var unit);

                // Total
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is ThicknessPointResult result ? result.TotalThicknessStat?.Mean?.GetValueAs(unit) : null,
                    pair => pair.Value is ThicknessPointResult result ? result.TotalThicknessStat?.Min?.GetValueAs(unit) : null,
                    pair => pair.Value is ThicknessPointResult result ? result.TotalThicknessStat?.Max?.GetValueAs(unit) : null,
                    pair => pair.Value is ThicknessPointResult result && result.TotalThicknessStat != null
                        ? MetroHelper.GetSymbol(result.TotalThicknessStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    TotalLayerName,
                    GetLayerColor(TotalLayerName),
                    12.0, $"{Length.GetUnitSymbol(unit)}", 10.0);

                double target = settings.TotalTarget.GetValueAs(unit);
                double tolerance = settings.TotalTolerance.GetAbsoluteTolerance(settings.TotalTarget).GetValueAs(unit);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance, TotalLayerName);

                currentLayerIndex++;

                // Wafer Thickness
                if (settings.HasWaferThicknesss)
                {
                    GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                        pair => pair.Value is ThicknessPointResult result ? result.WaferThicknessStat?.Mean?.GetValueAs(unit) : null,
                        pair => pair.Value is ThicknessPointResult result ? result.WaferThicknessStat?.Min?.GetValueAs(unit) : null,
                        pair => pair.Value is ThicknessPointResult result ? result.WaferThicknessStat?.Max?.GetValueAs(unit) : null,
                        pair => pair.Value is ThicknessPointResult result && result.WaferThicknessStat != null
                            ? MetroHelper.GetSymbol(result.WaferThicknessStat.State)
                            : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                        WaferThickness,
                        GetLayerColor(WaferThickness),
                        12.0, $"{Length.GetUnitSymbol(unit)}", 10.0);

                    GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance, WaferThickness);

                    currentLayerIndex++;
                }
            }

            // Layers
            foreach (var layer in settings.ThicknessLayers.Where(layer => layer.IsMeasured))
            {
                _layersUnit.TryGetValue(layer.Name, out var unit);

                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is ThicknessPointResult result && result.ThicknessLayerStats.TryGetValue(layer.Name, out var outputStat) ? outputStat.Mean?.GetValueAs(unit) : null,
                    pair => pair.Value is ThicknessPointResult result && result.ThicknessLayerStats.TryGetValue(layer.Name, out var outputStat) ? outputStat.Min?.GetValueAs(unit) : null,
                    pair => pair.Value is ThicknessPointResult result && result.ThicknessLayerStats.TryGetValue(layer.Name, out var outputStat) ? outputStat.Max?.GetValueAs(unit) : null,
                    pair => pair.Value is ThicknessPointResult result && result.ThicknessLayerStats.TryGetValue(layer.Name, out var outputStat)
                        ? MetroHelper.GetSymbol(outputStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    layer.Name,
                    GetLayerColor(layer.Name),
                    12.0, $"{Length.GetUnitSymbol(unit)}", 10.0);

                double target = layer.Target.GetValueAs(unit);
                double tolerance = layer.Tolerance.GetAbsoluteTolerance(layer.Target).GetValueAs(unit);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance, layer.Name);

                currentLayerIndex++;

                if (currentLayerIndex >= layerToDisplay)
                {
                    break;
                }
            }
        }

        #region Selected Points Layer Chart

        private void UpdateSelectedPointsLayerChartData()
        {
            var measurePointResults = PointSelector.CheckedPoints.OfType<ThicknessPointResult>().ToList();

            switch (PointSelector.SelectedLayer)
            {
                case TotalLayerName:
                case CrossSectionModeName:
                    {
                        measurePointResults = measurePointResults.OrderBy(point => point.TotalThicknessStat.State).ToList();

                        SelectedPointsLayerChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.TotalThicknessStat.Mean?.GetValueAs(CurrentUnit),
                            point => point.TotalThicknessStat.Min?.GetValueAs(CurrentUnit),
                            point => point.TotalThicknessStat.Max?.GetValueAs(CurrentUnit),
                            point => MetroHelper.GetSymbol(point.TotalThicknessStat.State));
                        break;
                    }
                case WaferThickness:
                    {
                        measurePointResults = measurePointResults.OrderBy(point => point.WaferThicknessStat.State).ToList();

                        SelectedPointsLayerChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.WaferThicknessStat.Mean?.GetValueAs(CurrentUnit),
                            point => point.WaferThicknessStat.Min?.GetValueAs(CurrentUnit),
                            point => point.WaferThicknessStat.Max?.GetValueAs(CurrentUnit),
                            point => MetroHelper.GetSymbol(point.WaferThicknessStat.State));
                        break;
                    }
                default:
                    {
                        measurePointResults = measurePointResults.OrderBy(point =>
                            point.ThicknessLayerStats.TryGetValue(PointSelector.SelectedLayer, out var statsContainer) ? statsContainer.State : MeasureState.NotMeasured).ToList();

                        SelectedPointsLayerChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.ThicknessLayerStats.TryGetValue(PointSelector.SelectedLayer, out var statsContainer) ? statsContainer?.Mean?.GetValueAs(CurrentUnit) : null,
                            point => point.ThicknessLayerStats.TryGetValue(PointSelector.SelectedLayer, out var statsContainer) ? statsContainer?.Min?.GetValueAs(CurrentUnit) : null,
                            point => point.ThicknessLayerStats.TryGetValue(PointSelector.SelectedLayer, out var statsContainer) ? statsContainer?.Max?.GetValueAs(CurrentUnit) : null,
                            point => point.ThicknessLayerStats.TryGetValue(PointSelector.SelectedLayer, out var statsContainer) ? MetroHelper.GetSymbol(statsContainer.State) : MetroHelper.GetSymbol(MeasureState.NotMeasured));
                        break;
                    }
            }
        }

        private void UpdateSelectedPointsLayerChartSelection()
        {
            if (PointSelector.SingleSelectedPoint is ThicknessPointResult point)
            {
                double xValue = PointSelector.PointToIndex[point] + 1;

                switch (PointSelector.SelectedLayer)
                {
                    case TotalLayerName:
                    case CrossSectionModeName:
                        if (point.TotalThicknessStat?.Mean != null)
                        {
                            double yValue = point.TotalThicknessStat.Mean.GetValueAs(CurrentUnit);
                            SelectedPointsLayerChart.UpdateSelectedPoint(xValue, yValue);
                        }
                        else
                        {
                            SelectedPointsLayerChart.UpdateSelectedPoint(null, null);
                        }
                        break;
                    case WaferThickness:
                        if (point.WaferThicknessStat?.Mean != null)
                        {
                            double yValue = point.WaferThicknessStat.Mean.GetValueAs(CurrentUnit);
                            SelectedPointsLayerChart.UpdateSelectedPoint(xValue, yValue);
                        }
                        else
                        {
                            SelectedPointsLayerChart.UpdateSelectedPoint(null, null);
                        }
                        break;
                    default:
                        if (point.ThicknessLayerStats.TryGetValue(PointSelector.SelectedLayer, out var statsContainer))
                        {
                            if (statsContainer.State == MeasureState.NotMeasured)
                            {
                                SelectedPointsLayerChart.UpdateSelectedPoint(null, null);
                                return;
                            }
                            SelectedPointsLayerChart.UpdateSelectedPoint(xValue, statsContainer.Mean.GetValueAs(CurrentUnit));
                        }
                        else
                        {
                            SelectedPointsLayerChart.UpdateSelectedPoint(null, null);
                        }
                        break;
                }
            }
            else
            {
                SelectedPointsLayerChart.UpdateSelectedPoint(null, null);
            }
        }

        private void UpdateSelectedPointBoxWhiskerChart()
        {
            SelectedPointBoxWhiskerChart.Clear();

            if (PointSelector.SingleSelectedPoint is ThicknessPointResult point && ThicknessResult != null)
            {
                int currentXValue = 0;

                double totalTarget = ThicknessResult.Settings.TotalTarget.GetValueAs(CurrentUnit);
                double totalTolerance = ThicknessResult.Settings.TotalTolerance.GetAbsoluteTolerance(ThicknessResult.Settings.TotalTarget).GetValueAs(CurrentUnit);

                // Total
                var totalStat = point.TotalThicknessStat;

                double GetTotalY(double value)
                {
                    return (totalTarget - value) / totalTolerance;
                }

                SelectedPointBoxWhiskerChart.AddBoxAndWhiskers(
                    GetTotalY(totalStat.Min.GetValueAs(CurrentUnit)),
                    GetTotalY(totalStat.Mean.GetValueAs(CurrentUnit) - totalStat.StdDev.GetValueAs(CurrentUnit)),
                    GetTotalY(totalStat.Mean.GetValueAs(CurrentUnit)),
                    GetTotalY(totalStat.Mean.GetValueAs(CurrentUnit) + totalStat.StdDev.GetValueAs(CurrentUnit)),
                    GetTotalY(totalStat.Max.GetValueAs(CurrentUnit)),
                    currentXValue,
                    TotalLayerName,
                    HasRepeta,
                    GetLayerColor(TotalLayerName),
                    MetroHelper.GetSymbol(totalStat.State));

                currentXValue++;

                // Wafer Thickness
                if (ThicknessResult.Settings.HasWaferThicknesss)
                {
                    var waferThicknessStat = point.WaferThicknessStat;

                    if (waferThicknessStat.State == MeasureState.NotMeasured)
                    {
                        SelectedPointBoxWhiskerChart.AddEmptyBox(currentXValue, WaferThickness);
                    }
                    else
                    {
                        SelectedPointBoxWhiskerChart.AddBoxAndWhiskers(
                         GetTotalY(waferThicknessStat.Min.GetValueAs(CurrentUnit)),
                         GetTotalY(waferThicknessStat.Mean.GetValueAs(CurrentUnit) - waferThicknessStat.StdDev.GetValueAs(CurrentUnit)),
                         GetTotalY(waferThicknessStat.Mean.GetValueAs(CurrentUnit)),
                         GetTotalY(waferThicknessStat.Mean.GetValueAs(CurrentUnit) + waferThicknessStat.StdDev.GetValueAs(CurrentUnit)),
                         GetTotalY(waferThicknessStat.Max.GetValueAs(CurrentUnit)),
                         currentXValue,
                         WaferThickness,
                         HasRepeta,
                         GetLayerColor(WaferThickness),
                         MetroHelper.GetSymbol(waferThicknessStat.State));
                    }

                    currentXValue++;
                }

                // Layers
                foreach (var layerSetting in ThicknessResult.Settings.ThicknessLayers.Where(settings => settings.IsMeasured))
                {
                    double target = layerSetting.Target.GetValueAs(CurrentUnit);
                    double tolerance = layerSetting.Tolerance.GetAbsoluteTolerance(layerSetting.Target).GetValueAs(CurrentUnit);
                    point.ThicknessLayerStats.TryGetValue(layerSetting.Name, out var layerStats);

                    if (layerStats == null || layerStats.State == MeasureState.NotMeasured)
                    {
                        SelectedPointBoxWhiskerChart.AddEmptyBox(currentXValue, layerSetting.Name);
                    }
                    else
                    {
                        double GetY(double value)
                        {
                            return (target - value) / tolerance;
                        }

                        SelectedPointBoxWhiskerChart.AddBoxAndWhiskers(
                            GetY(layerStats.Min.GetValueAs(CurrentUnit)),
                            GetY(layerStats.Mean.GetValueAs(CurrentUnit) - layerStats.Sigma3.GetValueAs(CurrentUnit) / 3),
                            GetY(layerStats.Mean.GetValueAs(CurrentUnit)),
                            GetY(layerStats.Mean.GetValueAs(CurrentUnit) + layerStats.Sigma3.GetValueAs(CurrentUnit) / 3),
                            GetY(layerStats.Max.GetValueAs(CurrentUnit)),
                            currentXValue,
                            layerSetting.Name,
                            HasRepeta,
                            GetLayerColor(layerSetting.Name),
                            MetroHelper.GetSymbol(layerStats.State));
                    }

                    currentXValue++;
                }

                SelectedPointBoxWhiskerChart.AutoScale();
            }
        }

        #endregion

        #region Repeta Chart

        private void UpdateSelectedPointLayerRepetaChartData()
        {
            switch (PointSelector.SelectedLayer)
            {
                case CrossSectionModeName:
                    break;

                case TotalLayerName:
                    {
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<ThicknessPointData>().OrderBy(data => data.TotalState).ToList();

                        SelectedPointLayerRepetaChart.SetData(pointData,
                            data => data.IndexRepeta + 1,
                            data => data.TotalThickness?.GetValueAs(CurrentUnit),
                            data => MetroHelper.GetSymbol(data.TotalState));

                        break;
                    }

                case WaferThickness:
                    {
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<ThicknessPointData>().OrderBy(data => data.WaferThicknessResult?.State).ToList();

                        SelectedPointLayerRepetaChart.SetData(pointData,
                            data => data.IndexRepeta + 1,
                            data => data.WaferThicknessResult?.Length?.GetValueAs(CurrentUnit),
                            data => MetroHelper.GetSymbol(data.WaferThicknessResult?.State ?? MeasureState.NotMeasured));

                        break;
                    }

                default:
                    {
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<ThicknessPointData>().OrderBy(data =>
                        {
                            var outputResult = data.ThicknessLayerResults.SingleOrDefault(result => result.Name.Equals(PointSelector.SelectedLayer));
                            return outputResult?.State ?? MeasureState.NotMeasured;
                        }).ToList();

                        SelectedPointLayerRepetaChart.SetData(pointData,
                            data => data.IndexRepeta + 1,
                            data =>
                            {
                                var outputResult = data.ThicknessLayerResults.SingleOrDefault(result => result.Name.Equals(PointSelector.SelectedLayer));
                                return outputResult?.Length?.GetValueAs(CurrentUnit);
                            },
                            data =>
                            {
                                var outputResult = data.ThicknessLayerResults.SingleOrDefault(result => result.Name.Equals(PointSelector.SelectedLayer));
                                return MetroHelper.GetSymbol(outputResult?.State ?? MeasureState.NotMeasured);
                            });
                        break;
                    }
            }
        }

        #endregion

        private void UpdateChartsTargetAndTolerance()
        {
            var settings = ThicknessResult?.Settings;

            if (settings == null)
            {
                SelectedPointsLayerChart.ClearTargetAndTolerance();
                SelectedPointLayerRepetaChart.ClearTargetAndTolerance();
                return;
            }

            _layersUnit.TryGetValue(PointSelector.SelectedLayer, out var unit);
            string yAxisTitle = $"{PointSelector.SelectedLayer} ({Length.GetUnitSymbol(unit)})";

            SelectedPointsLayerChart.UpdateYAxisTitle(yAxisTitle);
            SelectedPointLayerRepetaChart.UpdateYAxisTitle(yAxisTitle);

            switch (PointSelector.SelectedLayer)
            {
                case CrossSectionModeName:
                    break;
                case TotalLayerName:
                case WaferThickness:
                    {
                        if (settings.TotalTarget == null || settings.TotalTolerance == null)
                        {
                            SelectedPointsLayerChart.ClearTargetAndTolerance();
                            return;
                        }

                        double target = settings.TotalTarget.GetValueAs(CurrentUnit);
                        double tolerance = settings.TotalTolerance.GetAbsoluteTolerance(settings.TotalTarget).GetValueAs(CurrentUnit);
                        SelectedPointsLayerChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance);
                        SelectedPointLayerRepetaChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance);
                        break;
                    }
                default:
                    {
                        var outputSetting = settings.ThicknessLayers?.SingleOrDefault(output => output.Name.Equals(PointSelector.SelectedLayer));
                        if (outputSetting == null || outputSetting.Target == null || outputSetting.Tolerance == null)
                        {
                            SelectedPointsLayerChart.ClearTargetAndTolerance();
                            return;
                        }

                        double target = outputSetting.Target.GetValueAs(CurrentUnit);
                        double tolerance = outputSetting.Tolerance.GetAbsoluteTolerance(outputSetting.Target).GetValueAs(CurrentUnit);
                        SelectedPointsLayerChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance);
                        SelectedPointLayerRepetaChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance);
                        break;
                    }
            }
        }

        #endregion

        #region Overrides of MetroResultVM

        public override void Dispose()
        {
            PointSelector.SelectedPointChanged -= PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged -= PointSelectorOnCheckedPointsChanged;
            PointSelector.SelectedLayerChanged -= PointSelectorOnSelectedLayerChanged;
            PointSelector.CurrentRepetaPointsChanged -= PointSelectorCurrentRepetaPointsChanged;

            SelectedPointsLayerChart.PointSelected -= SelectedPointsChartOnPointSelected;

            _pointList.Dispose();
            GlobalStats.Dispose();
            DataRepeta.Dispose();
            HeatMapVM.Dispose();
            DieMap.Dispose();
            DieStats.Dispose();
            CrossSection.Dispose();
            MeasureTypeCategories.Dispose();
            PointsLocation.Dispose();

            GlobalStatsChart.Dispose();
            SelectedPointBoxWhiskerChart.Dispose();
            SelectedPointsLayerChart.Dispose();
            SelectedPointLayerRepetaChart.Dispose();

            base.Dispose();
        }

        #endregion
    }
}
