using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness
{
    public class ThicknessDataRepetaVM : MetroDataRepetaVM<ThicknessPointData>
    {
        private Dictionary<string, LengthUnit> _layerUnits = new Dictionary<string, LengthUnit>();

        #region Properties

        public List<GeneratedStateColumn> GeneratedColumns { get; } = new List<GeneratedStateColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get { return _generateColumnsFlag; }
            set { SetProperty(ref _generateColumnsFlag, value); }
        }

        private string _selectedLayer;

        public string SelectedLayer
        {
            get { return _selectedLayer; }
            set
            {
                if (SetProperty(ref _selectedLayer, value))
                {
                    UpdateValues();
                }
            }
        }

        private List<string> _layersSource;

        public List<string> LayersSource
        {
            get { return _layersSource; }
            set
            {
                if (SetProperty(ref _layersSource, value))
                {
                    UpdateValues();
                }
            }
        }

        #endregion

        public ThicknessDataRepetaVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
        }

        #region Overrides of MetroDataRepetaVM<NanoTopoPointData>

        protected override void InternalUpdateValues(MeasurePointResult measurePointResult)
        {
            var point = measurePointResult as ThicknessPointResult;
            var statsContainer = ExtractStats(point, SelectedLayer);

            var unit = LengthUnit.Undefined;
            if (!string.IsNullOrEmpty(SelectedLayer))
            {
                _layerUnits.TryGetValue(SelectedLayer, out unit);
            }

            Max = LengthToStringConverter.ConvertToString(statsContainer?.Max, Digits, true, "-", unit);
            Min = LengthToStringConverter.ConvertToString(statsContainer?.Min, Digits, true, "-", unit);
            Delta = LengthToStringConverter.ConvertToString(statsContainer?.Delta, Digits, true, "-", unit);
            Mean = LengthToStringConverter.ConvertToString(statsContainer?.Mean, Digits, true, "-", unit);
            Sigma3 = LengthToStringConverter.ConvertToString(statsContainer?.Sigma3, Digits, true, "-", unit);
        }

        private static IStatsContainer ExtractStats(ThicknessPointResult point, string selectedOutput)
        {
            if (point == null || string.IsNullOrEmpty(selectedOutput)) return null;

            switch (selectedOutput)
            {
                case ThicknessResultVM.CrossSectionModeName:
                case ThicknessResultVM.TotalLayerName:
                    return point.TotalThicknessStat;
                case ThicknessResultVM.WaferThickness:
                    return point.WaferThicknessStat;
                default:
                    return point.ThicknessLayerStats.TryGetValue(selectedOutput, out var stats) ? stats : null;
            }
        }

        #endregion

        public void UpdateLayerSource(List<string> newSource, Dictionary<string, LengthUnit> layerUnits)
        {
            LayersSource = newSource;
            _layerUnits = layerUnits;

            GeneratedColumns.Clear();

            foreach (string output in LayersSource)
            {
                _layerUnits.TryGetValue(output, out var unit);
                string stringUnit = Length.GetUnitSymbol(unit);
                
                Func<ThicknessPointData, Length> getValue;
                Func<ThicknessPointData, MeasureState> getState;
                string columnName;
                
                switch (output)
                {
                    // Defined in Xaml
                    case ThicknessResultVM.TotalLayerName:
                        getValue = data => data.TotalThickness;
                        getState = data => data.TotalState;
                        columnName = $"{output} ({stringUnit})";
                        break;

                    case ThicknessResultVM.WaferThickness:
                        columnName = $"{output} ({stringUnit})";
                        getValue = data => data.WaferThicknessResult?.Length;
                        getState = data => data.WaferThicknessResult?.State ?? MeasureState.NotMeasured;
                        break;

                    default:
                        columnName = $"{output} ({stringUnit})";
                        getValue = data =>
                        {
                            var externalProcessingResult = data.ThicknessLayerResults.SingleOrDefault(result => result.Name.Equals(output));
                            return externalProcessingResult?.Length;
                        };
                        getState = data =>
                        {
                            var externalProcessingResult = data.ThicknessLayerResults.SingleOrDefault(result => result.Name.Equals(output));
                            return externalProcessingResult?.State ?? MeasureState.NotMeasured;
                        };
                        break;
                }

                GenerateColumn(columnName, getValue, getState, unit);
            }

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;

            SelectedLayer = LayersSource.FirstOrDefault();
        }

        private void GenerateColumn(string columnName, Func<ThicknessPointData, Length> getValueFunc, Func<ThicknessPointData, MeasureState> getStateFunc, LengthUnit unit)
        {
            GeneratedColumns.Add(new GeneratedStateColumn
            {
                SortDefinition = RepetaSource.Sort.AddSortDefinition(data =>
                {
                    double? value = getValueFunc(data)?.Micrometers;
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
                            Source = new Func<ThicknessPointData, int, string>((result, digits) => LengthToStringConverter.ConvertToString(getValueFunc(result), digits, false, "-", unit)),
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
                        // Binding to convert the NanoTopoPointData to MeasurePointState
                        new Binding { Source = getStateFunc },
                        // Binding to the current context (The NanoTopoPointResult)
                        new Binding(".")
                    }
                }
            });
        }
    }
}
