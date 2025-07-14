using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo.GetZFocus
{
    [DataContract]
    public class GetZFocusResult : IFlowResult
    {
        public GetZFocusResult(){}
        private GetZFocusResult(double z)
        {
            Z = z;
            Status = new FlowStatus(FlowState.Success);
        }

        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public double Z { get; set; }

        public static GetZFocusResult Success(double z)
        {
            return new GetZFocusResult(z);
        }
    }
}
