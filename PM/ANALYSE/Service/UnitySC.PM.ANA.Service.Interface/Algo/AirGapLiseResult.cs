using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class AirGapLiseResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public Length AirGap { get; set; }

        [DataMember]
        public double Quality { get; set; }
    }
}
