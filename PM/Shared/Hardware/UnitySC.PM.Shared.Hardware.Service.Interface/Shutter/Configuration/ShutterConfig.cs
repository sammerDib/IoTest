using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Shutter
{
    [Serializable]
    [XmlInclude(typeof(Sh10pilShutterConfig))]
    [DataContract]
    public class ShutterConfig : DeviceBaseConfig
    {
    }
}
