using System.IO;

using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;
using UnitySC.PM.ANA.Service.Core.Shared;
using System.Linq;
using System;
using System.Collections.Generic;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;

namespace UnitySC.PM.ANA.Service.Core.CD
{
    public class CircleMetroCDFlow : FlowComponent<CircleCriticalDimensionInput, CircleCriticalDimensionResult, CircleMetroCDConfiguration>
    {
        private CalibrationManager _calibrationManager;
        private ShapeDetector _shapeDetectorLib;

        public CircleMetroCDFlow(CircleCriticalDimensionInput input, ShapeDetector shapeDetectorLib = null) : base(input, "CircleCriticalDimensionFlow")
        {
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();

            _shapeDetectorLib = shapeDetectorLib != null ? shapeDetectorLib : new ShapeDetector();
        }

        protected override void Process()
        {
            // Prior to possible exception thrown save input image as Result Image for result thumbnail
            Result.ResultImage = Input.Image;

            bool useReport = Configuration.IsAnyReportEnabled();
            if (useReport)
            {
                ImageReport.SaveImage(Input.Image, Path.Combine(ReportFolder, "CD_InpuImage.png"));
            }

            var objectiveCalibration = HardwareUtils.GetObjectiveParameters(_calibrationManager, Input.ObjectiveId);

            var circleFinderParams = new ShapeDetector.CircleCentralFinderParams(Input.ApproximateDiameter, Input.DiameterTolerance, objectiveCalibration.Image.PixelSizeX);

            UpdateAdvancedSettings(circleFinderParams);

            var circleDetectionResult = _shapeDetectorLib.ComputeCircleCentralDetection(Input.Image, circleFinderParams, useReport, ReportFolder, Configuration.Overlayflags);

            Result.Diameter = (circleDetectionResult.Diameter is null) ? Result.Diameter : Configuration.ResultCorrectionSettings.ApplyCorrectionAndLog(circleDetectionResult.Diameter, "Circle diameter", Logger);
            Result.ResultImage = circleDetectionResult.ImageWithCircles;

            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(Result.ResultImage, Path.Combine(ReportFolder, "CD_OverlayImage.png"));
                Configuration.Serialize(Path.Combine(ReportFolder, $"CDMetroconfig.txt"));
            }

            if (circleDetectionResult.Circles.IsEmpty())
            {
                throw new Exception("No circles detected.");
            }
        }

        private void UpdateAdvancedSettings(Shared.ShapeDetector.CircleCentralFinderParams advPrm)
        {
            var defaultConfig = new CircleMetroCDConfiguration();
            if (Configuration.SeekerNumber != defaultConfig.SeekerNumber)
                advPrm.SeekerNumber = Configuration.SeekerNumber;

            if (Configuration.SeekerWidth != defaultConfig.SeekerWidth)
                advPrm.SeekerWidth = Configuration.SeekerWidth;

            if (Configuration.Mode != defaultConfig.Mode)
                advPrm.Mode = Configuration.Mode;

            if (Configuration.KernelSize != defaultConfig.KernelSize)
                advPrm.KernelSize = Configuration.KernelSize;

            if (Configuration.EdgeLocPref != defaultConfig.EdgeLocPref)
                advPrm.EdgeLocaliz = Configuration.EdgeLocPref;

            if (Configuration.SigAnalysisThreshold != defaultConfig.SigAnalysisThreshold)
                advPrm.SigAnalysisThreshold = Configuration.SigAnalysisThreshold;

            if (Configuration.SigAnalysisPeakWindowSize != defaultConfig.SigAnalysisPeakWindowSize)
                advPrm.SigAnalysisPeakWindowSize = Configuration.SigAnalysisPeakWindowSize;
        }
    }
}
