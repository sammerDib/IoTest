using System.Collections.Generic;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IProbeSignal
    {
        string ProbeID { get; set; }
        List<double> RawValues { get; set; }
    }
}
