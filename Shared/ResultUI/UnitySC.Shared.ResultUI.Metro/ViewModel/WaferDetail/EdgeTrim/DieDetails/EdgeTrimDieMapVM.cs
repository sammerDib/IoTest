using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Format.Metro.EdgeTrim;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.EdgeTrim.DieDetails
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="DieMapView"/>
    /// </summary>
    public class EdgeTrimDieMapVM : MetroDieMapVM<EdgeTrimPointResult>
    {
        public EdgeTrimDieMapVM(EdgeTrimPointSelector pointSelector, int heatmapside) : base(pointSelector, heatmapside)
        {
        }
        
        #region Properties

        public EdgeTrimResult EdgeTrimResult { get; set; }
        
        #endregion

        #region Override Methods

        protected override System.Windows.Size GetMillimeterDieSize()
        {
            if (EdgeTrimResult?.DiesMap == null) return  System.Windows.Size.Empty;
            double dieHeight = EdgeTrimResult.DiesMap.DieSizeHeight.Millimeters;
            double dieWidth = EdgeTrimResult.DiesMap.DieSizeWidth.Millimeters;

            return new System.Windows.Size(dieWidth, dieHeight);
        }

        #endregion
    }
}

