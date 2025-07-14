using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.Converters;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Trench
{
    public class TrenchStatsVM : MetroStatsVM
    {
        #region Properties

        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }

        #endregion

        #region Overrides of MetroStatsVM

        protected override IEnumerable<string> GetCsvLines()
        {
            var lines = new List<string>
            {
                GeneratedColumns.Aggregate("Slot Id;State", (current, generatedColumn) => current + $";{generatedColumn.HeaderName}")
            };

            foreach (var resultData in WaferResultDatas)
            {
                string line = $"{resultData.SlotId};{((ResultState)resultData.ResultItem.State).ToHumanizedString()}";
                line = GeneratedColumns.Aggregate(line, (current, column) => current + $";{BindingHelper.GetValue(resultData, column.ValueBinding)}");
                lines.Add(line);
            }

            return lines;
        }

        protected override void RedrawChart()
        {
            var waferResults = WaferResultDatas.Where(waferResultData => waferResultData.ResultItem != null).ToList();


            var outputs = waferResults.
                SelectMany(data => data.ResultItem.ResultItemValues).
                Select(value => new KeyValuePair<string, UnitType>(value.Name, (UnitType)value.UnitType)).
                ToList();

            var distinctOutputName = outputs.Select(pair => pair.Key).Distinct().Take(3);
            var outputToDisplay = distinctOutputName.Select(output => outputs.First(pair => string.Equals(pair.Key, output))).ToList();


            foreach (var output in outputToDisplay)
            {
                SetData(waferResults, output.Key);
            }

            GenerateColumns(outputToDisplay);
        }

        #endregion

        private void GenerateColumns(List<KeyValuePair<string, UnitType>> outputs)
        {
            GeneratedColumns.Clear();

            foreach (var output in outputs)
            {
                string unit = output.Value.ToHumanizedString();
                if (!string.IsNullOrWhiteSpace(unit))
                {
                    unit = $" ({unit})";
                }

                GenerateColumn($"Mean {output.Key}{unit}", data => GetValue(data, output.Key, ResultValueType.Mean));
                GenerateColumn($"Min {output.Key}{unit}", data => GetValue(data, output.Key, ResultValueType.Min));
                GenerateColumn($"Max {output.Key}{unit}", data => GetValue(data, output.Key, ResultValueType.Max));
                GenerateStateColumn($"{output.Key} Status", data => GetState(data, output.Key));
            }

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;
        }

        private void GenerateColumn(string columnName, Func<WaferResultData, double?> getValueFunc)
        {
            GeneratedColumns.Add(new GeneratedColumn
            {
                SortDefinition = WaferResultDatas.Sort.AddSortDefinition(data => getValueFunc(data) ?? double.MinValue),
                HeaderName = columnName,
                ValueBinding = new MultiBinding
                {
                    Converter = new InvokeFuncMultiConverter(),
                    Bindings =
                    {
                        // Binding to convert the WaferResultData to a string value
                        new Binding
                        {
                            Source = new Func<WaferResultData, string>(result => LengthToStringConverter.ConvertToString(getValueFunc(result), 5, false, "-", LengthUnit.Micrometer)),
                        },
                        // Binding to the current context (The WaferResultData)
                        new Binding(".")
                    }
                }
            });
        }

        private void GenerateStateColumn(string columnName, Func<WaferResultData, MeasureState?> getStateFunc)
        {
            GeneratedColumns.Add(new GeneratedStateColumn
            {
                SortDefinition = WaferResultDatas.Sort.AddSortDefinition(data =>
                {
                    var value = getStateFunc(data);
                    return value.HasValue ? (int)value.Value : int.MaxValue;
                }),
                HeaderName = columnName,
                ValueBinding = new MultiBinding
                {
                    Converter = new InvokeFuncMultiConverter(),
                    Bindings =
                    {
                        // Binding to convert the WaferResultData to a string value
                        new Binding
                        {
                            Source = new Func<WaferResultData, string>(result => getStateFunc(result).ToHumanizedString())
                        },
                        // Binding to the current context (The WaferResultData)
                        new Binding(".")
                    }
                },
                StateBinding = new MultiBinding
                {
                    Converter = new InvokeFuncMultiConverter(),
                    Bindings =
                    {
                        // Binding to convert the WaferResultData to Tolerance
                        new Binding { Source = new Func<WaferResultData, Tolerance>(result => MeasureStateToToleranceDisplayerConverter.Convert(getStateFunc(result)))},
                        // Binding to the current context (The WaferResultData)
                        new Binding(".")
                    }
                }
            });
        }
    }
}
