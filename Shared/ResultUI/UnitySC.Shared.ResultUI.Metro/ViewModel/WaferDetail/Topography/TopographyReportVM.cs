using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography
{
    public class TopographyReportVM : ReportVM
    {
        public TopographyReportVM(PointSelectorBase pointSelector) : base(pointSelector)
        {

        }

        #region Overrides of ReportVM

        protected override string GetResultReport(MeasurePointDataResultBase pointData)
        {
            if (pointData is TopographyPointData repeta)
            {
                return repeta.ReportFileName;
            }

            return null;
        }

        #endregion
    }
}
