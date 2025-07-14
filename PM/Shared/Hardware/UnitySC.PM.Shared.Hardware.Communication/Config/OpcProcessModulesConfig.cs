using System.Collections.Generic;

namespace UnitySC.PM.Shared.Hardware.Communication
{
    public class OpcProcessModulesConfig
    {
        public Dictionary<string, OpcPmConfig> OpcPmsConfigs { get; set; }
    }

    public class OpcPmConfig
    {
        public OpcAttribute DeviceID { get; set; }

        public OpcAttribute IsEnabled { get; set; }

        public List<OpcDeviceConfig> DevicesConfigs { get; set; }
    }

    public class OpcDeviceConfig
    {
        public OpcAttribute DeviceName { get; set; }

        public OpcAttribute IsEnabled { get; set; }
    }
}