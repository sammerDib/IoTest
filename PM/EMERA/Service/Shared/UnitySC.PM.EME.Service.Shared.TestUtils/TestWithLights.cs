using System.Collections.Generic;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Light;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public interface ITestWithLights
    {
        EmeHardwareManager HardwareManager { get; set; }
        Dictionary<string, EMELightBase> SimulatedLights { get; set; }
    }

    public static class TestWithLightHelper
    {
        public static void Setup(ITestWithLights test)
        {
            test.SimulatedLights = new Dictionary<string, EMELightBase>
            {
                { "3", NewDummyLight("3", EMELightType.DirectionalDarkField0Degree) },
                { "4", NewDummyLight("4", EMELightType.DirectionalDarkField90Degree) }
            };
            test.HardwareManager.EMELights = test.SimulatedLights;
        }

        private static EMELight NewDummyLight(string deviceId, EMELightType type)
        {
            var config = new EMELightConfig { DeviceID = deviceId, Type = type };
            var logger = new SerilogLogger<EMELightBase>();
            var controllerConfig = new OpcControllerConfig { Name = "Dummy", DeviceID = "Controller" };
            var lightController = new DummyArduinoLightController(controllerConfig, null, logger);
            var light = new EMELight(config, lightController, null, logger);
            light.Init();
            return light;
        }
    }
}
