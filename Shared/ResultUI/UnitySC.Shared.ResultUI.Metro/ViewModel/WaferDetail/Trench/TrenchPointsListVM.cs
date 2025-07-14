using System.Collections.Generic;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench
{
    public class TrenchPointsListVM : MetroPointsListVM
    {
        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }

        private bool _hideDepth;

        public bool HideDepth
        {
            get => _hideDepth;
            private set => SetProperty(ref _hideDepth, value);
        }

        private bool _hideWidth;

        public bool HideWidth
        {
            get => _hideWidth;
            private set => SetProperty(ref _hideWidth, value);
        }


        public bool HideDepthRepeta => HideRepetaColumns || HideDepth;

        public bool HideWidthRepeta => HideRepetaColumns || HideWidth;



        #region Sort Definitions

        public SortDefinition SortByAvgDepth { get; }
        public SortDefinition SortBy3SigmaDepth { get; }
        public SortDefinition SortByMinDepth { get; }
        public SortDefinition SortByMaxDepth { get; }

        public SortDefinition SortByAvgWidth { get; }
        public SortDefinition SortBy3SigmaWidth { get; }
        public SortDefinition SortByMinWidth { get; }
        public SortDefinition SortByMaxWidth { get; }

        #endregion

        public TrenchPointsListVM(TrenchPointSelector pointSelector) : base(pointSelector)
        {
            SortByAvgDepth = SortedPoints.Sort.AddSortDefinition(result => result is TrenchPointResult trenchPointResult ? trenchPointResult.DepthStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaDepth = SortedPoints.Sort.AddSortDefinition(result => result is TrenchPointResult trenchPointResult ? trenchPointResult.DepthStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinDepth = SortedPoints.Sort.AddSortDefinition(result => result is TrenchPointResult trenchPointResult ? trenchPointResult.DepthStat.Min?.Micrometers : double.MinValue);
            SortByMaxDepth = SortedPoints.Sort.AddSortDefinition(result => result is TrenchPointResult trenchPointResult ? trenchPointResult.DepthStat.Max?.Micrometers : double.MinValue);

            SortByAvgWidth = SortedPoints.Sort.AddSortDefinition(result => result is TrenchPointResult trenchPointResult ? trenchPointResult.WidthStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaWidth = SortedPoints.Sort.AddSortDefinition(result => result is TrenchPointResult trenchPointResult ? trenchPointResult.WidthStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinWidth = SortedPoints.Sort.AddSortDefinition(result => result is TrenchPointResult trenchPointResult ? trenchPointResult.WidthStat.Min?.Micrometers : double.MinValue);
            SortByMaxWidth = SortedPoints.Sort.AddSortDefinition(result => result is TrenchPointResult trenchPointResult ? trenchPointResult.WidthStat.Max?.Micrometers : double.MinValue);
        }

        #region Overrides of MetroPointsListVM

        public void UpdateDepthAndWidthVisibility(bool hasDepth, bool hasWidth)
        {
            HideDepth = !hasDepth;
            HideWidth = !hasWidth;

            OnPropertyChanged(nameof(HideDepthRepeta));
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
