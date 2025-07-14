using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class EllipseCriticalDimensionConfiguration : DefaultConfiguration
    {
        public int CannyThreshold { get; set; } = 100; // Threeshold to configure edge detection into C++ algos library
        public ResultCorrectionSettings ResultCorrectionSettings { get; set; } = new ResultCorrectionSettings();
    }
}
