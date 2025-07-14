using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ionizers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Ionizer
{
    public static class IonizerFactory
    {
        public static IonizerBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, IonizerConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase controller;
            bool found = controllers.TryGetValue(config.ControllerID, out controller);

            if (!found || (controller == null))
                throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID + ", ControllerId = " + config.ControllerID + "]");

            switch (config)
            {
                case KeyenceIonizerConfig keyenceIonizerConfig:
                    if (controller is KeyenceIonizerDummyController)
                    {
                        return new KeyenceIonizerDummy(globalStatusServer, logger, keyenceIonizerConfig, (IonizerController)controller);
                    }
                    return new SJH108Ionizer(globalStatusServer, logger, keyenceIonizerConfig, (IonizerController)controller);

                default:
                    throw new Exception("Unknown config class" + config.GetType());
            }
        }
    }
}
