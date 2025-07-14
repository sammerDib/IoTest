using System;

using UnitySC.Shared.Logger;
namespace UnitySC.PM.Shared.Hardware.OpticalPowermeter
{
    static public class OpticalPowermeterFactory
    {
        static public OpticalPowermeterBase Create(OpticalPowermeterConfig config,ILogger logger)
        {
            switch (config)
            {
                case PM101OpticalPowermeterConfig opticalPowermeterConfig:
                    return new PM101OpticalPowermeter(opticalPowermeterConfig.Name, opticalPowermeterConfig.DeviceID,logger);

                default:
                    throw new Exception("Unknown OpticalPowermeterConfig class " + config.GetType());
            }
        }
    }
}
