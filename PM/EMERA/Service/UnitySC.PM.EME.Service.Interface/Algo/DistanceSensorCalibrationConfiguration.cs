using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class DistanceSensorCalibrationConfiguration : DefaultConfiguration
    {
        public XYPosition ReferencePosition { get; set; } = new XYPosition(new WaferReferential(), -194, 112);

        public Length ReferenceSmallestSideSize = 14.Millimeters();

        public double ApproximateReferenceDistance = 7500;

        public string ReferenceImageName = "DistanceRef.png";

        public Tolerance DistanceTolerance = new Tolerance(300, ToleranceUnit.AbsoluteValue);
        public Length MaximumSensorOffsetX { get; set; } = 20.Millimeters();
        public Length MaximumSensorOffsetY { get; set; } = 80.Millimeters();
        public int TrackingPeriodInMilliseconds { get; set; } = 20;
    }
}
