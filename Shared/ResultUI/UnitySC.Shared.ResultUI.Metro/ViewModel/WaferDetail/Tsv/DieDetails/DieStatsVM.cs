using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv.DieDetails
{
    public class DieStatsVM : MetroDieStatsVM<TSVPointResult>
    {
        #region Fields

        private readonly TsvPointSelector _pointSelector;

        #endregion

        public DieStatsVM(TsvPointSelector pointSelector) : base(pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.ViewerTypeChanged += PointSelectorOnViewerTypeChanged;
        }

        #region Handlers
        
        private void PointSelectorOnViewerTypeChanged(object sender, EventArgs e)
        {
            GenerateDieStats();
        }

        #endregion

        #region Overrides of MetroDieStatsVM<TSVPointResult>

        protected override IStatsContainer GetStats(List<TSVPointResult> points)
        {
            switch (_pointSelector.ViewerType)
            {
                case TsvResultViewerType.Depth:
                    {
                        var lengthData = points.Select(diePoint => diePoint.DepthTsvStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(lengthData);
                    }
                case TsvResultViewerType.Coplanarity:
                    {
                        // Insert Success State because there is not state assigned to coplanarity
                        var coplaData = points.Select(diePoint => new Tuple<Length, MeasureState>(diePoint.CoplaInDieValue, MeasureState.Success)).ToList();
                        return MetroStatsContainer.GenerateFromLength(coplaData);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Overrides of MetroDieStatsVM<TSVPointResult>

        public override void Dispose()
        {
            _pointSelector.ViewerTypeChanged -= PointSelectorOnViewerTypeChanged;
            base.Dispose();
        }

        #endregion
    }
}

