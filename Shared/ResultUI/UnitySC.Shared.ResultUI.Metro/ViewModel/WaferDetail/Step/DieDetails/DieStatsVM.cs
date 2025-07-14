using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Step.DieDetails
{
    public class DieStatsVM : MetroDieStatsVM<StepPointResult>
    {
        #region Fields

        private readonly StepPointSelector _pointSelector;

        #endregion

        public DieStatsVM(StepPointSelector pointSelector) : base(pointSelector)
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

        #region Overrides of MetroDieStatsVM<NanoTopoPointResult>

        protected override IStatsContainer GetStats(List<StepPointResult> points)
        {
            switch (_pointSelector.SelectedOutput)
            {
                case StepResultVM.StepHeightOutputName:
                    {
                        var statsData = points.Select(diePoint => diePoint.StepHeightStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(statsData);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        #endregion

        #region Overrides of MetroDieStatsVM<StepPointResult>

        public override void Dispose()
        {
            _pointSelector.SelectedOutputChanged -= PointSelectorOnViewerTypeChanged;
            base.Dispose();
        }

        #endregion
    }
}

