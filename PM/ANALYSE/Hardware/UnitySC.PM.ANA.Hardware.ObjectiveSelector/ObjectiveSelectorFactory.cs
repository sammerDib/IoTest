using System;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Hardware.ObjectiveSelector
{
    static public class ObjectiveSelectorFactory
    {
        static public IObjectiveSelector CreateObjectiveSelector(ObjectivesSelectorConfigBase config,ILogger logger)
        {
            switch (config)
            {
                case LineMotObjectivesSelectorConfig linMotConfig:
                    if (linMotConfig.IsSimulated)
                    {
                        logger.Information("Hardware simulated mode activate");
                        return new LinMotUdpDummy(linMotConfig, logger);
                    }
                    else
                        return new LinMotUdp(linMotConfig, logger);
                case SingleObjectiveSelectorConfig single:
                    return new SingleObjectiveSelector(single, logger);

                default:
                    throw new Exception("Unknow ObjectivesSelector configuration.");
            }
        }
    }
}
