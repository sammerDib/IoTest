using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Plc;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Plc
{
    [Serializable]
    [XmlInclude(typeof(BeckhoffPlcConfig))]
    [DataContract]
    public class PlcConfig : DeviceBaseConfig
    {
    }
}
