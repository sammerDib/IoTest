using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo.DieDetails
{
    public class DieStatsVM : MetroDieStatsVM<NanoTopoPointResult>
    {
        #region Fields

        private readonly NanotopoPointSelector _pointSelector;

        #endregion

        public DieStatsVM(NanotopoPointSelector pointSelector) : base(pointSelector)
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

        protected override IStatsContainer GetStats(List<NanoTopoPointResult> points)
        {
            switch (_pointSelector.SelectedOutput)
            {
                case NanotopoResultVM.RoughnessOutputName:
                    {
                        var statsData = points.Select(diePoint => diePoint.RoughnessStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(statsData);
                    }
                case NanotopoResultVM.StepHeightOutputName:
                    {
                        var statsData = points.Select(diePoint => diePoint.StepHeightStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(statsData);
                    }
                default:
                    {
                        var statsData = points.Select(diePoint => diePoint.ExternalProcessingStats.TryGetValue(_pointSelector.SelectedOutput, out var doubleStatsContainer) ? doubleStatsContainer : null).Where(container => container != null).ToList();
                        return MetroDoubleStatsContainer.GenerateFromStats(statsData);
                    }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _pointSelector.SelectedOutputChanged -= PointSelectorOnViewerTypeChanged;
        }

        #endregion
    }
}

