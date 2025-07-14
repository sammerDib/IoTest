using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public interface ILiseMeasureResult : IFlowResult
    {
        List<ProbeThicknessMeasure> LayersThickness { get; set; }
        DateTime Timestamp { get; set; }
    }
}
