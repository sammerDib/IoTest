using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Rfids;
using UnitySC.PM.Shared.Hardware.Service.Interface.Rfid;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Rfid
{
    public static class RfidFactory
    {
        public static RfidBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, RfidConfig config,
            Dictionary<string, ControllerBase> controllers)
        {
            bool found = controllers.TryGetValue(config.ControllerID, out var controller);

            if (!found || (controller == null))
                throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID + ", ControllerId = " + config.ControllerID + "]");

            switch (config)
            {
                case BisL405RfidConfig BisL405RfidConfig:
                    if (controller is DummyRfidController)
                    {
                        return new BisL405RfidDummy(globalStatusServer, logger, BisL405RfidConfig, (DummyRfidController)controller);
                    }
                    return new BisL405Rfid(globalStatusServer, logger, BisL405RfidConfig, (RfidController)controller);

                default:
                    throw new Exception("Unknown config class" + config.GetType());
            }
        }
    }
}
