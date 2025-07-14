using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Light;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class LightModuleConfig : IDeviceConfiguration
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public bool IsSimulated { get; set; }

        [DataMember]    
        public DeviceLogLevel LogLevel { get; set; }

        [DataMember]
        [XmlArray("Lights")]
        [XmlArrayItem("Light")]
        public List<LightConfig> Lights { get; set; }
    }
}
