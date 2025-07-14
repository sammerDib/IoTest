using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness.DieDetails
{
    public class ThicknessDieStatsVM : MetroDieStatsVM<ThicknessPointResult>
    {
        #region Fields

        private readonly ThicknessPointSelector _pointSelector;

        #endregion

        public ThicknessDieStatsVM(ThicknessPointSelector pointSelector) : base(pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.SelectedLayerChanged += PointSelectorOnSelectedLayerChanged;
        }

        #region Handlers
        
        private void PointSelectorOnSelectedLayerChanged(object sender, EventArgs e)
        {
            GenerateDieStats();
        }

        #endregion

        #region Overrides of MetroDieStatsVM<TSVPointResult>

        protected override IStatsContainer GetStats(List<ThicknessPointResult> points)
        {
            switch (_pointSelector.SelectedLayer)
            {
                case ThicknessResultVM.TotalLayerName:
                case ThicknessResultVM.CrossSectionModeName:
                    {
                        var lengthData = points.Select(diePoint => diePoint.TotalThicknessStat).ToList();
                        return MetroStatsContainer.GenerateFromStats(lengthData);
                    }
                default:
                    {
                        // Insert Success State because there is not state assigned to coplanarity
                        var layerStats = points.
                            Select(diePoint => diePoint.ThicknessLayerStats.TryGetValue(_pointSelector.SelectedLayer, out var container) ? container : null).
                            Where(container => container != null).ToList();
                        return MetroStatsContainer.GenerateFromStats(layerStats);
                    }
            }
        }

        #endregion

        #region Overrides of MetroDieStatsVM<ThicknessPointResult>

        public override void Dispose()
        {
            _pointSelector.SelectedLayerChanged -= PointSelectorOnSelectedLayerChanged;
            base.Dispose();
        }

        #endregion
    }
}

