using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench.DieDetails
{
    public class DieStatsVM : MetroDieStatsVM<TrenchPointResult>
    {
        #region Fields

        private readonly TrenchPointSelector _pointSelector;

        #endregion

        public DieStatsVM(TrenchPointSelector pointSelector) : base(pointSelector)
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

        protected override IStatsContainer GetStats(List<TrenchPointResult> points)
        {
            switch (_pointSelector.SelectedOutput)
            {
                case TrenchResultVM.WidthOutputName:
                    {
                        var statsData = points.Select(diePoint => diePoint.WidthStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(statsData);
                    }
                case TrenchResultVM.DepthOutputName:
                    {
                        var statsData = points.Select(diePoint => diePoint.DepthStat).ToList();
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

