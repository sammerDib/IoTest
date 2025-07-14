using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography
{
    public class TopographyGlobalStatsVM : GlobalStatsVM
    {
        private readonly TopographyPointSelector _pointSelector;

        public TopographyGlobalStatsVM(TopographyPointSelector pointSelector) : base(pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.SelectedOutputChanged += PointSelectorOnCheckedPointsChanged;
        }
        
        #region Overrides of GlobalStatsVM

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            var points = _pointSelector.CheckedPoints.OfType<TopographyPointResult>().ToList();

            if (points.Count == 0)
            {
                Stats = null;
                return;
            }

            if (_pointSelector.SelectedOutput != null)
            {
                var dynamicOutputData = points.Select(point => point.ExternalProcessingStats.TryGetValue(_pointSelector.SelectedOutput, out var statsContainer) ? statsContainer : null).Where(container => container != null).ToList();
                Stats = dynamicOutputData.Count > 0 ? MetroDoubleStatsContainer.GenerateFromStats(dynamicOutputData) : null;
            }

            QualityScore = points.Min(point => point.QualityScore);
        }

        #endregion

        #region Overrides of GlobalStatsVM

        public override void Dispose()
        {
            _pointSelector.SelectedOutputChanged -= PointSelectorOnCheckedPointsChanged;
            base.Dispose();
        }

        #endregion
    }
}
