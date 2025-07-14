using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Tolerances;

namespace UnitySC.PM.EME.Service.Interface.Algo.GetZFocus
{
    public class GetZFocusConfiguration : DefaultConfiguration
    {
        public double MaximumDistance { get; set; } = 10000;
        public int MaximumIterations { get; set; } = 10;
        public double StartZScan { get; set; }
        public double MinZScan { get; set; }
        public double MaxZScan { get; set; }
        public Tolerance Tolerance { get; set; } = new Tolerance(200, ToleranceUnit.AbsoluteValue);
    }
}
