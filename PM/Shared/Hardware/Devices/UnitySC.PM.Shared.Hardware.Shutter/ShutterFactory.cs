using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Shutters;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Shutter
{
    public static class ShutterFactory
    {
        public static ShutterBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, ShutterConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase controller;
            bool found = controllers.TryGetValue(config.ControllerID, out controller);

            if (!found || (controller == null))
                throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID + ", ControllerId = " + config.ControllerID + "]");

            switch (config)
            {
                case Sh10pilShutterConfig shutterSh10pilConfig:
                    if (controller is Sh10pilShutterDummyController)
                    {
                        return new Sh10pilShutterDummy(globalStatusServer, logger, shutterSh10pilConfig, (ShutterController)controller);
                    }
                    return new Sh10pilShutter(globalStatusServer, logger, shutterSh10pilConfig, (ShutterController)controller);

                default:
                    throw new Exception("Unknown config class" + config.GetType());
            }
        }
    }
}
