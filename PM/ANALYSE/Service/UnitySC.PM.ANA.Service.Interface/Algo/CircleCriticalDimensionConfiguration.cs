using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class CircleCriticalDimensionConfiguration : DefaultConfiguration
    {
        public int CannyThreshold { get; set; } = 300; // Threeshold to configure edge detection for Hough Transform into C++ algos library

        public bool UseScharrAlgorithm { get; set; } = true; // Scharr algorithm should improve Hough Transform into C++ algos library

        public bool UseMorphologicalOperations { get; set; } = true; // Morpholocial operations should improve preprocessing before Hough Transform into C++ algos library
        public ResultCorrectionSettings ResultCorrectionSettings { get; set; } = new ResultCorrectionSettings();
    }
}
