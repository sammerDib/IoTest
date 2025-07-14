using System.Runtime.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class TSVResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public Length Length { get; set; }

        [DataMember]
        public Length Width { get; set; }

        [DataMember]
        public Length Depth { get; set; }

        [DataMember]
        public ServiceImage ResultImage { get; set; }

        [DataMember]
        public byte[] DepthRawSignal { get; set; }

        [DataMember]
        public double QualityScore { get; set; }
    }
}
