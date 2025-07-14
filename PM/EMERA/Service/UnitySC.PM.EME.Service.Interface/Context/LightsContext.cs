using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.EME.Service.Interface.Context
{
    [DataContract]
    public class LightsContext : EMEContextBase
    {
        [DataMember]
        public List<LightContext> Lights { get; set; } = new List<LightContext>();
    }
}
