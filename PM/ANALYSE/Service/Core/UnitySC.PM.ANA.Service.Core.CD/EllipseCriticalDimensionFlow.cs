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
    public class EllipseCriticalDimensionFlow : FlowComponent<EllipseCriticalDimensionInput, EllipseCriticalDimensionResult, EllipseCriticalDimensionConfiguration>
    {
        private CalibrationManager _calibrationManager;
        private ShapeDetector _shapeDetectorLib;

        public EllipseCriticalDimensionFlow(EllipseCriticalDimensionInput input, ShapeDetector shapeDetectorLib = null) : base(input, "EllipseCriticalDimensionFlow")
        {
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();

            _shapeDetectorLib = shapeDetectorLib != null ? shapeDetectorLib : new ShapeDetector();
        }

        protected override void Process()
        {
            // Prior to possible exception thrown, save input image as Result Image for result thumbnail
            Result.ResultImage = Input.Image;

            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(Input.Image, Path.Combine(ReportFolder, "CD_initialImage_csharp.png"));
            }

            var objectiveCalibration = HardwareUtils.GetObjectiveParameters(_calibrationManager, Input.ObjectiveId);

            var ellipseFinderParams = new ShapeDetector.EllipseAreaFinderParams(Input.ApproximateLength, Input.ApproximateWidth, Input.LengthTolerance, Input.WidthTolerance, objectiveCalibration.Image.PixelSizeX, Configuration.CannyThreshold);
            var ellipseDetectionResult = _shapeDetectorLib.ComputeEllipseDetection(Input.Image, ellipseFinderParams, Input.RegionOfInterest);

            //WorkAround for
            bool UseCircleShape = (Input.ApproximateLength == Input.ApproximateWidth) && (Input.LengthTolerance == Input.WidthTolerance);
            if (UseCircleShape)
            {
                double diameter_um = (ellipseDetectionResult.Axis.Length.Micrometers + ellipseDetectionResult.Axis.Width.Micrometers) * 0.5;
                Result.Length = (diameter_um.Micrometers() is null) ? Result.Length : Configuration.ResultCorrectionSettings.ApplyCorrectionAndLog(diameter_um.Micrometers(), "Ellipse length", Logger);
                Result.Width = (diameter_um.Micrometers() is null) ? Result.Width : Configuration.ResultCorrectionSettings.ApplyCorrectionAndLog(diameter_um.Micrometers(), "Ellipse width", Logger);
            }
            else
            {
                Result.Length = (ellipseDetectionResult.Axis.Length is null) ? Result.Length : Configuration.ResultCorrectionSettings.ApplyCorrectionAndLog(ellipseDetectionResult.Axis.Length, "Ellipse length", Logger);
                Result.Width = (ellipseDetectionResult.Axis.Width is null) ? Result.Width : Configuration.ResultCorrectionSettings.ApplyCorrectionAndLog(ellipseDetectionResult.Axis.Width, "Ellipse width", Logger);
            }
            Result.ResultImage = ellipseDetectionResult.ImageWithEllipses;

            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(ellipseDetectionResult.PreprocessedImage, Path.Combine(ReportFolder, "CD_preprocessedImage_csharp.png"));
                ImageReport.SaveImage(Result.ResultImage, Path.Combine(ReportFolder, "CD_controlImage_csharp.png"));
                WriteReport(Path.Combine(ReportFolder, "CD_ellipses_detected.txt"), ellipseDetectionResult.Ellipses);
                Configuration.Serialize(Path.Combine(ReportFolder, $"config.txt"));
            }

            if (ellipseDetectionResult.Ellipses.IsEmpty())
            {
                throw new Exception("No ellipses detected.");
            }
        }

        private void WriteReport(string filepath, List<ShapeDetector.EllipseAxis> ellipsis)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(filepath))
                {
                    if (ellipsis == null || ellipsis.IsEmpty())
                    {
                        file.WriteLine("No ellipsis detected");
                        return;
                    }

                    file.WriteLine("Max Width (µm): " + ellipsis.Max(x => x.Width.Micrometers));
                    file.WriteLine("Min Width (µm): " + ellipsis.Min(x => x.Width.Micrometers));

                    var widthAverage = ellipsis.Average(x => x.Width.Micrometers);
                    var widthStdDev = Math.Sqrt(ellipsis.Average(x => Math.Pow(x.Width.Micrometers - widthAverage, 2)));
                    file.WriteLine("StdDev Width (µm): " + widthStdDev);

                    file.Write(Environment.NewLine);

                    file.WriteLine("Max Length (µm): " + ellipsis.Max(x => x.Length.Micrometers));
                    file.WriteLine("Min Length (µm): " + ellipsis.Min(x => x.Length.Micrometers));

                    var lengthAverage = ellipsis.Average(x => x.Length.Micrometers);
                    var lengthStdDev = Math.Sqrt(ellipsis.Average(x => Math.Pow(x.Length.Micrometers - lengthAverage, 2)));
                    file.WriteLine("StdDev Length (µm): " + lengthStdDev);

                    file.Write(Environment.NewLine);

                    foreach (var ellipse in ellipsis)
                    {
                        file.WriteLine("width & length (µm): " + ellipse.Width.Micrometers + " ; " + ellipse.Length.Micrometers);
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
