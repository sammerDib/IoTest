using System;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.Mppc
{
    [Serializable]
    public class MppcConfig : IDeviceConfiguration
    {
        public string Name { get; set; }
        public string DeviceID { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSimulated { get; set; }
        public DeviceLogLevel LogLevel { get; set; }
    }
}