using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Hardware.Light;
using UnitySC.PM.EME.Service.Core.Flows.DistanceSensorCalibration;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class DistanceSensorCalibrationTest : TestWithMockedHardware<DistanceSensorCalibrationTest>,
        ITestWithPhotoLumAxes, ITestWithCamera, ITestWithChuck, ITestWithLights
    {
        private Mock<PatternRecFlow> _simulatedPatternRecFlow;
        public DummyIDSCamera SimulatedCamera { get; set; }
        public IEmeraCamera EmeraCamera { get; set; }
        public Mock<ITestChuck> SimulatedChuck { get; set; }
        public Dictionary<string, EMELightBase> SimulatedLights { get; set; }
        public Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }

        protected override void PostGenericSetup()
        {
            _simulatedPatternRecFlow = new Mock<PatternRecFlow>(SimulatedData.ValidPatternRecInput(),
                Bootstrapper.SimulatedEmeraCamera.Object, null, null, null) { CallBase = true };
            var patternRecSuccess = new PatternRecResult { Status = new FlowStatus(FlowState.Success), ShiftX = 1.Millimeters(), ShiftY = 1.Millimeters()};
            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute()).Returns(patternRecSuccess);

            SimulatedMotionAxes.Setup(x => x.GetPosition()).Returns(new XYZPosition(new StageReferential(),1,1,1));
            SimulatedMotionAxes.Setup(x => x.WaitMotionEnd(It.IsAny<int>(), It.IsAny<bool>()))
                .Callback(() => Thread.Sleep(10));
        }

        [DataTestMethod]
        [DataRow(7500, FlowState.Success)]
        [DataRow(7000, FlowState.Error)]
        public void DistanceCalibrationFlowShouldSucceedOrNotAccordingToInitialPosition(int initialPosition, FlowState expectedState)
        {
            // Arrange
            var tracker = new PositionTracker(() => new TimestampedPosition(initialPosition.Millimeters(), DateTime.Now), 1);
            var flow = new DistanceSensorCalibrationFlow(new DistanceSensorCalibrationInput(),
                Bootstrapper.SimulatedEmeraCamera.Object, _simulatedPatternRecFlow.Object, tracker);

            // Act
            var result = flow.Execute();

            // Assert
            Assert.AreEqual(expectedState, result.Status.State);
        }
    }
}
