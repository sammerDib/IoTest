using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoFocus;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class PatternRecTest : TestWithMockedHardware<PatternRecTest>, ITestWithPhotoLumAxes, ITestWithCamera
    {
        private Mock<EdgeDetection> _simulatedEdgeDetectorLib;
        private Mock<ImageRegistration> _simulatedRegistrationLib;

        private Mock<GetZFocusFlow> _simulatedGetZFocusFlow;

        private PatternRecInput _defaultPatternRecInput;

        public Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }
        public DummyIDSCamera SimulatedCamera { get; set; }
        public IEmeraCamera EmeraCamera { get; set; }
        public Length PixelSize { get; set; }
        protected override void PreGenericSetup()
        {
            PixelSize = ClassLocator.Default.GetInstance<ICalibrationService>().GetCameraCalibrationData().Result.PixelSize;
        }

        protected override void PostGenericSetup()
        {
            _defaultPatternRecInput = SimulatedData.ValidPatternRecInput();
            _defaultPatternRecInput.RunAutofocus = false;

            _simulatedEdgeDetectorLib = new Mock<EdgeDetection> { CallBase = true };
            _simulatedEdgeDetectorLib.Setup(_ => _.Compute(It.IsAny<ServiceImage>(), It.IsAny<double>(), It.IsAny<RegionOfInterest>(), It.IsAny<Length>(), It.IsAny<bool>())).Returns(new ServiceImage());

            _simulatedRegistrationLib = new Mock<ImageRegistration> { CallBase = true };
            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 1.Millimeters(), 1.Millimeters(), null);
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<Length>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

            _simulatedGetZFocusFlow = new Mock<GetZFocusFlow>(_defaultPatternRecInput.GetZFocusInput) { CallBase = true };
            var AfSuccess = new GetZFocusResult();
            AfSuccess.Status = new FlowStatus(FlowState.Success);
            _simulatedGetZFocusFlow.SetupSequence(_ => _.Execute()).Returns(AfSuccess);

            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.IsAcquiring()).Returns(true);
            var sensedDummyUSPImage = new DummyUSPImage(10, 10, 255);
            TestWithCameraHelper.SetupCameraWithImagesForSingleScaledAcquisition(new List<DummyUSPImage>() { sensedDummyUSPImage });
            var fakeCameraInfo = new MatroxCameraInfo() { Height = sensedDummyUSPImage.Height };
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetMatroxCameraInfo()).Returns(fakeCameraInfo);

        }


        [TestMethod]
        public void Pattern_recognition_flow_nominal_case()
        {
            // Given : A sensed image shifted along a known x and y with respect to the reference image

            var expectedShiftX = 105 * PixelSize;
            var expectedShiftY = -5 * PixelSize;
            double expectedConfidence = 1;

            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), expectedConfidence, expectedShiftX, expectedShiftY, new DummyUSPImage(10, 10, 255).ToServiceImage());
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<Length>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

            var input = _defaultPatternRecInput;
            input.Data.Gamma = 0.5;
            input.RunAutofocus = true;
            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedGetZFocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            // When
            var result = flow.Execute();

            // Then : It's succeeded and find a nearly expected translation
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(expectedShiftX.Millimeters, result.ShiftX.Millimeters, 10e-10);
            Assert.AreEqual(expectedShiftY.Millimeters, result.ShiftY.Millimeters, 10e-10);
            Assert.AreEqual(1, result.Confidence);
        }

        [TestMethod]
        public void Source_and_sensed_images_must_have_the_same_aspect_ratio()
        {
            // Given : Images of different aspect ratios
            var ref_img = new DummyUSPImage(10, 10, 255);

            var input = _defaultPatternRecInput;
            input.Data.PatternReference = ref_img.ToExternalImage();

            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedGetZFocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            var sensed_img = new DummyUSPImage(10, 15, 255);
            var cameraInfo = new MatroxCameraInfo() { Height = sensed_img.Height };
            TestWithCameraHelper.SetupCameraWithSameImageContinuouslyForSingleAcquisition(sensed_img);
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetMatroxCameraInfo()).Returns(cameraInfo);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Pattern_rec_works_when_the_images_have_different_sizes_but_the_same_aspect_ratio()
        {
            // Given : Images of different sizes but same aspect ratio
            var ref_img = new DummyUSPImage(10, 10, 255);

            var input = _defaultPatternRecInput;
            input.Data.PatternReference = ref_img.ToExternalImage();

            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedGetZFocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            var sensed_img = new DummyUSPImage(15, 15, 255);
            var cameraInfo = new MatroxCameraInfo() { Height = sensed_img.Height };
            TestWithCameraHelper.SetupCameraWithSameImageContinuouslyForSingleAcquisition(sensed_img);
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetMatroxCameraInfo()).Returns(cameraInfo);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
        }

        [TestMethod]
        public void Pattern_recognition_fails_when_registration_quality_is_equal_to_or_less_than_the_similarity_threshold()
        {
            var input = _defaultPatternRecInput;
            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedGetZFocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);
            // Given: Registration quality is <= similarity threshold
            var quality = flow.Configuration.SimilarityThreshold;

            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), quality, 10.Millimeters(), 10.Millimeters(), new DummyUSPImage(10, 10, 255).ToServiceImage());
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<Length>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual(quality, result.Confidence);
        }

        [TestMethod]
        public void Pattern_recognition_success_when_registration_quality_is_greater_than_the_similarity_threshold()
        {
            var input = _defaultPatternRecInput;
            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedGetZFocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);
            // Given: Registration quality is > similarity threshold
            var quality = flow.Configuration.SimilarityThreshold + 0.01;

            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), quality, 10.Millimeters(), 10.Millimeters(), new DummyUSPImage(10, 10, 255).ToServiceImage());
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<Length>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(quality, result.Confidence);
        }

        [TestMethod]
        public void Preprocessing_is_computed_when_gamma_is_provided()
        {
            // Given
            var input = _defaultPatternRecInput;
            input.Data.Gamma = 0.5;

            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedGetZFocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            // When
            var result = flow.Execute();

            // Then: Preprocessing is computed for reference image and sensed image
            _simulatedEdgeDetectorLib.Verify(x => x.Compute(It.IsAny<ServiceImage>(), It.IsAny<double>(), It.IsAny<RegionOfInterest>(), It.IsAny<Length>(), It.IsAny<bool>()), Times.Exactly(2));
        }

        [TestMethod]
        public void Preprocessing_is_not_computed_when_gamma_is_not_provided()
        {
            // Given
            var input = _defaultPatternRecInput;
            input.Data.Gamma = double.NaN;

            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedGetZFocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            // When
            var result = flow.Execute();

            // Then: Preprocessing is not computed
            _simulatedEdgeDetectorLib.Verify(x => x.Compute(It.IsAny<ServiceImage>(), It.IsAny<double>(), It.IsAny<RegionOfInterest>(), It.IsAny<Length>(), It.IsAny<bool>()), Times.Exactly(0));
        }

        [TestMethod]
        public void Autofocus_is_not_computed_when_autofocus_is_disabled()
        {
            // Given
            var input = _defaultPatternRecInput;
            input.RunAutofocus = false;

            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedGetZFocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            // When
            var result = flow.Execute();

            // Then: No autofocus is computed
            _simulatedGetZFocusFlow.Verify(x => x.Execute(), Times.Exactly(0));
        }

        [TestMethod]
        public void Pattern_rec_nominal_case()
        {
            // Given: input with actual ref image, no autofocus, no ROI
            var input = _defaultPatternRecInput;
            input.Data.PatternReference = CreatePatternRecImage("refImage.png").ToExternalImage();
            input.RunAutofocus = false;
            input.Data.Gamma = 0.5;
            input.Data.RegionOfInterest = null;

            // Given camera with sensed image
            var sensedDummyUSPImage = CreatePatternRecImage("sensedImage.png");
            TestWithCameraHelper.SetupCameraWithImagesForSingleScaledAcquisition(new List<DummyUSPImage>() { sensedDummyUSPImage });
            var cameraInfo = new MatroxCameraInfo() { Height = sensedDummyUSPImage.Height };
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetMatroxCameraInfo()).Returns(cameraInfo);

            // When
            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object);
            var result = flow.Execute();

            // Then proper result is found
            Assert.AreEqual(32, result.ShiftX.Micrometers, 1);
            Assert.AreEqual(-421, result.ShiftY.Micrometers, 1);
            Assert.IsTrue(result.Confidence > 0.9);
        }

        [TestMethod]
        public void Pattern_rec_nominal_case_ROI()
        {
            // Given: input with actual ref image, no autofocus
            var input = _defaultPatternRecInput;
            input.Data.PatternReference = CreatePatternRecImage("refImage.png").ToExternalImage();
            input.RunAutofocus = false;

            // Given ROI just enclosing the pattern          
            input.Data.RegionOfInterest = new RegionOfInterest(1.0880352.Millimeters(), 0.8213184.Millimeters(), 0.52963.Millimeters(), 0.52451.Millimeters());

            // Given camera with sensed image
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.IsAcquiring()).Returns(true);
            var sensedDummyUSPImage = CreatePatternRecImage("sensedImage.png");
            TestWithCameraHelper.SetupCameraWithImagesForSingleScaledAcquisition(new List<DummyUSPImage>() { sensedDummyUSPImage });
            var cameraInfo = new MatroxCameraInfo() { Height = sensedDummyUSPImage.Height };
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetMatroxCameraInfo()).Returns(cameraInfo);

            // When
            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object);
            var result = flow.Execute();

            // Then proper result is found
            Assert.AreEqual(32, result.ShiftX.Micrometers, 1);
            Assert.AreEqual(-421, result.ShiftY.Micrometers, 1);
            Assert.IsTrue(result.Confidence > 0.9);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestPatternRecReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultPatternRecInput;

            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var filename = Path.Combine(flow.ReportFolder, $"input.txt");
            Assert.IsTrue(File.Exists(filename));

            Directory.Delete(flow.ReportFolder, true);
        }

        [TestMethod]
        public void Report_of_result_is_working()
        {
            //Given
            string directoryPath = "TestPatternRecReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultPatternRecInput;

            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var status = (flow.Result == null) ? "null" : ((flow.Result.Status == null) ? "ukn" : flow.Result.Status.State.ToString());
            var filename = Path.Combine(flow.ReportFolder, $"result_{status}.txt");
            Assert.IsTrue(File.Exists(filename));

            Directory.Delete(flow.ReportFolder, true);
        }

        [TestMethod]
        public void Report_of_images_is_working()
        {
            // Given

            var expectedShiftX = 105 * PixelSize;
            var expectedShiftY = -5 * PixelSize;
            double expectedConfidence = 1;

            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), expectedConfidence, expectedShiftX, expectedShiftY, new DummyUSPImage(10, 10, 255).ToServiceImage());
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<Length>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

            string directoryPath = "TestPatternRecReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultPatternRecInput;
            input.Data.Gamma = 0.5;
            input.RunAutofocus = true;
            var flow = new PatternRecFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedGetZFocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            // When
            var result = flow.Execute();

            // Then
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"registration_refImage_preprocessed_with_gamma_{input.Data.Gamma}_csharp.png")));
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"registration_sensedImage_preprocessed_with_gamma_{input.Data.Gamma}_csharp.png")));
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"registration_refImage_csharp.png")));
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"registration_sensedImage_csharp.png")));
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"registration_controlImage_csharp.png")));

            Directory.Delete(directoryPath, true);
        }

        private static DummyUSPImage CreatePatternRecImage(string filename)
        {
            const string PatternRecFolder = "patternRec";
            return CameraTestUtils.CreateProcessingImageFromFile(CameraTestUtils.GetDefaultDataPath(Path.Combine(PatternRecFolder, filename)));
        }

    }
}
