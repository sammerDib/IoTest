using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.ResultUI.Common;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp
{
    public enum WarpViewerType
    {
        // Median or surface Warp
        [Description("Warp (RPD)")]
        [ChartDisplayDescription("RPD")]
        [CompleteDisplayName("Reference Plane Deviation")]
        WARP,

        [Description("Total Thickness Variation (TT)")]
        [ChartDisplayDescription("TT")]
        [CompleteDisplayName("Total Thickness")]
        TTV
    }


    public class WarpResultVM : MetroResultVM
    {
        public override string FormatName => "Warp";

        protected override PointSelectorBase GetPointSelector() => PointSelector;

        #region properties

        private readonly WarpPointsListVM _pointList;
        public override MetroPointsListVM ResultPointsList => _pointList;

        public WarpResult WarpResult => MetroResult.MeasureResult as WarpResult;

        public WarpGlobalStatsVM GlobalStats { get; set; }

        public MetroHeatMapVM HeatMapVM { get; set; }
        public DataInToleranceLineChart GlobalStatsChart { get; }

        public DataInToleranceLineChart SelectedPointsOutputChart { get; }
        public DataInToleranceLineChart SelectedPointRepetaChart { get; }

        public WarpDataRepetaVM DataRepeta { get; }


        public WarpPointSelector PointSelector { get; } = new WarpPointSelector();

        public WarpDetailMeasureInfoVM DetailMeasureInfo { get; }


        public MeasureTypeCategoriesVM MeasureTypeCategoriesVM { get; }
        public PointsLocationVM PointsLocation { get; }

        public ObservableCollection<string> Outputs { get; } = new ObservableCollection<string>();

        #endregion

        public WarpResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _pointList = new WarpPointsListVM(PointSelector) { Digits = Digits };
            GlobalStats = new WarpGlobalStatsVM(PointSelector);
            DetailMeasureInfo = new WarpDetailMeasureInfoVM() { Digits = Digits };
            MeasureTypeCategoriesVM = new MeasureTypeCategoriesVM(PointSelector);
            PointsLocation = new PointsLocationVM(PointSelector);
            DataRepeta = new WarpDataRepetaVM(PointSelector) { Digits = Digits };

            HeatMapVM = new MetroHeatMapVM(PointSelector, InterpolationEngine<object>.DefaultHeatMapSide_Circle);

            GlobalStatsChart = new DataInToleranceLineChart("N°", true, Colors.MediumPurple, false);
            SelectedPointsOutputChart = new DataInToleranceLineChart("N°", $"Warp ({Length.GetUnitSymbol(LengthUnit.Micrometer)})", true, Colors.MediumPurple, false);
            SelectedPointRepetaChart = new DataInToleranceLineChart("N°", $"Warp ({Length.GetUnitSymbol(LengthUnit.Micrometer)})", false, Colors.MediumSeaGreen, false);


            // Wafer map
            PointSelector.SelectedPointChanged += PointSelectorOnSelectedPointChanged;
            // Right gridview 
            PointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;
            // Combo choice
            PointSelector.SelectedOutputChanged += PointSelectorOnSelectedOutputChanged;
            // Repeta gridview
            PointSelector.CurrentRepetaPointsChanged += PointSelectorCurrentRepetaPointsChanged;
            // Global Chart
            SelectedPointsOutputChart.PointSelected += SelectedPointsChartOnPointSelected;
        }


        #region Event handlers

        private void PointSelectorOnSelectedPointChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedMeasurePointResultIndex));

            DetailMeasureInfo.Point = PointSelector.SingleSelectedPoint as WarpPointResult;

            UpdateSelectedPointsOutputChartSelection();
        }

        private void UpdateSelectedPointsOutputChartSelection()
        {
            if (PointSelector.SingleSelectedPoint is WarpPointResult point)
            {
                double xValue = PointSelector.PointToIndex[point] + 1;

                WarpViewerType value = EnumUtils.GetEnumFromDescription<WarpViewerType>(PointSelector.SelectedOutput);
                if (value == WarpViewerType.WARP)
                {
                    if (point.RPDStat?.Mean != null)
                    {
                        double yValue = point.RPDStat.Mean.Micrometers;
                        SelectedPointsOutputChart.UpdateSelectedPoint(xValue, yValue);
                    }
                    else
                    {
                        SelectedPointsOutputChart.UpdateSelectedPoint(null, null);
                    }
                }
                else
                {
                    if (point.TotalThicknessStat?.Mean != null)
                    {
                        double yValue = point.TotalThicknessStat.Mean.Micrometers;
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

        private void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedMeasurePointResultIndex));
            UpdateSelectedPointsOutputChartData();
        }

        private void UpdateSelectedPointsOutputChartData()
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return;


            var measurePointResults = PointSelector.CheckedPoints.OfType<WarpPointResult>().ToList();

            bool isSurfaceWarp = WarpResult.Settings.IsSurfaceWarp;
            WarpViewerType viewerType = EnumUtils.GetEnumFromDescription<WarpViewerType>(PointSelector.SelectedOutput);
            if (viewerType == WarpViewerType.WARP)
            {
                measurePointResults = measurePointResults.OrderBy(point => point.RPDStat?.State).ToList();

                SelectedPointsOutputChart.SetData(measurePointResults,
                    point => PointSelector.PointToIndex[point] + 1,
                    point => point.RPDStat?.Mean?.Micrometers,
                    point => point.RPDStat?.Min?.Micrometers,
                    point => point.RPDStat?.Max?.Micrometers,
                    point => MetroHelper.GetSymbol(point.RPDStat.State));
            }
            else
            {
                measurePointResults = measurePointResults.OrderBy(point => point.TotalThicknessStat?.State).ToList();

                SelectedPointsOutputChart.SetData(measurePointResults,
                    point => PointSelector.PointToIndex[point] + 1,
                    point => point.TotalThicknessStat?.Mean?.Micrometers,
                    point => point.TotalThicknessStat?.Min?.Micrometers,
                    point => point.TotalThicknessStat?.Max?.Micrometers,
                    point => MetroHelper.GetSymbol(point.TotalThicknessStat.State));
            }
        }

        private void SelectedPointsChartOnPointSelected(object sender, int e)
        {
            if (PointSelector.SortedIndexToPoint.TryGetValue(e - 1, out var pointToSelect))
            {
                PointSelector.SetSelectedPoint(this, pointToSelect);
            }
        }

        private void PointSelectorOnSelectedOutputChanged(object sender, EventArgs e)
        {
            DetailMeasureInfo.Output = string.Empty;

            if (!string.IsNullOrEmpty(PointSelector.SelectedOutput))
            {
                UpdateDetailMeasureInfo();

                var outputSettings = WarpResult?.Settings;
                Digits = MetroHelper.GetDecimalCount(outputSettings?.WarpMax?.Value);

                UpdateSelectedPointsOutputChartData();
                UpdateSelectedPointsOutputChartSelection();
                UpdateSelectedPointOutputRepetaChartData();

                UpdateSelectedPointsOutputChart();
                DataRepeta.SelectedOutput = PointSelector.SelectedOutput;

                UpdateRPDHeatMap();
            }
        }

        private void UpdateSelectedPointsOutputChart()
        {
            WarpViewerType viewerType = EnumUtils.GetEnumFromDescription<WarpViewerType>(PointSelector.SelectedOutput);
            SelectedPointsOutputChart.UpdateYAxisTitle($"{viewerType.ToString()} ({Length.GetUnitSymbol(PointSelector.CurrentUnit)})");
        }

        private void UpdateDetailMeasureInfo()
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return;

            WarpViewerType viewerType = EnumUtils.GetEnumFromDescription<WarpViewerType>(PointSelector.SelectedOutput);

            var stats = viewerType == WarpViewerType.WARP ? WarpResult.WarpStat : WarpResult.TTVStat;
            DetailMeasureInfo.Update(WarpResult.Settings, PointSelector.SelectedOutput, stats, WarpResult.QualityScore, HasRepeta);
        }

        private void PointSelectorCurrentRepetaPointsChanged(object sender, EventArgs e)
        {
            UpdateSelectedPointOutputRepetaChartData();
        }
        #endregion 

        private void UpdateSelectedPointOutputRepetaChartData()
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return;


            var pointData = PointSelector.CurrentRepetaPoints.OfType<WarpPointData>().OrderBy(data => data.State).ToList();
            bool isSurfaceWarp = WarpResult.Settings.IsSurfaceWarp;

            WarpViewerType viewerType = EnumUtils.GetEnumFromDescription<WarpViewerType>(PointSelector.SelectedOutput);
            if (viewerType == WarpViewerType.WARP)
            {
                SelectedPointRepetaChart.SetData(pointData,
                        data => data.IndexRepeta + 1,
                        data => data.RPD?.Micrometers,
                        data => MetroHelper.GetSymbol(data.State));
            }
            else
            {
                SelectedPointRepetaChart.SetData(pointData,
                        data => data.IndexRepeta + 1,
                        data => data.TotalThickness?.Micrometers,
                        data => MetroHelper.GetSymbol(data.State));
            }
        }

        protected override void OnDigitsChanged()
        {
            DetailMeasureInfo.Digits = Digits;
            DataRepeta.Digits = Digits;
            ResultPointsList.Digits = Digits;

            return;
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

            var warpResult = MetroResult?.MeasureResult as WarpResult;

            Digits = MetroHelper.GetDecimalCount(warpResult?.Settings?.WarpMax?.Value);

            Outputs.Clear();

            var values = Enum.GetNames(typeof(WarpViewerType)).ToList();

            if (warpResult.Settings.IsSurfaceWarp)
            {
                values = new List<string>() { WarpViewerType.WARP.ToString() };
            }

            foreach (var item in values)
            {
                string desc = StatsFactory.GetEnumDescription<WarpViewerType>(item);

                Outputs.Add(desc);
            }

            bool hasWarp = false;
            _pointList.UpdateWarpVisibility(hasWarp);

            UpdateDetailMeasureInfo();

            DataRepeta.UpdateOutputSource(Outputs.ToList());

            HeatMapVM.Update(warpResult);

            UpdateGlobalChartData();

            OnPropertyChanged(nameof(WarpResult));

            PointSelector.SetViewerTypeAndRaiseEvents(Outputs.FirstOrDefault());

            MeasureTypeCategoriesVM.UpdateCategories(points);
            PointsLocation.PopulatePointsLocationCollection();
        }


        private void UpdateGlobalChartData()
        {
            GlobalStatsChart.ClearAll();

            var settings = WarpResult?.Settings;
            if (settings != null)
            {
                if (settings.WarpMax != null)
                {
                    GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                        pair => pair.Value is WarpPointResult result ? result.RPDStat?.Mean?.Micrometers : null,
                        pair => pair.Value is WarpPointResult result ? result.RPDStat?.Min?.Micrometers : null,
                        pair => pair.Value is WarpPointResult result ? result.RPDStat?.Max?.Micrometers : null,
                        pair => pair.Value is WarpPointResult result && result.RPDStat != null
                            ? MetroHelper.GetSymbol(result.RPDStat.State)
                            : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                        WarpViewerType.WARP.ToString(),
                        null, 14.0, $"{Length.GetUnitSymbol(PointSelector.CurrentUnit)}", 12.0);

                    GlobalStatsChart.SetTargetMinAndMax(0, settings.WarpMax.Micrometers);
                }
            }

            if (!settings.IsSurfaceWarp)
            {
                GlobalStatsChart.SetData(PointSelector.SortedIndexToPoint, pair => pair.Key + 1,
                    pair => pair.Value is WarpPointResult result ? result.TotalThicknessStat?.Mean?.Micrometers : null,
                    pair => pair.Value is WarpPointResult result ? result.TotalThicknessStat?.Min?.Micrometers : null,
                    pair => pair.Value is WarpPointResult result ? result.TotalThicknessStat?.Max?.Micrometers : null,
                    pair => pair.Value is WarpPointResult result && result.TotalThicknessStat != null
                        ? MetroHelper.GetSymbol(result.TotalThicknessStat.State)
                        : MetroHelper.GetSymbol(MeasureState.NotMeasured),
                    WarpViewerType.TTV.ToString(),
                    null, 14.0, $"{Length.GetUnitSymbol(PointSelector.CurrentUnit)}", 12.0);
            }
        }


        #region HeatMap
        private readonly Dictionary<string, InterpolationEngine<WarpPointResult>> _interpolationEngines = new Dictionary<string, InterpolationEngine<WarpPointResult>>();
        private InterpolationEngine<WarpPointResult> InterpolationEngine { get; set; }

        private InterpolationEngine<WarpPointResult> CreateInterpolationEngine(bool dieMode)
        {
            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return null;

            // we kept only circle interpolator
            if (!dieMode && _interpolationEngines.TryGetValue(PointSelector.SelectedOutput, out var interpolationEngine))
            {
                return interpolationEngine;
            }

            InterpolationEngine<WarpPointResult> newInterpEngine = null;

            WarpViewerType viewerEnum = EnumUtils.GetEnumFromDescription<WarpViewerType>(PointSelector.SelectedOutput);
            if (viewerEnum == WarpViewerType.WARP)
            {
                newInterpEngine = new InterpolationEngine<WarpPointResult>(
                    point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                    point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                    point => point.RPDStat?.Mean?.Micrometers,
                    LengthUnit.Micrometer,
                    null,
                    dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);
            }
            else
            {
                newInterpEngine = new InterpolationEngine<WarpPointResult>(
                           point => dieMode ? point.XPosition : point.WaferRelativeXPosition,
                           point => dieMode ? point.YPosition : point.WaferRelativeYPosition,
                           point => point.TotalThicknessStat?.Mean?.Micrometers,
                           LengthUnit.Micrometer,
                           null,
                           dieMode ? InterpolationEngine<object>.DefaultHeatMapSide_Square : InterpolationEngine<object>.DefaultHeatMapSide_Circle);
            }

            if (!dieMode)
            {
                _interpolationEngines.Add(PointSelector.SelectedOutput, newInterpEngine);
            }
            return newInterpEngine;
        }
        private void OnInterpolationDone(object sender, EventArgs e)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                HeatMapVM.SetSpecMinAndMax(InterpolationEngine.CurrentMinValue, InterpolationEngine.CurrentMaxValue);
                HeatMapVM.SetInterpolationResult(InterpolationEngine.InterpolationResults);
            });
        }

        private void UpdateRPDHeatMap()
        {
            DisposeInterpolationEngine();

            if (string.IsNullOrEmpty(PointSelector.SelectedOutput)) return;

            InterpolationEngine = CreateInterpolationEngine(false);

            // Global HeatMap
            //double? target = double.NaN; // this will remove inter target level
            double? target = null;  // this will hide buttons target-tolerance but insert intermediate level
            double?  tolerance = null;
            string unit = Length.GetUnitSymbol(LengthUnit.Micrometer);
            WarpViewerType viewerType = EnumUtils.GetEnumFromDescription<WarpViewerType>(PointSelector.SelectedOutput);
            string title = EnumUtils.GetAttribute<ChartDisplayDescriptionAttribute>(viewerType).DisplayDescription;

            HeatMapVM.SetTitle(title);
            HeatMapVM.SetTargetTolerance(target, tolerance);
            HeatMapVM.SetUnit(unit);
            HeatMapVM.SetPaletteType(HeatMapPaletteType.SpecValues);
            

            if (InterpolationEngine != null)
            {
                InterpolationEngine.InterpolationDone += OnInterpolationDone;

                double waferDiameter = MetroHelper.GetWaferDiameterMillimeters(WarpResult);
                InterpolationEngine.InterpolateCircle(PointSelector.AllPoints.OfType<WarpPointResult>().ToList(), waferDiameter);
            }
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
        #endregion

        public override void Dispose()
        {
            PointSelector.SelectedPointChanged -= PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged -= PointSelectorOnCheckedPointsChanged;
            PointSelector.SelectedOutputChanged -= PointSelectorOnSelectedOutputChanged;
            PointSelector.CurrentRepetaPointsChanged -= PointSelectorCurrentRepetaPointsChanged;

            SelectedPointsOutputChart.PointSelected -= SelectedPointsChartOnPointSelected;

            ResultPointsList.Dispose();
            _pointList.Dispose();
            GlobalStats.Dispose();
            HeatMapVM.Dispose();
            MeasureTypeCategoriesVM.Dispose();
            PointsLocation.Dispose();

            GlobalStatsChart.Dispose();
            SelectedPointsOutputChart.Dispose();
            SelectedPointRepetaChart.Dispose();

            base.Dispose();
        }
    }


    internal class ChartDisplayDescriptionAttribute : Attribute
    {
        public string DisplayDescription { get; set; }
        public ChartDisplayDescriptionAttribute(string displayDesc)
        {
            DisplayDescription = displayDesc;

        }
    }

    internal class CompleteDisplayNameAttribute : Attribute
    {
        public string Name { get; set; }
        public CompleteDisplayNameAttribute(string name)
        {
            Name = name;

        }
    }

    public static class EnumUtils
    {
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<T>().SingleOrDefault();
        }

        public static T GetEnumFromDescription<T>(this string description)
        {
            ValueDescription valueDesc = GetValuesAndDescriptions(typeof(T)).FirstOrDefault(vd => vd.Description == description);
            if (valueDesc != null)
            {
                return (T)valueDesc.Value;
            }
            else
            {
                throw new Exception();
            }
        }

        public static IEnumerable<ValueDescription> GetValuesAndDescriptions(Type enumType)
        {
            IEnumerable<object> values = Enum.GetValues(enumType).Cast<object>();
            IEnumerable<ValueDescription> valuesAndDescriptions = from value in values
                                                                  select new ValueDescription
                                                                  {
                                                                      Value = value,
                                                                      Description = value.GetType().GetMember(value.ToString())[0].GetCustomAttributes(true).OfType<DescriptionAttribute>().First().Description
                                                                  };
            return valuesAndDescriptions;
        }
        public class ValueDescription
        {
            public object Value { get; set; }
            public string Description { get; set; }
            public string ValueAsString
            {
                get
                {
                    if (Value == null) return null;
                    return Value.ToString();
                }
            }
        }
    }
}
