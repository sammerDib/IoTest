using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{

    public class LiseHFIntegrationTimeCalibrationConfiguration : DefaultConfiguration
    {
        public double TargetSpectroCountTolerance {get; set; } = 10.0; // percentage

        public int TargetSpectroCount { get; set; } = 50000;
        
        public int TargetSpectroCountLow { get; set; } = 25000;

        public int AverageRoughScan { get; set; } = 8;
        
        // Do not set 0 or negative values
        public double MinTimeRoughScan { get; set; } = 0.01; // in ms
        
        public double MaxTimeRoughScan { get; set; } = 250.0; // in ms

        public double StepTimeRoughScan { get; set; } = 0.5; 
        
        public int AverageFineScan { get; set; } = 32;
        
        public double StepTimeFineScan { get; set; } = 0.1;

        public double SearchSpanFineSpan { get; set; } = 3.0; // in ms

        public double CountBlindAcceptance { get; set; } = 200.0;

        public double StopAtMinimalInterval_ms { get; set; } = 0.025;

        public int RollingAverageWindowSize { get; set; } = 5;
    }

    public class LiseHFSpotCalibrationConfiguration : DefaultConfiguration
    {
        public LiseHFSpotCalibrationConfiguration():base()
        {
            WriteReportMode = FlowReportConfiguration.AlwaysWrite;
        }

        public int TargetGreyLevel { get; set; } = 150;

        public int TargetGLTolerance { get; set; } = 15;

        public double CameraFrameRate { get; set; } = 5.0;
    }

    public class LiseHFSpotCheckConfiguration : DefaultConfiguration
    {
        public LiseHFSpotCheckConfiguration() : base()
        {

        }

        public double CameraFrameRate { get; set; } = 5.0;
    }

    public class LiseHFDarkRefConfiguration : DefaultConfiguration
    {
        public LiseHFDarkRefConfiguration() : base()
        {

        }

        public int CalibrationNbAverage { get; set; } = 128;
    }



}
