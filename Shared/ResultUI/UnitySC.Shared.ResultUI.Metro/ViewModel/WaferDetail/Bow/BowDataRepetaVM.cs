using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow
{
    public class BowDataRepetaVM : MetroDataRepetaVM<BowTotalPointData>
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

        public BowDataRepetaVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
        }

        #region Overrides of MetroDataRepetaVM<xxxPointData>

        protected override void InternalUpdateValues(MeasurePointResult measurePointResult)
        {
            var point = measurePointResult as BowPointResult;
            var statsContainer = ExtractStats(point, SelectedOutput);

            var unit = LengthUnit.Undefined;
            if (SelectedOutput == BowResultVM.BowOutputName) unit = LengthUnit.Micrometer;

            Max = LengthToStringConverter.ConvertToString(statsContainer?.Max, Digits, true, "-", unit);
            Min = LengthToStringConverter.ConvertToString(statsContainer?.Min, Digits, true, "-", unit);
            Delta = LengthToStringConverter.ConvertToString(statsContainer?.Delta, Digits, true, "-", unit);
            Mean = LengthToStringConverter.ConvertToString(statsContainer?.Mean, Digits, true, "-", unit);
            Sigma3 = LengthToStringConverter.ConvertToString(statsContainer?.Sigma3, Digits, true, "-", unit);
        }

        private static IStatsContainer ExtractStats(BowPointResult point, string selectedOutput)
        {
            if (point == null || string.IsNullOrEmpty(selectedOutput)) return null;

            switch (selectedOutput)
            {
                case BowResultVM.BowOutputName:
                    return point.BowStat;

                default:
                    return null;
            }
        }

        #endregion

        public void UpdateOutputSource(List<string> newSource)
        {
            OutputSource = newSource;

            GeneratedColumns.Clear();

            foreach (string output in OutputSource)
            {
                Func<BowTotalPointData, double?> getValue;
                Func<BowTotalPointData, MeasureState> getState;
                string columnName;
                switch (output)
                {
                    // Defined in Xaml

                    case BowResultVM.BowOutputName:
                        getValue = data => data.Bow?.Micrometers;
                        getState = data => data.State;
                        columnName = $"{output} (µm)";
                        break;

                    default:
                        columnName = output;
                        getValue = data => 0;
                        getState = data => 0;
                        break;
                }

                GenerateColumn(columnName, getValue, getState);
            }

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;

            SelectedOutput = OutputSource.FirstOrDefault();
        }

        private void GenerateColumn(string columnName, Func<BowTotalPointData, double?> getValueFunc, Func<BowTotalPointData, MeasureState> getStateFunc)
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
                        // Binding to convert the BowPointData to a string value
                        new Binding
                        {
                            Source = new Func<BowTotalPointData, int, string>((result, digits) => LengthToStringConverter.ConvertToString(getValueFunc(result), digits, false, "-", LengthUnit.Micrometer)),
                        },
                        // Binding to the current context (The BowPointResult)
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
                        // Binding to convert the BowPointData to MeasureState
                        new Binding { Source = getStateFunc },
                        // Binding to the current context (The BowPointResult)
                        new Binding(".")
                    }
                }
            });
        }
    }
}
