using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.Efem.Configuration
{
    public class EfemConfiguration : DeviceConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the light curtain security activation.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool LightCurtainSecurityEnabled { get; set; }

        /// <summary>
        /// Gets or sets the light curtain wiring.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public LightCurtainWiring LightCurtainWiring { get; set; }

        #endregion Properties

        protected override void SetDefaults()
        {
            InitializationTimeout = 300;
            LightCurtainSecurityEnabled = true;
            LightCurtainWiring = LightCurtainWiring.NPN;
        }
    }
}
