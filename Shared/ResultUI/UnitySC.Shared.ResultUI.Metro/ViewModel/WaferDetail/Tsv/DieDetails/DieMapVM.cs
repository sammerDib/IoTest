using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv.DieDetails
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="DieMapView"/>
    /// </summary>
    public class DieMapVM : MetroDieMapVM<TSVPointResult>
    {
        #region Fields

        #endregion

        public DieMapVM(TsvPointSelector pointSelector, int heatmapside) : base(pointSelector, heatmapside)
        {
        }
        
        #region Properties

        public TSVResult TsvResult { get; set; }

        #endregion

        #region Private Methods

        protected override System.Windows.Size GetMillimeterDieSize()
        {
            if (TsvResult?.DiesMap == null) return System.Windows.Size.Empty;
            double dieHeight = TsvResult.DiesMap.DieSizeHeight.Millimeters;
            double dieWidth = TsvResult.DiesMap.DieSizeWidth.Millimeters;

            return new System.Windows.Size(dieWidth, dieHeight);
        }

        #endregion
    }
}

