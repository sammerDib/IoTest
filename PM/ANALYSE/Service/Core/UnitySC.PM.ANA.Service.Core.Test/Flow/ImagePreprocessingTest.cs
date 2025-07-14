using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class ImagePreprocessingTest : TestWithMockedHardware<ImagePreprocessingTest>, ITestWithCamera, ITestWithAxes
    {
        private Mock<EdgeDetection> _simulatedEdgeDetectorLib;

        private ImagePreprocessingInput _defaultImagePreprocessingInput;

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

        protected override void PostGenericSetup()
        {
            _simulatedEdgeDetectorLib = new Mock<EdgeDetection> { CallBase = true };
            _simulatedEdgeDetectorLib.Setup(_ => _.Compute(It.IsAny<ServiceImage>(), It.IsAny<double>(), It.IsAny<RegionOfInterest>(), It.IsAny<ImageParameters>(), It.IsAny<bool>())).Returns(new ServiceImage());

            _defaultImagePreprocessingInput = SimulatedData.ValidImagePreprocessingInput();
            _defaultImagePreprocessingInput.CameraId = CameraUpId;
            _defaultImagePreprocessingInput.RegionOfInterest = new RegionOfInterest(0.Millimeters(), 0.Millimeters(), 100.Micrometers(), 100.Micrometers(), 0.Millimeters(), 0.Millimeters());

            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { new DummyUSPImage(100, 100, 255) });
        }

        [TestMethod]
        public void Image_preprocessing_flow_nominal_case()
        {
            // Given
            int imageHeight = 100;
            int imageWidth = 150;
            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { new DummyUSPImage(imageWidth, imageHeight, 255) });

            var input = _defaultImagePreprocessingInput;
            var flow = new ImagePreprocessingFlow(input);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(imageHeight, result.PreprocessedImage.DataHeight);
            Assert.AreEqual(imageWidth, result.PreprocessedImage.DataWidth);
        }

        [TestMethod]
        public void Preprocessing_is_computed()
        {
            // Given
            var input = _defaultImagePreprocessingInput;
            input.Gamma = 0.5;
            var flow = new ImagePreprocessingFlow(input, _simulatedEdgeDetectorLib.Object);

            // When
            var result = flow.Execute();

            // Then
            _simulatedEdgeDetectorLib.Verify(x => x.Compute(It.IsAny<ServiceImage>(), It.IsAny<double>(), It.IsAny<RegionOfInterest>(), It.IsAny<ImageParameters>(), It.IsAny<bool>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestImagePreprocessingReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultImagePreprocessingInput;

            var flow = new ImagePreprocessingFlow(input);
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
            string directoryPath = "TestImagePreprocessingReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultImagePreprocessingInput;

            var flow = new ImagePreprocessingFlow(input);
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
            var input = _defaultImagePreprocessingInput;
            var flow = new ImagePreprocessingFlow(input);

            string directoryPath = "TestImagePreprocessingReport";
            Directory.CreateDirectory(directoryPath);

            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            // When
            var result = flow.Execute();

            // Then
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"initial_image_csharp.png")));
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"preprocessed_image_with_gamma_{input.Gamma}_csharp.png")));

            Directory.Delete(directoryPath, true);
        }
    }
}
