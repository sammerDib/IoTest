using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Service.Core.PSI;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.CameraTestUtils;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class PSITest : TestWithMockedHardware<PatternRecTest>, ITestWithAxes, ITestWithCamera
    {
        private Mock<USPCameraImageTracker> _simulatedCameraImageTracker;
        private Mock<PiezoController> _simulatedPiezoController;

        private PSIInput _defaultPSIInput;

        private const string PiezoAxisID = "PiezoAxisID";
        private AxisConfig _piezoAxisConfig;
        private const int StepNb = 7;
        private const int ImgPerStep = 1;
        private readonly Length _stepSize = 0.Millimeters();
        private int _nbExpectedPiezoGetCurrentPosition;
        private int _nbExpectedImageTrackerGetImages;
        private int _nbExpectedImageTrackingUntil;

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

        #region Post Generic Setup

        protected override void PostGenericSetup()
        {
            // Do not change the order in which functions are called
            SetupCameraImages();
            SetupPiezoController();
            SetupPiezoAxis();
            SetupPiezoGetCurrentPositionSequenceCall();
            SetupObjective();
            SetupPSIInput();
        }

        private void SetupCameraImages()
        {
            _simulatedCameraImageTracker = new Mock<USPCameraImageTracker>(SimulatedCameraUp.Object, null) { CallBase = false };

            var images = new List<USPImage>();

            string[] files = Directory.GetFiles(GetDefaultDataPath("psi"), "*.png");
            foreach (var file in files)
            {
                var img = new ServiceImage();
                img.LoadFromFile(file);
                var dummyImg = new DummyUSPImage(img);
                images.Add(dummyImg);
            }

            var imageTrackerSequence = _simulatedCameraImageTracker.SetupSequence(_ => _.Images);
            for (int stepIdx = 0; stepIdx < StepNb; stepIdx++)
            {
                var stepImgs = images.GetRange(0, ImgPerStep);
                stepImgs.Reverse();
                imageTrackerSequence.Returns(new ConcurrentBag<USPImage>(stepImgs));
            }

            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { new DummyUSPImage(10, 10, 255) });

            _nbExpectedImageTrackerGetImages = StepNb;
            _nbExpectedImageTrackingUntil = StepNb;
        }

        private void SetupPiezoController()
        {
            var piezoControllerConfig = new PiezoControllerConfig()
            {
                Name = "PiezoAxis",
                ControllerTypeName = "PiezoAxisConfig",
                DeviceID = ObjectiveUpId,
                PiezoAxisIDs = new List<string>() { PiezoAxisID }
            };
            _simulatedPiezoController = new Mock<PiezoController>(piezoControllerConfig) { CallBase = false };
            HardwareManager.Controllers.Add(ObjectiveUpId, _simulatedPiezoController.Object);
        }

        private void SetupPiezoAxis()
        {
            _piezoAxisConfig = new AxisConfig()
            {
                AxisID = "PiezoAxisID",
                PositionMax = 100.Micrometers(),
                PositionMin = 0.Millimeters(),
                PositionHome = 50.Micrometers(),
            };
            var piezoAxis = new ACSAxis(_piezoAxisConfig, null);
            _simulatedPiezoController.Setup(_ => _.AxesList).Returns(new List<IAxis>() { piezoAxis });
        }

        private void SetupPiezoGetCurrentPositionSequenceCall()
        {
            var piezoCurPosSeq = _simulatedPiezoController.SetupSequence(_ => _.GetCurrentPosition());
            piezoCurPosSeq.Returns(_piezoAxisConfig.PositionHome);    // PSIFlow : First call to retrieve initial piezo position
            for (int i = 0; i < StepNb; i++)
            {
                piezoCurPosSeq.Returns(_piezoAxisConfig.PositionHome - (i * _stepSize)); // PSIFlow : Piezo position for each image acquisition
            }
            piezoCurPosSeq.Returns(_piezoAxisConfig.PositionHome);    // PSIFlow : Restore piezo position at the end
            _nbExpectedPiezoGetCurrentPosition = StepNb + 2;
        }

        private void SetupObjective()
        {
            var objectiveSelectorConfig = new SingleObjectiveSelectorConfig();
            var objectiveConfig = new ObjectiveConfig()
            {
                DepthOfField = 0.01.Millimeters(),
                DeviceID = ObjectiveUpId,
                PiezoAxisID = PiezoAxisID
            };
            objectiveSelectorConfig.Objectives = new List<ObjectiveConfig>() { objectiveConfig };
            objectiveSelectorConfig.DeviceID = ObjectiveUpId;
            var hardwareLogger = Mock.Of<IHardwareLogger>();
            var simulatedObjectiveSelector = new Mock<SingleObjectiveSelector>(objectiveSelectorConfig, hardwareLogger);
            HardwareManager.ObjectivesSelectors.Clear();
            HardwareManager.ObjectivesSelectors[ObjectiveUpId] = simulatedObjectiveSelector.Object;
        }

        private void SetupPSIInput()
        {
            _defaultPSIInput = SimulatedData.ValidPSIInput();
            _defaultPSIInput.CameraId = CameraUpId;
            _defaultPSIInput.ObjectiveId = ObjectiveUpId;
            _defaultPSIInput.StepCount = StepNb;
            _defaultPSIInput.ImagesPerStep = ImgPerStep;
            _defaultPSIInput.StepSize = _stepSize;
        }

        #endregion Post Generic Setup

        private void VerifySequenceAndNbExpectedCalls()
        {
            _simulatedPiezoController.VerifyAll();
            _simulatedPiezoController.Verify(_ => _.GetCurrentPosition(), Times.Exactly(_nbExpectedPiezoGetCurrentPosition));

            _simulatedCameraImageTracker.VerifyAll();
            _simulatedCameraImageTracker.Verify(_ => _.Images, Times.Exactly(_nbExpectedImageTrackerGetImages));
            _simulatedCameraImageTracker.Verify(_ => _.StartTrackingUntil(ImgPerStep, It.IsAny<int>()), Times.Exactly(_nbExpectedImageTrackingUntil));

            SimulatedAxes.Verify(_ => _.Land(), Times.Once());

            // PSIFlow is supposed to restore piezoPostion, camera AOI and axes Landing
            _simulatedPiezoController.Verify(
                _ => _.SetPosAxisWithSpeedAndAccel(
                new List<double>() { _piezoAxisConfig.PositionHome.Millimeters },
                It.IsAny<List<IAxis>>(),
                It.IsAny<List<double>>(),
                It.IsAny<List<double>>()), Times.AtLeastOnce());

            SimulatedCameraUp.Verify(_ => _.SetAOI(It.IsAny<Rect>()), Times.AtLeastOnce());
            SimulatedAxes.Verify(_ => _.StopLanding(), Times.Once());
        }

        [TestMethod]
        public void Psi_flow_nominal_case()
        {
            //Given
            var flow = new PSIFlow(_defaultPSIInput, _simulatedCameraImageTracker.Object, null);

            //When: Run PSI
            flow.Execute();

            //Then
            Assert.AreEqual(FlowState.Success, flow.Result.Status.State);
            Assert.IsNotNull(flow.Result.NanoTopographyImage);
            Assert.AreEqual(ServiceImage.ImageType._3DA, flow.Result.NanoTopographyImage.Type);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Psi_flow_set_and_restore_camera_aoi_when_roi_is_given()
        {
            //Given
            _defaultPSIInput.ROI = new CenteredRegionOfInterest(100.Micrometers(), 100.Micrometers());
            var flow = new PSIFlow(_defaultPSIInput, _simulatedCameraImageTracker.Object, null);

            //When: Run PSI
            flow.Execute();

            //Then
            Assert.AreEqual(FlowState.Success, flow.Result.Status.State);
            Assert.IsNotNull(flow.Result.NanoTopographyImage);
            Assert.AreEqual(ServiceImage.ImageType._3DA, flow.Result.NanoTopographyImage.Type);
            SimulatedCameraUp.Verify(_ => _.SetAOI(It.IsAny<Rect>()), Times.Exactly(2));
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestPSIReport";
            Directory.CreateDirectory(directoryPath);

            var flow = new PSIFlow(_defaultPSIInput, _simulatedCameraImageTracker.Object, null);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run PSI
            flow.Execute();

            //Then
            var filename = Path.Combine(flow.ReportFolder, $"input.txt");
            Assert.IsTrue(File.Exists(filename));
            VerifySequenceAndNbExpectedCalls();

            Directory.Delete(flow.ReportFolder, true);
        }

        [TestMethod]
        public void Report_of_result_is_working()
        {
            //Given
            string directoryPath = "TestPSIReport";
            Directory.CreateDirectory(directoryPath);

            var flow = new PSIFlow(_defaultPSIInput, _simulatedCameraImageTracker.Object, null);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run PSI
            flow.Execute();

            //Then
            var status = (flow.Result == null) ? "null" : ((flow.Result.Status == null) ? "ukn" : flow.Result.Status.State.ToString());
            var filename = Path.Combine(flow.ReportFolder, $"result_{status}.txt");
            Assert.IsTrue(File.Exists(filename));
            VerifySequenceAndNbExpectedCalls();

            Directory.Delete(flow.ReportFolder, true);
        }

        [TestMethod]
        public void Report_of_images_is_working()
        {
            // Given
            string directoryPath = "TestPSIReport";
            Directory.CreateDirectory(directoryPath);

            var flow = new PSIFlow(_defaultPSIInput, _simulatedCameraImageTracker.Object, null);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run PSI
            flow.Execute();

            // Then
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"NanoTopography_image.png")));
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"normalized_NanoTopography_image.png")));
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"NanoTopography_image.3da")));
            VerifySequenceAndNbExpectedCalls();

            Directory.Delete(directoryPath, true);
        }

        [TestMethod]
        public void Psi_flow_creation_throw_when_camera_does_not_exists()
        {
            // Given
            _defaultPSIInput.CameraId = "No";

            //When and Then
            Assert.ThrowsException<InvalidOperationException>(() => new PSIFlow(_defaultPSIInput));
        }

        [TestMethod]
        public void Psi_flow_creation_throw_when_objective_does_not_exists()
        {
            // Given
            _defaultPSIInput.ObjectiveId = "No";

            //When and Then
            Assert.ThrowsException<InvalidOperationException>(() => new PSIFlow(_defaultPSIInput));
        }
    }
}
