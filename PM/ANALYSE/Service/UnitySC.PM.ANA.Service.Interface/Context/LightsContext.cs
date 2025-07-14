using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    public class LightsContext : ANAContextBase
    {
        [DataMember]
        public List<LightContext> Lights { get; set; } = new List<LightContext>();
    }
}
