using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    public class AdjustCurvatureDynamicsForRawCurvatureMapConfiguration : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; }
        
        public float DefaultCurvatureDynamicsCoefficient { get; set; }
        
        public int TargetBackgroundLevel { get; set; }
    }
}
