using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv.Copla;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv.DieDetails;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;
using System.Threading.Tasks;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.RawSignal;
using UnitySC.Shared.ResultUI.Common.ViewModel.Export;
using UnitySC.Shared.Tools;
using UnitySC.Shared.ResultUI.Common.Helpers;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv
{
    public enum TsvResultViewerType
    {
        Depth,
        Coplanarity
    }

    public class TsvResultVM : MetroResultVM
    {
        #region Constructor

        public TsvResultVM(IResultDisplay resultDisplay) : base(resultDisplay)
        {
            ResultPointsList = new TsvPointsListVM(PointSelector) { Digits = Digits };
            GlobalStats = new TsvGlobalStatsVM(PointSelector);
            TsvHeatMapVM = new MetroHeatMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Circle);
            ThumbnailViewerVm = new TsvThumbnailViewerVM(PointSelector);
            DetailMeasureInfo = new TsvDetailMeasureInfoVM { Digits = Digits };
            TsvDataRepetaVm = new TsvDataRepetaVM(PointSelector) { Digits = Digits };
            DieMap = new DieMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Square);
            DieStats = new DieStatsVM(PointSelector);
            BestFitPlane = new BestFitPlaneViewModel(PointSelector);
            MeasureTypeCategories = new MeasureTypeCategoriesVM(PointSelector);
            PointsLocation = new PointsLocationVM(PointSelector);
            
            GlobalStatsChart = new DataInToleranceLineChart("N°", true, Colors.MediumPurple, false);

            SelectedPointsDepthChart = new DataInToleranceLineChart("N°", $"Depth ({Length.GetUnitSymbol(LengthUnit.Micrometer)})", true, Colors.MediumPurple, false);
            SelectedPointsCoplaChart = new DataInToleranceLineChart("N°", "Coplanarity", false, Colors.MediumSeaGreen, true);
            DepthRepetaChart = new DataInToleranceLineChart("Repeta N°", $"Depth ({Length.GetUnitSymbol(LengthUnit.Micrometer)})", false, Color.FromRgb(172, 149, 110), false);
            LengthWidthRepetaChart = new TsvLengthWidthRepetaChart($"Length ({Length.GetUnitSymbol(LengthUnit.Micrometer)})", $"Width ({Length.GetUnitSymbol(LengthUnit.Micrometer)})");

            ExportRawSignalVm = new ExportSimpleVM();
            RawSignalChart = new RawFFTSignalChart();

            ExportRawSignalVm.OnSaveExportCommand += OnSaveRawSignalExport;


            PointSelector.SelectedPointChanged += PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;
            PointSelector.CurrentRepetaPointsChanged += PointSelectorOnCurrentRepetaPointsChanged;
            PointSelector.ViewerTypeChanged += PointSelectorOnViewerTypeChanged;

            SelectedPointsDepthChart.PointSelected += SelectedPointsDepthChartOnPointSelected;
            SelectedPointsCoplaChart.PointSelected += SelectedPointsDepthChartOnPointSelected;

        }

        private void SelectedPointsDepthChartOnPointSelected(object sender, int e)
        {
            if (PointSelector.SortedIndexToPoint.TryGetValue(e - 1, out var pointToSelect))
            {
                PointSelector.SetSelectedPoint(this, pointToSelect);
            }
        }

        #endregion Constructor

        #region Properties
        
        public TSVResult TsvResult => MetroResult.MeasureResult as TSVResult;
        
        public TsvPointSelector PointSelector { get; } = new TsvPointSelector();

        public TsvDetailMeasureInfoVM DetailMeasureInfo { get; }

        #region Overrides of MetroResultVM

        public override MetroPointsListVM ResultPointsList { get; }

        #endregion

        public TsvGlobalStatsVM GlobalStats { get; }

        public MetroHeatMapVM TsvHeatMapVM { get; }
        
        public DataInToleranceLineChart GlobalStatsChart { get; }

        public DataInToleranceLineChart SelectedPointsDepthChart { get; }

        public DataInToleranceLineChart SelectedPointsCoplaChart { get; }

        public DataInToleranceLineChart DepthRepetaChart { get; }

        public TsvLengthWidthRepetaChart LengthWidthRepetaChart { get; }

        public TsvDataRepetaVM TsvDataRepetaVm { get; }

        public DieMapVM DieMap { get; }

        public DieStatsVM DieStats { get; }

        public BestFitPlaneViewModel BestFitPlane { get; }

        private InterpolationEngine<TSVPointResult> InterpolationEngine { get; set; }

        public MeasureTypeCategoriesVM MeasureTypeCategories { get; }

        public PointsLocationVM PointsLocation { get; }

        public ExportSimpleVM ExportRawSignalVm { get; }

        public RawFFTSignalChart RawSignalChart { get; }

        private readonly Dictionary<TsvResultViewerType, InterpolationEngine<TSVPointResult>> _interpolationEngines = new Dictionary<TsvResultViewerType, InterpolationEngine<TSVPointResult>>();

        #endregion Properties

        #region Overrides of BaseResultWaferVM

        public override string FormatName => "TSV";
        
        #endregion Overrides of BaseResultWaferVM

        #region Overrides of IResultWaferVM

        protected override PointSelectorBase GetPointSelector() => PointSelector;

        protected override void OnDigitsChanged()
        {
            DetailMeasureInfo.Digits = Digits;
            TsvDataRepetaVm.Digits = Digits;
            ResultPointsList.Digits = Digits;
        }

        #region Overrides of MetroResultVM

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

            var tsvResult = MetroResult?.MeasureResult as TSVResult;

            if (tsvResult?.Settings?.LengthTarget == null || tsvResult.Settings?.WidthTarget == null || tsvResult.Settings?.DepthTarget == null ||
                tsvResult.Settings?.DepthTolerance == null || tsvResult.Settings?.LengthTolerance == null || tsvResult.Settings?.WidthTolerance == null)
            {
                throw new ArgumentNullException(nameof(tsvResult.Settings), "Some settings of TSV result are null");
            }

            Digits = MetroHelper.GetDecimalCount(TsvResult?.Settings?.DepthTarget?.Value, TsvResult?.Settings?.DepthTolerance?.Value);
            Digits = Math.Max(Digits, MetroHelper.GetDecimalCount(TsvResult?.Settings?.LengthTarget?.Value, TsvResult?.Settings?.LengthTolerance?.Value));
            Digits = Math.Max(Digits, MetroHelper.GetDecimalCount(TsvResult?.Settings?.WidthTarget?.Value, TsvResult?.Settings?.WidthTolerance?.Value));

            BestFitPlane.Update(tsvResult.BestFitPlan);

            TsvHeatMapVM.Update(tsvResult);
            UpdateChartGeneralData(tsvResult.Settings);

            HasThumbnail = points.Any(result => result.Datas.Any(data => data is TSVPointData tsvData && !string.IsNullOrWhiteSpace(tsvData.ResultImageFileName)));

            DetailMeasureInfo.Settings = tsvResult.Settings;
            DieMap.TsvResult = tsvResult;

            UpdateGlobalChartData();
            
            OnPropertyChanged(nameof(TsvResult));

            PointSelector.RaiseViewerTypeChangedEvents();

            MeasureTypeCategories.UpdateCategories(points);
            PointsLocation.PopulatePointsLocationCollection();
        }

        #endregion

        #endregion Overrides of IResultWaferVM
        
        #region Event Handlers

        private void PointSelectorOnSelectedPointChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedMeasurePointResultIndex));

            DetailMeasureInfo.Point = PointSelector.SingleSelectedPoint as TSVPointResult;
            DetailMeasureInfo.Die = PointSelector.GetDieFromPoint(PointSelector.SingleSelectedPoint);
            
            if (PointSelector.SingleSelectedPoint is TSVPointResult point)
            {
                double xValue = PointSelector.PointToIndex[point] + 1;

                // Depth Chart
                if (point.DepthTsvStat?.Mean != null)
                {
                    double yValue = point.DepthTsvStat.Mean.Micrometers;
                    SelectedPointsDepthChart.UpdateSelectedPoint(xValue, yValue);
                }
                else
                {
                    SelectedPointsDepthChart.UpdateSelectedPoint(null, null);
                }

                // Copla Chart
                var copla = point.CoplaInWaferValue == null ? point.CoplaInDieValue : point.CoplaInWaferValue;
                if (copla != null)
                {
                    double yValue = copla.Micrometers;
                    SelectedPointsCoplaChart.UpdateSelectedPoint(xValue, yValue);
                }
                else
                {
                    SelectedPointsCoplaChart.UpdateSelectedPoint(null, null);
                }
            }
            else
            {
                SelectedPointsDepthChart.UpdateSelectedPoint(null, null);
                SelectedPointsCoplaChart.UpdateSelectedPoint(null, null);
            }
        }

        private void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedMeasurePointResultIndex));

            switch (PointSelector.ViewerType)
            {
                case TsvResultViewerType.Depth:
                    UpdateSelectedPointsDepthChart();
                    break;
                case TsvResultViewerType.Coplanarity:
                    UpdateSelectedPointsCoplaChart();
                    break;
            }
        }

        private void PointSelectorOnViewerTypeChanged(object sender, EventArgs e)
        {
            switch (PointSelector.ViewerType)
            {
                case TsvResultViewerType.Depth:
                    UpdateSelectedPointsDepthChart();
                    break;
                case TsvResultViewerType.Coplanarity:
                    UpdateSelectedPointsCoplaChart();
                    break;
            }

            UpdateHeatMapChart();
        }

        private void PointSelectorOnCurrentRepetaPointsChanged(object sender, EventArgs e)
        {
            List<Tuple<TSVPointData, MeasureState>> lengthWidthData = null;
            Task.Run(() =>
                 lengthWidthData = PointSelector.CurrentRepetaPoints.OfType<TSVPointData>().
                Select(data => new Tuple<TSVPointData, MeasureState>(data, MergeStates(data.LengthState, data.WidthState))).
                OrderBy(tuple => tuple.Item2).ToList()

            ).Wait();          

            LengthWidthRepetaChart.SetData(lengthWidthData,
                data => data.Item1.Length?.Micrometers,
                data => data.Item1.Width?.Micrometers,
                data => MetroHelper.GetSymbol(data.Item2));

            var depthRepetaData = PointSelector.CurrentRepetaPoints.OfType<TSVPointData>().OrderBy(data => data.DepthState).ToList();

            DepthRepetaChart.SetData(depthRepetaData,
                data => data.IndexRepeta + 1,
                data => data.Depth?.Micrometers,
                data => MetroHelper.GetSymbol(data.DepthState));

            if (depthRepetaData.Any(x => x.DepthRawSignal != null))
            {
                HasRawData = true;
                RawSignalChart.Generate(depthRepetaData.Select(x => x.DepthRawSignal).ToList());
            }
            else
            {
                HasRawData = false;
            }
        }

        private static MeasureState MergeStates(MeasureState state1, MeasureState state2)
        {
            if (state1 == MeasureState.NotMeasured || state2 == MeasureState.NotMeasured) return MeasureState.NotMeasured;
            if (state1 == MeasureState.Success && state2 == MeasureState.Success) return MeasureState.Success;
            if (state1 == MeasureState.Error && state2 == MeasureState.Error) return MeasureState.Error;
            return MeasureState.Partial;
        }


        private void OnSaveRawSignalExport()
        {
            if (ExportRawSignalVm != null && !ExportRawSignalVm.IsExporting)
            {
                ExportRawSignalVm.IsExporting = true;

                Task.Run(() =>
                {
                    try
                    {
  
                        RawSignalChart.Export(ExportRawSignalVm.GetTargetFullPath());

                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            var notifierVm = ClassLocator.Default.GetInstance<UI.ViewModel.NotifierVM>();
                            notifierVm.AddMessage(new Tools.Service.Message(Tools.Service.MessageLevel.Information, $"Signal Exported in <{ExportRawSignalVm.GetTargetFullPath()}>"));
                        });
                    }
                    catch (Exception ex)
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            var notifierVm = ClassLocator.Default.GetInstance<UI.ViewModel.NotifierVM>();
                            notifierVm.AddMessage(new Tools.Service.Message(Tools.Service.MessageLevel.Error, $"Signal Export failure : {ex.Message}"));
                        });
                    }
                    finally
                    {
                        ExportRawSignalVm.IsExporting = false;
                        ExportRawSignalVm.IsStayPopup = false;
                    }

                }).ConfigureAwait(false);
            }
        }

        #endregion Event Handlers

        #region Commands

        private ICommand _openExportSignalPopupCommand;

        public ICommand OpenExportSignalPopupCommand => _openExportSignalPopupCommand ?? (_openExportSignalPopupCommand = new AutoRelayCommand(OpenExportSignalPopupCommandExecute, OpenExportSignalPopupCommandCanExecute));

        protected virtual bool OpenExportSignalPopupCommandCanExecute()
        {
            return true;
        }

        protected virtual void OpenExportSignalPopupCommandExecute()
        {
            ExportRawSignalVm.GenerateNewTargetPath($".csv");
            ExportRawSignalVm.IsStayPopup = true;
        }

        #endregion
        #region Private Methods

        #region HeatMap

        private InterpolationEngine<TSVPointResult> CreateInterpolationEngine(bool dieMode)
        {
            // we kept only circle interpolator
            if (!dieMode && _interpolationEngines.TryGetValue(PointSelector.ViewerType, out var interpolationEngine))
            {
                return interpolationEngine;
            }

            InterpolationEngine<TSVPointResult> newInterpEngine = null;
            switch (PointSelector.ViewerType)
            {
                case TsvResultViewerType.Depth:
                    {
                        newInterpEngine = new InterpolationEngine<TSVPointResult>(
                            point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                            point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                            point => point.DepthTsvStat?.Mean?.Micrometers,
                            LengthUnit.Micrometer,
                            TsvResult?.Settings?.DepthTarget?.Micrometers,
                            dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);
                    }
                    break;
                case TsvResultViewerType.Coplanarity:
                    {
                        newInterpEngine = new InterpolationEngine<TSVPointResult>(
                            point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                            point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                            point =>
                            {
                                if (point.CoplaInWaferValue != null) return point.CoplaInWaferValue.Micrometers;
                                if (point.CoplaInDieValue != null) return point.CoplaInDieValue.Micrometers;
                                return null;
                            },
                            0.0,
                            dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);
                    }
                    break;
            }

            // we kept only circle interpolator
            if (!dieMode && newInterpEngine != null)
            {
                _interpolationEngines.Add(PointSelector.ViewerType, newInterpEngine);
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

            double? target = null;
            double? tolerance = null;
            string unit = string.Empty; 
            var paletteType = HeatMapPaletteType.TargetTolerance;

            InterpolationEngine = CreateInterpolationEngine(false);

            switch (PointSelector.ViewerType)
            {
                case TsvResultViewerType.Depth:
                    {
                        paletteType = TsvResult?.Settings?.DepthTarget != null ? HeatMapPaletteType.TargetTolerance : HeatMapPaletteType.MinMax;
                        target = TsvResult?.Settings?.DepthTarget?.Micrometers;
                        tolerance = TsvResult?.Settings?.DepthTolerance?.GetAbsoluteTolerance(TsvResult.Settings.DepthTarget).Micrometers;
                        unit = Length.GetUnitSymbol(LengthUnit.Micrometer);
                    }
                    break;
                case TsvResultViewerType.Coplanarity:
                    {
                        paletteType = HeatMapPaletteType.ZeroToMax;
                        unit = string.Empty;
                    }
                    break;
            }

            // Global HeatMap
            TsvHeatMapVM.SetTitle(PointSelector.ViewerType.ToString());
            TsvHeatMapVM.SetTargetTolerance(target, tolerance);
            TsvHeatMapVM.SetUnit(unit);
            TsvHeatMapVM.SetPaletteType(paletteType);

            // Die HeatMap
            DieMap.SetTargetTolerance(target, tolerance);
            DieMap.SetUnit(unit);
            DieMap.SetPaletteType(paletteType);
            DieMap.SetInterpolationEngine(CreateInterpolationEngine(true)); // note RTi voir comment alimenter l'engine du die avec l'interpolator de l'engine du wafer circle !

            if (InterpolationEngine != null)
            {
                InterpolationEngine.InterpolationDone += OnInterpolationDone;

                double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(TsvResult);
                InterpolationEngine.InterpolateCircle(PointSelector.AllPoints.OfType<TSVPointResult>().ToList(), waferDiameter);
            }
        }

        private void OnInterpolationDone(object sender, EventArgs e)
        {
            Application.Current?.Dispatcher?.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() =>
            {
                TsvHeatMapVM.SetMinMax(InterpolationEngine.CurrentMinValue, InterpolationEngine.CurrentMaxValue);
                TsvHeatMapVM.SetInterpolationResult(InterpolationEngine.InterpolationResults);
            }));
        }

        #endregion

        private void UpdateSelectedPointsDepthChart()
        {
            var measurePointResults = PointSelector.CheckedPoints.OfType<TSVPointResult>().OrderBy(point => point.DepthTsvStat?.State).ToList();
            
            SelectedPointsDepthChart.SetData(measurePointResults,
                point => PointSelector.PointToIndex[point] + 1,
                point => point.DepthTsvStat?.Mean?.Micrometers,
                point => point.DepthTsvStat?.Min?.Micrometers,
                point => point.DepthTsvStat?.Max?.Micrometers,
                point => MetroHelper.GetSymbol(point.DepthTsvStat.State));
        }

        private void UpdateSelectedPointsCoplaChart()
        {
            var measurePointResults = PointSelector.CheckedPoints.OfType<TSVPointResult>().OrderBy(point => point.DepthTsvStat.State).ToList();

            SelectedPointsCoplaChart.SetData(measurePointResults,
                point => PointSelector.PointToIndex[point] + 1,
                point => point.CoplaInWaferValue == null ? point.CoplaInDieValue?.Micrometers : point.CoplaInWaferValue?.Micrometers,
                point => null);
        }
        
        private void UpdateGlobalChartData()
        {
            GlobalStatsChart.ClearAll();

            TSVResultSettings settings = null;
            if (MetroResult?.MeasureResult is TSVResult tsvResult)
            {
                settings = tsvResult.Settings;
            }

            // Length

            GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                pair => pair.Value is TSVPointResult result ? result.LengthTsvStat?.Mean?.Micrometers : null,
                pair => pair.Value is TSVPointResult result ? result.LengthTsvStat?.Min?.Micrometers : null,
                pair => pair.Value is TSVPointResult result ? result.LengthTsvStat?.Max?.Micrometers : null,
                pair => pair.Value is TSVPointResult result && result.LengthTsvStat != null
                    ? MetroHelper.GetSymbol(result.LengthTsvStat.State)
                    : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                nameof(TSVPointData.Length),
                null, 14.0,$"{Length.GetUnitSymbol(LengthUnit.Micrometer)}", 12.0);

            if (settings != null)
            {
                double target = settings.LengthTarget.Micrometers;
                double tolerance = settings.LengthTolerance.GetAbsoluteTolerance(TsvResult.Settings.LengthTarget).Micrometers;
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance, nameof(TSVPointData.Length));
            }

            // Width

            GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                pair => pair.Value is TSVPointResult result ? result.WidthTsvStat?.Mean?.Micrometers : null,
                pair => pair.Value is TSVPointResult result ? result.WidthTsvStat?.Min?.Micrometers : null,
                pair => pair.Value is TSVPointResult result ? result.WidthTsvStat?.Max?.Micrometers : null,
                pair => pair.Value is TSVPointResult result && result.WidthTsvStat != null
                    ? MetroHelper.GetSymbol(result.WidthTsvStat.State)
                    : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                nameof(TSVPointData.Width),
                null, 14.0, $"{Length.GetUnitSymbol(LengthUnit.Micrometer)}", 12.0);

            if (settings != null)
            {
                double target = settings.WidthTarget.Micrometers;
                double tolerance = settings.WidthTolerance.GetAbsoluteTolerance(TsvResult.Settings.WidthTarget).Micrometers;
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance, nameof(TSVPointData.Width));
            }

            // Depth

            GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                pair => pair.Value is TSVPointResult result ? result.DepthTsvStat?.Mean?.Micrometers : null,
                pair => pair.Value is TSVPointResult result ? result.DepthTsvStat?.Min?.Micrometers : null,
                pair => pair.Value is TSVPointResult result ? result.DepthTsvStat?.Max?.Micrometers : null,
                pair => pair.Value is TSVPointResult result && result.DepthTsvStat != null
                    ? MetroHelper.GetSymbol(result.DepthTsvStat.State)
                    : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                nameof(TSVPointData.Depth),
                null, 14.0, $"{Length.GetUnitSymbol(LengthUnit.Micrometer)}", 12.0);

            if (settings != null)
            {
                double target = settings.DepthTarget.Micrometers;
                double tolerance = settings.DepthTolerance.GetAbsoluteTolerance(TsvResult.Settings.DepthTarget).Micrometers;
                GlobalStatsChart.SetTargetAndTolerance(target, target - tolerance, target + tolerance, nameof(TSVPointData.Depth));
            }
        }
        
        private void UpdateChartGeneralData(TSVResultSettings settings)
        {
            if (settings != null)
            {
                double depthTarget = settings.DepthTarget.Micrometers;
                var depthtolerance = settings.DepthTolerance.GetAbsoluteTolerance(settings.DepthTarget).Micrometers;
                double minDepthTolerance = depthTarget - depthtolerance;
                double maxDepthTolerance = depthTarget + depthtolerance;

                SelectedPointsDepthChart.UpdateYAxisTitle(nameof(TSVPointData.Depth) + $" ({Length.GetUnitSymbol(LengthUnit.Micrometer)})");
                SelectedPointsDepthChart.SetTargetAndTolerance(depthTarget, minDepthTolerance, maxDepthTolerance);
                DepthRepetaChart.SetTargetAndTolerance(depthTarget, minDepthTolerance, maxDepthTolerance);

                LengthWidthRepetaChart.SetTargetAndTolerance(
                    settings.LengthTarget.Micrometers,
                    settings.LengthTolerance.GetAbsoluteTolerance(settings.LengthTarget).Micrometers,
                    settings.WidthTarget.Micrometers,
                    settings.WidthTolerance.GetAbsoluteTolerance(settings.WidthTarget).Micrometers);
            }
            else
            {
                SelectedPointsDepthChart.ClearTargetAndTolerance();
                DepthRepetaChart.ClearTargetAndTolerance();
                LengthWidthRepetaChart.ClearTargetAndTolerance();
            }
        }

        #endregion Private Methods

        #region Overrides of MetroResultVM

        public override void Dispose()
        {
            PointSelector.SelectedPointChanged -= PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged -= PointSelectorOnCheckedPointsChanged;
            PointSelector.CurrentRepetaPointsChanged -= PointSelectorOnCurrentRepetaPointsChanged;
            PointSelector.ViewerTypeChanged -= PointSelectorOnViewerTypeChanged;

            SelectedPointsDepthChart.PointSelected -= SelectedPointsDepthChartOnPointSelected;
            SelectedPointsCoplaChart.PointSelected -= SelectedPointsDepthChartOnPointSelected;

            ResultPointsList.Dispose();
            GlobalStats.Dispose();
            TsvHeatMapVM.Dispose();
            ThumbnailViewerVm.Dispose();
            TsvDataRepetaVm.Dispose();
            DieMap.Dispose();
            DieStats.Dispose();
            BestFitPlane.Dispose();
            MeasureTypeCategories.Dispose();
            PointsLocation.Dispose();

            GlobalStatsChart.Dispose();
            SelectedPointsDepthChart.Dispose();
            SelectedPointsCoplaChart.Dispose();
            DepthRepetaChart.Dispose();
            LengthWidthRepetaChart.Dispose();
            RawSignalChart.Dispose();

            base.Dispose();
        }

        #endregion
    }
}
