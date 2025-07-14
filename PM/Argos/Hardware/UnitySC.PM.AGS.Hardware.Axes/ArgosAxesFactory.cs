using System;
using System.Collections.Generic;

using UnitySC.PM.AGS.Hardware.Axes.Axes;
using UnitySC.PM.AGS.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.AGS.Hardware.Axes
{
    public static class ArgosAxesFactory
    {
        public static ArgosAxesBase Create(AxesConfig config, Dictionary<string, MotionControllerBase> controllers, IGlobalStatusServer globalStatusServer, HardwareLogger axesLogger, IReferentialManager referentialManager)
        {
            return new EdgeAxes(config, controllers, globalStatusServer, axesLogger, referentialManager);
        }
    }
}
