using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Metro.Converters;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Tsv
{
    public class TsvStatsVM : MetroStatsVM
    {
        private const string LengthName = "Length";
        private const string WidthName = "Width";
        private const string DepthName = "Depth";
        private const string NullDisplayValue = "-";

        #region Properties
        
        public Func<WaferResultData, string> WaferResultToMeanLengthFunc { get; } = data => GetFormatedValue(data, LengthName, ResultValueType.Mean, NullDisplayValue);

        public Func<WaferResultData, string> WaferResultToMinLengthFunc { get; } = data => GetFormatedValue(data, LengthName, ResultValueType.Min, NullDisplayValue);

        public Func<WaferResultData, string> WaferResultToMaxLengthFunc { get; } = data => GetFormatedValue(data, LengthName, ResultValueType.Max, NullDisplayValue);

        public Func<WaferResultData, string> WaferResultToLengthStatusFunc { get; } = data => GetState(data, LengthName).ToHumanizedString();

        public Func<WaferResultData, Tolerance> WaferResultToLengthToleranceFunc { get; } = data => MeasureStateToToleranceDisplayerConverter.Convert(GetState(data, LengthName));

        public Func<WaferResultData, string> WaferResultToMeanWidthFunc { get; } = data => GetFormatedValue(data, WidthName, ResultValueType.Mean, NullDisplayValue);

        public Func<WaferResultData, string> WaferResultToMinWidthFunc { get; } = data => GetFormatedValue(data, WidthName, ResultValueType.Min, NullDisplayValue);

        public Func<WaferResultData, string> WaferResultToMaxWidthFunc { get; } = data => GetFormatedValue(data, WidthName, ResultValueType.Max, NullDisplayValue);

        public Func<WaferResultData, string> WaferResultToWidthStatusFunc { get; } = data => GetState(data, WidthName).ToHumanizedString();

        public Func<WaferResultData, Tolerance> WaferResultToWidthToleranceFunc { get; } = data => MeasureStateToToleranceDisplayerConverter.Convert(GetState(data, WidthName));

        public Func<WaferResultData, string> WaferResultToMeanDepthFunc { get; } = data => GetFormatedValue(data, DepthName, ResultValueType.Mean, NullDisplayValue);

        public Func<WaferResultData, string> WaferResultToMinDepthFunc { get; } = data => GetFormatedValue(data, DepthName, ResultValueType.Min, NullDisplayValue);

        public Func<WaferResultData, string> WaferResultToMaxDepthFunc { get; } = data => GetFormatedValue(data, DepthName, ResultValueType.Max, NullDisplayValue);

        public Func<WaferResultData, string> WaferResultToDepthStatusFunc { get; } = data => GetState(data, DepthName).ToHumanizedString();

        public Func<WaferResultData, Tolerance> WaferResultToDepthToleranceFunc { get; } = data => MeasureStateToToleranceDisplayerConverter.Convert(GetState(data, DepthName));

        #region Sort Definitions

        public SortDefinition SortByMeanDepth { get; }
        public SortDefinition SortByMinDepth { get; }
        public SortDefinition SortByMaxDepth { get; }
        public SortDefinition SortByDepthStatus { get; }

        public SortDefinition SortByMeanWidth { get; }
        public SortDefinition SortByMinWidth { get; }
        public SortDefinition SortByMaxWidth { get; }
        public SortDefinition SortByWidthStatus { get; }

        public SortDefinition SortByMeanLength { get; }
        public SortDefinition SortByMinLength { get; }
        public SortDefinition SortByMaxLength { get; }
        public SortDefinition SortByLengthStatus { get; }

        #endregion

        #endregion Properties

        public TsvStatsVM()
        {
            SortByMeanDepth = WaferResultDatas.Sort.AddSortDefinition(data => GetValue(data, DepthName, ResultValueType.Mean) ?? double.MinValue);
            SortByMinDepth = WaferResultDatas.Sort.AddSortDefinition(data => GetValue(data, DepthName, ResultValueType.Min) ?? double.MinValue);
            SortByMaxDepth = WaferResultDatas.Sort.AddSortDefinition(data => GetValue(data, DepthName, ResultValueType.Max) ?? double.MinValue);
            SortByDepthStatus = WaferResultDatas.Sort.AddSortDefinition(data => GetStateIndex(data, DepthName));

            SortByMeanWidth = WaferResultDatas.Sort.AddSortDefinition(data => GetValue(data, WidthName, ResultValueType.Mean) ?? double.MinValue);
            SortByMinWidth = WaferResultDatas.Sort.AddSortDefinition(data => GetValue(data, WidthName, ResultValueType.Min) ?? double.MinValue);
            SortByMaxWidth = WaferResultDatas.Sort.AddSortDefinition(data => GetValue(data, WidthName, ResultValueType.Max) ?? double.MinValue);
            SortByWidthStatus = WaferResultDatas.Sort.AddSortDefinition(data => GetStateIndex(data, WidthName));

            SortByMeanLength = WaferResultDatas.Sort.AddSortDefinition(data => GetValue(data, LengthName, ResultValueType.Mean) ?? double.MinValue);
            SortByMinLength = WaferResultDatas.Sort.AddSortDefinition(data => GetValue(data, LengthName, ResultValueType.Min) ?? double.MinValue);
            SortByMaxLength = WaferResultDatas.Sort.AddSortDefinition(data => GetValue(data, LengthName, ResultValueType.Max) ?? double.MinValue);
            SortByLengthStatus = WaferResultDatas.Sort.AddSortDefinition(data => GetStateIndex(data, LengthName));
        }

        protected override IEnumerable<string> GetCsvLines()
        {
            var lines = new List<string>
            {
                "Slot Id;State;Avg Depth (µm);Min Depth (µm);Max Depth (µm);Depth Status;Avg Width (µm);Min Width (µm);Max Width (µm);Width Status;Avg Length (µm);Min Length (µm);Max Length (µm);Length Status"
            };

            foreach (var resultData in WaferResultDatas)
            {
                var line = new StringBuilder();
                line.Append($"{resultData.SlotId};{((ResultState)resultData.ResultItem.State).ToHumanizedString()};");

                // Depth
                line.Append($"{GetFormatedValue(resultData, DepthName, ResultValueType.Mean)};");
                line.Append($"{GetFormatedValue(resultData, DepthName, ResultValueType.Min)};");
                line.Append($"{GetFormatedValue(resultData, DepthName, ResultValueType.Max)};");
                line.Append($"{GetState(resultData, DepthName).ToHumanizedString()};");

                // Width
                line.Append($"{GetFormatedValue(resultData, WidthName, ResultValueType.Mean)};");
                line.Append($"{GetFormatedValue(resultData, WidthName, ResultValueType.Min)};");
                line.Append($"{GetFormatedValue(resultData, WidthName, ResultValueType.Max)};");
                line.Append($"{GetState(resultData, WidthName).ToHumanizedString()};");

                // Length
                line.Append($"{GetFormatedValue(resultData, LengthName, ResultValueType.Mean)};");
                line.Append($"{GetFormatedValue(resultData, LengthName, ResultValueType.Min)};");
                line.Append($"{GetFormatedValue(resultData, LengthName, ResultValueType.Max)};");
                line.Append($"{GetState(resultData, LengthName).ToHumanizedString()}");

                lines.Add(line.ToString());
            }

            return lines;
        }

        protected override void RedrawChart()
        {
            var waferResults = WaferResultDatas.Where(waferResultData => waferResultData.ResultItem != null).ToList();
            SetData(waferResults, nameof(TSVPointData.Length));
            SetData(waferResults, nameof(TSVPointData.Width));
            SetData(waferResults, nameof(TSVPointData.Depth));
        }
    }
}
