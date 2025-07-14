using System;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Light
{
    [Serializable]
    [XmlInclude(typeof(ACSLightConfig))]
    [XmlInclude(typeof(ENTTECLightConfig))]
    public class LightConfig : DeviceBaseConfig
    {
        /// <summary>Default intensity to set at the start of application (in %)</summary>
        public double DefaultIntensity { get; set; }

        public string Description { get; set; }

        public bool IsMainLight { get; set; }

        public Length Wavelength { get; set; }

        [XmlIgnore]
        public ModulePositions Position { get; set; } // UP or DOWN (TOP or BOTTOM)
    }
}
