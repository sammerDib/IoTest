using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [XmlInclude(typeof(PIE709ControllerConfig))]
    [DataContract]
    public class PiezoControllerConfig : ControllerConfig
    {
        [DataMember]
        [XmlArrayItem(ElementName = "PiezoAxisID")]
        public List<string> PiezoAxisIDs { get; set; }
    }
}
