using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Configuration;

namespace UnitySC.EFEM.Brooks.Devices.SubstrateIdReader.BrooksSubstrateIdReader.Configuration
{
    public class BrooksSubstrateIdReaderConfiguration : SubstrateIdReaderConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of aligner in Brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksReaderName { get; set; }

        #endregion Properties

        #region Override methods

        protected override void SetDefaults()
        {
            base.SetDefaults();

            BrooksReaderName = "SubstrateIdReader";
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"{nameof(BrooksReaderName)}: {BrooksReaderName}");

            return builder.ToString();
        }

        #endregion Override methods
    }
}
