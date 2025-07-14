using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Rfid
{
    [Serializable]
    [XmlInclude(typeof(BisL405RfidConfig))]
    [DataContract]
    public class RfidConfig : DeviceBaseConfig
    {
    }
}
