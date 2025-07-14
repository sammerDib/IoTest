using System.Collections.Generic;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar
{
    public class PillarPointsListVM : MetroPointsListVM
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



        #region Sort Definitions

        public SortDefinition SortByAvgHeight { get; }
        public SortDefinition SortBy3SigmaHeight { get; }
        public SortDefinition SortByMinHeight { get; }
        public SortDefinition SortByMaxHeight { get; }

        public SortDefinition SortByAvgWidth { get; }
        public SortDefinition SortBy3SigmaWidth { get; }
        public SortDefinition SortByMinWidth { get; }
        public SortDefinition SortByMaxWidth { get; }

        #endregion

        public PillarPointsListVM(PillarPointSelector pointSelector) : base(pointSelector)
        {
            SortByAvgHeight = SortedPoints.Sort.AddSortDefinition(result => result is PillarPointResult PillarPointResult ? PillarPointResult.HeightStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaHeight = SortedPoints.Sort.AddSortDefinition(result => result is PillarPointResult PillarPointResult ? PillarPointResult.HeightStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinHeight = SortedPoints.Sort.AddSortDefinition(result => result is PillarPointResult PillarPointResult ? PillarPointResult.HeightStat.Min?.Micrometers : double.MinValue);
            SortByMaxHeight = SortedPoints.Sort.AddSortDefinition(result => result is PillarPointResult PillarPointResult ? PillarPointResult.HeightStat.Max?.Micrometers : double.MinValue);

            SortByAvgWidth = SortedPoints.Sort.AddSortDefinition(result => result is PillarPointResult PillarPointResult ? PillarPointResult.WidthStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaWidth = SortedPoints.Sort.AddSortDefinition(result => result is PillarPointResult PillarPointResult ? PillarPointResult.WidthStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinWidth = SortedPoints.Sort.AddSortDefinition(result => result is PillarPointResult PillarPointResult ? PillarPointResult.WidthStat.Min?.Micrometers : double.MinValue);
            SortByMaxWidth = SortedPoints.Sort.AddSortDefinition(result => result is PillarPointResult PillarPointResult ? PillarPointResult.WidthStat.Max?.Micrometers : double.MinValue);

        }

        #region Overrides of MetroPointsListVM

        public void UpdateHeightAndWidthVisibility(bool hasHeight, bool hasWidth)
        {
            HideHeight = !hasHeight;
            HideWidth = !hasWidth;

            OnPropertyChanged(nameof(HideHeightRepeta));
            OnPropertyChanged(nameof(HideWidthRepeta));
        }

        #endregion

        #region Overrides of MetroPointsListVM

        public override void UpdatePointsSource(ICollection<MeasurePointResult> sourcePoint, bool showRepetaColumns, bool showDieIndex, bool showQualityScore, bool showSiteID)
        {
            base.UpdatePointsSource(sourcePoint, showRepetaColumns, showDieIndex, showQualityScore, showSiteID);

            GenerateColumnsFlag = !GenerateColumnsFlag;
        }

        #endregion
    }
}
