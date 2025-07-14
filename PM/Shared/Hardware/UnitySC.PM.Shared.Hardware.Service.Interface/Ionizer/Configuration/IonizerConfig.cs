using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer
{
    [Serializable]
    [XmlInclude(typeof(KeyenceIonizerConfig))]
    [DataContract]
    public class IonizerConfig : DeviceBaseConfig
    {
    }
}
