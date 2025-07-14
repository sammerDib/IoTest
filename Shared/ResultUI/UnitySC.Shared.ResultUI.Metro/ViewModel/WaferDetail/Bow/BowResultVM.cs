using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.RawSignal;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow
{
    public class BowResultVM : MetroResultVM
    {
        public const string BowOutputName = "Bow";

        #region Properties

        public BowResult BowResult => MetroResult.MeasureResult as BowResult;

        public BowPointSelector PointSelector { get; } = new BowPointSelector();

        public BowGlobalStatsVM GlobalStats { get; }

        public BowDetailMeasureInfoVM DetailMeasureInfo { get; }

        public BowDataRepetaVM DataRepeta { get; }

        public MetroHeatMapVM HeatMapVM { get; }

        public DataInToleranceLineChart GlobalStatsChart { get; }

        public DataInToleranceLineChart SelectedPointsOutputChart { get; }

        public DataInToleranceLineChart SelectedPointOutputRepetaChart { get; }

        public MeasureTypeCategoriesVM MeasureTypeCategories { get; }

        public PointsLocationVM PointsLocation { get; }

        public RawSignalChart RawSignalChart { get; }

        private List<string> _viewerTypeSource;

        public List<string> ViewerTypeSource
        {
            get { return _viewerTypeSource; }
            private set { SetProperty(ref _viewerTypeSource, value); }
        }

        private InterpolationEngine<BowPointResult> InterpolationEngine { get; set; }

        private readonly Dictionary<string, InterpolationEngine<BowPointResult>> _interpolationEngines = new Dictionary<string, InterpolationEngine<BowPointResult>>();

        #region Overrides of MetroResultVM

        private readonly BowPointsListVM _bowPointsListVM;

        public override MetroPointsListVM ResultPointsList => _bowPointsListVM;

        #endregion Overrides of MetroResultVM

        #endregion Properties

        public BowResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _bowPointsListVM = new BowPointsListVM(PointSelector) { Digits = Digits };
            GlobalStats = new BowGlobalStatsVM(PointSelector);
            DetailMeasureInfo = new BowDetailMeasureInfoVM
            {
                Digits = Digits
            };
            DataRepeta = new BowDataRepetaVM(PointSelector) { Digits = Digits };
            HeatMapVM = new MetroHeatMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Circle);

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
            DetailMeasureInfo.Point = PointSelector.SingleSelectedPoint as BowPointResult;
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
            DetailMeasureInfo.Settings = BowResult.Settings;

            // Update Digits number
            switch (PointSelector.SelectedOutput)
            {
                case BowOutputName:
                    {
                        var outputSettings = BowResult?.Settings;
                        Digits = MetroHelper.GetDecimalCount(outputSettings?.BowTargetMax?.Value, outputSettings?.BowTargetMin?.Value);
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

        #endregion Event Handlers

        #region HeatMap

        private InterpolationEngine<BowPointResult> CreateInterpolationEngine(bool dieMode)
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return null;

            // we kept only circle interpolator
            if (!dieMode && _interpolationEngines.TryGetValue(PointSelector.SelectedOutput, out var interpolationEngine))
            {
                return interpolationEngine;
            }

            InterpolationEngine<BowPointResult> newInterpEngine;

            if (PointSelector.SelectedOutput == BowOutputName)
            {
                newInterpEngine = new InterpolationEngine<BowPointResult>(
                    point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                    point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                    point => point.BowStat?.Mean?.Micrometers,
                    LengthUnit.Micrometer,
                    null,
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
            DisposeInterpolationEngine();

            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return;

            InterpolationEngine = CreateInterpolationEngine(false);

            HeatMapPaletteType paletteType;
            double? target;
            double? tolerance;
            double specMin = 0.0;
            double specMax = 0.0;
            string unit;

            switch (PointSelector.SelectedOutput)
            {
                case BowOutputName:
                    {
                        paletteType = HeatMapPaletteType.SpecValues;
                        target = double.NaN;
                        tolerance = null;
                        unit = Length.GetUnitSymbol(LengthUnit.Micrometer);
                        specMin = BowResult.Settings.BowTargetMin?.Micrometers ?? -1.0;
                        specMax = BowResult.Settings.BowTargetMax?.Micrometers ?? 1.0;
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
            HeatMapVM.SetSpecMinAndMax(specMin, specMax);
            if (InterpolationEngine == null) return;

            InterpolationEngine.InterpolationDone += OnInterpolationDone;

            double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(BowResult);
            InterpolationEngine.InterpolateCircle(PointSelector.AllPoints.OfType<BowPointResult>().ToList(), waferDiameter);
        }

        private void OnInterpolationDone(object sender, EventArgs e)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                HeatMapVM.SetMinMax(InterpolationEngine.CurrentMinValue, InterpolationEngine.CurrentMaxValue);
                HeatMapVM.SetInterpolationResult(InterpolationEngine.InterpolationResults);
            });
        }

        private void DisposeInterpolationEngine()
        {
            if (InterpolationEngine != null)
            {
                InterpolationEngine.InterpolationDone -= OnInterpolationDone;
                InterpolationEngine.Cancel();
                InterpolationEngine = null;
            }
        }

        #endregion HeatMap

        #region Overrides of ResultWaferVM

        public override string FormatName => "Bow";

        #endregion Overrides of ResultWaferVM

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

            #endregion Cancel previous interpolations

            var newSource = new List<string>();

            bool hasBow = false;

            if (BowResult.Settings?.BowTargetMin == null || BowResult.Settings?.BowTargetMax == null)
            {
                throw new ArgumentNullException(nameof(BowResult.Settings), "Some settings of bow result are null");
            }

            if (BowResult != null)
            {
                hasBow = (BowResult.Settings.BowTargetMin != null && BowResult.Settings.BowTargetMax != null);
                if (hasBow) newSource.Add(BowOutputName);

                ViewerTypeSource = newSource;
            }

            _bowPointsListVM.UpdateBowVisibility(hasBow);

            DetailMeasureInfo.Settings = BowResult?.Settings;
            DataRepeta.UpdateOutputSource(newSource);

            HeatMapVM.Update(BowResult);

            UpdateGlobalChartData();

            OnPropertyChanged(nameof(BowResult));

            PointSelector.SetOutputAndRaiseEvents(newSource.FirstOrDefault());

            MeasureTypeCategories.UpdateCategories(points);
            PointsLocation.PopulatePointsLocationCollection();
        }

        #endregion Overrides of ResultWaferVM

        #region Private Methods

        private void UpdateGlobalChartData()
        {
            GlobalStatsChart.ClearAll();

            var settings = BowResult?.Settings;
            if (settings == null)
            {
                return;
            }

            bool hasBow = (settings.BowTargetMax != null && settings.BowTargetMin != null);

            if (hasBow)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is BowPointResult result ? result.BowStat?.Mean?.Micrometers : null,
                    pair => pair.Value is BowPointResult result ? result.BowStat?.Min?.Micrometers : null,
                    pair => pair.Value is BowPointResult result ? result.BowStat?.Max?.Micrometers : null,
                    pair => pair.Value is BowPointResult result && result.BowStat != null
                        ? MetroHelper.GetSymbol(result.BowStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    BowOutputName,
                    null, 14.0, $"{Length.GetUnitSymbol(PointSelector.CurrentUnit)}", 12.0);

                GlobalStatsChart.SetTargetMinAndMax(settings.BowTargetMin.Micrometers, settings.BowTargetMax.Micrometers);
            }
        }

        private void UpdateSelectedPointsOutputChartData()
        {
            var measurePointResults = PointSelector.CheckedPoints.OfType<BowPointResult>().ToList();

            switch (PointSelector.SelectedOutput)
            {
                case BowOutputName:
                    {
                        measurePointResults = measurePointResults.OrderBy(point => point.BowStat.State).ToList();

                        SelectedPointsOutputChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.BowStat.Mean?.Micrometers,
                            point => point.BowStat.Min?.Micrometers,
                            point => point.BowStat.Max?.Micrometers,
                            point => MetroHelper.GetSymbol(point.BowStat.State));
                        break;
                    }
                default: break;
            }
        }

        private void UpdateSelectedPointsOutputChartSelection()
        {
            if (PointSelector.SingleSelectedPoint is BowPointResult point)
            {
                switch (PointSelector.SelectedOutput)
                {
                    case BowOutputName:
                        {
                            double xValue = PointSelector.PointToIndex[point] + 1;
                            if (point.BowStat.Mean != null)
                            {
                                double yValue = point.BowStat.Mean.Micrometers;
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
                case BowOutputName:
                    {
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<BowTotalPointData>().OrderBy(data => data.State).ToList(); // TODO Bow: to verify

                        SelectedPointOutputRepetaChart.SetData(pointData,
                            data => data.IndexRepeta + 1,
                            data => data.Bow?.Micrometers,
                            data => MetroHelper.GetSymbol(data.State));
                    }
                    break;

                default:
                    break;
            }
        }

        private void UpdateChartsTargetAndTolerance()
        {
            var settings = BowResult?.Settings;

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
                case BowOutputName:
                    {
                        if (settings.BowTargetMax == null || settings.BowTargetMin == null)
                        {
                            SelectedPointsOutputChart.ClearTargetAndTolerance();
                            return;
                        }

                        SelectedPointsOutputChart.SetTargetMinAndMax(settings.BowTargetMin.Micrometers, settings.BowTargetMax.Micrometers);
                        SelectedPointOutputRepetaChart.SetTargetMinAndMax(settings.BowTargetMin.Micrometers, settings.BowTargetMax.Micrometers);
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion Private Methods

        #region Overrides of MetroResultVM

        public override void Dispose()
        {
            PointSelector.SelectedPointChanged -= PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged -= PointSelectorOnCheckedPointsChanged;
            PointSelector.SelectedOutputChanged -= PointSelectorOnSelectedOutputChanged;
            PointSelector.CurrentRepetaPointsChanged -= PointSelectorCurrentRepetaPointsChanged;

            SelectedPointsOutputChart.PointSelected -= SelectedPointsChartOnPointSelected;

            _bowPointsListVM.Dispose();
            GlobalStats.Dispose();
            DataRepeta.Dispose();
            HeatMapVM.Dispose();

            MeasureTypeCategories.Dispose();
            PointsLocation.Dispose();

            GlobalStatsChart.Dispose();
            SelectedPointsOutputChart.Dispose();
            SelectedPointOutputRepetaChart.Dispose();

            RawSignalChart.Dispose();

            DisposeInterpolationEngine();

            base.Dispose();
        }

        #endregion Overrides of MetroResultVM
    }
}
