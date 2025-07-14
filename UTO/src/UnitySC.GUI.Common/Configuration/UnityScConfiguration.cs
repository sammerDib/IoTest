using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.GUI.Common.Vendor.Configuration;

namespace UnitySC.GUI.Common.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class UnityScConfiguration : ApplicationConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets configuration of the equipment
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public EquipmentConfiguration EquipmentConfig { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool InitRequiredAtStartup { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool UseWarmInit { get; set; }

        #endregion Properties

        #region Override methods

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(base.ToString());
            builder.AppendLine($"<{nameof(UnityScConfiguration)}>");
            builder.AppendLine(EquipmentConfig.ToString());
            builder.AppendLine($"{nameof(InitRequiredAtStartup)} : {InitRequiredAtStartup}");
            builder.AppendLine($"{nameof(UseWarmInit)} : {UseWarmInit}");
            return builder.ToString();
        }

        protected override void SetDefaults()
        {
            base.SetDefaults();

            EquipmentConfig = new EquipmentConfiguration();
            InitRequiredAtStartup = false;
            UseWarmInit = false;
        }

        #endregion Override methods
    }
}
