using System;
using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo
{
    public class NanotopoGlobalStatsVM : GlobalStatsVM
    {
        private readonly NanotopoPointSelector _pointSelector;

        public NanotopoGlobalStatsVM(NanotopoPointSelector pointSelector) : base(pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.SelectedOutputChanged += PointSelectorOnCheckedPointsChanged;
        }

        #region Overrides of GlobalStatsVM

        protected override void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            var points = _pointSelector.CheckedPoints.OfType<NanoTopoPointResult>().ToList();

            if (points.Count == 0)
            {
                Stats = null;
                return;
            }

            if (!(_pointSelector.SelectedOutput is null))
            {
                switch (_pointSelector.SelectedOutput)
                {
                    case NanotopoResultVM.RoughnessOutputName:
                        var roughnessData = points.Select(point => point.RoughnessStat).ToList();
                        Stats = MetroStatsContainer.GenerateFromStats(roughnessData);
                        break;

                    case NanotopoResultVM.StepHeightOutputName:
                        var stepHeightData = points.Select(point => point.StepHeightStat).ToList();
                        Stats = MetroStatsContainer.GenerateFromStats(stepHeightData);
                        break;

                    default:
                        var dynamicOutputData = points.Select(point => point.ExternalProcessingStats.TryGetValue(_pointSelector.SelectedOutput, out var statsContainer) ? statsContainer : null).Where(container => container != null).ToList();
                        Stats = dynamicOutputData.Count > 0 ? MetroDoubleStatsContainer.GenerateFromStats(dynamicOutputData) : null;
                        break;
                }
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

        #endregion Overrides of GlobalStatsVM
    }
}
