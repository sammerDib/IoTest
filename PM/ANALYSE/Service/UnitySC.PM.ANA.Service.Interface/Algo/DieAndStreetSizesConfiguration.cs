using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class DieAndStreetSizesConfiguration : AxesMovementConfiguration
    {
        public double OverlapForNextDieResearch { get; set; } = 0.4;

        public Length AdditionalEdgeExclusion { get; set; } = 7.5.Millimeters();
    }
}
