using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp
{
    public class WarpGlobalStatsVM : GlobalStatsVM
    {
        private readonly WarpPointSelector _pointSelector;

        public WarpGlobalStatsVM(WarpPointSelector pointSelector) : base(pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.SelectedOutputChanged += PointSelectorOnCheckedPointsChanged;
            var points = _pointSelector.CheckedPoints.OfType<WarpPointResult>().ToList();

        }

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            var points = _pointSelector.CheckedPoints.OfType<WarpPointResult>().ToList();

            if (points.Count == 0)
            {
                Stats = null;
                return;
            }

            WarpViewerType value = EnumUtils.GetEnumFromDescription<WarpViewerType>(_pointSelector.SelectedOutput);
            switch (value)
            {
                case WarpViewerType.WARP:
                default:
                    var rpdData = points.Select(_ => _.RPDStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(rpdData);
                    break;

                case WarpViewerType.TTV:
                    var ttvData = points.Select(_ => _.TotalThicknessStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(ttvData);
                    break;
            }           

            QualityScore = points.Min(point => point.QualityScore);
        }

        #region Overrides of GlobalStatsVM

        public override void Dispose()
        {
            _pointSelector.SelectedOutputChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion

    }
}
