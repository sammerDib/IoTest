using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeDualLiseConfig : DualProbeConfigBase, IProbeDualLiseConfig
    {
        public ProbeDualLiseConfig()
        {
        }

        [DataMember]
        public int ConfigLiseDoubleParam { get; set; }

        [DataMember]
        public string DeviceType { get; set; }
    }
}
