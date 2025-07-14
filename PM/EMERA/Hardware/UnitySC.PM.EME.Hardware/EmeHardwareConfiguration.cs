using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Wheel;

namespace UnitySC.PM.EME.Hardware
{
    [Serializable]
    public class EmeHardwareConfiguration : HardwareConfiguration
    {
        [XmlArray("Lights")]
        [XmlArrayItem("Light")]
        public List<EMELightConfig> Lights { get; set; }
        
        [XmlArrayItem(typeof(FilterWheelConfig))]
        public List<WheelConfig> WheelConfigs { get; set; }
    }
}
