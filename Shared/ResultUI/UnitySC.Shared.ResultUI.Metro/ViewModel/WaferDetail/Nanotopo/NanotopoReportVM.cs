using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo
{
    public class NanotopoReportVM : ReportVM
    {
        public NanotopoReportVM(PointSelectorBase pointSelector) : base(pointSelector)
        {

        }

        #region Overrides of ReportVM

        protected override string GetResultReport(MeasurePointDataResultBase pointData)
        {
            if (pointData is NanoTopoPointData repeta)
            {
                return repeta.ReportFileName;
            }

            return null;
        }

        #endregion
    }
}
