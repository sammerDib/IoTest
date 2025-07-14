using System.Collections.Generic;

using Moq;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.DistanceSensor;
using UnitySC.PM.Shared.Hardware.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public static class TestWithDistanceSensorHelper
    {
        public static Mock<DistanceSensorBase> s_distanceSensorMock;

        public static void Setup(ITestWithDistanceSensor test)
        {
            var logger = new SerilogLogger<DistanceSensorBase>();
            var distanceSensorConfig = new DistanceSensorConfig();
            var globalStatusService = new GlobalStatusService(new SerilogLogger<GlobalStatusService>());

            var controllerConfig = new OpcControllerConfig { Name = "Name", DeviceID = "DeviceId" };
            var distanceSensorDummyController = new DistanceSensorDummyController(controllerConfig, globalStatusService, logger);
            s_distanceSensorMock = new Mock<DistanceSensorBase>(globalStatusService, logger, distanceSensorConfig, distanceSensorDummyController);
            test.HardwareManager.DistanceSensor = s_distanceSensorMock.Object;
        }

        public static void SetupDistanceSensorWithTheSameDistanceContinuously(double distance)
        {
            s_distanceSensorMock.Invocations.Clear();
            s_distanceSensorMock.Setup(x => x.GetDistanceSensorHeight()).Returns(distance);
        }
    }
}
