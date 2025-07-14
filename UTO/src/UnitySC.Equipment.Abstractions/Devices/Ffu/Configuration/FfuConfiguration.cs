using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.Ffu.Configuration
{
    /// <summary>
    /// Class containing FFU parameters.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class FfuConfiguration : DeviceConfiguration
    {
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public double FfuSetPoint { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public double LowPressureThreshold { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public double LowSpeedThreshold { get; set; }

        protected override void SetDefaults()
        {
            base.SetDefaults();

            FfuSetPoint = 0;
            LowPressureThreshold = 0;
            LowSpeedThreshold = 0;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"FfuSetPoint: {FfuSetPoint}");
            builder.AppendLine($"LowPressureThreshold: {LowPressureThreshold}");
            builder.AppendLine($"LowSpeedThreshold: {LowSpeedThreshold}");

            return builder.ToString();
        }
    }
}
