using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    public class CurvatureDynamicsCalibrationConfiguration : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; }

        public int NumberOfPhaseShifts { get; set; }
    }
}
