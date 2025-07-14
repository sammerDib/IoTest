using System.Collections.Generic;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.PeriodicStruct;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.PeriodicStruct
{
    public class PeriodicStructPointsListVM : MetroPointsListVM
    {
        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }

        private bool _hideHeight;

        public bool HideHeight
        {
            get => _hideHeight;
            private set => SetProperty(ref _hideHeight, value);
        }

        private bool _hideWidth;

        public bool HideWidth
        {
            get => _hideWidth;
            private set => SetProperty(ref _hideWidth, value);
        }

        public bool HideHeightRepeta => HideRepetaColumns || HideHeight;

        public bool HideWidthRepeta => HideRepetaColumns || HideWidth;

        public bool HidePitchRepeta => HideRepetaColumns;

        #region Sort Definitions

        public SortDefinition SortByAvgHeight { get; }
        public SortDefinition SortBy3SigmaHeight { get; }
        public SortDefinition SortByMinHeight { get; }
        public SortDefinition SortByMaxHeight { get; }

        public SortDefinition SortByAvgWidth { get; }
        public SortDefinition SortBy3SigmaWidth { get; }
        public SortDefinition SortByMinWidth { get; }
        public SortDefinition SortByMaxWidth { get; }

        public SortDefinition SortByAvgPitch { get; }
        public SortDefinition SortBy3SigmaPitch { get; }
        public SortDefinition SortByMinPitch { get; }
        public SortDefinition SortByMaxPitch { get; }

        #endregion

        public PeriodicStructPointsListVM(PeriodicStructPointSelector pointSelector) : base(pointSelector)
        {
            SortByAvgHeight = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.HeightStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaHeight = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.HeightStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinHeight = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.HeightStat.Min?.Micrometers : double.MinValue);
            SortByMaxHeight = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.HeightStat.Max?.Micrometers : double.MinValue);

            SortByAvgWidth = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.WidthStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaWidth = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.WidthStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinWidth = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.WidthStat.Min?.Micrometers : double.MinValue);
            SortByMaxWidth = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.WidthStat.Max?.Micrometers : double.MinValue);

            SortByAvgPitch = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.PitchStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaPitch = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.PitchStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinPitch = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.PitchStat.Min?.Micrometers : double.MinValue);
            SortByMaxPitch = SortedPoints.Sort.AddSortDefinition(result => result is PeriodicStructPointResult PeriodicStructPointResult ? PeriodicStructPointResult.PitchStat.Max?.Micrometers : double.MinValue);

        }

        public void UpdateVisibility(bool hasHeight, bool hasWidth)
        {
            HideHeight = !hasHeight;
            HideWidth = !hasWidth;

            OnPropertyChanged(nameof(HideHeightRepeta));
            OnPropertyChanged(nameof(HideWidthRepeta));
            OnPropertyChanged(nameof(HidePitchRepeta));
        }

        #region Overrides of MetroPointsListVM

        public override void UpdatePointsSource(ICollection<MeasurePointResult> sourcePoint, bool showRepetaColumns, bool showDieIndex, bool showQualityScore, bool showSiteID)
        {
            base.UpdatePointsSource(sourcePoint, showRepetaColumns, showDieIndex, showQualityScore, showSiteID);

            GenerateColumnsFlag = !GenerateColumnsFlag;
        }

        #endregion
    }
}
