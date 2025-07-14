using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.AGS.Service.Interface.Flow
{
    [DataContract]
    public class AcquisitionResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }
    }
}
