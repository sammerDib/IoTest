using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.DistanceSensor
{
    static public class DistanceSensorFactory
    {
        public static DistanceSensorBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, DistanceSensorConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase controller;
            bool found = controllers.TryGetValue(config.ControllerID, out controller);

            if (!found || (controller == null))
                throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID + ", ControllerId = " + config.ControllerID + "]");


            switch (config)
            {
                case MicroEpsilonDistanceSensorConfig microEpsilonDistanceSensorConfig:
                    if (controller is DistanceSensorDummyController)
                    {
                        return new DistanceSensorDummy(globalStatusServer, logger, microEpsilonDistanceSensorConfig, (DistanceSensorController)controller);
                    }
                    return new MicroEpsilonDistanceSensor(globalStatusServer, logger, microEpsilonDistanceSensorConfig, (DistanceSensorController)controller);

                default:
                    throw new Exception("Unknown DistanceSensorConfig class" + config.GetType());
            }
        }
    }
}
