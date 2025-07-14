using System.Collections.Generic;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Step
{
    public class StepPointsListVM : MetroPointsListVM
    {
        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }

        private bool _hideStepHeight;

        public bool HideStepHeight
        {
            get => _hideStepHeight;
            private set => SetProperty(ref _hideStepHeight, value);
        }

        public bool HideStepHeightRepeta => HideRepetaColumns || HideStepHeight;

        #region Sort Definitions

        public SortDefinition SortByAvgStepHeight { get; }
        public SortDefinition SortBy3SigmaStepHeight { get; }
        public SortDefinition SortByMinStepHeight { get; }
        public SortDefinition SortByMaxStepHeight { get; }

        #endregion

        public StepPointsListVM(StepPointSelector pointSelector) : base(pointSelector)
        {
            SortByAvgStepHeight = SortedPoints.Sort.AddSortDefinition(result => result is StepPointResult stepPointResult ? stepPointResult.StepHeightStat.Mean?.Micrometers : double.MinValue);
            SortBy3SigmaStepHeight = SortedPoints.Sort.AddSortDefinition(result => result is StepPointResult stepPointResult ? stepPointResult.StepHeightStat.Sigma3?.Micrometers : double.MinValue);
            SortByMinStepHeight = SortedPoints.Sort.AddSortDefinition(result => result is StepPointResult stepPointResult ? stepPointResult.StepHeightStat.Min?.Micrometers : double.MinValue);
            SortByMaxStepHeight = SortedPoints.Sort.AddSortDefinition(result => result is StepPointResult stepPointResult ? stepPointResult.StepHeightStat.Max?.Micrometers : double.MinValue);
        }

        #region Overrides of MetroPointsListVM

        public void UpdateStepHeightVisibility(bool hasStepHeight)
        {
            HideStepHeight = !hasStepHeight;

            OnPropertyChanged(nameof(HideStepHeightRepeta));
        }

        public override void UpdatePointsSource(ICollection<MeasurePointResult> sourcePoint, bool showRepetaColumns, bool showDieIndex, bool showQualityScore, bool showSiteID)
        {
            base.UpdatePointsSource(sourcePoint, showRepetaColumns, showDieIndex, showQualityScore, showSiteID);

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;
        }

        #endregion
    }
}
