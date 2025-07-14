using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.RawSignal;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench.DieDetails;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench
{
    public class TrenchResultVM : MetroResultVM
    {
        public const string DepthOutputName = "Depth";
        public const string WidthOutputName = "Width";

        #region Properties

        public TrenchResult TrenchResult => MetroResult.MeasureResult as TrenchResult;

        public TrenchPointSelector PointSelector { get; } = new TrenchPointSelector();

        public TrenchGlobalStatsVM GlobalStats { get; }

        public TrenchDetailMeasureInfoVM DetailMeasureInfo { get; }

        public TrenchDataRepetaVM DataRepeta { get; }

        public MetroHeatMapVM HeatMapVM { get; }

        public DataInToleranceLineChart GlobalStatsChart { get; }

        public DataInToleranceLineChart SelectedPointsOutputChart { get; }

        public DataInToleranceLineChart SelectedPointOutputRepetaChart { get; }

        public DieMapVM DieMap { get; }

        public DieStatsVM DieStats { get; }

        public MeasureTypeCategoriesVM MeasureTypeCategories { get; }

        public PointsLocationVM PointsLocation { get; }

        public RawSignalChart RawSignalChart { get; }

        private List<string> _viewerTypeSource;

        public List<string> ViewerTypeSource
        {
            get { return _viewerTypeSource; }
            private set { SetProperty(ref _viewerTypeSource, value); }
        }

        private InterpolationEngine<TrenchPointResult> InterpolationEngine { get; set; }

        private readonly Dictionary<string, InterpolationEngine<TrenchPointResult>> _interpolationEngines = new Dictionary<string, InterpolationEngine<TrenchPointResult>>();

        #region Overrides of MetroResultVM

        private readonly TrenchPointsListVM _trenchPointsListVM;

        public override MetroPointsListVM ResultPointsList => _trenchPointsListVM;

        #endregion

        #endregion

        public TrenchResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _trenchPointsListVM = new TrenchPointsListVM(PointSelector) { Digits = Digits };
            GlobalStats = new TrenchGlobalStatsVM(PointSelector);
            DetailMeasureInfo = new TrenchDetailMeasureInfoVM
            {
                Digits = Digits
            };
            DataRepeta = new TrenchDataRepetaVM(PointSelector) { Digits = Digits };
            HeatMapVM = new MetroHeatMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Circle);
            DieMap = new DieMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Square);
            DieStats = new DieStatsVM(PointSelector);
            MeasureTypeCategories = new MeasureTypeCategoriesVM(PointSelector);
            PointsLocation = new PointsLocationVM(PointSelector);

            GlobalStatsChart = new DataInToleranceLineChart("N°", true, Colors.MediumPurple, false);
            SelectedPointsOutputChart = new DataInToleranceLineChart("N°", "Various", true, Colors.MediumPurple, false);
            SelectedPointOutputRepetaChart = new DataInToleranceLineChart("N°", "Various", false, Colors.MediumSeaGreen, false);

            RawSignalChart = new RawSignalChart("Height");

            PointSelector.SelectedPointChanged += PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;
            PointSelector.SelectedOutputChanged += PointSelectorOnSelectedOutputChanged;
            PointSelector.CurrentRepetaPointsChanged += PointSelectorCurrentRepetaPointsChanged;

            SelectedPointsOutputChart.PointSelected += SelectedPointsChartOnPointSelected;
        }

        #region Event Handlers

        private void PointSelectorOnSelectedPointChanged(object sender, EventArgs e)
        {
            DetailMeasureInfo.Point = PointSelector.SingleSelectedPoint as TrenchPointResult;
            DetailMeasureInfo.Die = PointSelector.GetDieFromPoint(PointSelector.SingleSelectedPoint);

            OnPropertyChanged(nameof(SelectedMeasurePointResultIndex));
            UpdateSelectedPointsOutputChartSelection();
        }

        private void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedMeasurePointResultIndex));
            UpdateSelectedPointsOutputChartData();
        }

        private void PointSelectorOnSelectedOutputChanged(object sender, EventArgs e)
        {
            DetailMeasureInfo.Settings = TrenchResult.Settings;

            // Update Digits number
            switch (PointSelector.SelectedOutput)
            {
                case DepthOutputName:
                    {
                        var outputSettings = TrenchResult?.Settings;
                        Digits = MetroHelper.GetDecimalCount(outputSettings?.DepthTarget?.Value, outputSettings?.DepthTolerance?.Value);
                    }
                    break;

                case WidthOutputName:
                    {
                        var outputSettings = TrenchResult?.Settings;
                        Digits = MetroHelper.GetDecimalCount(outputSettings?.WidthTarget?.Value, outputSettings?.WidthTolerance?.Value);
                    }
                    break;

                default: break;
            }

            UpdateSelectedPointsOutputChartData();
            UpdateSelectedPointsOutputChartSelection();
            UpdateSelectedPointOutputRepetaChartData();

            UpdateChartsTargetAndTolerance();

            UpdateHeatMapChart();

            DataRepeta.SelectedOutput = PointSelector.SelectedOutput;
        }

        private void SelectedPointsChartOnPointSelected(object sender, int e)
        {
            if (PointSelector.SortedIndexToPoint.TryGetValue(e - 1, out var pointToSelect))
            {
                PointSelector.SetSelectedPoint(this, pointToSelect);
            }
        }

        private void PointSelectorCurrentRepetaPointsChanged(object sender, EventArgs e)
        {
            UpdateSelectedPointOutputRepetaChartData();
        }

        #endregion

        #region HeatMap

        private InterpolationEngine<TrenchPointResult> CreateInterpolationEngine(bool dieMode)
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return null;

            // we kept only circle interpolator
            if (!dieMode && _interpolationEngines.TryGetValue(PointSelector.SelectedOutput, out var interpolationEngine))
            {
                return interpolationEngine;
            }

            InterpolationEngine<TrenchPointResult> newInterpEngine;
            if (PointSelector.SelectedOutput == DepthOutputName)
            {
                newInterpEngine = new InterpolationEngine<TrenchPointResult>(
                    point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                    point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                    point => point.DepthStat?.Mean?.Micrometers,
                    LengthUnit.Micrometer,
                    TrenchResult.Settings.DepthTarget?.Micrometers,
                    dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);

                if (!dieMode)
                {
                    _interpolationEngines.Add(PointSelector.SelectedOutput, newInterpEngine);
                }
                return newInterpEngine;
            }

            if (PointSelector.SelectedOutput == WidthOutputName)
            {
                newInterpEngine = new InterpolationEngine<TrenchPointResult>(
                    point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                    point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                    point => point.WidthStat?.Mean?.Micrometers,
                    LengthUnit.Micrometer,
                    TrenchResult.Settings.WidthTarget?.Micrometers,
                    dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);

                if (!dieMode)
                {
                    _interpolationEngines.Add(PointSelector.SelectedOutput, newInterpEngine);
                }
                return newInterpEngine;
            }


            return null;
        }

        private void UpdateHeatMapChart()
        {
            if (InterpolationEngine != null)
            {
                InterpolationEngine.InterpolationDone -= OnInterpolationDone;
                InterpolationEngine.Cancel();
                InterpolationEngine = null;
            }

            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return;

            InterpolationEngine = CreateInterpolationEngine(false);

            HeatMapPaletteType paletteType;
            double? target;
            double? tolerance;
            string unit;

            switch (PointSelector.SelectedOutput)
            {
                case DepthOutputName:
                    {
                        paletteType = TrenchResult.Settings.DepthTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                        target = TrenchResult.Settings.DepthTarget?.Micrometers;
                        tolerance = TrenchResult.Settings.DepthTolerance?.GetAbsoluteTolerance(TrenchResult.Settings.DepthTarget).Micrometers;
                        unit = Length.GetUnitSymbol(LengthUnit.Micrometer);
                        break;
                    }
                case WidthOutputName:
                    {
                        paletteType = TrenchResult.Settings.WidthTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                        target = TrenchResult.Settings.WidthTarget?.Micrometers;
                        tolerance = TrenchResult.Settings.WidthTolerance?.GetAbsoluteTolerance(TrenchResult.Settings.WidthTarget).Micrometers;
                        unit = Length.GetUnitSymbol(LengthUnit.Micrometer);
                        break;
                    }
                default:
                    {
                        paletteType = HeatMapPaletteType.MinMax;
                        target = null;
                        tolerance = null;
                        unit = string.Empty;
                        break;
                    }
            }

            // Global HeatMap
            HeatMapVM.SetTitle(PointSelector.SelectedOutput);
            HeatMapVM.SetTargetTolerance(target, tolerance);
            HeatMapVM.SetUnit(unit);
            HeatMapVM.SetPaletteType(paletteType);

            // Die HeatMap
            DieMap.SetTitle(PointSelector.SelectedOutput);
            DieMap.SetTargetTolerance(target, tolerance);
            DieMap.SetUnit(unit);
            DieMap.SetPaletteType(paletteType);
            DieMap.SetInterpolationEngine(CreateInterpolationEngine(true)); // note RTi voir comment alimenter l'engine du die avec l'interpolator de l'engine du wafer circle !

            if (InterpolationEngine == null) return;

            InterpolationEngine.InterpolationDone += OnInterpolationDone;

            double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(TrenchResult);
            InterpolationEngine.InterpolateCircle(PointSelector.AllPoints.OfType<TrenchPointResult>().ToList(), waferDiameter);
        }

        private void OnInterpolationDone(object sender, EventArgs e)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                HeatMapVM.SetMinMax(InterpolationEngine.CurrentMinValue, InterpolationEngine.CurrentMaxValue);
                HeatMapVM.SetInterpolationResult(InterpolationEngine.InterpolationResults);
            });
        }

        #endregion

        #region Overrides of ResultWaferVM

        public override string FormatName => "Trench";

        #endregion

        #region Overrides of ResultWaferVM

        protected override PointSelectorBase GetPointSelector() => PointSelector;
        protected override void OnDigitsChanged()
        {
            DetailMeasureInfo.Digits = Digits;
            DataRepeta.Digits = Digits;
            ResultPointsList.Digits = Digits;
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

            var newSource = new List<string>();

            bool hasDepth = false;
            bool hasWidth = false;

            if (TrenchResult.Settings?.DepthTarget == null || TrenchResult.Settings?.DepthTolerance == null||
                TrenchResult.Settings?.WidthTarget == null || TrenchResult.Settings?.WidthTolerance == null)
            {
                throw new ArgumentNullException(nameof(TrenchResult.Settings), "Some settings of trench result are null");
            }

            if (TrenchResult != null)
            {
                hasDepth = TrenchResult.Settings.DepthTarget != null;
                if (hasDepth) newSource.Add(DepthOutputName);

                hasWidth = TrenchResult.Settings.WidthTarget != null;
                if (hasWidth) newSource.Add(WidthOutputName);

                ViewerTypeSource = newSource;
            }

            _trenchPointsListVM.UpdateDepthAndWidthVisibility(hasDepth, hasWidth);

            DetailMeasureInfo.Settings = TrenchResult?.Settings;
            DataRepeta.UpdateOutputSource(newSource);

            HeatMapVM.Update(TrenchResult);

            DieMap.TrenchResult = TrenchResult;

            UpdateGlobalChartData();

            OnPropertyChanged(nameof(TrenchResult));

            PointSelector.SetOutputAndRaiseEvents(newSource.FirstOrDefault());

            MeasureTypeCategories.UpdateCategories(points);
            PointsLocation.PopulatePointsLocationCollection();
        }

        #endregion

        #region Private Methods

        private void UpdateGlobalChartData()
        {
            GlobalStatsChart.ClearAll();

            var settings = TrenchResult?.Settings;
            if (settings == null)
            {
                return;
            }

            bool hasDepth = settings.DepthTarget != null;
            bool hasWidth = settings.WidthTarget != null;
        
            if (hasDepth)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is TrenchPointResult result ? result.DepthStat?.Mean?.Micrometers : null,
                    pair => pair.Value is TrenchPointResult result ? result.DepthStat?.Min?.Micrometers : null,
                    pair => pair.Value is TrenchPointResult result ? result.DepthStat?.Max?.Micrometers : null,
                    pair => pair.Value is TrenchPointResult result && result.DepthStat != null
                        ? MetroHelper.GetSymbol(result.DepthStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    DepthOutputName,
                    null, 14.0, $"{Length.GetUnitSymbol(PointSelector.CurrentUnit)}", 12.0);

                double target = settings.DepthTarget.Micrometers;
                var tolerance = settings.DepthTolerance.GetAbsoluteTolerance(settings.DepthTarget);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers, DepthOutputName);
            }

            if (hasWidth)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is TrenchPointResult result ? result.WidthStat?.Mean?.Micrometers : null,
                    pair => pair.Value is TrenchPointResult result ? result.WidthStat?.Min?.Micrometers : null,
                    pair => pair.Value is TrenchPointResult result ? result.WidthStat?.Max?.Micrometers : null,
                    pair => pair.Value is TrenchPointResult result && result.WidthStat != null
                        ? MetroHelper.GetSymbol(result.WidthStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    WidthOutputName,
                    null, 14.0, $"{Length.GetUnitSymbol(PointSelector.CurrentUnit)}", 12.0);

                double target = settings.WidthTarget.Micrometers;
                var tolerance = settings.WidthTolerance.GetAbsoluteTolerance(settings.WidthTarget);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers, WidthOutputName);
            }
        }

        private void UpdateSelectedPointsOutputChartData()
        {
            var measurePointResults = PointSelector.CheckedPoints.OfType<TrenchPointResult>().ToList();

            switch (PointSelector.SelectedOutput)
            {
                case DepthOutputName:
                    {
                        measurePointResults = measurePointResults.OrderBy(point => point.DepthStat.State).ToList();

                        SelectedPointsOutputChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.DepthStat.Mean?.Micrometers,
                            point => point.DepthStat.Min?.Micrometers,
                            point => point.DepthStat.Max?.Micrometers,
                            point => MetroHelper.GetSymbol(point.DepthStat.State));
                        break;
                    }
                case WidthOutputName:
                    {
                        measurePointResults = measurePointResults.OrderBy(point => point.WidthStat.State).ToList();

                        SelectedPointsOutputChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.WidthStat.Mean?.Micrometers,
                            point => point.WidthStat.Min?.Micrometers,
                            point => point.WidthStat.Max?.Micrometers,
                            point => MetroHelper.GetSymbol(point.WidthStat.State));
                        break;
                    }
                default: break;
            }
        }

        private void UpdateSelectedPointsOutputChartSelection()
        {
            if (PointSelector.SingleSelectedPoint is TrenchPointResult point)
            {
                switch (PointSelector.SelectedOutput)
                {
                    case DepthOutputName:
                        {
                            double xValue = PointSelector.PointToIndex[point] + 1;
                            if (point.DepthStat.Mean != null)
                            {

                                double yValue = point.DepthStat.Mean.Micrometers;
                                SelectedPointsOutputChart.UpdateSelectedPoint(xValue, yValue);
                            }
                            else
                            {
                                SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
                            }
                        }
                        break;
                    case WidthOutputName:
                        {
                            double xValue = PointSelector.PointToIndex[point] + 1;
                            if (point.WidthStat.Mean != null)
                            {

                                double yValue = point.WidthStat.Mean.Micrometers;
                                SelectedPointsOutputChart.UpdateSelectedPoint(xValue, yValue);
                            }
                            else
                            {
                                SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
                            }
                        }
                        break;
                    default:
                        SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
                        break;
                }
            }
            else
            {
                SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
            }
        }

        private void UpdateSelectedPointOutputRepetaChartData()
        {
            switch (PointSelector.SelectedOutput)
            {
                case DepthOutputName:
                    {
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<TrenchPointData>().OrderBy(data => data.DepthState).ToList();

                        SelectedPointOutputRepetaChart.SetData(pointData,
                            data => data.IndexRepeta + 1,
                            data => data.Depth?.Micrometers,
                            data => MetroHelper.GetSymbol(data.DepthState));

                        if (pointData.Any(x => x.RawProfileScan?.RawPoints != null))
                        {
                            HasRawData = true;
                            RawSignalChart.Generate(pointData);
                        }
                        else
                        {
                            HasRawData = false;
                        }
                    }
                    break;
                case WidthOutputName:
                    {
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<TrenchPointData>().OrderBy(data => data.WidthState).ToList();

                        SelectedPointOutputRepetaChart.SetData(pointData,
                            data => data.IndexRepeta + 1,
                            data => data.Width?.Micrometers,
                            data => MetroHelper.GetSymbol(data.WidthState));

                        if (pointData.Any(x => x.RawProfileScan?.RawPoints != null))
                        {
                            HasRawData = true;
                            RawSignalChart.Generate(pointData);
                        }
                        else
                        {
                            HasRawData = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateChartsTargetAndTolerance()
        {
            var settings = TrenchResult?.Settings;

            if (settings == null)
            {
                SelectedPointsOutputChart.ClearTargetAndTolerance();
                SelectedPointOutputRepetaChart.ClearTargetAndTolerance();
                return;
            }

            RawSignalChart.UpdateYAxisTitle($"Height ({Length.GetUnitSymbol(PointSelector.CurrentUnit)})");
            SelectedPointsOutputChart.UpdateYAxisTitle($"{PointSelector.SelectedOutput} ({Length.GetUnitSymbol(PointSelector.CurrentUnit)})");
            SelectedPointOutputRepetaChart.UpdateYAxisTitle($"{PointSelector.SelectedOutput} ({Length.GetUnitSymbol(PointSelector.CurrentUnit)})");

            switch (PointSelector.SelectedOutput)
            {
                case DepthOutputName:
                    {
                        if (settings.DepthTarget == null || settings.DepthTolerance == null)
                        {
                            SelectedPointsOutputChart.ClearTargetAndTolerance();
                            return;
                        }

                        double target = settings.DepthTarget.Micrometers;
                        var tolerance = settings.DepthTolerance.GetAbsoluteTolerance(settings.DepthTarget);
                        SelectedPointsOutputChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers);
                        SelectedPointOutputRepetaChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers);
                    }
                    break;
                case WidthOutputName:
                    {
                        if (settings.WidthTarget == null || settings.WidthTolerance == null)
                        {
                            SelectedPointsOutputChart.ClearTargetAndTolerance();
                            return;
                        }

                        double target = settings.WidthTarget.Micrometers;
                        var tolerance = settings.WidthTolerance.GetAbsoluteTolerance(settings.WidthTarget);
                        SelectedPointsOutputChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers);
                        SelectedPointOutputRepetaChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers);
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Overrides of MetroResultVM

        public override void Dispose()
        {
            PointSelector.SelectedPointChanged -= PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged -= PointSelectorOnCheckedPointsChanged;
            PointSelector.SelectedOutputChanged -= PointSelectorOnSelectedOutputChanged;
            PointSelector.CurrentRepetaPointsChanged -= PointSelectorCurrentRepetaPointsChanged;

            SelectedPointsOutputChart.PointSelected -= SelectedPointsChartOnPointSelected;

            _trenchPointsListVM.Dispose();
            GlobalStats.Dispose();
            DataRepeta.Dispose();
            HeatMapVM.Dispose();
            DieMap.Dispose();
            DieStats.Dispose();
            MeasureTypeCategories.Dispose();
            PointsLocation.Dispose();

            GlobalStatsChart.Dispose();
            SelectedPointsOutputChart.Dispose();
            SelectedPointOutputRepetaChart.Dispose();

            RawSignalChart.Dispose();

            base.Dispose();
        }

        #endregion
    }
}
