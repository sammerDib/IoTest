using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class DistortionConfiguration : DefaultConfiguration
    {
        public int CalibrationPatternCircleNumber { get; set; } = 100;
        public XYPosition PatternPosition { get; set; } = new XYPosition(new WaferReferential(), 0.0, 0.0);
    }
}
