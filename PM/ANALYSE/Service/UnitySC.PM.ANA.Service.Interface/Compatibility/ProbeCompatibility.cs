using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Compatibility.Capability
{ 
    [DataContract]
    public class ProbeCompatibility
    {
        [DataMember]
        public List<Probe> Probes { get; set; } 
    }
}
