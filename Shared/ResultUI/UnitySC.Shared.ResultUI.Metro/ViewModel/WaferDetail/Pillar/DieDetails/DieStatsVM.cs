using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar.DieDetails
{
    public class DieStatsVM : MetroDieStatsVM<PillarPointResult>
    {
        #region Fields

        private readonly PillarPointSelector _pointSelector;

        #endregion

        public DieStatsVM(PillarPointSelector pointSelector) : base(pointSelector)
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

        protected override IStatsContainer GetStats(List<PillarPointResult> points)
        {
            switch (_pointSelector.SelectedOutput)
            {
                case PillarResultVM.WidthOutputName:
                    {
                        var statsData = points.Select(diePoint => diePoint.WidthStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(statsData);
                    }
                case PillarResultVM.HeightOutputName:
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

        #region Overrides of MetroDieStatsVM<PillarPointResult>

        public override void Dispose()
        {
            _pointSelector.SelectedOutputChanged -= PointSelectorOnViewerTypeChanged;
            base.Dispose();
        }

        #endregion
    }
}

