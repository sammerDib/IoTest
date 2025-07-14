using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Plc
{
    public static class PlcFactory
    {
        public static PlcBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, PlcConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase plcController;
            bool found = controllers.TryGetValue(config.ControllerID, out plcController);

            switch (config)
            {
                case BeckhoffPlcConfig beckhoffPlcConfig:
                    if (plcController is BeckhoffPlcDummyController)
                    {
                        return new BeckhoffPlcDummy(globalStatusServer, logger, beckhoffPlcConfig, (PlcController)plcController);
                    }
                    return new BeckhoffPlc(globalStatusServer, logger, beckhoffPlcConfig, (PlcController)plcController);

                default:
                    throw new Exception("Unknown PLC config class" + config.GetType());
            }
        }
    }
}
