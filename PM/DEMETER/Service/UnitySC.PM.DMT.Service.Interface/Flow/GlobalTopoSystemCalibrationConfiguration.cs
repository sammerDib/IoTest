using System;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class GlobalTopoSystemCalibrationConfiguration : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; }
        public bool UseAllCheckerBoards { get; set; }
        public float EdgeExclusionInMm { get; set; }
        public float NbPtsScreen { get; set; }
    }
}
