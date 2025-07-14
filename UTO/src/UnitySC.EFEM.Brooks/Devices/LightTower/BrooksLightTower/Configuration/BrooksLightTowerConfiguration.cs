using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Devices.LightTower.Configuration;

namespace UnitySC.EFEM.Brooks.Devices.LightTower.BrooksLightTower.Configuration
{
    public class BrooksLightTowerConfiguration : LightTowerConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of aligner in Brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksLightTowerName { get; set; }

        #endregion Properties

        #region Override methods

        protected override void SetDefaults()
        {
            base.SetDefaults();

            BrooksLightTowerName = "LightTower";
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"{nameof(BrooksLightTowerName)}: {BrooksLightTowerName}");

            return builder.ToString();
        }

        #endregion Override methods
    }
}
