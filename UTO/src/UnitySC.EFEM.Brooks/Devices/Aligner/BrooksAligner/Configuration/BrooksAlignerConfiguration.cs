using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Configuration;

namespace UnitySC.EFEM.Brooks.Devices.Aligner.BrooksAligner.Configuration
{
    public class BrooksAlignerConfiguration : AlignerConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of aligner in Brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksAlignerName { get; set; }

        /// <summary>
        /// Gets or sets the name of aligner chuck in Brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksChuckName { get; set; }

        #endregion Properties

        #region Override methods

        protected override void SetDefaults()
        {
            base.SetDefaults();

            BrooksAlignerName = "WaferAligner";
            BrooksChuckName = "EFEM.WaferAligner.Chuck";
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"{nameof(BrooksAlignerName)}: {BrooksAlignerName}");
            builder.AppendLine($"{nameof(BrooksChuckName)}: {BrooksChuckName}");

            return builder.ToString();
        }

        #endregion Override methods
    }
}
