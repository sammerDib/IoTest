using UnitySC.Shared.Format.Metro.PeriodicStruct;
using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.PeriodicStruct.DieDetails
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="DieMapView"/>
    /// </summary>
    public class DieMapVM : MetroDieMapVM<PeriodicStructPointResult>
    {
        public DieMapVM(PeriodicStructPointSelector pointSelector, int heatmapside) : base(pointSelector, heatmapside)
        {
        }
        
        #region Properties

        public PeriodicStructResult PeriodicStructPointResultResult { get; set; }
        
        #endregion

        #region Override Methods

        protected override System.Windows.Size GetMillimeterDieSize()
        {
            if (PeriodicStructPointResultResult?.DiesMap == null) return  System.Windows.Size.Empty;
            double dieHeight = PeriodicStructPointResultResult.DiesMap.DieSizeHeight.Millimeters;
            double dieWidth = PeriodicStructPointResultResult.DiesMap.DieSizeWidth.Millimeters;

            return new System.Windows.Size(dieWidth, dieHeight);
        }

        #endregion
    }
}

