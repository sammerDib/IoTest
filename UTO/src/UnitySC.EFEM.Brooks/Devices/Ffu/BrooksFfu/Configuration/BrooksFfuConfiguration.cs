using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Configuration;

namespace UnitySC.EFEM.Brooks.Devices.Ffu.BrooksFfu.Configuration
{
    public class BrooksFfuConfiguration : FfuConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of ffu in Brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksFfuName { get; set; }

        #endregion

        #region Override

        protected override void SetDefaults()
        {
            base.SetDefaults();

            BrooksFfuName = "FFU";
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"{nameof(BrooksFfuName)}: {BrooksFfuName}");

            return builder.ToString();
        }

        #endregion
    }
}
