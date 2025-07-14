using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness
{
    public class ThicknessGlobalStatsVM : GlobalStatsVM
    {
        private readonly ThicknessPointSelector _pointSelector;

        public ThicknessGlobalStatsVM(ThicknessPointSelector pointSelector) : base(pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.SelectedLayerChanged += PointSelectorOnCheckedPointsChanged;
        }
        
        #region Overrides of GlobalStatsVM

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            var points = _pointSelector.CheckedPoints.OfType<ThicknessPointResult>().ToList();

            if (points.Count == 0)
            {
                Stats = null;
                return;
            }

            switch (_pointSelector.SelectedLayer)
            {
                case ThicknessResultVM.TotalLayerName:
                case ThicknessResultVM.CrossSectionModeName:
                    var totalThicknessData = points.Select(point => point.TotalThicknessStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(totalThicknessData);
                    break;
                case ThicknessResultVM.WaferThickness:
                    var waferThicknessData = points.Select(point => point.WaferThicknessStat).ToList();
                    Stats = MetroStatsContainer.GenerateFromStats(waferThicknessData);
                    break;
                default:
                    var layoutStats = points.Select(point => point.ThicknessLayerStats.TryGetValue(_pointSelector.SelectedLayer, out var statsContainer) ? statsContainer : null).Where(container => container != null).ToList();
                    Stats = layoutStats.Count > 0 ? MetroStatsContainer.GenerateFromStats(layoutStats) : null;
                    break;
            }

            QualityScore = points.Min(point => point.QualityScore);
        }

        #endregion

        #region Overrides of GlobalStatsVM

        public override void Dispose()
        {
            _pointSelector.SelectedLayerChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion
    }
}
