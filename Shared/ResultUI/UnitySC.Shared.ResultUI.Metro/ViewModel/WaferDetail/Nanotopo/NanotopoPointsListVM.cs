using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo
{
    public class NanotopoPointsListVM : MetroPointsListVM
    {
        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }

        private bool _hideRoughness;

        public bool HideRoughness
        {
            get => _hideRoughness;
            private set => SetProperty(ref _hideRoughness, value);
        }

        public bool HideRoughnessRepeta => HideRepetaColumns || HideRoughness;
        
        private bool _hideStepHeight;

        public bool HideStepHeight
        {
            get => _hideStepHeight;
            private set => SetProperty(ref _hideStepHeight, value);
        }

        public bool HideStepHeightRepeta => HideRepetaColumns || HideStepHeight;

        #region Sort Definitions

        public SortDefinition SortByAvgRoughness { get; }
        public SortDefinition SortBy3SigmaRoughness { get; }
        public SortDefinition SortByMinRoughness { get; }
        public SortDefinition SortByMaxRoughness { get; }

        public SortDefinition SortByAvgStepHeight { get; }
        public SortDefinition SortBy3SigmaStepHeight { get; }
        public SortDefinition SortByMinStepHeight { get; }
        public SortDefinition SortByMaxStepHeight { get; }

        #endregion

        public NanotopoPointsListVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
            SortByAvgRoughness = SortedPoints.Sort.AddSortDefinition(result => result is NanoTopoPointResult nanoTopoPointResult ? nanoTopoPointResult.RoughnessStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaRoughness = SortedPoints.Sort.AddSortDefinition(result => result is NanoTopoPointResult nanoTopoPointResult ? nanoTopoPointResult.RoughnessStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinRoughness = SortedPoints.Sort.AddSortDefinition(result => result is NanoTopoPointResult nanoTopoPointResult ? nanoTopoPointResult.RoughnessStat.Min?.Micrometers : double.MinValue);
            SortByMaxRoughness = SortedPoints.Sort.AddSortDefinition(result => result is NanoTopoPointResult nanoTopoPointResult ? nanoTopoPointResult.RoughnessStat.Max?.Micrometers : double.MinValue);

            SortByAvgStepHeight = SortedPoints.Sort.AddSortDefinition(result => result is NanoTopoPointResult nanoTopoPointResult ? nanoTopoPointResult.StepHeightStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaStepHeight = SortedPoints.Sort.AddSortDefinition(result => result is NanoTopoPointResult nanoTopoPointResult ? nanoTopoPointResult.StepHeightStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinStepHeight = SortedPoints.Sort.AddSortDefinition(result => result is NanoTopoPointResult nanoTopoPointResult ? nanoTopoPointResult.StepHeightStat.Min?.Micrometers : double.MinValue);
            SortByMaxStepHeight = SortedPoints.Sort.AddSortDefinition(result => result is NanoTopoPointResult nanoTopoPointResult ? nanoTopoPointResult.StepHeightStat.Max?.Micrometers : double.MinValue);
        }

        public void UpdateRoughnessAndStepHeightVisibility(bool hasRoughness, bool hasStepHeight)
        {
            HideRoughness = !hasRoughness;
            HideStepHeight = !hasStepHeight;

            OnPropertyChanged(nameof(HideRoughnessRepeta));
            OnPropertyChanged(nameof(HideStepHeightRepeta));
        }

        #region Overrides of MetroPointsListVM

        public override void UpdatePointsSource(ICollection<MeasurePointResult> sourcePoint, bool showRepetaColumns, bool showDieIndex, bool showQualityScore, bool showSiteID)
        {
            base.UpdatePointsSource(sourcePoint, showRepetaColumns, showDieIndex, showQualityScore, showSiteID);

            GeneratedColumns.Clear();
            
            var externalProcessingKeys = sourcePoint.OfType<NanoTopoPointResult>().SelectMany(point => point.ExternalProcessingStats.Keys).Distinct();

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
                    if (result is NanoTopoPointResult nanoTopoPointResult)
                    {
                        if (nanoTopoPointResult.ExternalProcessingStats.TryGetValue(externalProcessingKey, out var statsContainer))
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
                        // Binding to convert the NanoTopoPointResult to a string value
                        new Binding
                        {
                            Source = new Func<NanoTopoPointResult, int, string>((result, digits) =>
                            {
                                if (result == null) return string.Empty;

                                if (result.ExternalProcessingStats.TryGetValue(externalProcessingKey, out var statsContainer))
                                {
                                    return LengthToStringConverter.ConvertToString(getValueFunc(statsContainer), digits);
                                }
                                
                                return string.Empty;
                            }),
                        },
                        // Binding to the current context (The NanoTopoPointResult)
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
