using System.Runtime.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    public class AutoFocusCameraResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public double SensorDistance { get; set; }

        [DataMember]
        public ServiceImage ResultImage { get; set; }
    }
}
