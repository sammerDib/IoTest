using System.Runtime.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class AFCameraResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public double ZPosition { get; set; }

        [DataMember]
        public double QualityScore { get; set; }

        [DataMember]
        public ServiceImage ResultImage { get; set; }
    }
}
