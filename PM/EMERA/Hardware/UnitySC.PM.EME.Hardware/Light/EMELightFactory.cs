using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Hardware.Light
{
    public static class EMELightFactory
    {
        public static EMELightBase Create(EMELightConfig config, Dictionary<string, ControllerBase> controllers, IGlobalStatusServer globalStatusServer, ILogger logger)
        {
            if (!controllers.TryGetValue(config.ControllerID, out var lightController))
            {
                throw new Exception("Light Controller not found: " + config.GetType());
            }

            if (!(lightController is IPowerLightController powerLightController))
            {
                throw new Exception("Light Controller is not valid type: " + lightController.GetType());
            }

            return new EMELight(config, powerLightController, globalStatusServer, logger);
        }
    }
}
