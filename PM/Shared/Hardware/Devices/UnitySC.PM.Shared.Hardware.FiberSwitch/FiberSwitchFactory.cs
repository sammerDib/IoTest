using System;

namespace UnitySC.PM.Shared.Hardware.FiberSwitch
{
    static public class FiberSwitchFactory
    {
        static public FiberSwitchBase Create(FiberSwitchConfig config)
        {
            switch (config)
            {
                case EOLFiberSwitchConfig fiberSwitchConfig:
                    return new EOLFiberSwitch(fiberSwitchConfig.Name, fiberSwitchConfig.DeviceID);

                default:
                    throw new Exception("Unknown FiberSwitchConfig class" + config.GetType());
            }
        }
    }
}