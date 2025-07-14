using System;
using System.IO;
using System.Windows;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoFocus;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.EME.Service.Core.Flows.PatternRec
{
    public class PatternRecFlow : FlowComponent<PatternRecInput, PatternRecResult, PatternRecConfiguration>
    {
        private GetZFocusFlow _getZFocusFlow;
        private readonly ICalibrationService _calibration;

        private readonly IEmeraCamera _camera;

        private readonly EdgeDetection _edgeDetectorLib;

        private readonly FlowsConfiguration _flowsConfiguration;
        private readonly PhotoLumAxes _motionAxes;
        private readonly ImageRegistration _registrationLib;

        public PatternRecFlow(PatternRecInput input, IEmeraCamera camera, GetZFocusFlow getZFocusFlow = null,
            EdgeDetection edgeDetectorLib = null, ImageRegistration registrationLib = null) : base(input,
            "PatternRecFlow")
        {
            _flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>() as FlowsConfiguration;
            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            _camera = camera;
            _calibration = ClassLocator.Default.GetInstance<ICalibrationService>();

            if (hardwareManager.MotionAxes is PhotoLumAxes motionAxes)
            {
                _motionAxes = motionAxes;
            }
            else
            {
                throw new Exception("MotionAxes should be PhotoLumAxes");
            }

            if (hardwareManager.Cameras.IsNullOrEmpty())
            {
                throw new Exception("No camera found.");
            }

            _edgeDetectorLib = edgeDetectorLib ?? new EdgeDetection();
            _registrationLib = registrationLib ?? new ImageRegistration();

            if (Input.RunAutofocus) _getZFocusFlow = getZFocusFlow ?? new GetZFocusFlow(Input.GetZFocusInput);
        }

        protected override void Process()
        {
            if (Input.RunAutofocus)
            {
                var initialPosition = _motionAxes.GetPosition() as XYZPosition;
                _getZFocusFlow = _getZFocusFlow ?? new GetZFocusFlow(Input.GetZFocusInput);
                var result = _getZFocusFlow.Execute();
                bool autofocusSucceeded = result.Status.State == FlowState.Success;

                if (!autofocusSucceeded)
                {
                    Logger.Debug($"{LogHeader} Autofocus failed.");
                    _motionAxes.GoToPosition(initialPosition);
                }
            }

            double imageResolutionScale = _flowsConfiguration.ImageScale;
            var refImg = new ServiceImage(Input.Data.PatternReference);

            if (refImg.DataHeight == 0)
            {
                throw new Exception("The reference image could not be read.");
            }

            double refImageScale = (double)refImg.DataHeight / _camera.GetMatroxCameraInfo().Height;
            //Resizing the ref image to match the input image scale if they don't match
            if (!refImageScale.Near(_flowsConfiguration.ImageScale, 1e-6))
            {
                double coeffToMatchImageScale = (_camera.GetMatroxCameraInfo().Height * _flowsConfiguration.ImageScale) / refImg.DataHeight;
                var refImgData = AlgorithmLibraryUtils.CreateImageData(refImg);
                var croppingRoi = new UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest { X = 0, Y = 0, Width = 0, Height = 0 };
                var resizedRefImgData = UnitySCSharedAlgosOpenCVWrapper.ImageOperators.Resize(refImgData, croppingRoi, coeffToMatchImageScale);
                refImg = AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(resizedRefImgData);
            }

            var sensedImg = _camera.SingleScaledAcquisition(Int32Rect.Empty, imageResolutionScale);

            sensedImg = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(sensedImg);

            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(refImg, Path.Combine(ReportFolder, "registration_refImage_csharp.png"));
                ImageReport.SaveImage(sensedImg, Path.Combine(ReportFolder, "registration_sensedImage_csharp.png"));
            }

            bool imagesDimensionsAreNotCompatible = refImg.DataWidth != sensedImg.DataWidth ||
                                                    refImg.DataHeight != sensedImg.DataHeight ||
                                                    refImg.Depth != sensedImg.Depth;
            if (imagesDimensionsAreNotCompatible)
            {
                throw new Exception("The reference image and the analyzed image must have the same size and type.");
            }

            var pixelSize = _calibration.GetCameraCalibrationData().Result.PixelSize / imageResolutionScale;

            if (Input.RunPreprocessing)
            {
                Logger.Debug(
                    $"{LogHeader} Preprocess images with an edge detection filter (gamma = {Input.Data.Gamma}).");

                //removeNoise is set to true on the ref image to remove the camera noise
                //it is set to false on the sensed image because as long as the noise isn't present on both images it will not interfere during the matching process.
                //the setting true / false allows the pattern present on both images to have more impact while lessening the impact of things that are different between the 2 image (noise)
                var refImgPreprocessed = _edgeDetectorLib.Compute(refImg, Input.Data.Gamma, Input.Data.RegionOfInterest,
                    pixelSize, true);
                var sensedImgPreprocessed =
                    _edgeDetectorLib.Compute(sensedImg, Input.Data.Gamma, null, pixelSize, false);
                if (Configuration.IsAnyReportEnabled())
                {
                    ImageReport.SaveImage(refImgPreprocessed,
                        Path.Combine(ReportFolder,
                            $"registration_refImage_preprocessed_with_gamma_{Input.Data.Gamma}_csharp.png"));
                    ImageReport.SaveImage(sensedImgPreprocessed,
                        Path.Combine(ReportFolder,
                            $"registration_sensedImage_preprocessed_with_gamma_{Input.Data.Gamma}_csharp.png"));
                }

                //here removeNoise is set to true for both ref and sensed since we are doing a pixel by pixel image comparison
                var registrationData = new ImageRegistration.RegistrationData(refImg, refImgPreprocessed, sensedImg,
                    sensedImgPreprocessed, Input.Data.RegionOfInterest, Configuration.AngleTolerance,
                    Configuration.ScaleTolerance, Configuration.DilationMaskSize);
                bool writeReport = Configuration.IsAnyReportEnabled();
                Result = _registrationLib.Compute(registrationData, pixelSize, ReportFolder, writeReport);
            }
            else
            {
                bool writeReport = Configuration.IsAnyReportEnabled();
                var registrationData = new ImageRegistration.RegistrationData(refImg, refImg, sensedImg, sensedImg,
                    Input.Data.RegionOfInterest, Configuration.AngleTolerance, Configuration.ScaleTolerance,
                    Configuration.DilationMaskSize);
                Result = _registrationLib.Compute(registrationData, pixelSize, ReportFolder, writeReport);
            }

            if (Configuration.IsAnyReportEnabled())
            {
                string filenameHybridImg = "registration_controlImage_csharp.png";
                Result.ControlImage.SaveToFile(Path.Combine(ReportFolder, filenameHybridImg));
            }

            bool registrationSucceeded = Result.Confidence > Configuration.SimilarityThreshold;
            if (!registrationSucceeded)
            {
                throw new Exception(
                    "The spatial transformation found between the analyzed image and the reference image is of too low quality");
            }

            Logger.Information(
                $"{LogHeader} Spatial transformation found between the analyzed image and the reference image : Shift(mm) x: {Result.ShiftX} y: {Result.ShiftY}");
        }
    }
}
