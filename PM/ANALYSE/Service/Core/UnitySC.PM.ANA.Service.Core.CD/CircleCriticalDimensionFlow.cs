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

namespace UnitySC.PM.ANA.Service.Core.CD
{
    public class CircleCriticalDimensionFlow : FlowComponent<CircleCriticalDimensionInput, CircleCriticalDimensionResult, CircleCriticalDimensionConfiguration>
    {
        private CalibrationManager _calibrationManager;
        private ShapeDetector _shapeDetectorLib;

        public CircleCriticalDimensionFlow(CircleCriticalDimensionInput input, ShapeDetector shapeDetectorLib = null) : base(input, "CircleCriticalDimensionFlow")
        {
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();

            _shapeDetectorLib = shapeDetectorLib != null ? shapeDetectorLib : new ShapeDetector();
        }

        protected override void Process()
        {
            // Prior to possible exception thrown save input image as Result Image for result thumbnail
            Result.ResultImage = Input.Image;
            bool useMorphologicalOperations = Configuration.UseMorphologicalOperations;

            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(Input.Image, Path.Combine(ReportFolder, "CD_initialImage_csharp.png"));
            }

            var objectiveCalibration = HardwareUtils.GetObjectiveParameters(_calibrationManager, Input.ObjectiveId);

            var circleFinderParams = new ShapeDetector.CircleAreaFinderParams(Input.ApproximateDiameter, Input.DiameterTolerance, objectiveCalibration.Image.PixelSizeX, Configuration.CannyThreshold, Configuration.UseScharrAlgorithm, Configuration.UseMorphologicalOperations);
            var circleDetectionResult = _shapeDetectorLib.ComputeCircleAreaDetection(Input.Image, circleFinderParams, Input.RegionOfInterest);

            if (circleDetectionResult.Circles.IsEmpty())
            {
                useMorphologicalOperations = !useMorphologicalOperations;
                circleFinderParams.UseMorphologicalOperations = useMorphologicalOperations;
                circleDetectionResult = _shapeDetectorLib.ComputeCircleAreaDetection(Input.Image, circleFinderParams, Input.RegionOfInterest);
            }
            Result.Diameter = (circleDetectionResult.Diameter is null) ? Result.Diameter : Configuration.ResultCorrectionSettings.ApplyCorrectionAndLog(circleDetectionResult.Diameter, "Circle diameter", Logger);
            Result.ResultImage = circleDetectionResult.ImageWithCircles;

            if (Configuration.IsAnyReportEnabled())
            {
                string preprocessType = useMorphologicalOperations ? "WithMorphologicalOperations" : "WithoutMorphologicalOperations";
                ImageReport.SaveImage(circleDetectionResult.PreprocessedImage, Path.Combine(ReportFolder, "CD_preprocessedImage_csharp_" + preprocessType + ".png"));
                ImageReport.SaveImage(Result.ResultImage, Path.Combine(ReportFolder, "CD_controlImage_csharp.png"));
                WriteReport(Path.Combine(ReportFolder, "CD_circles_detected.txt"), circleDetectionResult.Circles);
                Configuration.Serialize(Path.Combine(ReportFolder, $"config.txt"));
            }

            if (circleDetectionResult.Circles.IsEmpty())
            {
                throw new Exception("No circles detected.");
            }
        }

        public void WriteReport(string filepath, List<Length> circles)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(filepath))
                {
                    if (circles == null || circles.IsEmpty())
                    {
                        file.WriteLine("No circles detected");
                        return;
                    }

                    file.WriteLine("Max Diameter (µm): " + circles.Max(diameter => diameter.Micrometers));
                    file.WriteLine("Min Diameter (µm): " + circles.Min(diameter => diameter.Micrometers));

                    var diameterAverage = circles.Average(diameter => diameter.Micrometers);
                    var diameterStdDev = Math.Sqrt(circles.Average(diameter => Math.Pow(diameter.Micrometers - diameterAverage, 2)));
                    file.WriteLine("StdDev Diameter (µm): " + diameterStdDev);

                    file.Write(Environment.NewLine);

                    foreach (var circle in circles)
                    {
                        file.WriteLine("diameter (µm): " + circle.Micrometers);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{LogHeader} Reporting failed : {e.Message}");
            }
        }
    }
}
