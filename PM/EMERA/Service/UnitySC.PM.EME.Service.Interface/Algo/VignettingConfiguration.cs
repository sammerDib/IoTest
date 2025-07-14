using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class VignettingConfiguration : AxesMovementConfiguration
    {
        public double SmallRangeCoeff { get; set; } = 0.2;

        public double MediumRangeCoeff { get; set; } = 0.5;

        public double LargeRangeCoeff { get; set; } = 1;

        public Length MinStepSize { get; set; } = 0.5.Millimeters();

        public Length MaxStepSize { get; set; } = 5.Millimeters();

        public Length ScanRange { get; set; } = 50.Millimeters();
    }
}
