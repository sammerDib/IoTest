using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class EK9000Configuration : DeviceConfiguration
    {
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public ModbusConfiguration ModbusConfiguration { get; set; }

        [DataMember] public double CoefPressure { get; set; }
        [DataMember] public double CoefSpeed { get; set; }


        protected override void SetDefaults()
        {
            ModbusConfiguration = new ModbusConfiguration();
            CoefPressure = 6.6; //TODO update when we have the real value
            CoefSpeed = 1;//TODO update when we have the real value
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.Append(ModbusConfiguration);
            builder.AppendLine($"Pressure transformation coefficient: {CoefPressure}");
            builder.AppendLine($"Speed transformation coefficient: {CoefSpeed}");

            return builder.ToString();
        }
    }
}
