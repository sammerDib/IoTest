using System;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv
{
    public enum DataRepetaType
    {
        Depth,
        Length,
        Width
    }

    public class TsvDataRepetaVM : MetroDataRepetaVM<TSVPointData>
    {
        #region Properties

        private DataRepetaType _selectedRepetaType;

        public DataRepetaType SelectedRepetaType
        {
            get { return _selectedRepetaType; }
            set
            {
                if (SetProperty(ref _selectedRepetaType, value))
                {
                    UpdateValues();
                }
            }
        }
        
        public SortDefinition SortByDepth { get; }
        public SortDefinition SortByLength { get; }
        public SortDefinition SortByWidth { get; }
        
        #endregion

        #region Ctor

        public TsvDataRepetaVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
            SortByDepth = RepetaSource.Sort.AddSortDefinition(data => data.Depth == null ? double.NaN : data.Depth.Micrometers);
            SortByLength = RepetaSource.Sort.AddSortDefinition(data => data.Length == null ? double.NaN : data.Length.Micrometers);
            SortByWidth = RepetaSource.Sort.AddSortDefinition(data => data.Width == null ? double.NaN : data.Width.Micrometers);
        }

        #endregion

        protected override void InternalUpdateValues(MeasurePointResult point)
        {
            var tsvPoint = point as TSVPointResult;
            var statsContainer = ExtractStats(tsvPoint, SelectedRepetaType);
            Max = LengthToStringConverter.ConvertToString(statsContainer?.Max, Digits, true, "-", LengthUnit.Micrometer);
            Min = LengthToStringConverter.ConvertToString(statsContainer?.Min, Digits, true, "-", LengthUnit.Micrometer);
            Delta = LengthToStringConverter.ConvertToString(statsContainer?.Delta, Digits, true, "-", LengthUnit.Micrometer);
            Mean = LengthToStringConverter.ConvertToString(statsContainer?.Mean, Digits, true, "-", LengthUnit.Micrometer);
            Sigma3 = LengthToStringConverter.ConvertToString(statsContainer?.Sigma3, Digits, true, "-", LengthUnit.Micrometer);
        }

        private static MetroStatsContainer ExtractStats(TSVPointResult point, DataRepetaType type)
        {
            if (point == null) return null;

            switch (type)
            {
                case DataRepetaType.Depth:
                    return point.DepthTsvStat;
                case DataRepetaType.Length:
                    return point.LengthTsvStat;
                case DataRepetaType.Width:
                    return point.WidthTsvStat;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
