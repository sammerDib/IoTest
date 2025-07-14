using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Client.Proxy
{
    public interface IProbesFactory
    {
        ProbeBaseVM Create(IProbeConfig probeConfig, IProbeService probeSupervisor);

    }
}
