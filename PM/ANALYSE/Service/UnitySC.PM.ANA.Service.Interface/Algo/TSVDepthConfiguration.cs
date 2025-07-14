using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class TSVDepthConfiguration : AxesMovementConfiguration
    {
        public Length DetectionTolerance { get; set; } = 10.Millimeters();

        public ResultCorrectionSettings ResultCorrectionSettings { get; set; } = new ResultCorrectionSettings();
    }
}
