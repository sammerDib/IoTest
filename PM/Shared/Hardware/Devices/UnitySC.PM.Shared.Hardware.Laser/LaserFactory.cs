using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Laser
{
    public static class LaserFactory
    {
        public static LaserBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, LaserConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase controller;
            bool found = controllers.TryGetValue(config.ControllerID, out controller);

            if (!found || (controller == null))
                throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID + ", ControllerId = " + config.ControllerID + "]");

            switch (config)
            {
                case Piano450LaserConfig laserPiano450Config:
                    if (controller is Piano450LaserDummyController)
                    {
                        return new Piano450LaserDummy(globalStatusServer, logger, laserPiano450Config, (LaserController)controller);
                    }
                    return new Piano450Laser(globalStatusServer, logger, laserPiano450Config, (LaserController)controller);

                case SMD12LaserConfig laserSMD12Config:
                    if (controller is SMD12LaserDummyController)
                    {
                        return new SMD12LaserDummy(globalStatusServer, logger, laserSMD12Config, (LaserController)controller);
                    }
                    return new SMD12Laser(globalStatusServer, logger, laserSMD12Config, (LaserController)controller);

                default:
                    throw new Exception("Unknown config class" + config.GetType());
            }
        }
    }
}
