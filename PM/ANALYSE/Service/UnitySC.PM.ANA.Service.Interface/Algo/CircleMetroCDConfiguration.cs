
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class CircleMetroCDConfiguration : DefaultConfiguration
    {
        public ResultCorrectionSettings ResultCorrectionSettings { get; set; } = new ResultCorrectionSettings();

        // should be hidden -- for expert use only
        public int SeekerNumber { get; set; } = 0;
        public double SeekerWidth { get; set; } = 0.0;
        public int Mode { get; set; } = 0;
        public int EdgeLocPref { get; set; } = 0;
        public uint Overlayflags { get; set; } = (uint)(MetroCDDrawFlag.DrawFit | MetroCDDrawFlag.DrawDetection | MetroCDDrawFlag.DrawSkipDetection);
        public int KernelSize { get; set; } = 3;
        public double SigAnalysisThreshold { get; set; } = 10.0;
        public int SigAnalysisPeakWindowSize { get; set; } = 100;

    }
}
