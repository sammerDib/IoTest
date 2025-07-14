using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbesVieModelFactory : IProbesFactory
    {
        public ProbeBaseVM Create(IProbeConfig probeConfig, IProbeService probeSupervisor)
        {
            switch (probeConfig)
            {
                case ProbeLiseConfig _:
                    {
                        var newProbe = new ProbeLiseVM(probeSupervisor, probeConfig.DeviceID);

                        var mapper = ClassLocator.Default.GetInstance<Mapper>();
                        newProbe.Configuration = mapper.AutoMap.Map<ProbeConfigurationLiseVM>(probeConfig);
                        return newProbe;
                    }

                case ProbeDualLiseConfig _:

                    {
                        var newProbe = new ProbeLiseDoubleVM(probeSupervisor, probeConfig.DeviceID);

                        var mapper = ClassLocator.Default.GetInstance<Mapper>();
                        newProbe.Configuration = mapper.AutoMap.Map<ProbeConfigurationLiseDoubleVM>(probeConfig);
                        return newProbe;
                    }

                case ProbeLiseHFConfig _:
                    {
                        var newProbe = new ProbeLiseHFVM(probeSupervisor, probeConfig.DeviceID);
                        newProbe.Configuration = probeConfig;
                        return newProbe;
                    }


                default:
                    throw (new Exception("This configuration type is not associated to a probe view model"));
            }
        }
    }
}
