using System;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.Wheel
{
    [Serializable]
    public class WheelConfig : IDeviceConfiguration
    {
        public string Name { get; set; }
        public string DeviceID { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSimulated { get; set; }
        public DeviceLogLevel LogLevel { get; set; }
    }
}
