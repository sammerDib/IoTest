using System;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.Led
{
    [Serializable]
    public class LedConfig : IDeviceConfiguration
    {
        public string Name { get; set; }
        public string DeviceID { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSimulated { get; set; }
        public DeviceLogLevel LogLevel { get; set; }

        /// <summary>
        /// IP of the TCP connection
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Port of the TCP connection
        /// </summary>
        public int Port { get; set; }
    }
}