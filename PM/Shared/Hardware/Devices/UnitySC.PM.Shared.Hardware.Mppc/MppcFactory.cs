using System;

using UnitySC.Shared.Logger;


namespace UnitySC.PM.Shared.Hardware.Mppc
{
    static public class MppcFactory
    {
        static public MppcBase Create(MppcConfig config,ILogger logger)
        {
            switch (config)
            {
                case C13336MppcConfig mppcConfig:
                    return new C13336Mppc(mppcConfig.Name, mppcConfig.DeviceID,logger);

                default:
                    throw new Exception("Unknown MppcConfig class" + config.GetType());
            }
        }
    }
}
