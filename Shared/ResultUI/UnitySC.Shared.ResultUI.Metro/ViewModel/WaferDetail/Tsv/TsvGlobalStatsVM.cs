using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv
{
    public class TsvGlobalStatsVM : GlobalStatsVM
    {
        private readonly TsvPointSelector _pointSelector;

        public TsvGlobalStatsVM(TsvPointSelector pointSelector) : base(pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.ViewerTypeChanged += PointSelectorOnCheckedPointsChanged;
        }

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            var points = _pointSelector.CheckedPoints.OfType<TSVPointResult>().ToList();

            if (points.Count == 0)
            {
                Stats = null;
                return;
            }

            switch (_pointSelector.ViewerType)
            {
                case TsvResultViewerType.Depth:
                    {
                        if (!points.Any(x => x.DepthTsvStat != null))
                        {
                            Stats = null;
                            return;
                        }

                        var lengthData = points.Select(diePoint => diePoint.DepthTsvStat).ToList();
                        Stats = MetroStatsContainer.GenerateFromStats(lengthData);
                        break;
                    }
                case TsvResultViewerType.Coplanarity:
                    {
                        // Insert Success State because there is not state assigned to coplanarity
                        var coplaData = points.Select(diePoint =>
                        {
                            var copla = diePoint.CoplaInWaferValue == null ? diePoint.CoplaInDieValue : diePoint.CoplaInWaferValue;
                            return new Tuple<Length, MeasureState>(copla, MeasureState.Success);
                        }).ToList();
                        Stats = MetroStatsContainer.GenerateFromLength(coplaData);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            QualityScore = points.Min(point => point.QualityScore);
        }

        #region Overrides of GlobalStatsVM

        public override void Dispose()
        {
            _pointSelector.ViewerTypeChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion
    }
}
