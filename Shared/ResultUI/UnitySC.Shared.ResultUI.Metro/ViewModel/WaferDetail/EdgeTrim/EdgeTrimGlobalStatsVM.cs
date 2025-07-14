using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.EdgeTrim
{
    public class EdgeTrimGlobalStatsVM : GlobalStatsVM
    {
        private readonly EdgeTrimPointSelector _edgeTrimPointSelector;

        public EdgeTrimGlobalStatsVM(EdgeTrimPointSelector edgeTrimPointSelector) : base(edgeTrimPointSelector)
        {
            _edgeTrimPointSelector = edgeTrimPointSelector;
            _edgeTrimPointSelector.SelectedOutputChanged += PointSelectorOnCheckedPointsChanged;
        }
        
        #region Overrides of GlobalStatsVM

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            Stats = null;

            switch (_edgeTrimPointSelector.SelectedOutput)
            {
                case EdgeTrimResultVM.HeightOutputName:
                    var points = _edgeTrimPointSelector.CheckedPoints.OfType<EdgeTrimPointResult>().ToList();
                    if (points.Count == 0) return;
                    var heightData = points.Select(point => point.HeightStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(heightData);
                    QualityScore = points.Min(point => point.QualityScore);
                    break;
                case EdgeTrimResultVM.WidthOutputName:
                    var wpoints = _edgeTrimPointSelector.CheckedPoints.OfType<EdgeTrimPointResult>().ToList();
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
            _edgeTrimPointSelector.SelectedOutputChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion
    }
}
