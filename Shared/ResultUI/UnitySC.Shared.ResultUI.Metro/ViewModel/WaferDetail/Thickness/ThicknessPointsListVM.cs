using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness
{
    public class ThicknessPointsListVM : MetroPointsListVM
    {
        private Dictionary<string, LengthUnit> _layersUnit = new Dictionary<string, LengthUnit>();

        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }
        

        public ThicknessPointsListVM(PointSelectorBase pointSelector) : base(pointSelector)
        {

        }
        
        #region Overrides of MetroPointsListVM

        public override void UpdatePointsSource(ICollection<MeasurePointResult> sourcePoint, bool showRepetaColumns, bool showDieIndex, bool showQualityScore, bool showSiteID)
        {
            base.UpdatePointsSource(sourcePoint, showRepetaColumns, showDieIndex, showQualityScore, showSiteID);

            GeneratedColumns.Clear();

            var externalProcessingKeys = new List<string>();
            if (sourcePoint.OfType<ThicknessPointResult>().Any(result => result.TotalThicknessStat.Mean != null))
            {
                externalProcessingKeys.Add(ThicknessResultVM.TotalLayerName);
            }

            if (sourcePoint.OfType<ThicknessPointResult>().Any(result => result.WaferThicknessStat.Mean != null))
            {
                externalProcessingKeys.Add(ThicknessResultVM.WaferThickness);
            }

            externalProcessingKeys.AddRange(sourcePoint.OfType<ThicknessPointResult>().SelectMany(point => point.ThicknessLayerStats.Keys).Distinct());

            foreach (string externalProcessingKey in externalProcessingKeys)
            {
                _layersUnit.TryGetValue(externalProcessingKey, out var unit);
                string unitSymbol = $"({Length.GetUnitSymbol(unit)})";

                if (showRepetaColumns)
                {
                    GenerateColumn($"Avg {externalProcessingKey} {unitSymbol}", externalProcessingKey, container => container.Mean, unit);
                    GenerateColumn($"3σ {externalProcessingKey} {unitSymbol}", externalProcessingKey, container => container.Sigma3, unit);
                    GenerateColumn($"Min {externalProcessingKey} {unitSymbol}", externalProcessingKey, container => container.Min, unit);
                    GenerateColumn($"Max {externalProcessingKey} {unitSymbol}", externalProcessingKey, container => container.Max, unit);
                }
                else
                {
                    GenerateColumn($"{externalProcessingKey} {unitSymbol}", externalProcessingKey, container => container.Mean, unit);
                }
            }

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;
        }

        private void GenerateColumn(string columnName, string externalProcessingKey, Func<MetroStatsContainer, Length> getValueFunc, LengthUnit unit)
        {
            GeneratedColumns.Add(new GeneratedColumn
            {
                SortDefinition = SortedPoints.Sort.AddSortDefinition(result =>
                {
                    if (result is ThicknessPointResult thicknessPointResult)
                    {
                        if (externalProcessingKey == ThicknessResultVM.TotalLayerName)
                        {
                            var valueFunc = getValueFunc(thicknessPointResult.TotalThicknessStat);
                            return valueFunc != null ? valueFunc.Micrometers : double.MinValue;
                        }

                        if (externalProcessingKey == ThicknessResultVM.WaferThickness)
                        {
                            var valueFunc = getValueFunc(thicknessPointResult.WaferThicknessStat);
                            return valueFunc != null ? valueFunc.Micrometers : double.MinValue;
                        }

                        if (thicknessPointResult.ThicknessLayerStats.TryGetValue(externalProcessingKey, out var statsContainer))
                        {
                            var valueFunc = getValueFunc(statsContainer);
                            return valueFunc != null ? valueFunc.Micrometers : double.MinValue;
                        }
                    }

                    return double.MinValue;
                }),
                HeaderName = columnName,
                ValueBinding = new MultiBinding
                {
                    Converter = new InvokeFuncMultiConverter(),
                    Bindings =
                    {
                        // Binding to convert the ThicknessPointResult to a string value
                        new Binding
                        {
                            Source = new Func<ThicknessPointResult, int, string>((result, digits) =>
                            {
                                if (result == null) return string.Empty;

                                if (externalProcessingKey == ThicknessResultVM.TotalLayerName)
                                {
                                    return LengthToStringConverter.ConvertToString(getValueFunc(result.TotalThicknessStat), digits, false, "-", unit);
                                }

                                if (externalProcessingKey == ThicknessResultVM.WaferThickness)
                                {
                                    if (result.WaferThicknessStat != null)
                                    {
                                        return LengthToStringConverter.ConvertToString(getValueFunc(result.WaferThicknessStat), digits, false, "-", unit);
                                    }
                                }

                                if (result.ThicknessLayerStats.TryGetValue(externalProcessingKey, out var statsContainer))
                                {
                                    return LengthToStringConverter.ConvertToString(getValueFunc(statsContainer), digits, false, "-", unit);
                                }
                                
                                return string.Empty;
                            }),
                        },
                        // Binding to the current context (The ThicknessPointResult)
                        new Binding("."),
                        new Binding(nameof(Digits))
                        {
                            Source = this
                        }
                    }
                }
            });
        }

        #endregion

        public void SetLayersUnit(Dictionary<string, LengthUnit> layersUnit)
        {
            _layersUnit = layersUnit;
        }
    }
}
