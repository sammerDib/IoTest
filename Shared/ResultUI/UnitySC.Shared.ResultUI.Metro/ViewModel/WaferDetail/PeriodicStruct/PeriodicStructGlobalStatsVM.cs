using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.PeriodicStruct;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.PeriodicStruct
{
    public class PeriodicStructGlobalStatsVM : GlobalStatsVM
    {
        private readonly PeriodicStructPointSelector _periodicStructPointSelector;

        public PeriodicStructGlobalStatsVM(PeriodicStructPointSelector periodicStructPointSelector) : base(periodicStructPointSelector)
        {
            _periodicStructPointSelector = periodicStructPointSelector;
            _periodicStructPointSelector.SelectedOutputChanged += PointSelectorOnCheckedPointsChanged;
        }
        
        #region Overrides of GlobalStatsVM

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            Stats = null;

            switch (_periodicStructPointSelector.SelectedOutput)
            {
                case PeriodicStructResultVM.HeightOutputName:
                    var points = _periodicStructPointSelector.CheckedPoints.OfType<PeriodicStructPointResult>().ToList();
                    if (points.Count == 0) return;
                    var HeightData = points.Select(point => point.HeightStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(HeightData);
                    QualityScore = points.Min(point => point.QualityScore);
                    break;
                case PeriodicStructResultVM.WidthOutputName:
                    var wpoints = _periodicStructPointSelector.CheckedPoints.OfType<PeriodicStructPointResult>().ToList();
                    if (wpoints.Count == 0) return;
                    var WidthData = wpoints.Select(wpoint => wpoint.WidthStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(WidthData);
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
            _periodicStructPointSelector.SelectedOutputChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion
    }
}
