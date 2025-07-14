using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.AxisOrthogonality;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class AxisOrthogonalityTest : TestWithMockedHardware<AxisOrthogonalityTest>, ITestWithPhotoLumAxes, ITestWithCamera
    {
        private Mock<PatternRecFlow> _simulatedPatternRecFlow;

        private Mock<AutoExposureFlow> _simulatedAutoExposureFlow { get; set; }

        private AxisOrthogonalityInput _defaultAxisOrthogonalityInput;

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
            _defaultAxisOrthogonalityInput = SimulatedData.ValidAxisOrthogonalityInput();

            _simulatedPatternRecFlow = new Mock<PatternRecFlow>(SimulatedData.ValidPatternRecInput(), Bootstrapper.SimulatedEmeraCamera.Object, null, null, null) { CallBase = true };
            var PatternRecSuccess = new PatternRecResult();
            PatternRecSuccess.Status = new FlowStatus(FlowState.Success);
            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute()).Returns(PatternRecSuccess);

            var fakeCameraInfo = new CameraInfo() { Width = 1920, Height = 1080 };
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetCameraInfo()).Returns(fakeCameraInfo);

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
        public void Axis_orthogonality_returns_an_angle_of_zero_when_pattern_rec_shifts_follow_the_axes()
        {
            var flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>();
            var configuration = flowsConfiguration.Flows.OfType<AxisOrthogonalityConfiguration>().FirstOrDefault();

            var PatternRecX = new PatternRecResult();
            PatternRecX.Status = new FlowStatus(FlowState.Success);
            PatternRecX.ShiftX = -configuration.ShiftLength;
            PatternRecX.ShiftY = 0.0.Millimeters();
            var PatternRecY = new PatternRecResult();
            PatternRecY.Status = new FlowStatus(FlowState.Success);
            PatternRecY.ShiftX = 0.0.Millimeters();
            PatternRecY.ShiftY = -configuration.ShiftLength;

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute()).Returns(PatternRecX).Returns(PatternRecY);

            var input = _defaultAxisOrthogonalityInput;
            var flow = new AxisOrthogonalityFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedPatternRecFlow.Object, _simulatedAutoExposureFlow.Object);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(0, result.XAngle.Value);
            Assert.AreEqual(0, result.YAngle.Value);
        }

        [TestMethod]
        public void Axis_orthogonality_returns_an_angle_of_45_degrees_when_pattern_rec_shifts_are_diagonal()
        {
            var flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>();
            var configuration = flowsConfiguration.Flows.OfType<AxisOrthogonalityConfiguration>().FirstOrDefault();

            var PatternRecX = new PatternRecResult();
            PatternRecX.Status = new FlowStatus(FlowState.Success);
            PatternRecX.ShiftX = -configuration.ShiftLength;
            PatternRecX.ShiftY = -configuration.ShiftLength;
            var PatternRecY = new PatternRecResult();
            PatternRecY.Status = new FlowStatus(FlowState.Success);
            PatternRecY.ShiftX = configuration.ShiftLength;
            PatternRecY.ShiftY = -configuration.ShiftLength;

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute()).Returns(PatternRecX).Returns(PatternRecY);

            var input = _defaultAxisOrthogonalityInput;
            var flow = new AxisOrthogonalityFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedPatternRecFlow.Object, _simulatedAutoExposureFlow.Object);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual(45, result.XAngle.Value);
            Assert.AreEqual(45, result.YAngle.Value);
        }

    }
}
