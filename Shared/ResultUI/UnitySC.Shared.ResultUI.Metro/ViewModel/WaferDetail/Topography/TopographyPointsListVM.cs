using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography
{
    public class TopographyPointsListVM : MetroPointsListVM
    {
        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }

        public TopographyPointsListVM(PointSelectorBase pointSelector) : base(pointSelector)
        {

        }

        #region Overrides of MetroPointsListVM

        public override void UpdatePointsSource(ICollection<MeasurePointResult> sourcePoint, bool showRepetaColumns, bool showDieIndex, bool showQualityScore, bool showSiteID)
        {
            base.UpdatePointsSource(sourcePoint, showRepetaColumns, showDieIndex, showQualityScore, showSiteID);

            GeneratedColumns.Clear();
            
            var externalProcessingKeys = sourcePoint.OfType<TopographyPointResult>().SelectMany(point => point.ExternalProcessingStats.Keys).Distinct();

            foreach (string externalProcessingKey in externalProcessingKeys)
            {
                if (showRepetaColumns)
                {
                    GenerateColumn($"Avg {externalProcessingKey}", externalProcessingKey, container => container.Mean);
                    GenerateColumn($"3σ {externalProcessingKey}", externalProcessingKey, container => container.Sigma3);
                    GenerateColumn($"Min {externalProcessingKey}", externalProcessingKey, container => container.Min);
                    GenerateColumn($"Max {externalProcessingKey}", externalProcessingKey, container => container.Max);
                }
                else
                {
                    GenerateColumn(externalProcessingKey, externalProcessingKey, container => container.Mean);
                }
            }

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;
        }

        private void GenerateColumn(string columnName, string externalProcessingKey, Func<MetroDoubleStatsContainer, double> getValueFunc)
        {
            GeneratedColumns.Add(new GeneratedColumn
            {
                SortDefinition = SortedPoints.Sort.AddSortDefinition(result =>
                {
                    if (result is TopographyPointResult TopographyPointResult)
                    {
                        if (TopographyPointResult.ExternalProcessingStats.TryGetValue(externalProcessingKey, out var statsContainer))
                        {
                            return getValueFunc(statsContainer);
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
                        // Binding to convert the TopographyPointResult to a string value
                        new Binding
                        {
                            Source = new Func<TopographyPointResult, int, string>((result, digits) =>
                            {
                                if (result == null) return string.Empty;

                                if (result.ExternalProcessingStats.TryGetValue(externalProcessingKey, out var statsContainer))
                                {
                                    return LengthToStringConverter.ConvertToString(getValueFunc(statsContainer), digits);
                                }
                                
                                return string.Empty;
                            }),
                        },
                        // Binding to the current context (The TopographyPointResult)
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
    }
}
