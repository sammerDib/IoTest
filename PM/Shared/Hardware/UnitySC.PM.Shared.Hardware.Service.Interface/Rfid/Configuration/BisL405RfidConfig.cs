using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Rfid
{
    [Serializable]
    [DataContract]
    public class BisL405RfidConfig : RfidConfig
    {
        [DataMember]
        [XmlArray]
        [XmlArrayItem(ElementName = "RfidTag")]
        public List<RfidTag> RfidTags { get; set; }

        [DataMember]
        public Length Size { get; set; }
    }

    [Serializable]
    [DataContract]
    public class RfidTag
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public Length Size { get; set; }
    }
}
