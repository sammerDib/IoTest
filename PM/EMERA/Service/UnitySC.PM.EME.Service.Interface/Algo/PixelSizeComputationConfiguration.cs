using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class PixelSizeComputationConfiguration : DefaultConfiguration
    {
        public Length ShiftLength { get; set; } = 4.Millimeters();

        public double PatternRecGamma { get; set; } = 0.3;
    }
}
