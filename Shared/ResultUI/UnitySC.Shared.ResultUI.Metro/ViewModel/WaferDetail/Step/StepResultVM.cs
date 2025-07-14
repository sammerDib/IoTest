using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.RawSignal;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Step.DieDetails;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Step
{
    public class StepResultVM : MetroResultVM
    {
        public const string StepHeightOutputName = "Step";

        #region Properties

        public StepResult StepResult => MetroResult.MeasureResult as StepResult;

        public StepPointSelector PointSelector { get; } = new StepPointSelector();

        public StepGlobalStatsVM GlobalStats { get; }

        public StepDetailMeasureInfoVM DetailMeasureInfo { get; }

        public StepDataRepetaVM DataRepeta { get; }

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

        private InterpolationEngine<StepPointResult> InterpolationEngine { get; set; }

        private readonly Dictionary<string, InterpolationEngine<StepPointResult>> _interpolationEngines = new Dictionary<string, InterpolationEngine<StepPointResult>>();

        #region Overrides of MetroResultVM

        private readonly StepPointsListVM _stepPointsListVM;

        public override MetroPointsListVM ResultPointsList => _stepPointsListVM;

        #endregion

        #endregion

        public StepResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _stepPointsListVM = new StepPointsListVM(PointSelector) { Digits = Digits };
            GlobalStats = new StepGlobalStatsVM(PointSelector);
            DetailMeasureInfo = new StepDetailMeasureInfoVM
            {
                Digits = Digits
            };
            DataRepeta = new StepDataRepetaVM(PointSelector) { Digits = Digits };
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
            DetailMeasureInfo.Point = PointSelector.SingleSelectedPoint as StepPointResult;
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
            DetailMeasureInfo.Output = PointSelector.SelectedOutput;
            DetailMeasureInfo.Settings = StepResult.Settings;

            // Update Digits number
            if (PointSelector.SelectedOutput == StepHeightOutputName)
            {
                var outputSettings = StepResult?.Settings;
                Digits = MetroHelper.GetDecimalCount(outputSettings?.StepHeightTarget?.Value, outputSettings?.StepHeightTolerance?.Value);
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

        private InterpolationEngine<StepPointResult> CreateInterpolationEngine(bool dieMode)
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return null;

            // we kept only circle interpolator
            if (!dieMode && _interpolationEngines.TryGetValue(PointSelector.SelectedOutput, out var interpolationEngine))
            {
                return interpolationEngine;
            }

            InterpolationEngine<StepPointResult> newInterpEngine;
            if (PointSelector.SelectedOutput == StepHeightOutputName)
            {
                newInterpEngine = new InterpolationEngine<StepPointResult>(
                    point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                    point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                    point => point.StepHeightStat?.Mean?.Micrometers,
                    LengthUnit.Micrometer,
                    StepResult.Settings.StepHeightTarget?.Micrometers,
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

            if (PointSelector.SelectedOutput == StepHeightOutputName)
            {
                HeatMapPaletteType paletteType;
                double? target;
                double? tolerance;
                string unit;

                paletteType = StepResult.Settings.StepHeightTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                target = StepResult.Settings.StepHeightTarget?.Micrometers;
                tolerance = StepResult.Settings.StepHeightTolerance?.GetAbsoluteTolerance(StepResult.Settings.StepHeightTarget).Micrometers;
                unit = Length.GetUnitSymbol(LengthUnit.Micrometer);

                // Global HeatMap
                HeatMapVM.SetTitle(PointSelector.SelectedOutput);
                HeatMapVM.SetTargetTolerance(target, tolerance);
                HeatMapVM.SetUnit(unit);
                HeatMapVM.SetPaletteType(paletteType);

                // Die HeatMap
                DieMap.SetTargetTolerance(target, tolerance);
                DieMap.SetUnit(unit);
                DieMap.SetPaletteType(paletteType);
                DieMap.SetInterpolationEngine(CreateInterpolationEngine(true)); // note RTi voir comment alimenter l'engine du die avec l'interpolator de l'engine du wafer circle !
            }

            if (InterpolationEngine == null) return;

            InterpolationEngine.InterpolationDone += OnInterpolationDone;

            double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(StepResult);
            InterpolationEngine.InterpolateCircle(PointSelector.AllPoints.OfType<StepPointResult>().ToList(), waferDiameter);
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

        public override string FormatName => "Step";

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

            bool hasStepHeight = false;

            if (StepResult.Settings?.StepHeightTarget == null || StepResult.Settings?.StepHeightTolerance == null)
            {
                throw new ArgumentNullException(nameof(StepResult.Settings), "Settings of step result are null");
            }

            if (StepResult != null)
            {
                hasStepHeight = StepResult.Settings.StepHeightTarget != null;

                if (hasStepHeight) newSource.Add(StepHeightOutputName);

                ViewerTypeSource = newSource;
            }

            _stepPointsListVM.UpdateStepHeightVisibility(hasStepHeight);

            DetailMeasureInfo.Settings = StepResult.Settings;
            DataRepeta.UpdateOutputSource(newSource);

            HeatMapVM.Update(StepResult);

            DieMap.StepResult = StepResult;

            UpdateGlobalChartData();

            OnPropertyChanged(nameof(StepResult));

            PointSelector.SetOutputAndRaiseEvents(newSource.FirstOrDefault());

            MeasureTypeCategories.UpdateCategories(points);
            PointsLocation.PopulatePointsLocationCollection();
        }

        #endregion

        #region Private Methods

        private void UpdateGlobalChartData()
        {
            GlobalStatsChart.ClearAll();

            var settings = StepResult?.Settings;
            if (settings == null)
            {
                return;
            }

            bool hasStepHeight = settings.StepHeightTarget != null;

            if (hasStepHeight)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is StepPointResult result ? result.StepHeightStat?.Mean?.Micrometers : null,
                    pair => pair.Value is StepPointResult result ? result.StepHeightStat?.Min?.Micrometers : null,
                    pair => pair.Value is StepPointResult result ? result.StepHeightStat?.Max?.Micrometers : null,
                    pair => pair.Value is StepPointResult result && result.StepHeightStat != null
                        ? MetroHelper.GetSymbol(result.StepHeightStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    StepHeightOutputName,
                    null, 14.0, $"{Length.GetUnitSymbol(PointSelector.CurrentUnit)}", 12.0);

                double target = settings.StepHeightTarget.Micrometers;
                var tolerance = settings.StepHeightTolerance.GetAbsoluteTolerance(settings.StepHeightTarget);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers, StepHeightOutputName);
            }
        }

        private void UpdateSelectedPointsOutputChartData()
        {
            var measurePointResults = PointSelector.CheckedPoints.OfType<StepPointResult>().ToList();

            if (PointSelector.SelectedOutput == StepHeightOutputName)
            {
                measurePointResults = measurePointResults.OrderBy(point => point.StepHeightStat.State).ToList();

                SelectedPointsOutputChart.SetData(measurePointResults,
                    point => PointSelector.PointToIndex[point] + 1,
                    point => point.StepHeightStat.Mean?.Micrometers,
                    point => point.StepHeightStat.Min?.Micrometers,
                    point => point.StepHeightStat.Max?.Micrometers,
                    point => MetroHelper.GetSymbol(point.StepHeightStat.State));
            }
        }

        private void UpdateSelectedPointsOutputChartSelection()
        {
            if (PointSelector.SingleSelectedPoint is StepPointResult point)
            {
                if (PointSelector.SelectedOutput == StepHeightOutputName)
                {
                    double xValue = PointSelector.PointToIndex[point] + 1;
                    if (point.StepHeightStat.Mean != null)
                    {

                        double yValue = point.StepHeightStat.Mean.Micrometers;
                        SelectedPointsOutputChart.UpdateSelectedPoint(xValue, yValue);
                    }
                    else
                    {
                        SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
                    }
                }
            }
            else
            {
                SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
            }
        }

        private void UpdateSelectedPointOutputRepetaChartData()
        {
            if (PointSelector.SelectedOutput == StepHeightOutputName)
            {
                var pointData = PointSelector.CurrentRepetaPoints.OfType<StepPointData>().OrderBy(data => data.State).ToList();

                SelectedPointOutputRepetaChart.SetData(pointData,
                    data => data.IndexRepeta + 1,
                    data => data.StepHeight?.Micrometers,
                    data => MetroHelper.GetSymbol(data.State));

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
        }

        private void UpdateChartsTargetAndTolerance()
        {
            var settings = StepResult?.Settings;

            if (settings == null)
            {
                SelectedPointsOutputChart.ClearTargetAndTolerance();
                SelectedPointOutputRepetaChart.ClearTargetAndTolerance();
                return;
            }
            RawSignalChart.UpdateYAxisTitle($"Height ({Length.GetUnitSymbol(PointSelector.CurrentUnit)})");
            SelectedPointsOutputChart.UpdateYAxisTitle($"{PointSelector.SelectedOutput} ({Length.GetUnitSymbol(PointSelector.CurrentUnit)})");
            SelectedPointOutputRepetaChart.UpdateYAxisTitle($"{PointSelector.SelectedOutput} ({Length.GetUnitSymbol(PointSelector.CurrentUnit)})");

            if (PointSelector.SelectedOutput == StepHeightOutputName)
            {
                if (settings.StepHeightTarget == null || settings.StepHeightTolerance == null)
                {
                    SelectedPointsOutputChart.ClearTargetAndTolerance();
                    return;
                }

                double target = settings.StepHeightTarget.Micrometers;
                var tolerance = settings.StepHeightTolerance.GetAbsoluteTolerance(settings.StepHeightTarget);
                SelectedPointsOutputChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers);
                SelectedPointOutputRepetaChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers);
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

            _stepPointsListVM.Dispose();
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
