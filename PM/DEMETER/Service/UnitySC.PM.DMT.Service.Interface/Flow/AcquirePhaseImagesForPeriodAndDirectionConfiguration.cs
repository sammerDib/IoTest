using System;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    
    [Serializable]
    public class AcquirePhaseImagesForPeriodAndDirectionConfiguration : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; }
    }
}
