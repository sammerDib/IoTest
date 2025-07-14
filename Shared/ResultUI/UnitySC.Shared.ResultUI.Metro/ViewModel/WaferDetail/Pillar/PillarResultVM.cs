using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.RawSignal;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar.DieDetails;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar
{
    public class PillarResultVM : MetroResultVM
    {
        public const string HeightOutputName = "Height";
        public const string WidthOutputName = "Width";

        #region Properties

        public PillarResult PillarResult => MetroResult.MeasureResult as PillarResult;

        public PillarPointSelector PointSelector { get; } = new PillarPointSelector();

        public PillarGlobalStatsVM GlobalStats { get; }

        public PillarDetailMeasureInfoVM DetailMeasureInfo { get; }

        public PillarDataRepetaVM DataRepeta { get; }

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

        private InterpolationEngine<PillarPointResult> InterpolationEngine { get; set; }

        private readonly Dictionary<string, InterpolationEngine<PillarPointResult>> _interpolationEngines = new Dictionary<string, InterpolationEngine<PillarPointResult>>();

        #region Overrides of MetroResultVM

        private readonly PillarPointsListVM _pillarPointsListVM;

        public override MetroPointsListVM ResultPointsList => _pillarPointsListVM;

        #endregion

        #endregion

        public PillarResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _pillarPointsListVM = new PillarPointsListVM(PointSelector) { Digits = Digits };
            GlobalStats = new PillarGlobalStatsVM(PointSelector);
            DetailMeasureInfo = new PillarDetailMeasureInfoVM
            {
                Digits = Digits
            };
            DataRepeta = new PillarDataRepetaVM(PointSelector) { Digits = Digits };
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
            DetailMeasureInfo.Point = PointSelector.SingleSelectedPoint as PillarPointResult;
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
            DetailMeasureInfo.Settings = PillarResult.Settings;

            // Update Digits number
            switch (PointSelector.SelectedOutput)
            {
                case HeightOutputName:
                    {
                        var outputSettings = PillarResult?.Settings;
                        Digits = MetroHelper.GetDecimalCount(outputSettings?.HeightTarget?.Value, outputSettings?.HeightTolerance?.Value);
                    }
                    break;

                case WidthOutputName:
                    {
                        var outputSettings = PillarResult?.Settings;
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

        private InterpolationEngine<PillarPointResult> CreateInterpolationEngine(bool dieMode)
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return null;

            // we kept only circle interpolator
            if (!dieMode && _interpolationEngines.TryGetValue(PointSelector.SelectedOutput, out var interpolationEngine))
            {
                return interpolationEngine;
            }

            InterpolationEngine<PillarPointResult> newInterpEngine;
            if (PointSelector.SelectedOutput == HeightOutputName)
            {
                newInterpEngine = new InterpolationEngine<PillarPointResult>(
                    point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                    point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                    point => point.HeightStat?.Mean?.Micrometers,
                    LengthUnit.Micrometer,
                    PillarResult.Settings.HeightTarget?.Micrometers,
                    dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);

                if (!dieMode)
                {
                    _interpolationEngines.Add(PointSelector.SelectedOutput, newInterpEngine);
                }
                return newInterpEngine;
            }

            if (PointSelector.SelectedOutput == WidthOutputName)
            {
                newInterpEngine = new InterpolationEngine<PillarPointResult>(
                    point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                    point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                    point => point.WidthStat?.Mean?.Micrometers,
                    LengthUnit.Micrometer,
                    PillarResult.Settings.WidthTarget?.Micrometers,
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
                case HeightOutputName:
                    {
                        paletteType = PillarResult.Settings.HeightTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                        target = PillarResult.Settings.HeightTarget?.Micrometers;
                        tolerance = PillarResult.Settings.HeightTolerance?.GetAbsoluteTolerance(PillarResult.Settings.HeightTarget).Micrometers;
                        unit = Length.GetUnitSymbol(LengthUnit.Micrometer);
                        break;
                    }
                case WidthOutputName:
                    {
                        paletteType = PillarResult.Settings.WidthTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                        target = PillarResult.Settings.WidthTarget?.Micrometers;
                        tolerance = PillarResult.Settings.WidthTolerance?.GetAbsoluteTolerance(PillarResult.Settings.WidthTarget).Micrometers;
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
            DieMap.SetTargetTolerance(target, tolerance);
            DieMap.SetUnit(unit);
            DieMap.SetPaletteType(paletteType);
            DieMap.SetInterpolationEngine(CreateInterpolationEngine(true)); // note RTi voir comment alimenter l'engine du die avec l'interpolator de l'engine du wafer circle !

            if (InterpolationEngine == null) return;

            InterpolationEngine.InterpolationDone += OnInterpolationDone;

            double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(PillarResult);
            InterpolationEngine.InterpolateCircle(PointSelector.AllPoints.OfType<PillarPointResult>().ToList(), waferDiameter);
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

        public override string FormatName => "Pillar";

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
            
            bool hasHeight = false;
            bool hasWidth = false;

            if (PillarResult.Settings?.HeightTarget == null || PillarResult.Settings?.HeightTolerance == null ||
                PillarResult.Settings?.WidthTarget == null || PillarResult.Settings?.WidthTolerance == null)
            {
                throw new ArgumentNullException(nameof(PillarResult.Settings), "Some settings of pillar result are null");
            }

            if (PillarResult != null)
            {
                hasHeight = PillarResult.Settings.HeightTarget != null;
                if (hasHeight) newSource.Add(HeightOutputName);

                hasWidth = PillarResult.Settings.WidthTarget != null;
                if (hasWidth) newSource.Add(WidthOutputName);

                ViewerTypeSource = newSource;
            }

            _pillarPointsListVM.UpdateHeightAndWidthVisibility(hasHeight, hasWidth);

            DetailMeasureInfo.Settings = PillarResult?.Settings;
            DataRepeta.UpdateOutputSource(newSource);

            HeatMapVM.Update(PillarResult);

            DieMap.PillarResult = PillarResult;

            UpdateGlobalChartData();

            OnPropertyChanged(nameof(PillarResult));

            PointSelector.SetOutputAndRaiseEvents(newSource.FirstOrDefault());

            MeasureTypeCategories.UpdateCategories(points);
            PointsLocation.PopulatePointsLocationCollection();
        }

        #endregion

        #region Private Methods

        private void UpdateGlobalChartData()
        {
            GlobalStatsChart.ClearAll();

            var settings = PillarResult?.Settings;
            if (settings == null)
            {
                return;
            }

            bool hasHeight = settings.HeightTarget != null;
            bool hasWidth = settings.WidthTarget != null;
        
            if (hasHeight)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is PillarPointResult result ? result.HeightStat?.Mean?.Micrometers : null,
                    pair => pair.Value is PillarPointResult result ? result.HeightStat?.Min?.Micrometers : null,
                    pair => pair.Value is PillarPointResult result ? result.HeightStat?.Max?.Micrometers : null,
                    pair => pair.Value is PillarPointResult result && result.HeightStat != null
                        ? MetroHelper.GetSymbol(result.HeightStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    HeightOutputName,
                    null, 14.0, $"{Length.GetUnitSymbol(PointSelector.CurrentUnit)}", 12.0);

                double target = settings.HeightTarget.Micrometers;
                var tolerance = settings.HeightTolerance.GetAbsoluteTolerance(settings.HeightTarget);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance.Micrometers, target + tolerance.Micrometers, HeightOutputName);
            }

            if (hasWidth)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is PillarPointResult result ? result.WidthStat?.Mean?.Micrometers : null,
                    pair => pair.Value is PillarPointResult result ? result.WidthStat?.Min?.Micrometers : null,
                    pair => pair.Value is PillarPointResult result ? result.WidthStat?.Max?.Micrometers : null,
                    pair => pair.Value is PillarPointResult result && result.WidthStat != null
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
            var measurePointResults = PointSelector.CheckedPoints.OfType<PillarPointResult>().ToList();

            switch (PointSelector.SelectedOutput)
            {
                case HeightOutputName:
                    {
                        measurePointResults = measurePointResults.OrderBy(point => point.HeightStat.State).ToList();

                        SelectedPointsOutputChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.HeightStat.Mean?.Micrometers,
                            point => point.HeightStat.Min?.Micrometers,
                            point => point.HeightStat.Max?.Micrometers,
                            point => MetroHelper.GetSymbol(point.HeightStat.State));
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
            if (PointSelector.SingleSelectedPoint is PillarPointResult point)
            {
                switch (PointSelector.SelectedOutput)
                {
                    case HeightOutputName:
                        {
                            double xValue = PointSelector.PointToIndex[point] + 1;
                            if (point.HeightStat.Mean != null)
                            {

                                double yValue = point.HeightStat.Mean.Micrometers;
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
                case HeightOutputName:
                    {
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<PillarPointData>().OrderBy(data => data.HeightState).ToList();

                        SelectedPointOutputRepetaChart.SetData(pointData,
                            data => data.IndexRepeta + 1,
                            data => data.Height?.Micrometers,
                            data => MetroHelper.GetSymbol(data.HeightState));

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
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<PillarPointData>().OrderBy(data => data.WidthState).ToList();

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
            var settings = PillarResult?.Settings;

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
                case HeightOutputName:
                    {
                        if (settings.HeightTarget == null || settings.HeightTolerance == null)
                        {
                            SelectedPointsOutputChart.ClearTargetAndTolerance();
                            return;
                        }

                        double target = settings.HeightTarget.Micrometers;
                        var tolerance = settings.HeightTolerance.GetAbsoluteTolerance(settings.HeightTarget);
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

            _pillarPointsListVM.Dispose();
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
