using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class AxisOrthogonalityConfiguration : DefaultConfiguration
    {
        public XYZPosition ReferenceCrossPosition { get; set; }

        public Length ShiftLength { get; set; } = 4.Millimeters();

        public Angle AngleThreshold { get; set; }
    }
}
