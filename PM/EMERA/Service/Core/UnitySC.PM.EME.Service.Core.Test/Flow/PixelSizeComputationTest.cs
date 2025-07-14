using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Core.Flows.PixelSize;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class PixelSizeComputationTest : TestWithMockedHardware<PixelSizeComputationTest>, ITestWithPhotoLumAxes, ITestWithCamera
    {
        private Mock<PatternRecFlow> _simulatedPatternRecFlow;

        private Mock<AutoExposureFlow> _simulatedAutoExposureFlow { get; set; }

        private PixelSizeComputationInput _defaultPixelSizeComputationInput;

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
            _defaultPixelSizeComputationInput = SimulatedData.ValidPixelSizeComputationInput();

            _simulatedPatternRecFlow = new Mock<PatternRecFlow>(SimulatedData.ValidPatternRecInput(), Bootstrapper.SimulatedEmeraCamera.Object, null, null, null) { CallBase = true };

            var fakeCameraInfo = new CameraInfo() { Width = 1920, Height = 1080 };
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetCameraInfo()).Returns(fakeCameraInfo);

            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.IsAcquiring()).Returns(true);

            TestWithCameraHelper.SetupCameraWithImagesForSingleScaledAcquisition(new List<DummyUSPImage>() { new DummyUSPImage(10, 10, 255) });

            Bootstrapper.SimulatedEmeraCamera.Setup(x => x.GetCameraExposureTime()).Returns(100.0);

            _simulatedAutoExposureFlow = new Mock<AutoExposureFlow>(SimulatedData.ValidAutoExposureInput(), Bootstrapper.SimulatedEmeraCamera.Object) { CallBase = true };

            var autoExposureSuccess = new AutoExposureResult()
            {
                Brightness = 1.0,
                ExposureTime = 100,
                Status = new FlowStatus(FlowState.Success)
            };
            _simulatedAutoExposureFlow.Setup(_ => _.Execute()).Returns(autoExposureSuccess);
        }

        [TestMethod]
        public void Pixel_size_computation_returns_the_expected_pixel_size_with_the_right_settings()
        {
            var expectedPixelSize = 2.0.Micrometers();

            var patternRecX = new PatternRecResult();
            patternRecX.Status = new FlowStatus(FlowState.Success);
            patternRecX.ShiftX = 4.04.Millimeters();
            patternRecX.ShiftY = 0.0.Millimeters();

            _simulatedPatternRecFlow.Setup(_ => _.Execute()).Returns(patternRecX);
            _simulatedPatternRecFlow.Object.Result = patternRecX;

            var input = _defaultPixelSizeComputationInput;
            var flow = new PixelSizeComputationFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedPatternRecFlow.Object, _simulatedAutoExposureFlow.Object);
            flow.ImageScale = 0.5;
            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(flow.Result.PixelSize, expectedPixelSize);

        }

        [TestMethod]
        public void Pixel_size_computation_fails_when_pattern_rec_fails()
        {
            var expectedPixelSize = 2.0.Micrometers();

            var patternRecX = new PatternRecResult();
            patternRecX.Status = new FlowStatus(FlowState.Error);

            _simulatedPatternRecFlow.Setup(_ => _.Execute()).Returns(patternRecX);
            _simulatedPatternRecFlow.Object.Result = patternRecX;

            var input = _defaultPixelSizeComputationInput;
            var flow = new PixelSizeComputationFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedPatternRecFlow.Object, _simulatedAutoExposureFlow.Object);
            flow.ImageScale = 0.5;
            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);

        }

    }
}
