using System.IO;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.PatternRec
{
    public class ImagePreprocessingFlow : FlowComponent<ImagePreprocessingInput, ImagePreprocessingResult, ImagePreprocessingConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;
        private readonly ICameraManager _cameraManager;

        private readonly EdgeDetection _edgeDetectorLib;

        public ImagePreprocessingFlow(ImagePreprocessingInput input, EdgeDetection edgeDetectorLib = null) : base(input, "ImageProcessingFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            _cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();

            _edgeDetectorLib = edgeDetectorLib != null ? edgeDetectorLib : new EdgeDetection();
        }

        protected override void Process()
        {
            var objectiveCalibration = HardwareUtils.GetObjectiveParametersUsedByCamera(_hardwareManager, _calibrationManager, Input.CameraId);
            HardwareUtils.MoveAxesTo(_hardwareManager.Axes, Input.Position, Configuration.Speed);

            Logger.Debug($"{LogHeader} Preprocess images with an edge detection filter (gamma = {Input.Gamma}).");

            var initialImage = HardwareUtils.AcquireCameraImage(_hardwareManager, _cameraManager, Input.CameraId);

            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(initialImage, Path.Combine(ReportFolder, $"initial_image_csharp.png"));
            }
            //removeNoise set to true to remove noise for vizualization purposes, but could also be set to false
            var preprocessedImage = _edgeDetectorLib.Compute(initialImage, Input.Gamma, Input.RegionOfInterest, objectiveCalibration.Image, true);
            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(preprocessedImage, Path.Combine(ReportFolder, $"preprocessed_image_with_gamma_{Input.Gamma}_csharp.png"));
            }

            Result.PreprocessedImage = preprocessedImage;
        }
    }
}
