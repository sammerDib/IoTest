using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Modules.Calibration
{
    public class CalibrationConfiguration
    {
        public Length TargetPixelSize { get; set; }
        public double TargetBrightness { get; set; }
        public Tolerance ToleranceBrightness { get; set; }
    }
}
