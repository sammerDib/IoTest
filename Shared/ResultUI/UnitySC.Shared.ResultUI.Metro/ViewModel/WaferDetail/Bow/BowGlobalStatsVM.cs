using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow
{
    public class BowGlobalStatsVM : GlobalStatsVM
    {
        private readonly BowPointSelector _bowPointSelector;

        public BowGlobalStatsVM(BowPointSelector BowPointSelector) : base(BowPointSelector)
        {
            _bowPointSelector = BowPointSelector;
            _bowPointSelector.SelectedOutputChanged += PointSelectorOnCheckedPointsChanged;
        }

        #region Overrides of GlobalStatsVM

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            Stats = null;

            switch (_bowPointSelector.SelectedOutput)
            {

                case BowResultVM.BowOutputName:
                    var points = _bowPointSelector.CheckedPoints.OfType<BowPointResult>().ToList();
                    if (points.Count == 0) return;
                    var bowData = points.Select(point => point.BowStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(bowData);
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
            _bowPointSelector.SelectedOutputChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion
    }
}
