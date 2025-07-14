using System;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class SystemUniformityCalibrationConfiguration : FlowConfigurationBase
    {
        public double PolynomialFitPatternThreshold { get; set; } = 0.80;
        public override FlowReportConfiguration WriteReportMode { get; set; }
    }
}
