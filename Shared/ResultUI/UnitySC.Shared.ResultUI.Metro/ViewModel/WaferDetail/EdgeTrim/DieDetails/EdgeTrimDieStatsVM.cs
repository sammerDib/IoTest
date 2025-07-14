using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Format.Metro.EdgeTrim;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.EdgeTrim.DieDetails
{
    public class EdgeTrimDieStatsVM : MetroDieStatsVM<EdgeTrimPointResult>
    {
        #region Fields

        private readonly EdgeTrimPointSelector _pointSelector;

        #endregion

        public EdgeTrimDieStatsVM(EdgeTrimPointSelector pointSelector) : base(pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.SelectedOutputChanged += PointSelectorOnViewerTypeChanged;
        }

        #region Handlers

        private void PointSelectorOnViewerTypeChanged(object sender, EventArgs e)
        {
            GenerateDieStats();
        }

        #endregion

        #region Overrides of MetroDieStatsVM<xxxPointResult>

        protected override IStatsContainer GetStats(List<EdgeTrimPointResult> points)
        {
            switch (_pointSelector.SelectedOutput)
            {
                case EdgeTrimResultVM.WidthOutputName:
                    {
                        var statsData = points.Select(diePoint => diePoint.WidthStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(statsData);
                    }
                case EdgeTrimResultVM.HeightOutputName:
                    {
                        var statsData = points.Select(diePoint => diePoint.HeightStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(statsData);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        #endregion

        #region Overrides of MetroDieStatsVM<TrenchPointResult>

        public override void Dispose()
        {
            _pointSelector.SelectedOutputChanged -= PointSelectorOnViewerTypeChanged;
            base.Dispose();
        }

        #endregion
    }
}

