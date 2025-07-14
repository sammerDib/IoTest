using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.MultiSizeChuck;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class MultiSizeChuckTest : TestWithMockedHardware<MultiSizeChuckTest>, ITestWithPhotoLumAxes, ITestWithCamera, ITestWithChuck
    {
        private Mock<PatternRecFlow> _simulatedPatternRecFlow;

        private Mock<AutoExposureFlow> _simulatedAutoExposureFlow { get; set; }

        private MultiSizeChuckInput _defaultMultiSizeChuckInput;
        private IReferentialService _referentialService;
        public DummyIDSCamera SimulatedCamera { get; set; }
        public IEmeraCamera EmeraCamera { get; set; }
        public Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }
        public Mock<ITestChuck> SimulatedChuck { get; set; }

        protected override void PostGenericSetup()
        {
            _referentialService = ClassLocator.Default.GetInstance<IReferentialService>();
            _defaultMultiSizeChuckInput = SimulatedData.ValidMultiSizeChuckInput();

            _simulatedPatternRecFlow = new Mock<PatternRecFlow>(SimulatedData.ValidPatternRecInput(), Bootstrapper.SimulatedEmeraCamera.Object, null, null, null) { CallBase = true };
            var PatternRecSuccess = new PatternRecResult();
            PatternRecSuccess.Status = new FlowStatus(FlowState.Success);
            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute()).Returns(PatternRecSuccess);

            var fakeCameraInfo = new CameraInfo() { Width = 1920, Height = 1080 };
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetCameraInfo()).Returns(fakeCameraInfo);

            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() { new DummyUSPImage(10, 10, 255) });

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
        public void SetToEmptyReferentialWafer_ShouldSetShiftXAndShiftYToZero()
        {
            var flow = new MultiSizeChuckFlow(_defaultMultiSizeChuckInput, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedPatternRecFlow.Object, _simulatedAutoExposureFlow.Object);
            // Arrange
            var expectedShiftX = 0.Millimeters();
            var expectedShiftY = 0.Millimeters();

            // Act
            flow.SetToEmptyReferentialWafer();

            //// Assert
            var actualSettings = _referentialService.GetSettings(ReferentialTag.Wafer)?.Result as WaferReferentialSettings; // Remplacez par la méthode réelle pour obtenir les paramètres du référentiel
            Assert.AreEqual(expectedShiftX, actualSettings.ShiftX);
            Assert.AreEqual(expectedShiftY, actualSettings.ShiftY);
        }
        [TestMethod]
        public void GetCenterPosition_ShouldReturnCorrectPosition()
        {
            var flow = new MultiSizeChuckFlow(_defaultMultiSizeChuckInput, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedPatternRecFlow.Object, _simulatedAutoExposureFlow.Object);
            // Arrange
            var waferConfiguration = new SubstSlotWithPositionConfig()
            {
                PositionSensor = new XYPosition(new MotorReferential(), 10.0, 20.0)                
            };
            // Act
            var result = MultiSizeChuckFlow.GetCenterPosition(waferConfiguration);
            // Assert
            Assert.AreEqual(10, result.X);
            Assert.AreEqual(20, result.Y);
        }

        [TestMethod]
        public void MultiSizeChuck_Flow_Test()
        {
            var flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>();
            var configuration = flowsConfiguration.Flows.OfType<MultiSizeChuckConfiguration>().FirstOrDefault();
            var PatternRecX = new PatternRecResult();
            PatternRecX.Status = new FlowStatus(FlowState.Success);
            PatternRecX.ShiftX = 12.Millimeters();
            PatternRecX.ShiftY = 15.Millimeters();

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute()).Returns(PatternRecX);

            var input = _defaultMultiSizeChuckInput;
            var flow = new MultiSizeChuckFlow(input, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedPatternRecFlow.Object, _simulatedAutoExposureFlow.Object);
            var waferConfig = flow.GetWaferSlotConfiguration();
            var positionSensor = waferConfig.PositionSensor as XYPosition;
            // When
            var result = flow.Execute();

            // Then
            Assert.IsNotNull(waferConfig);
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(positionSensor.X.Millimeters() + PatternRecX.ShiftX, result.ShiftX);
            Assert.AreEqual(positionSensor.Y.Millimeters() + PatternRecX.ShiftY, result.ShiftY);
        }
    }
}
