using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ffus;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Ffu
{
    public static class FfuFactory
    {
        public static FfuBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, FfuConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase controller;
            bool found = controllers.TryGetValue(config.ControllerID, out controller);

            if (!found || (controller == null))
                throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID + ", ControllerId = " + config.ControllerID + "]");

            switch (config)
            {
                case Astrofan612FfuConfig astrofan612FfuConfig:
                    if (controller is DummyFfuController)
                    {
                        return new FfuDummy(globalStatusServer, logger, astrofan612FfuConfig, (DummyFfuController)controller);
                    }
                    return new Astrofan612Ffu(globalStatusServer, logger, astrofan612FfuConfig, (FfuController)controller);

                default:
                    throw new Exception("Unknown config class" + config.GetType());
            }
        }
    }
}
