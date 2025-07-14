using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Configuration;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Dio0Configuration : GenericRC5xxConfiguration
    {
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool IsPressureSensorAvailable { get; set; }

        protected override void SetDefaults()
        {
            base.SetDefaults();

            IsPressureSensorAvailable = true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"{nameof(IsPressureSensorAvailable)}: {IsPressureSensorAvailable}");

            return builder.ToString();
        }
    }
}
