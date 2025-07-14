using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography.DieDetails;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography
{
    public class TopographyResultVM : MetroResultVM
    {
        #region Properties

        public TopographyResult TopographyResult => MetroResult.MeasureResult as TopographyResult;

        public TopographyPointSelector PointSelector { get; } = new TopographyPointSelector();

        public TopographyGlobalStatsVM GlobalStats { get; }

        public TopographyDetailMeasureInfoVM DetailMeasureInfo { get; }

        public TopographyDataRepetaVM DataRepeta { get; }

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

        private InterpolationEngine<TopographyPointResult> InterpolationEngine { get; set; }

        private readonly Dictionary<string, InterpolationEngine<TopographyPointResult>> _interpolationEngines = new Dictionary<string, InterpolationEngine<TopographyPointResult>>();

        #region Overrides of MetroResultVM

        private readonly TopographyPointsListVM _topographyPointsListVM;

        public override MetroPointsListVM ResultPointsList => _topographyPointsListVM;

        #endregion

        #endregion

        public TopographyResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _topographyPointsListVM = new TopographyPointsListVM(PointSelector) { Digits = Digits };
            GlobalStats = new TopographyGlobalStatsVM(PointSelector);
            ReportVm = new TopographyReportVM(PointSelector);
            ThumbnailViewerVm = new MatrixThumbnailViewerVM(PointSelector, point => point is TopographyPointData topographyPoint ? topographyPoint.ResultImageFileName : null);
            DetailMeasureInfo = new TopographyDetailMeasureInfoVM { Digits = Digits };
            DataRepeta = new TopographyDataRepetaVM(PointSelector) { Digits = Digits };
            HeatMapVM = new MetroHeatMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Circle);
            DieMap = new DieMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Square);
            DieStats = new DieStatsVM(PointSelector);
            MeasureTypeCategories = new MeasureTypeCategoriesVM(PointSelector);
            PointsLocation = new PointsLocationVM(PointSelector);

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
            DetailMeasureInfo.Point = PointSelector.SingleSelectedPoint as TopographyPointResult;
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
            var outputSettings = TopographyResult?.Settings?.ExternalProcessingOutputs?.FirstOrDefault(output => output.Name == PointSelector.SelectedOutput);
            Digits = MetroHelper.GetDecimalCount(outputSettings?.OutputTarget, outputSettings?.OutputTolerance?.Value);

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

        private InterpolationEngine<TopographyPointResult> CreateInterpolationEngine(bool dieMode)
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return null;

            // we kept only circle interpolator
            if (!dieMode && _interpolationEngines.TryGetValue(PointSelector.SelectedOutput, out var interpolationEngine))
            {
                return interpolationEngine;
            }

            var outputSetting = TopographyResult?.Settings?.ExternalProcessingOutputs.SingleOrDefault(output => output.Name.Equals(PointSelector.SelectedOutput));
            var newInterpEngine = new InterpolationEngine<TopographyPointResult>(
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
            string unit = string.Empty;

            var outputSetting = TopographyResult?.Settings?.ExternalProcessingOutputs.SingleOrDefault(output => output.Name.Equals(PointSelector.SelectedOutput));
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

                double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(TopographyResult);
                InterpolationEngine.InterpolateCircle(PointSelector.AllPoints.OfType<TopographyPointResult>().ToList(), waferDiameter);
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

        public override string FormatName => "Topography";

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

            if (TopographyResult.Settings == null || TopographyResult.Settings.ExternalProcessingOutputs.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(TopographyResult.Settings.ExternalProcessingOutputs), "Settings of topography result are null");
            }

            if (TopographyResult != null)
            {
                newSource.AddRange(TopographyResult.Settings.ExternalProcessingOutputs.Select(output => output.Name));
                ViewerTypeSource = newSource;
            }

            HasThumbnail = points.Any(result => result.Datas.Any(data => data is TopographyPointData topographyData && !string.IsNullOrWhiteSpace(topographyData.ResultImageFileName)));

            HasReport = points.Any(result => result.Datas.Any(data => data is TopographyPointData topographyData && !string.IsNullOrWhiteSpace(topographyData.ReportFileName)));

            DetailMeasureInfo.Settings = TopographyResult?.Settings;
            DataRepeta.UpdateOutputSource(newSource);

            HeatMapVM.Update(TopographyResult);

            DieMap.TopographyResult = TopographyResult;

            UpdateGlobalChartData();

            OnPropertyChanged(nameof(TopographyResult));

            PointSelector.SetOutputAndRaiseEvents(newSource.FirstOrDefault());

            MeasureTypeCategories.UpdateCategories(points);
            PointsLocation.PopulatePointsLocationCollection();
        }

        #endregion

        #region Private Methods

        private void UpdateGlobalChartData()
        {
            GlobalStatsChart.ClearAll();

            var settings = TopographyResult?.Settings;

            int takeOutput = 3;
            var externalProcessingOutputs = settings?.ExternalProcessingOutputs?.Take(takeOutput).ToList();

            if (externalProcessingOutputs == null) return;

            foreach (var output in externalProcessingOutputs)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is TopographyPointResult result && result.ExternalProcessingStats.TryGetValue(output.Name, out var outputStat) ? (double?)outputStat.Mean : null,
                    pair => pair.Value is TopographyPointResult result && result.ExternalProcessingStats.TryGetValue(output.Name, out var outputStat) ? (double?)outputStat.Min : null,
                    pair => pair.Value is TopographyPointResult result && result.ExternalProcessingStats.TryGetValue(output.Name, out var outputStat) ? (double?)outputStat.Max : null,
                    pair => pair.Value is TopographyPointResult result && result.ExternalProcessingStats.TryGetValue(output.Name, out var outputStat)
                        ? MetroHelper.GetSymbol(outputStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    output.Name, null, 11.0);

                double target = output.OutputTarget;
                double tolerance = output.OutputTolerance.GetAbsoluteTolerance(output.OutputTarget);
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance, output.Name);
            }
        }

        private void UpdateSelectedPointsOutputChartData()
        {
            if (PointSelector.SelectedOutput == null) return;

            var measurePointResults = PointSelector.CheckedPoints.OfType<TopographyPointResult>().ToList();

            measurePointResults = measurePointResults.OrderBy(point =>
                point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? statsContainer.State : MeasureState.NotMeasured).ToList();

            SelectedPointsOutputChart.SetData(measurePointResults,
                point => PointSelector.PointToIndex[point] + 1,
                point => point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? (double?)statsContainer.Mean : null,
                point => point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? (double?)statsContainer.Min : null,
                point => point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? (double?)statsContainer.Max : null,
                point => point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer) ? MetroHelper.GetSymbol(statsContainer.State) : MetroHelper.GetSymbol(MeasureState.NotMeasured));
                    
        }

        private void UpdateSelectedPointsOutputChartSelection()
        {
            if (PointSelector.SelectedOutput == null) return;

            if (PointSelector.SingleSelectedPoint is TopographyPointResult point)
            {
                double xValue = PointSelector.PointToIndex[point] + 1;
                if (point.ExternalProcessingStats.TryGetValue(PointSelector.SelectedOutput, out var statsContainer))
                {
                    SelectedPointsOutputChart.UpdateSelectedPoint(xValue, statsContainer.Mean);
                }
                else
                {
                    SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
                }
            }
            else
            {
                SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
            }
        }

        private void UpdateSelectedPointOutputRepetaChartData()
        {
            if (PointSelector.SelectedOutput == null) return;

            var pointData = PointSelector.CurrentRepetaPoints.OfType<TopographyPointData>().OrderBy(data =>
            {
                if (data != null && data.ExternalProcessingResults != null)
                {
                    var outputResult = data?.ExternalProcessingResults.FirstOrDefault(result => result.Name.Equals(PointSelector.SelectedOutput));
                    return outputResult?.State ?? MeasureState.NotMeasured;
                }
                else
                {
                    return MeasureState.NotMeasured;
                }
            }).ToList();

            SelectedPointOutputRepetaChart.SetData(pointData,
                data => data.IndexRepeta + 1,
                data =>
                {
                    if (data != null && data.ExternalProcessingResults != null)
                    {
                        var outputResult = data?.ExternalProcessingResults.FirstOrDefault(result => result.Name.Equals(PointSelector.SelectedOutput));
                        return outputResult?.Value;
                    }
                    else
                    {
                        return double.NaN;
                    }
                },
                data =>
                {
                    if (data != null && data.ExternalProcessingResults != null)
                    {
                        var outputResult = data?.ExternalProcessingResults.FirstOrDefault(result => result.Name.Equals(PointSelector.SelectedOutput));
                        return MetroHelper.GetSymbol(outputResult?.State ?? MeasureState.NotMeasured);
                    }
                    else
                    {
                        return MetroHelper.GetSymbol(MeasureState.NotMeasured);
                    }

                });
        }

        private void UpdateChartsTargetAndTolerance()
        {
            var settings = TopographyResult?.Settings;

            if (settings == null || PointSelector.SelectedOutput == null)
            {
                SelectedPointsOutputChart.ClearTargetAndTolerance();
                SelectedPointOutputRepetaChart.ClearTargetAndTolerance();
                return;
            }

            SelectedPointsOutputChart.UpdateYAxisTitle(PointSelector.SelectedOutput);
            SelectedPointOutputRepetaChart.UpdateYAxisTitle(PointSelector.SelectedOutput);

            var outputSetting = settings.ExternalProcessingOutputs?.SingleOrDefault(output => output.Name.Equals(PointSelector.SelectedOutput));
            if (outputSetting == null || outputSetting.OutputTolerance == null)
            {
                SelectedPointsOutputChart.ClearTargetAndTolerance();
                return;
            }

            double target = outputSetting.OutputTarget;
            double tolerance = outputSetting.OutputTolerance.GetAbsoluteTolerance(outputSetting.OutputTarget);
            SelectedPointsOutputChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance);
            SelectedPointOutputRepetaChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance);

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

            _topographyPointsListVM.Dispose();
            GlobalStats.Dispose();
            ReportVm.Dispose();
            ThumbnailViewerVm.Dispose();
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
