using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo
{
    public class NanotopoDataRepetaVM : MetroDataRepetaVM<NanoTopoPointData>
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

        public NanotopoDataRepetaVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
        }

        #region Overrides of MetroDataRepetaVM<NanoTopoPointData>

        protected override void InternalUpdateValues(MeasurePointResult measurePointResult)
        {
            var point = measurePointResult as NanoTopoPointResult;
            var statsContainer = ExtractStats(point, SelectedOutput);

            var unit = LengthUnit.Undefined;
            if (SelectedOutput == NanotopoResultVM.RoughnessOutputName || SelectedOutput == NanotopoResultVM.StepHeightOutputName) unit = LengthUnit.Nanometer;

            Max = LengthToStringConverter.ConvertToString(statsContainer?.Max, Digits, true, "-", unit);
            Min = LengthToStringConverter.ConvertToString(statsContainer?.Min, Digits, true, "-", unit);
            Delta = LengthToStringConverter.ConvertToString(statsContainer?.Delta, Digits, true, "-", unit);
            Mean = LengthToStringConverter.ConvertToString(statsContainer?.Mean, Digits, true, "-", unit);
            Sigma3 = LengthToStringConverter.ConvertToString(statsContainer?.Sigma3, Digits, true, "-", unit);
        }

        private static IStatsContainer ExtractStats(NanoTopoPointResult point, string selectedOutput)
        {
            if (point == null || string.IsNullOrEmpty(selectedOutput)) return null;

            switch (selectedOutput)
            {
                case NanotopoResultVM.RoughnessOutputName:
                    return point.RoughnessStat;
                case NanotopoResultVM.StepHeightOutputName:
                    return point.StepHeightStat;
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
                Func<NanoTopoPointData, double?> getValue;
                Func<NanoTopoPointData, MeasureState> getState;
                string columnName;
                switch (output)
                {
                    case NanotopoResultVM.RoughnessOutputName:
                        getValue = data => data.Roughness?.Nanometers;
                        getState = data => data.RoughnessState;
                        columnName = $"{output} (nm)";
                        break;
                    // Defined in Xaml
                    case NanotopoResultVM.StepHeightOutputName:
                        getValue = data => data.StepHeight?.Nanometers;
                        getState = data => data.StepHeightState;
                        columnName = $"{output} (nm)";
                        break;
                    default:
                        columnName = output;
                        getValue = data =>
                        {
                            var externalProcessingResult = data?.ExternalProcessingResults.SingleOrDefault(result => result.Name.Equals(output));
                            return externalProcessingResult?.Value;
                        };
                        getState = data =>
                        {
                            var externalProcessingResult = data?.ExternalProcessingResults.SingleOrDefault(result => result.Name.Equals(output));
                            return externalProcessingResult?.State ?? MeasureState.NotMeasured;
                        };
                        break;
                }

                GenerateColumn(columnName, getValue, getState);
            }

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;

            SelectedOutput = OutputSource.FirstOrDefault();
        }

        private void GenerateColumn(string columnName, Func<NanoTopoPointData, double?> getValueFunc, Func<NanoTopoPointData, MeasureState> getStateFunc)
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
                        // Binding to convert the NanoTopoPointData to a string value
                        new Binding
                        {
                            Source = new Func<NanoTopoPointData, int, string>((result, digits) => LengthToStringConverter.ConvertToString(getValueFunc(result), digits, false, "-", LengthUnit.Nanometer)),
                        },
                        // Binding to the current context (The NanoTopoPointResult)
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
                        // Binding to convert the NanoTopoPointData to MeasureState
                        new Binding { Source = getStateFunc },
                        // Binding to the current context (The NanoTopoPointResult)
                        new Binding(".")
                    }
                }
            });
        }
    }
}
