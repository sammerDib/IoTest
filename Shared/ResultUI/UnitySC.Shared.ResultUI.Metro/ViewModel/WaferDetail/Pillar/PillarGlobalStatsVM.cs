using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar
{
    public class PillarGlobalStatsVM : GlobalStatsVM
    {
        private readonly PillarPointSelector _pillarPointSelector;

        public PillarGlobalStatsVM(PillarPointSelector pillarPointSelector) : base(pillarPointSelector)
        {
            _pillarPointSelector = pillarPointSelector;
            _pillarPointSelector.SelectedOutputChanged += PointSelectorOnCheckedPointsChanged;
        }
        
        #region Overrides of GlobalStatsVM

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            Stats = null;

            switch (_pillarPointSelector.SelectedOutput)
            {
                case PillarResultVM.HeightOutputName:
                    var points = _pillarPointSelector.CheckedPoints.OfType<PillarPointResult>().ToList();
                    if (points.Count == 0) return;
                    var heightData = points.Select(point => point.HeightStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(heightData);
                    QualityScore = points.Min(point => point.QualityScore);
                    break;
                case PillarResultVM.WidthOutputName:
                    var wpoints = _pillarPointSelector.CheckedPoints.OfType<PillarPointResult>().ToList();
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
            _pillarPointSelector.SelectedOutputChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion
    }
}
