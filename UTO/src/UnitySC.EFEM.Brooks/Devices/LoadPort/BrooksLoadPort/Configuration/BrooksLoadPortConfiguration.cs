using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;

namespace UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort.Configuration
{
    public class BrooksLoadPortConfiguration : LoadPortConfiguration
    {
        /// <summary>
        /// Gets or sets the name of load port in Brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksLoadPortName { get; set; }


        /// <summary>
        /// Gets or sets the name of light curtain signal name.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string LightCurtainNodeSignal { get; set; }

        protected override void SetDefaults()
        {
            BrooksLoadPortName = "LoadPortA";
            LightCurtainNodeSignal = "EFEM.LoadPortA.LightCurtain";
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"{nameof(BrooksLoadPortName)}: {BrooksLoadPortName}");
            builder.AppendLine($"{nameof(LightCurtainNodeSignal)}: {LightCurtainNodeSignal}");

            return builder.ToString();
        }
    }
}
