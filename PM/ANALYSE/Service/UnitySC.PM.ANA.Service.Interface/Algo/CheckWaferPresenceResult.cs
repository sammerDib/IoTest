using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class CheckWaferPresenceResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public bool IsWaferPresent { get; set; } = false;
    }
}
