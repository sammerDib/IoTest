using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography
{
    public class TopographyDataRepetaVM : MetroDataRepetaVM<TopographyPointData>
    {
        #region Properties

        public List<GeneratedStateColumn> GeneratedColumns { get; } = new List<GeneratedStateColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get { return _generateColumnsFlag; }
            set { SetProperty(ref _generateColumnsFlag, value); }
        }

        private string _selectedOutput;

        public string SelectedOutput
        {
            get { return _selectedOutput; }
            set
            {
                if (SetProperty(ref _selectedOutput, value))
                {
                    UpdateValues();
                }
            }
        }

        private List<string> _outputSource;

        public List<string> OutputSource
        {
            get { return _outputSource; }
            set
            {
                if (SetProperty(ref _outputSource, value))
                {
                    UpdateValues();
                }
            }
        }

        #endregion

        public TopographyDataRepetaVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
        }

        #region Overrides of MetroDataRepetaVM<TopographyPointData>

        protected override void InternalUpdateValues(MeasurePointResult measurePointResult)
        {
            var point = measurePointResult as TopographyPointResult;
            var statsContainer = ExtractStats(point, SelectedOutput);
            Max = LengthToStringConverter.ConvertToString(statsContainer?.Max, Digits);
            Min = LengthToStringConverter.ConvertToString(statsContainer?.Min, Digits);
            Delta = LengthToStringConverter.ConvertToString(statsContainer?.Delta, Digits);
            Mean = LengthToStringConverter.ConvertToString(statsContainer?.Mean, Digits);
            Sigma3 = LengthToStringConverter.ConvertToString(statsContainer?.Sigma3, Digits);
        }

        private static IStatsContainer ExtractStats(TopographyPointResult point, string selectedOutput)
        {
            if (point == null || string.IsNullOrEmpty(selectedOutput)) return null;

            switch (selectedOutput)
            {
                default:
                    return point.ExternalProcessingStats.TryGetValue(selectedOutput, out var stats) ? stats : null;
            }
        }

        #endregion

        public void UpdateOutputSource(List<string> newSource)
        {
            OutputSource = newSource;

            GeneratedColumns.Clear();

            foreach (string output in OutputSource)
            {
                Func<TopographyPointData, double?> getValue;
                Func<TopographyPointData, MeasureState> getState;
                string columnName;
                switch (output)
                {
                    default:
                        columnName = output;
                        getValue = data =>
                        {
                            if (data != null && data.ExternalProcessingResults != null)
                            {
                                var externalProcessingResult = data?.ExternalProcessingResults.SingleOrDefault(result => result.Name.Equals(output));
                                return externalProcessingResult?.Value;
                            }
                            else
                            {
                                return double.NaN;
                            }
                        };
                        getState = data =>
                        {
                            if (data != null && data.ExternalProcessingResults != null)
                            {
                                var externalProcessingResult = data?.ExternalProcessingResults.SingleOrDefault(result => result.Name.Equals(output));
                                return externalProcessingResult?.State ?? MeasureState.NotMeasured;
                            }
                            else
                            {
                                return MeasureState.NotMeasured;
                            }
                        };
                        break;
                }

                GenerateColumn(columnName, getValue, getState);
            }

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;

            SelectedOutput = OutputSource.FirstOrDefault();
        }

        private void GenerateColumn(string columnName, Func<TopographyPointData, double?> getValueFunc, Func<TopographyPointData, MeasureState> getStateFunc)
        {
            GeneratedColumns.Add(new GeneratedStateColumn
            {
                SortDefinition = RepetaSource.Sort.AddSortDefinition(data =>
                {
                    double? value = getValueFunc(data);
                    return value ?? double.MinValue;
                }),
                HeaderName = columnName,
                ValueBinding = new MultiBinding
                {
                    Converter = new InvokeFuncMultiConverter(),
                    Bindings =
                    {
                        // Binding to convert the TopographyPointData to a string value
                        new Binding
                        {
                            Source = new Func<TopographyPointData, int, string>((result, digits) => LengthToStringConverter.ConvertToString(getValueFunc(result), digits)),
                        },
                        // Binding to the current context (The TopographyPointResult)
                        new Binding("."),
                        new Binding(nameof(Digits))
                        {
                            Source = this
                        }
                    }
                },
                StateBinding = new MultiBinding
                {
                    Converter = new InvokeFuncMultiConverter(),
                    Bindings =
                    {
                        // Binding to convert the TopographyPointData to MeasureState
                        new Binding { Source = getStateFunc },
                        // Binding to the current context (The TopographyPointResult)
                        new Binding(".")
                    }
                }
            });
        }
    }
}
