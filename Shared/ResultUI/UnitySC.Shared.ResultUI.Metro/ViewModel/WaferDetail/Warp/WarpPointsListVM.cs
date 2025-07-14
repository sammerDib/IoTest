using System;
using System.Collections.Generic;
using System.Windows.Data;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp
{
    public class WarpPointsListVM : MetroPointsListVM
    {
        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }

        private bool _hideWarp;

        public bool HideWarp
        {
            get => _hideWarp;
            private set => SetProperty(ref _hideWarp, value);
        }

        public bool HideWarpRepeta => HideRepetaColumns || HideWarp;

        public SortDefinition SortByRPD { get; }
        public SortDefinition SortByMinRPD { get; }
        public SortDefinition SortByMaxRPD { get; }
        public SortDefinition SortBy3SigmaRPD { get; }

        public SortDefinition SortByTT { get; }
        public SortDefinition SortByMinTT { get; }
        public SortDefinition SortByMaxTT { get; }
        public SortDefinition SortBy3SigmaTT { get; }

        public WarpPointsListVM(PointSelectorBase pointSelector) : base(pointSelector)
        {

            SortByRPD = SortedPoints.Sort.AddSortDefinition(result => result is WarpPointResult warpResult ? warpResult.RPDStat.Mean?.Micrometers : double.MinValue);
            SortByMinRPD = SortedPoints.Sort.AddSortDefinition(result => result is WarpPointResult warpResult ? warpResult.RPDStat.Min?.Micrometers : double.MinValue);
            SortByMaxRPD = SortedPoints.Sort.AddSortDefinition(result => result is WarpPointResult warpResult ? warpResult.RPDStat.Max?.Micrometers : double.MinValue);
            SortBy3SigmaRPD = SortedPoints.Sort.AddSortDefinition(result => result is WarpPointResult warpResult ? warpResult.RPDStat.Sigma3?.Micrometers : double.MinValue);

            SortByTT = SortedPoints.Sort.AddSortDefinition(result => result is WarpPointResult warpResult ? warpResult.TotalThicknessStat.Mean?.Micrometers : double.MinValue);
            SortByMinTT = SortedPoints.Sort.AddSortDefinition(result => result is WarpPointResult warpResult ? warpResult.TotalThicknessStat.Min?.Micrometers : double.MinValue);
            SortByMaxTT = SortedPoints.Sort.AddSortDefinition(result => result is WarpPointResult warpResult ? warpResult.TotalThicknessStat.Max?.Micrometers : double.MinValue);
            SortBy3SigmaTT = SortedPoints.Sort.AddSortDefinition(result => result is WarpPointResult warpResult ? warpResult.TotalThicknessStat.Sigma3?.Micrometers : double.MinValue);
        }

        public void UpdateWarpVisibility(bool hasWarp)
        {
            HideWarp = !hasWarp;

            OnPropertyChanged(nameof(HideWarpRepeta));
        }

        #region Overrides of MetroPointsListVM

        public override void UpdatePointsSource(ICollection<MeasurePointResult> sourcePoint, bool showRepetaColumns, bool showDieIndex, bool showQualityScore, bool showSiteID)
        {
            base.UpdatePointsSource(sourcePoint, showRepetaColumns, showDieIndex, showQualityScore, showSiteID);

            GeneratedColumns.Clear();
        }

        #endregion
    }
}
