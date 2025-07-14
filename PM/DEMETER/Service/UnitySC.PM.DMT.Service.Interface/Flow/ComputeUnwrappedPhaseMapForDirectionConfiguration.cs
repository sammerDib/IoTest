using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    public class ComputeUnwrappedPhaseMapForDirectionConfiguration : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; }
        
        public bool ProduceUntiltedSlopeMaps { get; set; }
    }
}
