using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography.DieDetails
{
    public class DieStatsVM : MetroDieStatsVM<TopographyPointResult>
    {
        #region Fields

        private readonly TopographyPointSelector _pointSelector;

        #endregion

        public DieStatsVM(TopographyPointSelector pointSelector) : base(pointSelector)
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

        #region Overrides of MetroDieStatsVM<TopographyPointResult>

        protected override IStatsContainer GetStats(List<TopographyPointResult> points)
        {
            switch (_pointSelector.SelectedOutput)
            {
                default:
                    {
                        var statsData = points.Select(diePoint => diePoint.ExternalProcessingStats.TryGetValue(_pointSelector.SelectedOutput, out var doubleStatsContainer) ? doubleStatsContainer : null).Where(container => container != null).ToList();
                        if(statsData.Count == 0)
                            return null;
                        return MetroDoubleStatsContainer.GenerateFromStats(statsData);
                    }
            }
        }

        #endregion

        #region Overrides of MetroDieStatsVM<TopographyPointResult>

        public override void Dispose()
        {
            _pointSelector.SelectedOutputChanged -= PointSelectorOnViewerTypeChanged;
            base.Dispose();
        }

        #endregion
    }
}

