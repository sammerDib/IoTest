using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor
{
    [Serializable]
    [XmlInclude(typeof(MicroEpsilonDistanceSensorConfig))]
    [DataContract]
    public class DistanceSensorConfig : DeviceBaseConfig
    {
    }
}
