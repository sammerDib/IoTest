using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Service.Core.VSI;
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
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.CameraTestUtils;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class VSITest : TestWithMockedHardware<PatternRecTest>, ITestWithAxes, ITestWithCamera
    {
        private Mock<PiezoController> _simulatedPiezoController;
        private Mock<IReferentialManager> _simulatedReferentialManager;

        private VSIInput _defaultVSIInput;

        private const string PiezoAxisID = "PiezoAxisID";
        private readonly Length _realStepSize = 50.Nanometers();    // Represent real step size in test images data
        private AxisConfig _piezoAxisConfig;
        private int _stepNb;
        private Length _stepSize;
        private int _nbExpectedPiezoGetCurrentPosition;
        private int _nbExpectedGetNextCameraImageCall;

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
            SetupVSIInput();
            SetupReferentialManager();
        }

        private void SetupReferentialManager()
        {
            _simulatedReferentialManager = Bootstrapper.SimulatedReferentialManager;
            _simulatedReferentialManager.Setup(_ => _.ConvertTo(It.IsAny<XYZTopZBottomPosition>(), It.IsAny<ReferentialTag>())).Returns(_defaultVSIInput.StartPosition);
        }

        private void SetupCameraImages()
        {
            // Use a stepSize two time greater than real step size two reduce computation times.
            // We will then use the half of the total images.
            _stepSize = _realStepSize * 2;

            var images = new List<DummyUSPImage>();
            string[] files = Directory.GetFiles(GetDataPathInSharedAlgos("VSI\\Tests\\Data\\VSI_crop"), "*.png");
            int i = 0;
            foreach (var file in files)
            {
                if (i % (_stepSize.Nanometers / _realStepSize.Nanometers) == 0)
                {
                    var img = new ServiceImage();
                    img.LoadFromFile(file);
                    var dummyImg = new DummyUSPImage(img);
                    images.Add(dummyImg);
                }
                i++;
            }
            TestWithCameraHelper.SetupCameraWithImages(images);
            _stepNb = images.Count;
            _nbExpectedGetNextCameraImageCall = _stepNb;
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
            piezoCurPosSeq.Returns(_piezoAxisConfig.PositionHome);    // VSIFlow : First call to retrieve initial piezo position
            piezoCurPosSeq.Returns(_piezoAxisConfig.PositionMax);     // VSIFlow : Image acquisition starting position
            for (int i = 0; i < _stepNb; i++)
            {
                piezoCurPosSeq.Returns(_piezoAxisConfig.PositionMax - (i * _stepSize)); // VSIFlow : Piezo position for each image acquisition
                _nbExpectedPiezoGetCurrentPosition++;
            }
            piezoCurPosSeq.Returns(_piezoAxisConfig.PositionHome);    // VSIFlow : Restore piezo position at the end
            _nbExpectedPiezoGetCurrentPosition = 3 + _stepNb;
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

        private void SetupVSIInput()
        {
            _defaultVSIInput = SimulatedData.ValidVSIInput();
            _defaultVSIInput.CameraId = CameraUpId;
            _defaultVSIInput.ObjectiveId = ObjectiveUpId;
            _defaultVSIInput.StepCount = _stepNb;
            _defaultVSIInput.StepSize = _stepSize;
        }

        #endregion Post Generic Setup

        private void VerifySequenceAndNbExpectedCalls()
        {
            _simulatedPiezoController.VerifyAll();
            _simulatedPiezoController.Verify(_ => _.GetCurrentPosition(), Times.Exactly(_nbExpectedPiezoGetCurrentPosition));
            Bootstrapper.SimulatedCameraManager.VerifyAll();
            Bootstrapper.SimulatedCameraManager.Verify(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()), Times.Exactly(_nbExpectedGetNextCameraImageCall));

            SimulatedAxes.Verify(_ => _.Land(), Times.Once());

            // VSIFlow is supposed to restore piezoPostion, camera AOI and axes Landing
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
        public void Vsi_flow_nominal_case()
        {
            //Given
            var flow = new VSIFlow(_defaultVSIInput);

            //When: Run VSI
            flow.Execute();

            //Then
            Assert.AreEqual(FlowState.Success, flow.Result.Status.State);
            Assert.IsNotNull(flow.Result.TopographyImage);
            Assert.AreEqual(ServiceImage.ImageType._3DA, flow.Result.TopographyImage.Type);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Vsi_flow_set_and_restore_camera_aoi_when_roi_is_given()
        {
            //Given
            _defaultVSIInput.ROI = new CenteredRegionOfInterest(100.Micrometers(), 100.Micrometers());
            var flow = new VSIFlow(_defaultVSIInput);

            //When: Run VSI
            flow.Execute();

            //Then
            Assert.AreEqual(FlowState.Success, flow.Result.Status.State);
            Assert.IsNotNull(flow.Result.TopographyImage);
            Assert.AreEqual(ServiceImage.ImageType._3DA, flow.Result.TopographyImage.Type);
            SimulatedCameraUp.Verify(_ => _.SetAOI(It.IsAny<Rect>()), Times.Exactly(2));
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestVSIReport";
            Directory.CreateDirectory(directoryPath);

            var flow = new VSIFlow(_defaultVSIInput);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run VSI
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
            string directoryPath = "TestVSIReport";
            Directory.CreateDirectory(directoryPath);

            var flow = new VSIFlow(_defaultVSIInput);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run VSI
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
            string directoryPath = "TestVSIReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultVSIInput;

            var flow = new VSIFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            // When: Run VSI
            flow.Execute();

            // Then
            Assert.IsTrue(File.Exists(Path.Combine(flow.ReportFolder, $"Topography_image.3da")));
            VerifySequenceAndNbExpectedCalls();

            Directory.Delete(directoryPath, true);
        }

        [TestMethod]
        public void Vsi_flow_creation_throw_when_camera_does_not_exists()
        {
            // Given
            _defaultVSIInput.CameraId = "No";

            //When and Then
            Assert.ThrowsException<InvalidOperationException>(() => new VSIFlow(_defaultVSIInput));
        }

        [TestMethod]
        public void Vsi_flow_creation_throw_when_objective_does_not_exists()
        {
            // Given
            _defaultVSIInput.ObjectiveId = "No";

            //When and Then
            Assert.ThrowsException<InvalidOperationException>(() => new VSIFlow(_defaultVSIInput));
        }
    }
}
