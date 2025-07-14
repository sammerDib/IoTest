using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Spectrometer;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Spectrometer
{
    public static class SpectrometerFactory
    {
        public static SpectrometerBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, SpectrometerConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase spectroController;
            bool found = controllers.TryGetValue(config.ControllerID, out spectroController);
            switch (config)
            {
                case SpectrometerConfig spectroConfig:
                    if (spectroController is SpectrometerDummyController)
                    {
                        return new SpectrometerAvantesDummy(globalStatusServer, logger, spectroConfig, spectroController);
                    }
                    return new SpectrometerAvantes(globalStatusServer, logger, spectroConfig, spectroController);

                default:
                    throw new Exception("Unknown SpectrometerConfig class" + config.GetType());
            }
        }
    }
}
