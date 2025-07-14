using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Laser
{
    [Serializable]
    [XmlInclude(typeof(Piano450LaserConfig))]
    [XmlInclude(typeof(SMD12LaserConfig))]
    [DataContract]
    public class LaserConfig : DeviceBaseConfig
    {
        [DataMember]
        public double MinPower { get; set; }

        [DataMember]
        public double MaxPower { get; set; }
    }
}
