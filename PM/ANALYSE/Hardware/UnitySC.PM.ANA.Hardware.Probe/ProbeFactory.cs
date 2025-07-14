using System;
using System.Threading;


using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.Probe
{
    static public class ProbeFactory
    {
        static public IProbeLise CreateProbe(ProbeLiseConfig config,ILogger logger)
        {
            switch (config)
            {
                case ProbeLiseConfig plc when plc.IsSimulated:
                    return new ProbeLiseDummy(config,logger);

                case ProbeLiseConfig plc when !plc.IsSimulated:
                    return new ProbeLise(config, logger);
            }
            return null;
        }



        static public IProbe CreateProbe(ProbeLiseHFConfig config, ProbeLiseHFDevices probeDevices, ILogger logger)
        {


            switch (config)
            {
                case ProbeLiseHFConfig plc when plc.IsSimulated:
                    return new ProbeLiseHFDummy(config, probeDevices,logger);

                case ProbeLiseHFConfig plc when !plc.IsSimulated:

                    return new ProbeLiseHF(config, probeDevices, logger);
            }

            return null;
        }


        static public IProbeLise CreateProbe(ProbeDualLiseConfig config, ProbeLiseConfig configUp, ProbeLiseConfig configDown, ILogger logger)
        {
            switch (config)
            {
                case ProbeDualLiseConfig plc when plc.IsSimulated:
                    return new ProbeDualLiseDummy(config, configUp, configDown, logger);

                case ProbeDualLiseConfig plc when !plc.IsSimulated:
                    return new ProbeDualLise(config, configUp, configDown, logger:logger);
            }

            return null;
        }

    }
}
