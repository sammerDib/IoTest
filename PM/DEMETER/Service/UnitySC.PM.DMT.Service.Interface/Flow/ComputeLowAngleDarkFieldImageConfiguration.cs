using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    public class ComputeLowAngleDarkFieldImageConfiguration : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; }
        
        public float PercentageOfLowSaturation { get; set; }
    }
}
