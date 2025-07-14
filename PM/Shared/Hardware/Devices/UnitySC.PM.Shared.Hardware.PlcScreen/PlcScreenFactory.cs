using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens;
using UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.PlcScreen
{
    public static class PlcScreenFactory
    {
        public static PlcScreenBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, ScreenConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase controller;
            bool found = controllers.TryGetValue(config.ControllerID, out controller);

            if (!found || (controller == null))
                throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID + ", ControllerId = " + config.ControllerID + "]");

            switch (config)
            {
                case DensitronDM430GNScreenConfig DensitronDM430GNScreenConfig:
                    return new DensitronScreen(globalStatusServer, logger, DensitronDM430GNScreenConfig, (ScreenController)controller);

                default:
                    throw new Exception("Unknown config class" + config.GetType());
            }
        }
    }
}
