using System;
using System.Collections.Generic;

namespace UnitySC.PM.ANA.Service.Interface.Probe
{
    public interface IProbeThicknessesResult : IProbeResult
    {
        List<ProbeThicknessMeasure> LayersThickness { get; set; }
        DateTime Timestamp { get; set; }
    }
}
