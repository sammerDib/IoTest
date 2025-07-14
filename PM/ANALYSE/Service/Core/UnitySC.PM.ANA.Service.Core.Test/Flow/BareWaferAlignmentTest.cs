using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.CameraTestUtils;

using BareWaferAlignmentResult = UnitySC.PM.ANA.Service.Interface.Algo.BareWaferAlignmentResult;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class BareWaferAlignmentTest : WaferAlignmentTestBase, ITestWithChuck, ITestWithLight
    {
        public Mock<ITestChuck> SimulatedChuck { get; set; }
        public string LightId { get; set; }
        public Mock<LightBase> SimulatedLight { get; set; }

        [TestMethod]
        public void Expect_BWA_flow_to_use_provided_images_for_left_and_right()
        {
            setupLights();
            // Given: Left and right images are provided
            var waferCharacteristics = new WaferDimensionalCharacteristic { Diameter = 300.Millimeters(), WaferShape = WaferShape.Notch };
           

            var imagePositions = ImageSetCentroidFactory.GetImageDataListFor(waferCharacteristics);
            int nbStitchedNotchImages = imagePositions[3].StitchColumns * imagePositions[3].StitchRows;

            SetupCameraWithImagesForBWA300Notched(nbStitchedNotchImages);

            var right = CreateProcessingImageFromFile(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_1_2X_VIS_X_-26047_Y_-147721.png"));
            var left = CreateProcessingImageFromFile(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_3_2X_VIS_X_139077_Y_56190.png"));
            var flowInput = new BareWaferAlignmentInput(waferCharacteristics, CameraUpId)
            {
                ImageLeft = new ServiceImageWithPosition
                {
                    CenterPosition = new XYZTopZBottomPosition(new StageReferential(), 1, 1, 0, 0),
                    Image = left.ToServiceImage(), // CameraService.ConvertImage(left),
                },
                ImageRight = new ServiceImageWithPosition
                {
                    CenterPosition = new XYZTopZBottomPosition(new StageReferential(), 3, 3, 0, 0),
                    Image = right.ToServiceImage(), // CameraService.ConvertImage(right),
                },
            };

            var flow = new BareWaferAlignmentFlow(flowInput);

            // When: execute BWA
            setupAxisConfigs();

            flow.Execute();

            // Then : BWA captured pictures only for top and bottom (moves to specified locations and takes a picture)
            // Positions need to be in mm instead of µm as used in BWA flow.
            var stageReferential = new StageReferential();
            var topPosition = new XYZTopZBottomPosition(stageReferential,       TestWithChuckHelper.XChuckOffset_mm, TestWithChuckHelper.YChuckOffset_mm - 150.0, 0.0, 0.0);
            var bottomPosition = new XYZTopZBottomPosition(stageReferential,    TestWithChuckHelper.XChuckOffset_mm, TestWithChuckHelper.YChuckOffset_mm + 150.0, 0.0, 0.0);
            var leftPosition = new XYZTopZBottomPosition(stageReferential,      TestWithChuckHelper.XChuckOffset_mm - 150.0, TestWithChuckHelper.YChuckOffset_mm, 0.0, 0.0);
            var rightPosition = new XYZTopZBottomPosition(stageReferential,     TestWithChuckHelper.XChuckOffset_mm + 150.0, TestWithChuckHelper.YChuckOffset_mm, 0.0, 0.0);

            Bootstrapper.SimulatedCameraManager.Verify(_ => _.GetNextCameraImage(It.Is<CameraBase>(camera => camera.Name == "cameraUp")), Times.Exactly(5));
            SimulatedAxes.Verify(_ => _.GotoPosition(topPosition.ToXYZTopZBottomPosition(), AxisSpeed.Normal), Times.Exactly(4));
            SimulatedAxes.Verify(_ => _.GotoPosition(bottomPosition.ToXYZTopZBottomPosition(), AxisSpeed.Normal), Times.Once());
            SimulatedAxes.Verify(_ => _.GotoPosition(leftPosition.ToXYZTopZBottomPosition(), AxisSpeed.Normal), Times.Never());
            SimulatedAxes.Verify(_ => _.GotoPosition(rightPosition.ToXYZTopZBottomPosition(), AxisSpeed.Normal), Times.Never());
        }

        [TestMethod]
        public void Expect_BWA_flow_to_provide_consistent_results_with_preacquired_image()
        {
            setupLights();
            // Given
            // images provided for test are not LEFT,TOP,RIGHT,BOTTOM but comes from FMPS which use different location.
            // To ensure consistency between test images and location data, we enforce image centroid coordinates,
            // providing images and their centroid as if user manually acquire them, by using algorithm input.

            double expectedWaferShiftX = 111;
            double expectedWaferShiftY = 246;
            double expectedShiftXInMicrons = expectedWaferShiftX * PixelSizeX.Micrometers - TestWithChuckHelper.XChuckOffset_mm * 1000.0;
            double expectedShiftYInMicrons = expectedWaferShiftY * PixelSizeY.Micrometers - TestWithChuckHelper.YChuckOffset_mm * 1000.0;
            double expectedShiftStageXInMicrons = expectedWaferShiftX * PixelSizeX.Micrometers;
            double expectedShiftStageYInMicrons = expectedWaferShiftY * PixelSizeY.Micrometers;
            double expectedAngleInDegrees = -0.78;
            double expectedAngleInRadians = expectedAngleInDegrees * Math.PI / 180;

            var waferCharacteristics = new WaferDimensionalCharacteristic { Diameter = 300.Millimeters(), WaferShape = WaferShape.Notch };
            var imagePositions = ImageSetCentroidFactory.GetImageDataListFor(waferCharacteristics);
            int nbStitchedNotchImages = imagePositions[3].StitchColumns * imagePositions[3].StitchRows;

            SetupCameraWithImagesForBWA300Notched(nbStitchedNotchImages);

            var left = new DummyUSPImage(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_1_2X_VIS_X_-26047_Y_-147721.png"));
            var top = new DummyUSPImage(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_2_2X_VIS_X_23465_Y_148153.png"));
            var right = new DummyUSPImage(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_3_2X_VIS_X_139077_Y_56190.png"));
            var bottom = new DummyUSPImage(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_0_2X_VIS_X_0_Y_-150000.png"));

            var flowInput = new BareWaferAlignmentInput(waferCharacteristics, CameraUpId)
            {
                ImageBottom = new ServiceImageWithPosition
                {
                    CenterPosition = new XYZTopZBottomPosition(new StageReferential(), 0, -150000 / 1000.0, 0, 0),
                    Image = bottom.ToServiceImage(), //CameraService.ConvertImage(bottom),
                },
                ImageLeft = new ServiceImageWithPosition
                {
                    CenterPosition = new XYZTopZBottomPosition(new StageReferential(), -26047 / 1000.0, -147721 / 1000.0, 0, 0),
                    Image = left.ToServiceImage(), //CameraService.ConvertImage(left),
                },
                ImageRight = new ServiceImageWithPosition
                {
                    CenterPosition = new XYZTopZBottomPosition(new StageReferential(), 139077 / 1000.0, 56190 / 1000.0, 0, 0),
                    Image = right.ToServiceImage(), //CameraService.ConvertImage(right),
                },
                ImageTop = new ServiceImageWithPosition
                {
                    CenterPosition = new XYZTopZBottomPosition(new StageReferential(), 23465 / 1000.0, 148153 / 1000.0, 0, 0),
                    Image = top.ToServiceImage(), //CameraService.ConvertImage(top),
                },
            };

            // Given chuck to clamp wafer.
            if (this is ITestWithChuck testWithChuck)
                TestWithChuckHelper.Setup(testWithChuck);
            PostGenericSetup();
            var flow = new BareWaferAlignmentFlow(flowInput);
            // When

            var result = flow.Execute();

            // Then

            Assert.AreEqual(FlowState.Success, result.Status.State, result.Status.Message);
            Assert.IsTrue(result is BareWaferAlignmentResult);

            double rotationToleranceInRadians = 0.001;
            double shiftToleranceInMicrons = 10 * PixelSizeX.Micrometers;
            Assert.AreEqual(expectedShiftXInMicrons, (result as BareWaferAlignmentResult).ShiftX.Micrometers, shiftToleranceInMicrons, "X shift does not match expected value");
            Assert.AreEqual(expectedShiftYInMicrons, (result as BareWaferAlignmentResult).ShiftY.Value, shiftToleranceInMicrons, "Y shift does not match expected value");
            Assert.AreEqual(expectedShiftStageXInMicrons, (result as BareWaferAlignmentResult).ShiftStageX.Micrometers, shiftToleranceInMicrons, "X shift STAGE does not match expected value");
            Assert.AreEqual(expectedShiftStageYInMicrons, (result as BareWaferAlignmentResult).ShiftStageY.Value, shiftToleranceInMicrons, "Y shift STAGE does not match expected value");
            Assert.AreEqual(expectedAngleInRadians, (result as BareWaferAlignmentResult).Angle.Radians, rotationToleranceInRadians, "rotation angle does not match expected value");
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            setupLights();
            //Given
            string directoryPath = "TestBWAReport";
            Directory.CreateDirectory(directoryPath);

            var waferCharacteristics = new WaferDimensionalCharacteristic { Diameter = 300.Millimeters(), WaferShape = WaferShape.Notch };
            var input = new BareWaferAlignmentInput(waferCharacteristics, CameraUpId);

            var flow = new BareWaferAlignmentFlow(input);
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
            setupLights();
            //Given
            string directoryPath = "TestBWAReport";
            Directory.CreateDirectory(directoryPath);

            var waferCharacteristics = new WaferDimensionalCharacteristic { Diameter = 300.Millimeters(), WaferShape = WaferShape.Notch };
            var input = new BareWaferAlignmentInput(waferCharacteristics, CameraUpId);

            var flow = new BareWaferAlignmentFlow(input);
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
            setupLights();
            //Given
            string directoryPath = "TestBWAReport";
            Directory.CreateDirectory(directoryPath);

            var waferCharacteristics = new WaferDimensionalCharacteristic { Diameter = 300.Millimeters(), WaferShape = WaferShape.Notch };

            var imagePositions = ImageSetCentroidFactory.GetImageDataListFor(waferCharacteristics);
            int nbStitchedNotchImages = imagePositions[3].StitchColumns * imagePositions[3].StitchRows;

            SetupCameraWithImagesForBWA300Notched(nbStitchedNotchImages);

            var input = new BareWaferAlignmentInput(waferCharacteristics, CameraUpId);

            var flow = new BareWaferAlignmentFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var filenameLeftImg = Path.Combine(flow.ReportFolder, $"LEFT.jpg");
            var filenameRightImg = Path.Combine(flow.ReportFolder, $"RIGHT.jpg");
            var filenameTopImg = Path.Combine(flow.ReportFolder, $"TOP.jpg");
            var filenameBottomImg = Path.Combine(flow.ReportFolder, $"BOTTOM.jpg");

            Assert.IsTrue(File.Exists(filenameLeftImg));
            Assert.IsTrue(File.Exists(filenameRightImg));
            Assert.IsTrue(File.Exists(filenameTopImg));
            Assert.IsTrue(File.Exists(filenameBottomImg));

            Directory.Delete(flow.ReportFolder, true);
        }


        // TODO : prevoir un test untiaire avec des chuckcenter offest non null 

        protected override void PostGenericSetup()
        {
            setupAxisConfigs();
            setupChuck();
        }

        private void setupAxisConfigs()
        {
            // Setup Axis config
            var axesConfig = new AxesConfig()
            {
                AxisConfigs = new List<AxisConfig>()
                {
                    new AxisConfig()
                    {
                        MovingDirection = MovingDirection.ZBottom,
                        PositionMax = 2.9.Millimeters()
                    },
                    new AxisConfig()
                    {
                        MovingDirection = MovingDirection.ZTop,
                        PositionMax = 19.9.Millimeters()
                    }
                }
            };
            SimulatedAxes.Setup(a => a.AxesConfiguration).Returns(axesConfig);
        }

        private void setupChuck()
        {
            // By default, have a non open chuck with clamped wafer
            SimulatedChuck.Object.Configuration.IsOpenChuck = false;
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter),  MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));
        }

        private void setupLights()
        {
            SimulatedLight.Setup(_ => _.IsMainLight).Returns(true);
        }
    }
}
