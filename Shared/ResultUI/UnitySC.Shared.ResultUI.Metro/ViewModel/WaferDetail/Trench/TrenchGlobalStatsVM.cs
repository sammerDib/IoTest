using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench
{
    public class TrenchGlobalStatsVM : GlobalStatsVM
    {
        private readonly TrenchPointSelector _trenchPointSelector;

        public TrenchGlobalStatsVM(TrenchPointSelector trenchPointSelector) : base(trenchPointSelector)
        {
            _trenchPointSelector = trenchPointSelector;
            _trenchPointSelector.SelectedOutputChanged += PointSelectorOnCheckedPointsChanged;
        }
        
        #region Overrides of GlobalStatsVM

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            Stats = null;

            switch (_trenchPointSelector.SelectedOutput)
            {
                case TrenchResultVM.DepthOutputName:
                    var points = _trenchPointSelector.CheckedPoints.OfType<TrenchPointResult>().ToList();
                    if (points.Count == 0) return;
                    var depthData = points.Select(point => point.DepthStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(depthData);
                    QualityScore = points.Min(point => point.QualityScore);
                    break;
                case TrenchResultVM.WidthOutputName:
                    var wpoints = _trenchPointSelector.CheckedPoints.OfType<TrenchPointResult>().ToList();
                    if (wpoints.Count == 0) return;
                    var widthData = wpoints.Select(wpoint => wpoint.WidthStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(widthData);
                    QualityScore = wpoints.Min(wpoint => wpoint.QualityScore);
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
            _trenchPointSelector.SelectedOutputChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion
    }
}
