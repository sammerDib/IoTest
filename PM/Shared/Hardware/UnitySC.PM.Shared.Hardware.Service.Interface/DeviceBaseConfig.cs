using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class DeviceBaseConfig : IDeviceConfiguration
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        public DeviceLogLevel LogLevel { get; set; }

        [DataMember]
        public string ControllerID { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public bool IsSimulated { get; set; }
    }
}
