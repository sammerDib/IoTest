using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class MeasureDualLiseResult : ILiseMeasureResult
    {
        [DataMember]
        public List<ProbeThicknessMeasure> LayersThickness { get; set; }

        [DataMember]
        public Length AirGapUp { get; set; }

        [DataMember]
        public Length AirGapDown { get; set; }

        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public double Quality { get; set; }
    }
}
