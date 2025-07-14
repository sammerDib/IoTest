using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows.Data;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp
{
    public class WarpDataRepetaVM : MetroDataRepetaVM<WarpPointData>
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

        private List<string> _outputSources;

        public List<string> OutputSources
        {
            get { return _outputSources; }
            set
            {
                if (SetProperty(ref _outputSources, value))
                {
                    UpdateValues();
                }
            }
        }

        #endregion

        public WarpDataRepetaVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
        }

        #region Overrides of MetroDataRepetaVM<WarpPointData>

        protected override void InternalUpdateValues(MeasurePointResult measurePointResult)
        {
            var point = measurePointResult as WarpPointResult;
            var statsContainer = ExtractStats(point, SelectedOutput);

            Max = LengthToStringConverter.ConvertToString(statsContainer?.Max, Digits, true, "-", LengthUnit.Micrometer);
            Min = LengthToStringConverter.ConvertToString(statsContainer?.Min, Digits, true, "-", LengthUnit.Micrometer);
            Delta = LengthToStringConverter.ConvertToString(statsContainer?.Delta, Digits, true, "-", LengthUnit.Micrometer);
            Mean = LengthToStringConverter.ConvertToString(statsContainer?.Mean, Digits, true, "-", LengthUnit.Micrometer);
            Sigma3 = LengthToStringConverter.ConvertToString(statsContainer?.Sigma3, Digits, true, "-", LengthUnit.Micrometer);
        }
        private static IStatsContainer ExtractStats(WarpPointResult point, string selectedOutput)
        {
            if (point == null || string.IsNullOrEmpty(selectedOutput)) return null;

            WarpViewerType viewerType = EnumUtils.GetEnumFromDescription<WarpViewerType>(selectedOutput);

            return viewerType == WarpViewerType.WARP ? point.RPDStat : point.TotalThicknessStat;

        }
        #endregion

        public void UpdateOutputSource(List<string> newSources)
        {
            OutputSources = newSources;

            GeneratedColumns.Clear();

            foreach (string output in OutputSources)
            {
                Func<WarpPointData, double?> getValue;
                Func<WarpPointData, MeasureState> getState;
                string columnName;

                WarpViewerType viewerType = EnumUtils.GetEnumFromDescription<WarpViewerType>(output);
                if (viewerType == WarpViewerType.WARP)
                {
                    getValue = data => data.RPD?.Micrometers;
                    getState = data => data.State;
                    columnName = $"{output} (µm)";
                }
                else
                {
                    getValue = data => data.TotalThickness?.Micrometers;
                    getState = data => data.State;
                    columnName = $"{output} (µm)";
                }

                GenerateColumn(columnName, getValue, getState);
            }

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;

            SelectedOutput = OutputSources.FirstOrDefault();
        }

        private void GenerateColumn(string columnName, Func<WarpPointData, double?> getValueFunc, Func<WarpPointData, MeasureState> getStateFunc)
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
                        // Binding to convert the WarpPointData to a string value
                        new Binding
                        {
                            Source = new Func<WarpPointData, int, string>((result, digits) => LengthToStringConverter.ConvertToString(getValueFunc(result), digits)),
                        },
                        // Binding to the current context (The WarpPointResult)
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
                        // Binding to convert the WarpPointData to MeasureState
                        new Binding { Source = getStateFunc },
                        // Binding to the current context (The WarpPointResult)
                        new Binding(".")
                    }
                }
            });
        }
    }
}
