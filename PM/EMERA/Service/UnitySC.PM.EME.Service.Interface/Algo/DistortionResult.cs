using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{

    [DataContract]
    public class DistortionData
    {
        [DataMember]
        public double[] NewOptimalCameraMat { get; set; }

        [DataMember]
        public double[] CameraMat { get; set; }

        [DataMember]
        public double[] DistortionMat { get; set; }

        [DataMember]
        public double[] TranslationVec { get; set; }

        [DataMember]
        public double[] RotationVec { get; set; }
    }

    [DataContract]
    public class DistortionResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public DistortionData DistortionData { get; set; }
    }
}
