using System;
using System.IO;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Flows.ImageProcessing
{
    public class ImagePreprocessingFlow : FlowComponent<ImagePreprocessingInput, ImagePreprocessingResult, AxesMovementConfiguration>
    {
        private readonly EdgeDetection _edgeDetectorLib;
        private readonly PhotoLumAxes _motionAxes;
        private readonly ICameraServiceEx _camera;
        private readonly ICalibrationService _calibration;
        private readonly FlowsConfiguration _flowsConfiguration;

        public ImagePreprocessingFlow(ImagePreprocessingInput input, EdgeDetection edgeDetectorLib = null) : base(input, "ImageProcessingFlow")
        {
            _flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>() as FlowsConfiguration;
            _calibration = ClassLocator.Default.GetInstance<ICalibrationService>();
            _camera = ClassLocator.Default.GetInstance<ICameraServiceEx>();

            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            if (hardwareManager.MotionAxes is PhotoLumAxes motionAxes)
            {
                _motionAxes = motionAxes;
            }
            else
            {
                throw new Exception($"MotionAxes should be PhotoLumAxes");
            }

            _edgeDetectorLib = edgeDetectorLib ?? new EdgeDetection();
        }

        protected override void Process()
        {
            Logger.Debug($"{LogHeader} Preprocess images with an edge detection filter (gamma = {Input.Gamma}).");

            var initialImage = Input.PreAcquiredImage;
            if (initialImage == null)
            {
                _motionAxes.GoToPosition(Input.Position);
                initialImage = _camera.GetCameraImage().Result;
                initialImage = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(initialImage);
            }

            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(initialImage, Path.Combine(ReportFolder, $"initial_image_csharp.png"));
            }

            double imageResolutionScale = _flowsConfiguration.ImageScale;
            var pixelSize = _calibration.GetCameraCalibrationData().Result.PixelSize / imageResolutionScale;

            //removeNoise set to true to remove noise for visualization purposes, but could also be set to false
            var preprocessedImage = _edgeDetectorLib.Compute(initialImage, Input.Gamma, Input.RegionOfInterest, pixelSize, true);
            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(preprocessedImage, Path.Combine(ReportFolder, $"preprocessed_image_with_gamma_{Input.Gamma}_csharp.png"));
            }

            Result.PreprocessedImage = preprocessedImage;
        }
    }
}
