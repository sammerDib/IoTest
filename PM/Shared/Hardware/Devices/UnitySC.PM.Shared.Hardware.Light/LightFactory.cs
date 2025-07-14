using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Light
{
    public static class LightFactory
    {
        public static LightBase Create(LightConfig config, Dictionary<string, ControllerBase> controllers, IGlobalStatusServer globalStatusServer, ILogger logger)
        {            
            ControllerBase lightController;
            bool found = controllers.TryGetValue(config.ControllerID, out lightController);

            if (found && lightController.ControllerConfiguration.IsSimulated)
            {
                return new DummyLight(config, (LightController)lightController, globalStatusServer, logger);
            }

            switch (config)
            {
                case ACSLightConfig acsLightConfig:                                        
                    return new ACSLight(acsLightConfig, (LightController)lightController, globalStatusServer, logger);

                case ENTTECLightConfig enttecLightConfig:

                    NICouplerController _ioController = (NICouplerController)controllers.FirstOrDefault(
                        x => x.Value.DeviceID == config.ControllerID).Value;
                                        
                    return new ENTTECLight(enttecLightConfig, (LightController)lightController, _ioController, globalStatusServer, logger);

                default:
                    throw new Exception("Unknown Light config class" + config.GetType());
            }
        }
    }
}
