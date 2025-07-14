using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen
{
    [Serializable]
    [XmlInclude(typeof(DensitronDM430GNScreenConfig))]
    [DataContract]
    public class ScreenConfig : DeviceBaseConfig
    {
        [DataMember]
        public short BacklightPercentage { get; set; }

        [DataMember]
        public short BrightnessPercentage { get; set; }

        [DataMember]
        public short ContrastPercentage { get; set; }

        [DataMember]
        public int Sharpness { get; set; }
    }
}
