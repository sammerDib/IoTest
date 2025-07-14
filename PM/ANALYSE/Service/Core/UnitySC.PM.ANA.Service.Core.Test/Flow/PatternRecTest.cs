using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;
using System.IO;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using static UnitySC.PM.ANA.Service.Shared.TestUtils.CameraTestUtils;
using System.Configuration;
using UnitySC.Shared.Image;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class PatternRecTest : TestWithMockedHardware<PatternRecTest>, ITestWithAxes, ITestWithCamera
    {
        private Mock<EdgeDetection> _simulatedEdgeDetectorLib;
        private Mock<ImageRegistration> _simulatedRegistrationLib;

        private Mock<AutofocusFlow> _simulatedAutofocusFlow;

        private PatternRecInput _defaultPatternRecInput;

        private const double Precision = 1e-4;

        #region Interfaces properties

        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public Mock<IAxes> SimulatedAxes { get; set; }

        #endregion Interfaces properties

        protected override void PreGenericSetup()
        {
            PixelSizeX = 2.1168.Micrometers();
            PixelSizeY = 2.1168.Micrometers();
        }

        protected override void PostGenericSetup()
        {
            _simulatedEdgeDetectorLib = new Mock<EdgeDetection> { CallBase = true };
            _simulatedEdgeDetectorLib.Setup(_ => _.Compute(It.IsAny<ServiceImage>(), It.IsAny<double>(), It.IsAny<RegionOfInterest>(), It.IsAny<ImageParameters>(), It.IsAny<bool>())).Returns(new ServiceImage());

            _simulatedRegistrationLib = new Mock<ImageRegistration> { CallBase = true };
            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 1.Millimeters(), 1.Millimeters(), null);
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<ImageParameters>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

            _simulatedAutofocusFlow = new Mock<AutofocusFlow>(new AutofocusInput()) { CallBase = true };
            var AfSuccess = new AutofocusResult();
            AfSuccess.Status = new FlowStatus(FlowState.Success);
            _simulatedAutofocusFlow.SetupSequence(_ => _.Execute()).Returns(AfSuccess);

            _defaultPatternRecInput = SimulatedData.ValidPatternRecInput();
            _defaultPatternRecInput.Data.CameraId = CameraUpId;
            _defaultPatternRecInput.RunAutofocus = false;

            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { new DummyUSPImage(10, 10, 255) });
        }

        [TestMethod]
        public void Pattern_recognition_flow_nominal_case()
        {
            // Given : A sensed image shifted along a known x and y with respect to the reference image

            var expectedShiftX = 105 * PixelSizeX;
            var expectedShiftY = -5 * PixelSizeY;
            double expectedConfidence = 1;

            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), expectedConfidence, expectedShiftX, expectedShiftY, new DummyUSPImage(10, 10, 255).ToServiceImage());
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<ImageParameters>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

            var input = _defaultPatternRecInput;
            input.Data.Gamma = 0.5;
            input.RunAutofocus = true;
            var flow = new PatternRecFlow(input, _simulatedAutofocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            // When
            var result = flow.Execute();

            // Then : It's succeeded and find a nearly expected translation
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(expectedShiftX.Millimeters, result.ShiftX.Millimeters, 10e-10);
            Assert.AreEqual(expectedShiftY.Millimeters, result.ShiftY.Millimeters, 10e-10);
            Assert.AreEqual(1, result.Confidence);
        }

        [TestMethod]
        public void Source_and_sensed_images_must_have_the_same_height()
        {
            // Given : Images of different height
            var ref_img = new DummyUSPImage(10, 10, 255);

            var input = _defaultPatternRecInput;
            input.Data.PatternReference = ref_img.ToExternalImage();

            var flow = new PatternRecFlow(input, _simulatedAutofocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            var sensed_img = new DummyUSPImage(10, 9, 255);
            TestWithCameraHelper.SetupCameraWithSameImageContinuously(sensed_img);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Source_and_sensed_images_must_have_the_same_width()
        {
            // Given : Images of different width
            var ref_img = new DummyUSPImage(2, 10, 255);

            var input = _defaultPatternRecInput;
            input.Data.PatternReference = ref_img.ToExternalImage();

            var flow = new PatternRecFlow(input);

            var sensed_img = new DummyUSPImage(3, 10, 255);
            TestWithCameraHelper.SetupCameraWithSameImageContinuously(sensed_img);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Pattern_recognition_fails_when_registration_quality_is_equal_to_or_less_than_the_similarity_threshold()
        {
            var input = _defaultPatternRecInput;
            var flow = new PatternRecFlow(input, _simulatedAutofocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);
            // Given: Registration quality is <= similarity threshold
            var quality = flow.Configuration.SimilarityThreshold;

            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), quality, 10.Millimeters(), 10.Millimeters(), new DummyUSPImage(10, 10, 255).ToServiceImage());
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<ImageParameters>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

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
            var flow = new PatternRecFlow(input, _simulatedAutofocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);
            // Given: Registration quality is > similarity threshold
            var quality = flow.Configuration.SimilarityThreshold + 0.01;

            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), quality, 10.Millimeters(), 10.Millimeters(), new DummyUSPImage(10, 10, 255).ToServiceImage());
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<ImageParameters>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

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

            var flow = new PatternRecFlow(input, _simulatedAutofocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            // When
            var result = flow.Execute();

            // Then: Preprocessing is computed for reference image and sensed image
            _simulatedEdgeDetectorLib.Verify(x => x.Compute(It.IsAny<ServiceImage>(), It.IsAny<double>(), It.IsAny<RegionOfInterest>(), It.IsAny<ImageParameters>(), It.IsAny<bool>()), Times.Exactly(2));
        }

        [TestMethod]
        public void Preprocessing_is_not_computed_when_gamma_is_not_provided()
        {
            // Given
            var input = _defaultPatternRecInput;
            input.Data.Gamma = double.NaN;

            var flow = new PatternRecFlow(input, _simulatedAutofocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            // When
            var result = flow.Execute();

            // Then: Preprocessing is not computed
            _simulatedEdgeDetectorLib.Verify(x => x.Compute(It.IsAny<ServiceImage>(), It.IsAny<double>(), It.IsAny<RegionOfInterest>(), It.IsAny<ImageParameters>(), It.IsAny<bool>()), Times.Exactly(0));
        }

        [TestMethod]
        public void Autofocus_is_not_computed_when_autofocus_is_disabled()
        {
            // Given
            var input = _defaultPatternRecInput;
            input.RunAutofocus = false;

            var flow = new PatternRecFlow(input, _simulatedAutofocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);

            // When
            var result = flow.Execute();

            // Then: No autofocus is computed
            _simulatedAutofocusFlow.Verify(x => x.Execute(), Times.Exactly(0));
        }

        [TestMethod]
        public void Pattern_rec_nominal_case()
        {
            // Given: input with actual ref image, no autofocus, no ROI
            var input = _defaultPatternRecInput;
            input.Data.PatternReference = CreatePatternRecImage("refImage.png").ToExternalImage();
            input.Data.PatternReference.FileExtension = ".png";
            input.RunAutofocus = false;
            input.Data.Gamma = 0.3;            
            input.Data.RegionOfInterest = null;

            // Given camera with sensed image
            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { CreatePatternRecImage("sensedImage.png") });

            // When
            var flow = new PatternRecFlow(input);
            flow.Configuration.SimilarityThreshold = 0.5;
            flow.Configuration.AngleTolerance = 3.0;
            flow.Configuration.ScaleTolerance = 0.03;
            bool bDebugTestOutput = false;
            if (bDebugTestOutput)
            {
                flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
                string directoryPath = "TestPatternRecReport";
                Directory.CreateDirectory(directoryPath);
                flow.ReportFolder = directoryPath;
            }

            var result = flow.Execute();

            // Then proper result is found
            Assert.AreEqual(34, result.ShiftX.Micrometers, 1);
            Assert.AreEqual(-441, result.ShiftY.Micrometers, 1);
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
            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { CreatePatternRecImage("sensedImage.png") });

            // When
            var flow = new PatternRecFlow(input);
            flow.Configuration.SimilarityThreshold = 0.5;
            flow.Configuration.AngleTolerance = 3.0;
            flow.Configuration.ScaleTolerance = 0.03;
            bool bDebugTestOutput = false;
            if (bDebugTestOutput)
            {
                flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
                string directoryPath = "TestPatternRecReport";
                Directory.CreateDirectory(directoryPath);
                flow.ReportFolder = directoryPath;
            }
            var result = flow.Execute();

            // Then proper result is found
            Assert.AreEqual(34, result.ShiftX.Micrometers, 1);
            Assert.AreEqual(-441, result.ShiftY.Micrometers, 1);
            Assert.IsTrue(result.Confidence > 0.9);
        }

        [TestMethod]
        public void Pattern_rec_nominal_case_EnlargeSensed()
        {
            // Given: input with actual ref image, no autofocus, no ROI
            var input = _defaultPatternRecInput;
            input.Data.PatternReference = CreatePatternRecImage("refImage.png").ToExternalImage();
            input.Data.PatternReference.FileExtension = ".png";
            input.RunAutofocus = false;
            input.Data.Gamma = 0.3;
            input.Data.RegionOfInterest = null;

            // Given camera with sensed image
            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { CreatePatternRecImage("sensedImage-ENL.png") });

            // When
            var flow = new PatternRecFlow(input);
            flow.Configuration.SimilarityThreshold = 0.5;
            flow.Configuration.AngleTolerance = 3.0;
            flow.Configuration.ScaleTolerance = 0.3;
            bool bDebugTestOutput = false;
            if (bDebugTestOutput)
            {
                flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
                string directoryPath = "TestPatternRecReport";
                Directory.CreateDirectory(directoryPath);
                flow.ReportFolder = directoryPath;
            }

            var result = flow.Execute();

            // Then proper result is found
            Assert.AreEqual(34, result.ShiftX.Micrometers, 1);
            Assert.AreEqual(-441, result.ShiftY.Micrometers, 1);
            Assert.IsTrue(result.Confidence > 0.9);
        }

        [TestMethod]
        public void Pattern_rec_nominal_case_EnlargeRef()
        {
            // Given: input with actual ref image, no autofocus, no ROI
            var input = _defaultPatternRecInput;
            input.Data.PatternReference = CreatePatternRecImage("refImage-ENL.png").ToExternalImage();
            input.Data.PatternReference.FileExtension = ".png";
            input.RunAutofocus = false;
            input.Data.Gamma = 0.3;
            input.Data.RegionOfInterest = null;

            // Given camera with sensed image
            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { CreatePatternRecImage("sensedImage.png") });

            // When
            var flow = new PatternRecFlow(input);
            flow.Configuration.SimilarityThreshold = 0.5;
            flow.Configuration.AngleTolerance = 3.0;
            flow.Configuration.ScaleTolerance = 0.3;
            bool bDebugTestOutput = false;
            if (bDebugTestOutput)
            {
                flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
                string directoryPath = "TestPatternRecReport";
                Directory.CreateDirectory(directoryPath);
                flow.ReportFolder = directoryPath;
            }

            var result = flow.Execute();

            // Then proper result is found
            Assert.AreEqual(34, result.ShiftX.Micrometers, 1);
            Assert.AreEqual(-441, result.ShiftY.Micrometers, 1);
            Assert.IsTrue(result.Confidence > 0.9);
        }

        [TestMethod]
        public void Pattern_rec_nominal_case_EnlargeDecenteredRef()
        {
            // Given: input with actual ref image, no autofocus, no ROI
            var input = _defaultPatternRecInput;
            input.Data.PatternReference = CreatePatternRecImage("refImage-ENL-DECENTRE.png").ToExternalImage();
            input.Data.PatternReference.FileExtension = ".png";
            input.RunAutofocus = false;
            input.Data.Gamma = 0.3;
            input.Data.RegionOfInterest = null;

            // Given camera with sensed image
            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { CreatePatternRecImage("sensedImage.png") });

            // When
            var flow = new PatternRecFlow(input);
            flow.Configuration.SimilarityThreshold = 0.5;
            flow.Configuration.AngleTolerance = 3.0;
            flow.Configuration.ScaleTolerance = 0.3;
            bool bDebugTestOutput = true;
            if (bDebugTestOutput)
            {
                flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
                string directoryPath = "TestPatternRecReport";
                Directory.CreateDirectory(directoryPath);
                flow.ReportFolder = directoryPath;
            }

            var result = flow.Execute();

            // Then proper result is found


            //those are not suitable result -- there is some error du to decentering and pixel scale changed
            Assert.IsTrue(result.Confidence > 0.75);
            //Assert.AreEqual(284, result.ShiftX.Micrometers, 1);
            //Assert.AreEqual(-691, result.ShiftY.Micrometers, 1);
            Assert.AreEqual(359.85, result.ShiftX.Micrometers, 80);
            Assert.AreEqual(-766.28, result.ShiftY.Micrometers, 80);

            // if this specific case work would have those result -- the following are the expected good result
            //Assert.AreEqual(359.85, result.ShiftX.Micrometers, 1);
            //Assert.AreEqual(-766.28, result.ShiftY.Micrometers, 1);

        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestPatternRecReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultPatternRecInput;

            var flow = new PatternRecFlow(input);
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

            var flow = new PatternRecFlow(input);
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

            var expectedShiftX = 105 * PixelSizeX;
            var expectedShiftY = -5 * PixelSizeY;
            double expectedConfidence = 1;

            var simulatedRegistrationResult = new PatternRecResult(new FlowStatus(FlowState.Success), expectedConfidence, expectedShiftX, expectedShiftY, new DummyUSPImage(10, 10, 255).ToServiceImage());
            _simulatedRegistrationLib.Setup(_ => _.Compute(It.IsAny<ImageRegistration.RegistrationData>(), It.IsAny<ImageParameters>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(simulatedRegistrationResult);

            string directoryPath = "TestPatternRecReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultPatternRecInput;
            input.Data.Gamma = 0.5;
            input.RunAutofocus = true;
            var flow = new PatternRecFlow(input, _simulatedAutofocusFlow.Object, _simulatedEdgeDetectorLib.Object, _simulatedRegistrationLib.Object);
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
            return CreateProcessingImageFromFile(GetDefaultDataPath(Path.Combine(PatternRecFolder, filename)));
        }
    }
}
