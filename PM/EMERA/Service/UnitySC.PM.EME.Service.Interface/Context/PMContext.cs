using System.Runtime.Serialization;

namespace UnitySC.PM.EME.Service.Interface.Context
{
    [DataContract]
    public class PMContext : EMEContextBase
    {
        [DataMember]
        public ChamberContext Context { get; set; }

        [DataMember]
        public XYPositionContext XyPosition { get; set; }

        [DataMember]
        public LightsContext Lights { get; set; }
    }
}
