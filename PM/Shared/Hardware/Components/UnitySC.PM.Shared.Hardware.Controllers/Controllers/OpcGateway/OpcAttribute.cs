using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.Controller
{
    public class OpcAttribute
    {
        public string DisplayName { get; set; }

        public string Identifier { get; set; }

        public object Value { get; set; }
    }

    public class OpcDevice
    {
        public string DeviceName { get; set; }

        public string DeviceIdentifier { get; set; }

        public Dictionary<string, List<OpcAttribute>> Attributes { get; set; }
    }
}
