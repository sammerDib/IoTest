using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar.DieDetails
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="DieMapView"/>
    /// </summary>
    public class DieMapVM : MetroDieMapVM<PillarPointResult>
    {
        public DieMapVM(PillarPointSelector pointSelector, int heatmapside) : base(pointSelector, heatmapside)
        {
        }
        
        #region Properties

        public PillarResult PillarResult { get; set; }
        
        #endregion

        #region Override Methods

        protected override System.Windows.Size GetMillimeterDieSize()
        {
            if (PillarResult?.DiesMap == null) return  System.Windows.Size.Empty;
            double dieHeight = PillarResult.DiesMap.DieSizeHeight.Millimeters;
            double dieWidth = PillarResult.DiesMap.DieSizeWidth.Millimeters;

            return new System.Windows.Size(dieWidth, dieHeight);
        }

        #endregion
    }
}

