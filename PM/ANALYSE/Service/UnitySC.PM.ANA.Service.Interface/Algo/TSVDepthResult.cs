using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class TSVDepthResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public Length Depth { get; set; }

        [DataMember]
        public byte[] DepthRawSignal { get; set; } // FTT raw signal

        [DataMember]
        public double Quality { get; set; }
    }
}
