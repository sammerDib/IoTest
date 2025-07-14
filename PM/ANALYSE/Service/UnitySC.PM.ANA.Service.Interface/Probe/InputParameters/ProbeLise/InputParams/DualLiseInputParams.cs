using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    [KnownType(typeof(SingleLiseInputParams))]
    [KnownType(typeof(ModulePositions))]
    public class DualLiseInputParams : IDualLiseInputParams
    {
        public DualLiseInputParams(ProbeSample probeSample)
        {
            ProbeSample = probeSample;
        }

        public DualLiseInputParams(ProbeSample probeSample, SingleLiseInputParams liseUpParams, SingleLiseInputParams liseDownParams)
        {
            ProbeSample = probeSample;
            ProbeUpParams = liseUpParams;
            ProbeDownParams = liseDownParams;
        }

        #region Properties

        [DataMember]
        public SingleLiseInputParams ProbeUpParams { get; set; }

        [DataMember]
        public SingleLiseInputParams ProbeDownParams { get; set; }

        [DataMember]
        public IProbeSample ProbeSample { get; set; }

        [DataMember]
        public string CurrentProbeAcquisition { get; set; }

        [DataMember]
        public ModulePositions CurrentProbeModule { get; set; }

        #endregion Properties
    }
}
