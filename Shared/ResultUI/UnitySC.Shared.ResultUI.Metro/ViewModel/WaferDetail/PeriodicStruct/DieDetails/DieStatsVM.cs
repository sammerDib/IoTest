using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.PeriodicStruct;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.PeriodicStruct.DieDetails
{
    public class DieStatsVM : MetroDieStatsVM<PeriodicStructPointResult>
    {
        #region Fields

        private readonly PeriodicStructPointSelector _pointSelector;

        #endregion

        public DieStatsVM(PeriodicStructPointSelector pointSelector) : base(pointSelector)
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

        protected override IStatsContainer GetStats(List<PeriodicStructPointResult> points)
        {
            switch (_pointSelector.SelectedOutput)
            {
                case PeriodicStructResultVM.WidthOutputName:
                    {
                        var statsData = points.Select(diePoint => diePoint.WidthStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(statsData);
                    }
                case PeriodicStructResultVM.HeightOutputName:
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

        #region Overrides of MetroDieStatsVM<PeriodicStructPointResult>

        public override void Dispose()
        {
            _pointSelector.SelectedOutputChanged -= PointSelectorOnViewerTypeChanged;
            base.Dispose();
        }

        #endregion
    }
}

