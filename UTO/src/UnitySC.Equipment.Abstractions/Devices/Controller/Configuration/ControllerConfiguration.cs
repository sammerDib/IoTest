using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.Controller.Configuration
{
    public class ControllerConfiguration : DeviceConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets if the wafer flow will only use upper arm to load the equipment.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool UseOnlyUpperArmToLoadEquipment { get; set; }

        #endregion Properties

        protected override void SetDefaults()
        {
            InitializationTimeout = 300;
            UseOnlyUpperArmToLoadEquipment = false;
        }
    }
}
