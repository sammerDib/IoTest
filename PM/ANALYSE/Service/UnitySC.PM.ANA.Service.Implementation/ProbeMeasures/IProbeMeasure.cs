using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Implementation.ProbeMeasures
{
    public interface IProbeMeasure
    {
        IProbeResult DoMeasure(IProbeInputParams inputParameters);
    }
}
