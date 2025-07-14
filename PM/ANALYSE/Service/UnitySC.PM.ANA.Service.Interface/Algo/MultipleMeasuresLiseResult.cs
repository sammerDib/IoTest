using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class MultipleMeasuresLiseResult : IFlowResult
    {
        public MultipleMeasuresLiseResult()
        {
            ProbeThicknessMeasures = new List<List<ProbeThicknessMeasure>>();
        }

        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public List<List<ProbeThicknessMeasure>> ProbeThicknessMeasures { get; set; }

        [DataMember]
        public double Quality { get; set; }
    }
}
