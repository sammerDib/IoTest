using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Common.ViewModel.Export;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo.DieDetails;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo
{
    public class NanotopoResultVM : MetroResultVM
    {
        public const string RoughnessOutputName = "Roughness";
        public const string StepHeightOutputName = "Step Height";

        #region Properties

        public NanoTopoResult NanoTopoResult => MetroResult.MeasureResult as NanoTopoResult;

        public NanotopoPointSelector PointSelector { get; } = new NanotopoPointSelector();

        public NanotopoGlobalStatsVM GlobalStats { get; }

        public NanotopoDetailMeasureInfoVM DetailMeasureInfo { get; }

        public NanotopoDataRepetaVM DataRepeta { get; }

        public MetroHeatMapVM HeatMapVM { get; }

        public DataInToleranceLineChart GlobalStatsChart { get; }

        public DataInToleranceLineChart SelectedPointsOutputChart { get; }

        public DataInToleranceLineChart SelectedPointOutputRepetaChart { get; }

        public DieMapVM DieMap { get; }

        public DieStatsVM DieStats { get; }

        public MeasureTypeCategoriesVM MeasureTypeCategories { get; }

        public PointsLocationVM PointsLocation { get; }

        private List<string> _viewerTypeSource;

        public List<string> ViewerTypeSource
        {
            get { return _viewerTypeSource; }
            private set { SetProperty(ref _viewerTypeSource, value); }
        }

        private InterpolationEngine<NanoTopoPointResult> InterpolationEngine { get; set; }

        private readonly Dictionary<string, InterpolationEngine<NanoTopoPointResult>> _interpolationEngines = new Dictionary<string, InterpolationEngine<NanoTopoPointResult>>();

        #region Overrides of MetroResultVM

        private readonly NanotopoPointsListVM _nanotopoPointsListVM;

        public override MetroPointsListVM ResultPointsList => _nanotopoPointsListVM;

        #endregion

        #endregion

        public NanotopoResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _nanotopoPointsListVM = new NanotopoPointsListVM(PointSelector) { Digits = Digits };
            GlobalStats = new NanotopoGlobalStatsVM(PointSelector);
            ThumbnailViewerVm = new MatrixThumbnailViewerVM(PointSelector, point => point is NanoTopoPointData nanoTopoPoint ? nanoTopoPoint.ResultImageFileName : null);
            ReportVm = new NanotopoReportVM(PointSelector);
            DetailMeasureInfo = new NanotopoDetailMeasureInfoVM { Digits = Digits };
            DataRepeta = new NanotopoDataRepetaVM(PointSelector) { Digits = Digits };
            HeatMapVM = new MetroHeatMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Circle);
            DieMap = new DieMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Square);
            DieStats = new DieStatsVM(PointSelector);
            MeasureTypeCategories = new MeasureTypeCategoriesVM(PointSelector);
            PointsLocation = new PointsLocationVM(PointSelector);

            ExportResultVM.AdditionalEntries.Add(new ExportEntry(MetroExportResult.PdfExport));

            GlobalStatsChart = new DataInToleranceLineChart("N°", true, Colors.MediumPurple, false);
            SelectedPointsOutputChart = new DataInToleranceLineChart("N°", "Various", true, Colors.MediumPurple, false);
            SelectedPointOutputRepetaChart = new DataInToleranceLineChart("N°", "Various", false, Colors.MediumSeaGreen, false);

            PointSelector.SelectedPointChanged += PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;
            PointSelector.SelectedOutputChanged += PointSelectorOnSelectedOutputChanged;
            PointSelector.CurrentRepetaPointsChanged += PointSelectorCurrentRepetaPointsChanged;

            SelectedPointsOutputChart.PointSelected += SelectedPointsChartOnPointSelected;
        }
        
        #region Event Handlers

        private void PointSelectorOnSelectedPointChanged(object sender, EventArgs e)
        {
            DetailMeasureInfo.Point = PointSelector.SingleSelectedPoint as NanoTopoPointResult;
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

            // Update Digits number
            switch (PointSelector.SelectedOutput)
            {
                case RoughnessOutputName:
                    {
                        var outputSettings = NanoTopoResult?.Settings;
                        Digits = MetroHelper.GetDecimalCount(outputSettings?.RoughnessTarget?.Value, outputSettings?.RoughnessTolerance?.Value);
                        break;
                    }
                case StepHeightOutputName:
                    {
                        var outputSettings = NanoTopoResult?.Settings;
                        Digits = MetroHelper.GetDecimalCount(outputSettings?.StepHeightTarget?.Value, outputSettings?.StepHeightTolerance?.Value);
                        break;
                    }
                default:
                    {
                        var outputSettings = NanoTopoResult?.Settings?.ExternalProcessingOutputs?.FirstOrDefault(output => output.Name == PointSelector.SelectedOutput);
                        Digits = MetroHelper.GetDecimalCount(outputSettings?.OutputTarget, outputSettings?.OutputTolerance?.Value);
                        break;
                    }
            }

            UpdateSelectedPointsOutputChartData();
            UpdateSelectedPointsOutputChartSelection();
            UpdateSelectedPointOutputRepetaChartData();

            UpdateChartsTargetAndTolerance();
            UpdateGlobalChartData();

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

        private InterpolationEngine<NanoTopoPointResult> CreateInterpolationEngine(bool dieMode)
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return null;

            // we kept only circle interpolator
            if (!dieMode && _interpolationEngines.TryGetValue(PointSelector.SelectedOutput, out var interpolationEngine))
            {
                return interpolationEngine;
            }

            InterpolationEngine<NanoTopoPointResult> newInterpEngine;
            switch (PointSelector.SelectedOutput)
            {
                case RoughnessOutputName:
                    {
                        newInterpEngine = new InterpolationEngine<NanoTopoPointResult>(
                            point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                            point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                            point => point.RoughnessStat?.Mean?.Nanometers,
                            LengthUnit.Nanometer,
                            NanoTopoResult.Settings.RoughnessTarget?.Nanometers,
                            dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);
                    }
                    break;
                case StepHeightOutputName:
                    {
                        newInterpEngine = new InterpolationEngine<NanoTopoPointResult>(
                            point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                            point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                            point => point.StepHeightStat?.Mean?.Nanometers,
                            LengthUnit.Nanometer,
                            NanoTopoResult.Settings.StepHeightTarget?.Nanometers,
                            dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle) ;
                    }
                    break;
                default:
                    {
                        var outputSetting = NanoTopoResult?.Settings?.ExternalProcessingOutputs.FirstOrDefault(output => output.Name.Equals(PointSelector.SelectedOutput));
                        newInterpEngine = new InterpolationEngine<NanoTopoPointResult>(
                            point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                            point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                            point =>
                            {
                                if (point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer))
                                {
                                    return statsContainer.Mean;
                                }

                                return null;
                            },
                            LengthUnit.Undefined,
                            outputSetting?.OutputTarget,
                            dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);
                    }
                    break;
            }

            // we kept only circle interpolator
            if (!dieMode)
            {
                _interpolationEngines.Add(PointSelector.SelectedOutput, newInterpEngine);
            }
            return newInterpEngine;
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
                case RoughnessOutputName:
                    {
                        paletteType = NanoTopoResult.Settings.RoughnessTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                        target = NanoTopoResult.Settings.RoughnessTarget?.Nanometers;
                        tolerance = NanoTopoResult.Settings.RoughnessTolerance.GetAbsoluteTolerance(NanoTopoResult.Settings.RoughnessTarget).Nanometers;
                        unit = Length.GetUnitSymbol(LengthUnit.Nanometer);
                        break;
                    }
                case StepHeightOutputName:
                    {
                        paletteType = NanoTopoResult.Settings.StepHeightTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                        target = NanoTopoResult.Settings.StepHeightTarget?.Nanometers;
                        tolerance = NanoTopoResult.Settings.StepHeightTolerance?.GetAbsoluteTolerance(NanoTopoResult.Settings.StepHeightTarget).Nanometers;
                        unit = Length.GetUnitSymbol(LengthUnit.Nanometer);
                        break;
                    }
                default:
                    {
                        var outputSetting = NanoTopoResult?.Settings?.ExternalProcessingOutputs.FirstOrDefault(output => output.Name.Equals(PointSelector.SelectedOutput));
                        if (outputSetting != null)
                        {
                            paletteType = HeatMapPaletteType.TargetTolerance;
                            target = outputSetting.OutputTarget;
                            tolerance = outputSetting.OutputTolerance?.GetAbsoluteTolerance(outputSetting.OutputTarget);
                        }
                        else
                        {
                            paletteType = HeatMapPaletteType.MinMax;
                            target = null;
                            tolerance = null;
                        }

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

            if (InterpolationEngine != null)
            {
                InterpolationEngine.InterpolationDone += OnInterpolationDone;

                double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(NanoTopoResult);
                InterpolationEngine.InterpolateCircle(PointSelector.AllPoints.OfType<NanoTopoPointResult>().ToList(), waferDiameter);
            }
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

        public override string FormatName => "Nanotopo";

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

            bool hasRoughness = false;
            bool hasStepHeight = false;

            if (NanoTopoResult.Settings == null || ((NanoTopoResult.Settings.StepHeightTarget == null || NanoTopoResult.Settings.StepHeightTolerance == null) &&
                (NanoTopoResult.Settings.RoughnessTarget == null || NanoTopoResult.Settings.RoughnessTolerance == null) && NanoTopoResult.Settings.ExternalProcessingOutputs.IsNullOrEmpty()))
            {
                throw new ArgumentNullException(nameof(NanoTopoResult.Settings), "Some settings of Nanotopo result are null");
            }

            if (NanoTopoResult != null)
            {
                hasRoughness = NanoTopoResult.Settings.RoughnessTarget != null;
                if (hasRoughness) newSource.Add(RoughnessOutputName);
                
                hasStepHeight = NanoTopoResult.Settings.StepHeightTarget != null;
                if (hasStepHeight) newSource.Add(StepHeightOutputName);
                
                if(NanoTopoResult.Settings.ExternalProcessingOutputs != null)
                    newSource.AddRange(NanoTopoResult.Settings.ExternalProcessingOutputs.Select(output => output.Name));
                ViewerTypeSource = newSource;
            }

            HasThumbnail = points.Any(result => result.Datas.Any(data => data is NanoTopoPointData nanotopoData && !string.IsNullOrWhiteSpace(nanotopoData.ResultImageFileName)));

            HasReport = points.Any(result => result.Datas.Any(data => data is NanoTopoPointData nanotopoData && !string.IsNullOrWhiteSpace(nanotopoData.ReportFileName)));

            _nanotopoPointsListVM.UpdateRoughnessAndStepHeightVisibility(hasRoughness, hasStepHeight);

            DetailMeasureInfo.Settings = NanoTopoResult?.Settings;
            DataRepeta.UpdateOutputSource(newSource);

            HeatMapVM.Update(NanoTopoResult);

            DieMap.NanotopoResult = NanoTopoResult;

            UpdateGlobalChartData();

            OnPropertyChanged(nameof(NanoTopoResult));

            PointSelector.SetOutputAndRaiseEvents(newSource.FirstOrDefault());

            MeasureTypeCategories.UpdateCategories(points);
            PointsLocation.PopulatePointsLocationCollection();
        }

        #endregion

        #region Private Methods

        private bool IsStepOrRoughness
        {
            get => PointSelector.SelectedOutput == StepHeightOutputName ||
                   PointSelector.SelectedOutput == RoughnessOutputName;
        }

        private string GetUnitOrEmptyString(bool isStepOrRoughness) => isStepOrRoughness ? $" ({Length.GetUnitSymbol(LengthUnit.Nanometer)})" : string.Empty;

        private void UpdateGlobalChartData()
        {
            GlobalStatsChart.ClearAll();

            var settings = NanoTopoResult?.Settings;
            if (settings == null)
            {
                return;
            }

            bool hasRoughness = settings.RoughnessTarget != null;
            bool hasStepHeight = settings.StepHeightTarget != null;

            int takeOutput = 3;
            if (hasRoughness) takeOutput--;
            if (hasStepHeight) takeOutput--;

            var externalProcessingOutputs = settings.ExternalProcessingOutputs?.Take(takeOutput).ToList();

            if (hasRoughness)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is NanoTopoPointResult result ? result.RoughnessStat?.Mean?.Nanometers : null,
                    pair => pair.Value is NanoTopoPointResult result ? result.RoughnessStat?.Min?.Nanometers : null,
                    pair => pair.Value is NanoTopoPointResult result ? result.RoughnessStat?.Max?.Nanometers : null,
                    pair => pair.Value is NanoTopoPointResult result && result.RoughnessStat != null
                        ? MetroHelper.GetSymbol(result.RoughnessStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    RoughnessOutputName,
                    null, 12.0, $"{Length.GetUnitSymbol(LengthUnit.Nanometer)}", 10.0);

                double target = settings.RoughnessTarget.Nanometers;
                var tolerance = settings.RoughnessTolerance.GetAbsoluteTolerance(settings.RoughnessTarget);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance.Nanometers,
                    target + tolerance.Nanometers, RoughnessOutputName);
            }

            if (hasStepHeight)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is NanoTopoPointResult result ? result.StepHeightStat?.Mean?.Nanometers : null,
                    pair => pair.Value is NanoTopoPointResult result ? result.StepHeightStat?.Min?.Nanometers : null,
                    pair => pair.Value is NanoTopoPointResult result ? result.StepHeightStat?.Max?.Nanometers : null,
                    pair => pair.Value is NanoTopoPointResult result && result.StepHeightStat != null
                        ? MetroHelper.GetSymbol(result.StepHeightStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    StepHeightOutputName,
                    null, 12.0, $"{Length.GetUnitSymbol(LengthUnit.Nanometer)}", 10.0);

                double target = settings.StepHeightTarget.Nanometers;
                var tolerance = settings.StepHeightTolerance.GetAbsoluteTolerance(settings.StepHeightTarget);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance.Nanometers,
                    target + tolerance.Nanometers, StepHeightOutputName);
            }

            if (externalProcessingOutputs == null) return;

            foreach (var output in externalProcessingOutputs)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is NanoTopoPointResult result && result.ExternalProcessingStats.TryGetValue(output.Name, out var outputStat) ? (double?)outputStat.Mean : null,
                    pair => pair.Value is NanoTopoPointResult result && result.ExternalProcessingStats.TryGetValue(output.Name, out var outputStat) ? (double?)outputStat.Min : null,
                    pair => pair.Value is NanoTopoPointResult result && result.ExternalProcessingStats.TryGetValue(output.Name, out var outputStat) ? (double?)outputStat.Max : null,
                    pair => pair.Value is NanoTopoPointResult result && result.ExternalProcessingStats.TryGetValue(output.Name, out var outputStat)
                        ? MetroHelper.GetSymbol(outputStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    output.Name,
                    null, 12.0);

                double target = output.OutputTarget;
                double tolerance = output.OutputTolerance.GetAbsoluteTolerance(output.OutputTarget);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance, output.Name);
            }

        }

        private void UpdateSelectedPointsOutputChartData()
        {
            var measurePointResults = PointSelector.CheckedPoints.OfType<NanoTopoPointResult>().ToList();
            if (PointSelector.SelectedOutput is null)
                return;
            switch (PointSelector.SelectedOutput)
            {
                case RoughnessOutputName:
                    {
                        measurePointResults = measurePointResults.OrderBy(point => point.RoughnessStat.State).ToList();

                        SelectedPointsOutputChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.RoughnessStat.Mean?.Nanometers,
                            point => point.RoughnessStat.Min?.Nanometers,
                            point => point.RoughnessStat.Max?.Nanometers,
                            point => MetroHelper.GetSymbol(point.RoughnessStat.State));
                        break;
                    }
                case StepHeightOutputName:
                    {
                        measurePointResults = measurePointResults.OrderBy(point => point.StepHeightStat.State).ToList();

                        SelectedPointsOutputChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.StepHeightStat.Mean?.Nanometers,
                            point => point.StepHeightStat.Min?.Nanometers,
                            point => point.StepHeightStat.Max?.Nanometers,
                            point => MetroHelper.GetSymbol(point.StepHeightStat.State));
                        break;
                    }
                default:
                    {
                        measurePointResults = measurePointResults.OrderBy(point =>
                            point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? statsContainer.State : MeasureState.NotMeasured).ToList();

                        SelectedPointsOutputChart.SetData(measurePointResults,
                            point => PointSelector.PointToIndex[point] + 1,
                            point => point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? (double?)statsContainer.Mean : null,
                            point => point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? (double?)statsContainer.Min : null,
                            point => point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? (double?)statsContainer.Max : null,
                            point => point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? MetroHelper.GetSymbol(statsContainer.State) : MetroHelper.GetSymbol(MeasureState.NotMeasured));
                        break;
                    }
            }
        }

        private void UpdateSelectedPointsOutputChartSelection()
        {
            if (PointSelector.SingleSelectedPoint is NanoTopoPointResult point)
            {
                double xValue = PointSelector.PointToIndex[point] + 1;
                
                switch (PointSelector.SelectedOutput)
                {
                    case RoughnessOutputName:
                        if (point.RoughnessStat.Mean != null)
                        {
                            double yValue = point.RoughnessStat.Mean.Nanometers;
                            SelectedPointsOutputChart.UpdateSelectedPoint(xValue, yValue);
                        }
                        else
                        {
                            SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
                        }
                        break;
                    case StepHeightOutputName:
                        if (point.StepHeightStat.Mean != null)
                        {
                            double yValue = point.StepHeightStat.Mean.Nanometers;
                            SelectedPointsOutputChart.UpdateSelectedPoint(xValue, yValue);
                        }
                        else
                        {
                            SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
                        }
                        break;
                    default:
                        if ((!(PointSelector.SelectedOutput is null)) && (point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer)))
                        {
                            SelectedPointsOutputChart.UpdateSelectedPoint(xValue, statsContainer.Mean);
                        }
                        else
                        {
                            SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
                        }
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
                case RoughnessOutputName:
                    {
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<NanoTopoPointData>().OrderBy(data => data.RoughnessState).ToList();

                        SelectedPointOutputRepetaChart.SetData(pointData,
                            data => data.IndexRepeta + 1,
                            data => data.Roughness?.Nanometers,
                            data => MetroHelper.GetSymbol(data.RoughnessState));

                        break;
                    }
                case StepHeightOutputName:
                    {
                        var pointData = PointSelector.CurrentRepetaPoints.OfType<NanoTopoPointData>().OrderBy(data => data.StepHeightState).ToList();

                        SelectedPointOutputRepetaChart.SetData(pointData,
                            data => data.IndexRepeta + 1,
                            data => data.StepHeight?.Nanometers,
                            data => MetroHelper.GetSymbol(data.StepHeightState));
                        break;
                    }
                default:
                    {
                        if (!(PointSelector.SelectedOutput is null))
                        {
                            var pointData = PointSelector.CurrentRepetaPoints.OfType<NanoTopoPointData>().OrderBy(data =>
                                {
                                    var outputResult = data?.ExternalProcessingResults.SingleOrDefault(result => result.Name.Equals(PointSelector.SelectedOutput));
                                    return outputResult?.State ?? MeasureState.NotMeasured;
                                }
                                ).ToList();

                            SelectedPointOutputRepetaChart.SetData(pointData,
                                data => data.IndexRepeta + 1,
                                data =>
                                {
                                    var outputResult = data?.ExternalProcessingResults.SingleOrDefault(result => result.Name.Equals(PointSelector.SelectedOutput));
                                    return outputResult?.Value;
                                },
                                data =>
                                {
                                    var outputResult = data?.ExternalProcessingResults.SingleOrDefault(result => result.Name.Equals(PointSelector.SelectedOutput));
                                    return MetroHelper.GetSymbol(outputResult?.State ?? MeasureState.NotMeasured);
                                });
                        }
                        break;
                    }
            }
        }

        private void UpdateChartsTargetAndTolerance()
        {
            var settings = NanoTopoResult?.Settings;

            if (settings == null)
            {
                SelectedPointsOutputChart.ClearTargetAndTolerance();
                SelectedPointOutputRepetaChart.ClearTargetAndTolerance();
                return;
            }

            string yAxisTitle = $"{PointSelector.SelectedOutput + GetUnitOrEmptyString(IsStepOrRoughness)}";

            SelectedPointsOutputChart.UpdateYAxisTitle(yAxisTitle);
            SelectedPointOutputRepetaChart.UpdateYAxisTitle(yAxisTitle);

            switch (PointSelector.SelectedOutput)
            {
                case RoughnessOutputName:
                    {
                        if (settings.RoughnessTarget == null || settings.RoughnessTolerance == null)
                        {
                            SelectedPointsOutputChart.ClearTargetAndTolerance();
                            return;
                        }

                        double target = settings.RoughnessTarget.Nanometers;
                        var tolerance = settings.RoughnessTolerance.GetAbsoluteTolerance(settings.RoughnessTarget);
                        SelectedPointsOutputChart.SetTargetAndTolerance(target, target - tolerance.Nanometers, target + tolerance.Nanometers);
                        SelectedPointOutputRepetaChart.SetTargetAndTolerance(target, target - tolerance.Nanometers, target + tolerance.Nanometers);
                        break;
                    }
                case StepHeightOutputName:
                    {
                        if (settings.StepHeightTarget == null || settings.StepHeightTolerance == null)
                        {
                            SelectedPointsOutputChart.ClearTargetAndTolerance();
                            return;
                        }

                        double target = settings.StepHeightTarget.Nanometers;
                        var tolerance = settings.StepHeightTolerance.GetAbsoluteTolerance(settings.StepHeightTarget);
                        SelectedPointsOutputChart.SetTargetAndTolerance(target, target - tolerance.Nanometers, target + tolerance.Nanometers);
                        SelectedPointOutputRepetaChart.SetTargetAndTolerance(target, target - tolerance.Nanometers, target + tolerance.Nanometers);
                        break;
                    }
                default:
                    {
                        var outputSetting = settings.ExternalProcessingOutputs?.FirstOrDefault(output => output.Name.Equals(PointSelector.SelectedOutput));
                        if (outputSetting == null || outputSetting.OutputTolerance == null)
                        {
                            SelectedPointsOutputChart.ClearTargetAndTolerance();
                            return;
                        }

                        double target = outputSetting.OutputTarget;
                        double tolerance = outputSetting.OutputTolerance.GetAbsoluteTolerance(outputSetting.OutputTarget);
                        SelectedPointsOutputChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance);
                        SelectedPointOutputRepetaChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance);
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
            PointSelector.SelectedOutputChanged -= PointSelectorOnSelectedOutputChanged;
            PointSelector.CurrentRepetaPointsChanged -= PointSelectorCurrentRepetaPointsChanged;

            SelectedPointsOutputChart.PointSelected -= SelectedPointsChartOnPointSelected;

            _nanotopoPointsListVM.Dispose();
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

            base.Dispose();
        }

        #endregion
    }
}
