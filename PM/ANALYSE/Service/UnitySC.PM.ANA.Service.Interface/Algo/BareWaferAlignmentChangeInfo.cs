
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class BareWaferAlignmentChangeInfo : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

    }
}
