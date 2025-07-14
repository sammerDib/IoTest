using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitySC.PM.EME.Service.Interface.Algo;
using Moq;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using System.Threading;
using System.Windows;
using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoFocus;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.EME.Hardware.Light;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class AutoFocusCameraTest : TestWithMockedHardware<AutoFocusCameraTest>, ITestWithPhotoLumAxes, ITestWithCamera, ITestWithLights, ITestWithDistanceSensor
    {
        public Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }
        public DummyIDSCamera SimulatedCamera { get; set; }
        public IEmeraCamera EmeraCamera { get; set; }
        public Dictionary<string, EMELightBase> SimulatedLights { get; set; }

        protected override void PostGenericSetup()
        {
            TestWithDistanceSensorHelper.SetupDistanceSensorWithTheSameDistanceContinuously(8000.0);
            SimulatedMotionAxes.Setup(_ => _.WaitMotionEnd(It.IsAny<int>(), It.IsAny<bool>())).Callback(() => { Thread.Sleep(200); });
            SimulatedCamera.SetImageResolution(new Size(10, 20));
        }

        [TestMethod]
        public void ShouldMoveAxisToComputedAdequateSpeed()
        {
            var afCameraInput = new AutoFocusCameraInput()
            {
                RangeType = ScanRangeType.Small,
            };

            var afCameraFlow = new AutoFocusCameraFlow(afCameraInput, Bootstrapper.SimulatedEmeraCamera.Object);
            var result = afCameraFlow.Execute();
            Assert.AreEqual(FlowState.Success, result.Status.State);
            SimulatedMotionAxes.Verify(x => x.Move(It.Is<PMAxisMove>(y => y.Speed.MillimetersPerSecond == 10.0)));
        }
        [TestMethod]
        public void AxisMoveSpeedShouldNotExceedSpeedMaxScanWhenTheFramerateIsTooHigh()
        {
            var afCameraInput = new AutoFocusCameraInput()
            {
                RangeType = ScanRangeType.Small,
            };

            var highFramerateCameraInfo = new MatroxCameraInfo()
            {
                //very high framerate which would result in a very high axis speed
                MaxFrameRate = 10000,
                Width = SimulatedCamera.Width,
                Height = SimulatedCamera.Height,
            };
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetMatroxCameraInfo()).Returns(highFramerateCameraInfo);

            var afCameraFlow = new AutoFocusCameraFlow(afCameraInput, Bootstrapper.SimulatedEmeraCamera.Object);
            var result = afCameraFlow.Execute();
            Assert.AreEqual(FlowState.Success, result.Status.State);
            var axesConfigs = SimulatedMotionAxes.Object.AxesConfiguration.AxisConfigs;
            var zAxisConfig = axesConfigs.Find(axisConfig => axisConfig.MovingDirection == MovingDirection.Z);
            //The speed should truncate to the max scan speed of the axis
            SimulatedMotionAxes.Verify(x => x.Move(It.Is<PMAxisMove>(y => y.Speed.MillimetersPerSecond == zAxisConfig.SpeedMaxScan)));
        }

        [DataTestMethod]
        [DataRow(ScanRangeType.Small, -0.25, 0.25, 0.01)]
        [DataRow(ScanRangeType.Medium, -1.25, 1.25, 0.1)]
        [DataRow(ScanRangeType.Large, -2.5, 2.5, 0.1)]
        [DataRow(ScanRangeType.AllAxisRange, -19.1, 2.9, 0.1)]
        public void GetScanRangeShouldGiveToExpectedRangeAccordingToScanRangeType(ScanRangeType scanRangeType, double expectedScanMin, double expectedScanMax, double expectedStep)
        {
            var afCameraInput = new AutoFocusCameraInput()
            {
                RangeType = scanRangeType,
            };

            var flowsConfig = ClassLocator.Default.GetInstance<IFlowsConfiguration>().Flows.OfType<AutoFocusCameraConfiguration>().FirstOrDefault(); ;

            var afCameraFlow = new AutoFocusCameraFlow(afCameraInput, Bootstrapper.SimulatedEmeraCamera.Object);
            var result = afCameraFlow.GetScanRange(afCameraInput.RangeType, flowsConfig, 0, -19.1.Millimeters(), 2.9.Millimeters());
            Assert.AreEqual(expectedScanMin, result.Min);
            Assert.AreEqual(expectedScanMax, result.Max);
            Assert.AreEqual(expectedStep, result.Step);
        }

        [TestMethod]
        public void GetScanRangeShouldNotExceedTheAxisLimits()
        {
            var afCameraInput = new AutoFocusCameraInput()
            {
                RangeType = ScanRangeType.Medium,
            };

            var flowsConfig = ClassLocator.Default.GetInstance<IFlowsConfiguration>().Flows.OfType<AutoFocusCameraConfiguration>().FirstOrDefault();

            var afCameraFlow = new AutoFocusCameraFlow(afCameraInput, Bootstrapper.SimulatedEmeraCamera.Object);

            //Very small axis limits :
            Length axisMinLimit = -0.01.Millimeters();
            Length axisMaxLimit = 0.01.Millimeters();

            var result = afCameraFlow.GetScanRange(afCameraInput.RangeType, flowsConfig, 0, axisMinLimit, axisMaxLimit);
            //ScanRange min & max should be truncated to the limits
            Assert.AreEqual(axisMinLimit.Millimeters, result.Min);
            Assert.AreEqual(axisMaxLimit.Millimeters, result.Max);
        }
    }
}
