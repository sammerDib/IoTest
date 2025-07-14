using System;
using System.IO;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.PatternRec
{
    public class PatternRecFlow : FlowComponent<PatternRecInput, PatternRecResult, PatternRecConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        private readonly ICameraManager _cameraManager;
        private readonly IAxes _axes;

        private readonly EdgeDetection _edgeDetectorLib;
        private readonly ImageRegistration _registrationLib;

        private readonly AutofocusFlow _autofocusFlow;

        public PatternRecFlow(
            PatternRecInput input,
            AutofocusFlow autofocusFlow = null,
            EdgeDetection edgeDetectorLib = null,
            ImageRegistration registrationLib = null) : base(input, "PatternRecFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            _cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
            _axes = ClassLocator.Default.GetInstance<AnaHardwareManager>().Axes;

            _edgeDetectorLib = edgeDetectorLib != null ? edgeDetectorLib : new EdgeDetection();
            _registrationLib = registrationLib != null ? registrationLib : new ImageRegistration();

            _autofocusFlow = autofocusFlow != null ? autofocusFlow : new AutofocusFlow(new AutofocusInput(Input.AutoFocusSettings));

            FdcProvider = ClassLocator.Default.GetInstance<PatternRecFlowFDCProvider>();
        }

        protected override void Process()
        {
            if (Input.RunAutofocus)
            {
                var initialPosition = HardwareUtils.GetAxesPosition(_axes);

                _autofocusFlow.Input.Settings = Input.AutoFocusSettings;
                var result = _autofocusFlow.Execute();
                bool autofocusSucceeded = result.Status.State == FlowState.Success;

                if (!autofocusSucceeded)
                {
                    Logger.Debug($"{LogHeader} Autofocus failed.");
                    HardwareUtils.MoveAxesTo(_axes, initialPosition, Configuration.Speed);
                }
            }

            var refImg = new ServiceImage(Input.Data.PatternReference);
            var sensedImg = HardwareUtils.AcquireCameraImage(_hardwareManager, _cameraManager, Input.Data.CameraId);

            if (Configuration.IsAnyReportEnabled())
            {
                ImageReport.SaveImage(refImg, Path.Combine(ReportFolder, $"registration_refImage_csharp.png"));
                ImageReport.SaveImage(sensedImg, Path.Combine(ReportFolder, $"registration_sensedImage_csharp.png"));
            }

            bool imagesDimensionsAreNotCompatible = refImg.DataWidth != sensedImg.DataWidth || refImg.DataHeight != sensedImg.DataHeight || refImg.Depth != sensedImg.Depth;
            if (imagesDimensionsAreNotCompatible)
            {
                throw new Exception($"The reference image and the analyzed image must have the same size and type.");
            }

            var objectiveCalibration = HardwareUtils.GetObjectiveParametersUsedByCamera(_hardwareManager, _calibrationManager, Input.Data.CameraId);

            if (Input.RunPreprocessing)
            {
                Logger.Debug($"{LogHeader} Preprocess images with an edge detection filter (gamma = {Input.Data.Gamma}).");

                //removeNoise is set to true on the ref image to remove the camera noise
                //it is set to false on the sensed image because as long as the noise isn't present on both images it will not interfere during the matching process.
                //the setting true / false allows the pattern present on both images to have more impact while lessening the impact of things that are different between the 2 image (noise)
                var refImgPreprocessed = _edgeDetectorLib.Compute(refImg, Input.Data.Gamma, Input.Data.RegionOfInterest, objectiveCalibration.Image, true);
                var sensedImgPreprocessed = _edgeDetectorLib.Compute(sensedImg, Input.Data.Gamma, null, objectiveCalibration.Image, false);
                if (Configuration.IsAnyReportEnabled())
                {
                    ImageReport.SaveImage(refImgPreprocessed, Path.Combine(ReportFolder, $"registration_refImage_preprocessed_with_gamma_{Input.Data.Gamma}_csharp.png"));
                    ImageReport.SaveImage(sensedImgPreprocessed, Path.Combine(ReportFolder, $"registration_sensedImage_preprocessed_with_gamma_{Input.Data.Gamma}_csharp.png"));
                }
                //here removeNoise is set to true for both ref and sensed since we are doing a pixel by pixel image comparison
                var registrationData = new ImageRegistration.RegistrationData(refImg, refImgPreprocessed, sensedImg, sensedImgPreprocessed, Input.Data.RegionOfInterest, Configuration.AngleTolerance, Configuration.ScaleTolerance, Configuration.DilationMaskSize);
                bool writeReport = Configuration.IsAnyReportEnabled();
                Result = _registrationLib.Compute(registrationData, objectiveCalibration.Image, ReportFolder, writeReport);
            }
            else
            {
                bool writeReport = Configuration.IsAnyReportEnabled();
                var registrationData = new ImageRegistration.RegistrationData(refImg, refImg, sensedImg, sensedImg, Input.Data.RegionOfInterest, Configuration.AngleTolerance, Configuration.ScaleTolerance, Configuration.DilationMaskSize);
                Result = _registrationLib.Compute(registrationData, objectiveCalibration.Image, ReportFolder, writeReport);
            }

            if (Configuration.IsAnyReportEnabled())
            {
                string filenameHybridImg = $"registration_controlImage_csharp.png";
                Result.ControlImage.SaveToFile(Path.Combine(ReportFolder, filenameHybridImg));
            }

            bool registrationSucceeded = Result.Confidence > Configuration.SimilarityThreshold;
            if (!registrationSucceeded)
            {
                throw new Exception($"The spatial transformation found between the analyzed image and the reference image is of too low quality ({Result.Confidence:0.000}<{Configuration.SimilarityThreshold:0.000})");
            }

            Logger.Information($"{LogHeader} Spatial transformation found between the analyzed image and the reference image : Shift(mm) x: {Result.ShiftX} y: {Result.ShiftY}");

            FdcProvider.CreateFDC();
        }
    }
}
