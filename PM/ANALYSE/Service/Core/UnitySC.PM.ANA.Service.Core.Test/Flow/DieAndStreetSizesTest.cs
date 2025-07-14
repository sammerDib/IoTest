using System;
using System.Drawing;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.WaferMap;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class DieAndStreetSizesTest : TestWithMockedHardware<DieAndStreetSizesTest>, ITestWithAxes, ITestWithCamera
    {
        private Mock<PatternRecFlow> _simulatedPatternRecFlow;

        private DieAndStreetSizesInput _defaultDieAndStreetSizesInput;

        #region Interfaces properties

        public Mock<IAxes> SimulatedAxes { get; set; }
        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }

        #endregion Interfaces properties

        protected override void PostGenericSetup()
        {
            _simulatedPatternRecFlow = new Mock<PatternRecFlow>(new PatternRecInput(null), null, null, null);

            SimulatedCameraUp.Setup(_ => _.Width).Returns(1280);
            SimulatedCameraUp.Setup(_ => _.Height).Returns(1024);

            _defaultDieAndStreetSizesInput = SimulatedData.ValidDieAndStreetSizesInput();
            _defaultDieAndStreetSizesInput.CameraID = CameraUpId;
        }

        [TestMethod]
        public void Die_and_street_sizes_flow_nominal_case()
        {
            // Given
            double dieWidth = 7.778;
            double streetWidth = 0;
            double dieHeight = 6.36;
            double streetHeight = 0;

            var angle = -45.Degrees();

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 1, -10, 0, 0);
            input.Wafer.Diameter = 300.Millimeters();

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 90.0, 4.5.Millimeters(), -5.5.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 99.0, -4.5.Millimeters(), 5.5.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Success), 98.0, 5.5.Millimeters() - dieWidth.Millimeters(), -5.5.Millimeters(), null);
            var patternRecResultForWidthVerification = new PatternRecResult(new FlowStatus(FlowState.Success), 80.0, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForHeightVerification = new PatternRecResult(new FlowStatus(FlowState.Success), 85.0, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation)
                .Returns(patternRecResultForWidthVerification)
                .Returns(patternRecResultForHeightVerification);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            double expectedConfidence = (90.0 + 99.0 + 98.0 + 80.0 + 85.0) / 5;
            Assert.AreEqual(expectedConfidence, result.Confidence, 10e-1);
            Assert.AreEqual(angle.Degrees, result.DieDimensions.DieAngle.Degrees, 1);
            Assert.AreEqual(dieWidth, result.DieDimensions.DieWidth.Millimeters, 10e-1);
            Assert.AreEqual(dieHeight, result.DieDimensions.DieHeight.Millimeters, 10e-1);
            Assert.AreEqual(streetWidth, result.DieDimensions.StreetWidth.Millimeters, 10e-1);
            Assert.AreEqual(streetHeight, result.DieDimensions.StreetHeight.Millimeters, 10e-1);
        }

        [TestMethod]
        public void Die_dimensions_are_correct_when_dies_are_perfectly_aligned_on_wafer()
        {
            // Given
            var dieSize = new Size(7, 8);
            var streetSize = new Size(1, 3);

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), dieSize.Width, -dieSize.Height, 0, 0);

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, streetSize.Width.Millimeters(), 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), -streetSize.Height.Millimeters(), null);
            var patternRecResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResult)
                .Returns(patternRecResult)
                .Returns(patternRecResult);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(dieSize.Width, result.DieDimensions.DieWidth.Millimeters);
            Assert.AreEqual(dieSize.Height, result.DieDimensions.DieHeight.Millimeters);
            Assert.AreEqual(streetSize.Width, result.DieDimensions.StreetWidth.Millimeters);
            Assert.AreEqual(streetSize.Height, result.DieDimensions.StreetHeight.Millimeters);
        }

        [TestMethod]
        public void Angle_is_zero_correct_when_dies_are_perfectly_aligned_on_wafer()
        {
            // Given
            var dieSize = new Size(7, 8);
            var streetSize = new Size(1, 3);

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), dieSize.Width, -dieSize.Height, 0, 0);

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, streetSize.Width.Millimeters(), 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), -streetSize.Height.Millimeters(), null);
            var patternRecResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResult)
                .Returns(patternRecResult)
                .Returns(patternRecResult);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(0, result.DieDimensions.DieAngle.Degrees);
        }

        [TestMethod]
        public void Die_dimensions_are_correct_when_dies_are_not_aligned_on_wafer()
        {
            // Given
            double dieWidth = 7.778;
            double streetWidth = 0;
            double dieHeight = 6.36;
            double streetHeight = 0;

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 1, -10, 0, 0);
            input.Wafer.Diameter = 20.Millimeters();

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 4.5.Millimeters(), -5.5.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, -4.5.Millimeters(), 5.5.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 5.5.Millimeters() - dieWidth.Millimeters(), -5.5.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(dieWidth, result.DieDimensions.DieWidth.Millimeters, 10e-1);
            Assert.AreEqual(dieHeight, result.DieDimensions.DieHeight.Millimeters, 10e-1);
            Assert.AreEqual(streetWidth, result.DieDimensions.StreetWidth.Millimeters, 10e-1);
            Assert.AreEqual(streetHeight, result.DieDimensions.StreetHeight.Millimeters, 10e-1);
        }

        [TestMethod]
        public void Angle_is_correct_when_dies_are_not_aligned_on_wafer()
        {
            // Given
            double dieWidth = 7.778;
            var angle = -45.Degrees();

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 1, -10, 0, 0);
            input.Wafer.Diameter = 20.Millimeters();

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 4.5.Millimeters(), -5.5.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, -4.5.Millimeters(), 5.5.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 5.5.Millimeters() - dieWidth.Millimeters(), -5.5.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(angle.Degrees, result.DieDimensions.DieAngle.Degrees, 1);
        }

        [TestMethod]
        public void Confidence_is_the_mean_of_all_pattern_recognition_results_when_there_is_no_verification_step()
        {
            // Given
            var input = _defaultDieAndStreetSizesInput;

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 0.90, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 0.99, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Success), 0.80, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            double expectedConfidence = (0.90 + 0.99 + 0.80) / 3;
            Assert.AreEqual(expectedConfidence, result.Confidence);
        }

        [TestMethod]
        public void Die_dimensions_are_correct_when_width_street_size_is_greater_than_camera_image_width()
        {
            // Given
            var dieSize = new Size(11, 11);
            var finalShiftX = 0.8.Millimeters();
            var finalShiftY = -0.4.Millimeters();

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), dieSize.Width, -dieSize.Height, 0, 0);

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, finalShiftX, 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), finalShiftY, null);
            var patternRecResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Throws(new Exception("pattern not found")) // stepX = 1
                .Throws(new Exception("pattern not found")) // stepX = 2
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResult)
                .Returns(patternRecResult)
                .Returns(patternRecResult);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            var cameraImageWidth = SimulatedCameraUp.Object.Width * PixelSizeX.Millimeters;
            var overlap = dieAndStreetSizes.Configuration.OverlapForNextDieResearch;
            var stepX = 2;
            var expectedStreetSizeX = Math.Abs(((cameraImageWidth * (1 - overlap)) * stepX) + finalShiftX.Millimeters);
            var expectedStreetSizeY = Math.Abs(finalShiftY.Millimeters);
            Assert.AreEqual(dieSize.Width, result.DieDimensions.DieWidth.Millimeters);
            Assert.AreEqual(dieSize.Height, result.DieDimensions.DieHeight.Millimeters);
            Assert.AreEqual(expectedStreetSizeX, result.DieDimensions.StreetWidth.Millimeters, 1E-3);
            Assert.AreEqual(expectedStreetSizeY, result.DieDimensions.StreetHeight.Millimeters, 1E-3);
        }

        [TestMethod]
        public void Die_dimensions_are_correct_when_width_street_size_is_greater_than_camera_image_height()
        {
            // Given
            var dieSize = new Size(11, 11);
            var finalShiftX = 0.8.Millimeters();
            var finalShiftY = -0.4.Millimeters();

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), dieSize.Width, -dieSize.Height, 0, 0);

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, finalShiftX, 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), finalShiftY, null);
            var patternRecResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Throws(new Exception("pattern not found")) // stepY = 1
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResult)
                .Returns(patternRecResult)
                .Returns(patternRecResult);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            var cameraImageHeight = SimulatedCameraUp.Object.Height * PixelSizeY.Millimeters;
            var overlap = dieAndStreetSizes.Configuration.OverlapForNextDieResearch;
            var stepY = 1;
            var expectedStreetSizeX = Math.Abs(finalShiftX.Millimeters);
            var expectedStreetSizeY = Math.Abs(-((cameraImageHeight * (1 - overlap)) * stepY) + finalShiftY.Millimeters);
            Assert.AreEqual(dieSize.Width, result.DieDimensions.DieWidth.Millimeters);
            Assert.AreEqual(dieSize.Height, result.DieDimensions.DieHeight.Millimeters);
            Assert.AreEqual(expectedStreetSizeX, result.DieDimensions.StreetWidth.Millimeters, 1E-3);
            Assert.AreEqual(expectedStreetSizeY, result.DieDimensions.StreetHeight.Millimeters, 1E-3);
        }

        [TestMethod]
        public void Die_dimensions_are_correct_when_width_street_size_is_greater_than_camera_image_width_and_height()
        {
            // Given
            var dieSize = new Size(11, 11);
            var finalShiftX = 0.8.Millimeters();
            var finalShiftY = -0.4.Millimeters();

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), dieSize.Width, -dieSize.Height, 0, 0);

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, finalShiftX, 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), finalShiftY, null);
            var patternRecResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Throws(new Exception("pattern not found")) // stepX = 1
                .Returns(patternRecResultAtRight)
                .Throws(new Exception("pattern not found")) // stepY = 1
                .Throws(new Exception("pattern not found")) // stepY = 2
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResult)
                .Returns(patternRecResult)
                .Returns(patternRecResult);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            var cameraImageWidth = SimulatedCameraUp.Object.Width * PixelSizeX.Millimeters;
            var cameraImageHeight = SimulatedCameraUp.Object.Height * PixelSizeY.Millimeters;
            var overlap = dieAndStreetSizes.Configuration.OverlapForNextDieResearch;
            var stepX = 1;
            var stepY = 2;
            var expectedStreetSizeX = Math.Abs(((cameraImageWidth * (1 - overlap)) * stepX) + finalShiftX.Millimeters);
            var expectedStreetSizeY = Math.Abs(-((cameraImageHeight * (1 - overlap)) * stepY) + finalShiftY.Millimeters);
            Assert.AreEqual(dieSize.Width, result.DieDimensions.DieWidth.Millimeters);
            Assert.AreEqual(dieSize.Height, result.DieDimensions.DieHeight.Millimeters);
            Assert.AreEqual(expectedStreetSizeX, result.DieDimensions.StreetWidth.Millimeters, 1E-3);
            Assert.AreEqual(expectedStreetSizeY, result.DieDimensions.StreetHeight.Millimeters, 1E-3);
        }

        [TestMethod]
        public void Die_dimensions_are_correct_when_width_street_size_is_negative()
        {
            // Given
            var dieSize = new Size(11, 11);
            var finalShiftX = -0.8.Millimeters();
            var finalShiftY = -0.4.Millimeters();

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), dieSize.Width, -dieSize.Height, 0, 0);

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, finalShiftX, 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), finalShiftY, null);
            var patternRecResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResult)
                .Returns(patternRecResult)
                .Returns(patternRecResult);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            var expectedStreetSizeX = 0;
            var expectedStreetSizeY = Math.Abs(finalShiftY.Millimeters);
            Assert.AreEqual(dieSize.Width, result.DieDimensions.DieWidth.Millimeters);
            Assert.AreEqual(dieSize.Height, result.DieDimensions.DieHeight.Millimeters);
            Assert.AreEqual(expectedStreetSizeX, result.DieDimensions.StreetWidth.Millimeters, 1E-3);
            Assert.AreEqual(expectedStreetSizeY, result.DieDimensions.StreetHeight.Millimeters, 1E-3);
        }

        [TestMethod]
        public void Die_dimensions_are_correct_when_height_street_size_is_negative()
        {
            // Given
            var dieSize = new Size(11, 11);
            var finalShiftX = 0.8.Millimeters();
            var finalShiftY = 0.4.Millimeters();

            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), dieSize.Width, -dieSize.Height, 0, 0);

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, finalShiftX, 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), finalShiftY, null);
            var patternRecResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResult)
                .Returns(patternRecResult)
                .Returns(patternRecResult);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            var expectedStreetSizeX = Math.Abs(finalShiftX.Millimeters);
            var expectedStreetSizeY = 0;
            Assert.AreEqual(dieSize.Width, result.DieDimensions.DieWidth.Millimeters);
            Assert.AreEqual(dieSize.Height, result.DieDimensions.DieHeight.Millimeters);
            Assert.AreEqual(expectedStreetSizeX, result.DieDimensions.StreetWidth.Millimeters, 1E-3);
            Assert.AreEqual(expectedStreetSizeY, result.DieDimensions.StreetHeight.Millimeters, 1E-3);
        }

        [TestMethod]
        public void Confidence_is_the_mean_of_all_pattern_recognition_results_when_there_is_verification_step()
        {
            // Given
            var input = _defaultDieAndStreetSizesInput;
            input.Wafer.Diameter = 300.Millimeters();

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 0.91, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 0.92, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Success), 0.80, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForWidthVerification = new PatternRecResult(new FlowStatus(FlowState.Success), 0.98, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForHeightVerification = new PatternRecResult(new FlowStatus(FlowState.Success), 0.85, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation)
                .Returns(patternRecResultForWidthVerification)
                .Returns(patternRecResultForHeightVerification);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            double expectedConfidence = (0.91 + 0.92 + 0.80 + 0.98 + 0.85) / 5;
            Assert.AreEqual(expectedConfidence, result.Confidence);
        }

        [TestMethod]
        public void Die_width_must_not_be_zero()
        {
            // Given: the x position of the lower right corner is confused with the x position of the upper left corner
            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, -1, 0, 0);

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Die_height_must_not_be_zero()
        {
            // Given: the y position of the lower right corner is confused with the y position of the upper left corner
            var input = _defaultDieAndStreetSizesInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            input.BottomRightCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 1, 0, 0, 0);

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Die_and_street_sizes_flow_fails_if_angle_cannot_be_found()
        {
            // Given: Pattern rec for angle computation fails
            var input = _defaultDieAndStreetSizesInput;

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Error), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Die_and_street_sizes_flow_fails_if_width_cannot_be_found()
        {
            // Given: Pattern rec for width computation fails
            var input = _defaultDieAndStreetSizesInput;

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Error), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Die_and_street_sizes_flow_fails_if_height_cannot_be_found()
        {
            // Given: Pattern rec for height computation fails
            var input = _defaultDieAndStreetSizesInput;

            var patternRecResultAtRight = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultAtBottom = new PatternRecResult(new FlowStatus(FlowState.Error), 1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResultForRotation = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultAtRight)
                .Returns(patternRecResultAtBottom)
                .Returns(patternRecResultForRotation);

            // When
            var dieAndStreetSizes = new DieAndStreetSizesFlow(input, _simulatedPatternRecFlow.Object);

            var result = dieAndStreetSizes.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestDieAndStreetSizesReport";
            Directory.CreateDirectory(directoryPath);

            var input = SimulatedData.ValidDieAndStreetSizesInput();

            var flow = new DieAndStreetSizesFlow(input);
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
            string directoryPath = "TestDieAndStreetSizesReport";
            Directory.CreateDirectory(directoryPath);

            var input = SimulatedData.ValidDieAndStreetSizesInput();

            var flow = new DieAndStreetSizesFlow(input);
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
        public void AdditionalEdgeExclusion_is_set()
        {
            //Given
            var input = SimulatedData.ValidDieAndStreetSizesInput();

            //When:
            var flow = new DieAndStreetSizesFlow(input);

            //Then
            Assert.AreEqual(7.5.Millimeters(), flow.Configuration.AdditionalEdgeExclusion);
        }
    }
}
