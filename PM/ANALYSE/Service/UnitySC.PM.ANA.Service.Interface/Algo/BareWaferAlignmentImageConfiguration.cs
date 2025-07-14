using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class BareWaferAlignmentImageConfiguration : DefaultConfiguration
    {
        public int EdgeDetectionVersion { get; set; } = 2;

        public int CannyThreshold { get; set; } = 200;
    }
}
