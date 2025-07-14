using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo.DieDetails
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="DieMapView"/>
    /// </summary>
    public class DieMapVM : MetroDieMapVM<NanoTopoPointResult>
    {
        public DieMapVM(NanotopoPointSelector pointSelector, int heatmapside) : base(pointSelector, heatmapside)
        {
        }
        
        #region Properties

        public NanoTopoResult NanotopoResult { get; set; }
        
        #endregion

        #region Override Methods

        protected override System.Windows.Size GetMillimeterDieSize()
        {
            if (NanotopoResult?.DiesMap == null) return  System.Windows.Size.Empty;
            double dieHeight = NanotopoResult.DiesMap.DieSizeHeight.Millimeters;
            double dieWidth = NanotopoResult.DiesMap.DieSizeWidth.Millimeters;

            return new System.Windows.Size(dieWidth, dieHeight);
        }

        #endregion
    }
}

