using System.Collections.Generic;

namespace UnitySC.PM.Shared.Hardware.Communication
{
    public class OpcProcessModules
    {
        public Dictionary<string, List<OpcDevice>> OpcPms { get; set; }
    }

    public class OpcDevice
    {
        public string DeviceName { get; set; }

        public string DeviceIdentifier { get; set; }

        public Dictionary<string, List<OpcAttribute>> Attributes { get; set; }
    }
}