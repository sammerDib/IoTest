using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Step
{
    public class StepGlobalStatsVM : GlobalStatsVM
    {
        private readonly StepPointSelector _stepPointSelector;

        public StepGlobalStatsVM(StepPointSelector stepPointSelector) : base(stepPointSelector)
        {
            _stepPointSelector = stepPointSelector;
            _stepPointSelector.SelectedOutputChanged += PointSelectorOnCheckedPointsChanged;
        }
        
        #region Overrides of GlobalStatsVM

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            Stats = null;

            switch (_stepPointSelector.SelectedOutput)
            {
                case StepResultVM.StepHeightOutputName:
                    var points = _stepPointSelector.CheckedPoints.OfType<StepPointResult>().ToList();
                    if (points.Count == 0) return;
                    var stepHeightData = points.Select(point => point.StepHeightStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(stepHeightData);
                    QualityScore = points.Min(point => point.QualityScore);
                    break;
                default:
                    Stats = null;
                    break;
            }
        }

        #endregion

        #region Overrides of GlobalStatsVM

        public override void Dispose()
        {
            _stepPointSelector.SelectedOutputChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion
    }
}
