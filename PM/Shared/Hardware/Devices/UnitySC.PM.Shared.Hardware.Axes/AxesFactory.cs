using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public static class AxesFactory
    {
        public static IAxes CreateAxes(AxesConfig config, Dictionary<string, ControllerBase> controllersDico, IGlobalStatusServer globalStatusServer, ILogger logger, IReferentialManager referentialManager)
        {
            if (config.IsSimulated)
            {
                return new DummyAxes(config, globalStatusServer, logger, referentialManager);
            }
            else
            {
                switch (config)
                {
                    case TMAPAxesConfig tmapAxesConfig:
                        return new TMAPAxes(tmapAxesConfig, controllersDico, globalStatusServer, logger, referentialManager);

                    case NSTAxesConfig nstAxesConfig:
                        return new NSTAxes(nstAxesConfig, controllersDico, globalStatusServer, logger, referentialManager);

                    default:
                        throw new Exception("Unknown AxesConfig class" + config.GetType().ToString());
                }
            }
        }

        #region MotionAxes

        public static MotionAxesBase CreateMotionAxes(AxesConfig config, Dictionary<string, MotionControllerBase> controllersDico, IGlobalStatusServer globalStatusServer, ILogger logger, IReferentialManager referentialManager)
        {
            switch (config)
            {
                case LiseHfAxesConfig liseHfAxesConfig: return new LiseHfAxes(liseHfAxesConfig, controllersDico, globalStatusServer, logger, referentialManager);
                case PSDAxesConfig psdAxesConfig: return new PSDAxes(psdAxesConfig, controllersDico, globalStatusServer, logger, referentialManager);
                default:
                    throw new Exception("Unknown AxesConfig class" + config.GetType().ToString());
            }
        }

        #endregion
    }
}
