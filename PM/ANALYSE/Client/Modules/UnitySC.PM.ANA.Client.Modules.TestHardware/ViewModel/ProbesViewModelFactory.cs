using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class ProbesViewModelFactory : IProbesFactory
    {
        public ProbeBaseVM Create(IProbeConfig probeconfig, IProbeService probeSupervisor)
        {
            switch (probeconfig)
            {
                case ProbeLiseConfig _:
                    {
                        var newProbe = new ProbeLiseVM(probeSupervisor, probeconfig.DeviceID);

                        var mapper = ClassLocator.Default.GetInstance<Mapper>();
                        newProbe.Configuration = mapper.AutoMap.Map<ProbeConfigurationLiseVM>(probeconfig);
                        return newProbe;
                    }

                case ProbeDualLiseConfig _:

                    {
                        var newProbe = new ProbeLiseDoubleVM(probeSupervisor, probeconfig.DeviceID);

                        var mapper = ClassLocator.Default.GetInstance<Mapper>();
                        newProbe.Configuration = mapper.AutoMap.Map<ProbeConfigurationLiseDoubleVM>(probeconfig);
                        return newProbe;
                    }

                default:
                    throw (new Exception("This configuration type is not associated to a probe view model"));
            }
        }
    }
}
