using System.Runtime.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class PSIResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public ServiceImage NanoTopographyImage { get; set; }

       // [DataMember]
       // public MatrixFloatFile Image3DA { get; set; }

        [DataMember]
        public Length StepHeight { get; set; }

        [DataMember]
        public Length Roughness { get; set; }

        [DataMember]
        public double QualityScore { get; set; }
    }
}
