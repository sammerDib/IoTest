using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Tolerances;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class AutoExposureConfiguration : DefaultConfiguration
    {
        public int MaxIteration { get; set; } = 10;
        public double TargetBrightness { get; set; } = 0.75;
        public Tolerance ToleranceBrightness { get; set; } = new Tolerance(5, ToleranceUnit.Percentage);
    }
}
