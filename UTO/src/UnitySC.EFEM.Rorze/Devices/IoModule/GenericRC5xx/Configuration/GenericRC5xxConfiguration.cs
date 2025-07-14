using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class GenericRC5xxConfiguration : DeviceConfiguration
    {
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public CommunicationConfiguration CommunicationConfig { get; set; }

        protected override void SetDefaults()
        {
            base.SetDefaults();

            CommunicationConfig = new CommunicationConfiguration();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.Append(CommunicationConfig);

            return builder.ToString();
        }
    }
}
