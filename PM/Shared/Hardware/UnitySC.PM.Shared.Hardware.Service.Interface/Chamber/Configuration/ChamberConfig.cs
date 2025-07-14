using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Chamber;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    [Serializable]
    [DataContract]
    public class SlitDoorConfiguration
    {
        public bool Available { get; set; }
        public int ClosingTimeout_ms { get; set; }
        public int OpeningTimeout_ms { get; set; }
    }

    [Serializable]
    [XmlInclude(typeof(DMTChamberConfig))]
    [XmlInclude(typeof(ANAChamberConfig))]
    [XmlInclude(typeof(EMEChamberConfig))]
    [DataContract]
    public class ChamberConfig : DeviceBaseConfig
    {
        [XmlArrayItem(ElementName = "Url")]
        public List<string> WebcamUrls { get; set; }


        [DataMember]
        public SlitDoorConfiguration SlitDoorConfig { get; set; }
}
}
