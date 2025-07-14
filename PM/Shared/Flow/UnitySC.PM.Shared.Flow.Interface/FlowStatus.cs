using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Flow.Interface
{
    [DataContract]
    public class FlowStatus
    {
        public FlowStatus()
        { }

        public FlowStatus(FlowState state, string message = null)
        {
            State = state;
            Message = message;
        }

        [DataMember]
        public FlowState State { get; set; }

        [DataMember]
        public string Message { get; set; }

        public bool IsFinished => State == FlowState.Error || State == FlowState.Canceled || State == FlowState.Success || State == FlowState.Partial;
    }
}
