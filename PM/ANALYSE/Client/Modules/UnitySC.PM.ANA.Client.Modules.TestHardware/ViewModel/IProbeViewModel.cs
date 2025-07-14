using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Client.Proxy.Probe;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public interface IProbeLiseViewModel
    {
        ProbeLiseBaseVM Probe { get; set; }
    }
}
