using System;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    
    [Serializable]
    public class SaveImageConfiguration : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; }

        public bool UsePerspectiveCalibration { get; set; } = false;
    }
}
