using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Common.HeatMap;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness.DieDetails
{
    public class ThicknessDieMapVM : MetroDieMapVM<ThicknessPointResult>
    {
        public ThicknessDieMapVM(ThicknessPointSelector pointSelector, int heatmapside) : base(pointSelector, heatmapside)
        {
        }
        
        #region Properties

        public ThicknessResult ThicknessResult { get; set; }
        
        #endregion
        
        #region Private Methods

        protected override System.Windows.Size GetMillimeterDieSize()
        {
            if (ThicknessResult?.DiesMap == null) return System.Windows.Size.Empty;
            double dieHeight = ThicknessResult.DiesMap.DieSizeHeight.Millimeters;
            double dieWidth = ThicknessResult.DiesMap.DieSizeWidth.Millimeters;

            return new System.Windows.Size(dieWidth, dieHeight);
        }

        #endregion
    }
}

