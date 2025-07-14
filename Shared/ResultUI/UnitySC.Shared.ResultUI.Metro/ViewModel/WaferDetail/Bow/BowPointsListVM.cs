using System.Collections.Generic;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow
{
    public class BowPointsListVM : MetroPointsListVM
    {
        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }

        private bool _hideBow;

        public bool HideBow
        {
            get => _hideBow;
            private set => SetProperty(ref _hideBow, value);
        }

        public bool HideBowRepeta => HideRepetaColumns || HideBow;

        #region Sort Definitions

        public SortDefinition SortByAvgBow { get; }
        public SortDefinition SortBy3SigmaBow { get; }
        public SortDefinition SortByMinBow { get; }
        public SortDefinition SortByMaxBow { get; }


        #endregion

        public BowPointsListVM(BowPointSelector pointSelector) : base(pointSelector)
        {
            SortByAvgBow = SortedPoints.Sort.AddSortDefinition(result => result is BowPointResult BowPointResult ? BowPointResult.BowStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaBow = SortedPoints.Sort.AddSortDefinition(result => result is BowPointResult BowPointResult ? BowPointResult.BowStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinBow = SortedPoints.Sort.AddSortDefinition(result => result is BowPointResult BowPointResult ? BowPointResult.BowStat.Min?.Micrometers : double.MinValue);
            SortByMaxBow = SortedPoints.Sort.AddSortDefinition(result => result is BowPointResult BowPointResult ? BowPointResult.BowStat.Max?.Micrometers : double.MinValue);
        }

        #region Overrides of MetroPointsListVM

        public void UpdateBowVisibility(bool hasBow)
        {
            HideBow = !hasBow;

            OnPropertyChanged(nameof(HideBowRepeta));
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
