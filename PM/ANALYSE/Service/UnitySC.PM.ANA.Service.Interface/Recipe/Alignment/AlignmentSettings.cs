using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Alignment
{
    [DataContract]
    public class AlignmentSettings
    {
        [DataMember]
        public AutoFocusLiseParameters AutoFocusLise { get; set; }

        [DataMember]
        public AutoLightParameters AutoLight { get; set; }

        [DataMember]
        public BareWaferAlignmentParameters BareWaferAlignment { get; set; }
    }
}
