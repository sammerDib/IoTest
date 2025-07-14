using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Compatibility.Capability;

namespace UnitySC.PM.ANA.Service.Interface.Compatibility
{
    [Serializable]
    [DataContract]
    public class Probe
    {
        [XmlArrayItem(typeof(CrossLayer))]
        [XmlArrayItem(typeof(DistanceMeasure))]
        [XmlArrayItem(typeof(ThicknessMeasure))]
        [DataMember]
        public List<CapabilityBase> Capabilities { get; set; }

        [XmlAttribute("Name")]
        [DataMember]
        public string Name { get; set; }
    }
}
