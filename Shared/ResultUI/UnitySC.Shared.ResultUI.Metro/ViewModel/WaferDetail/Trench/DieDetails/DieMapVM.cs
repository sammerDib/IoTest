using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench.DieDetails
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="DieMapView"/>
    /// </summary>
    public class DieMapVM : MetroDieMapVM<TrenchPointResult>
    {
        public DieMapVM(TrenchPointSelector pointSelector, int heatmapside) : base(pointSelector, heatmapside)
        {
        }
        
        #region Properties

        public TrenchResult TrenchResult { get; set; }
        
        #endregion

        #region Override Methods

        protected override System.Windows.Size GetMillimeterDieSize()
        {
            if (TrenchResult?.DiesMap == null) return  System.Windows.Size.Empty;
            double dieHeight = TrenchResult.DiesMap.DieSizeHeight.Millimeters;
            double dieWidth = TrenchResult.DiesMap.DieSizeWidth.Millimeters;

            return new System.Windows.Size(dieWidth, dieHeight);
        }

        #endregion
    }
}

