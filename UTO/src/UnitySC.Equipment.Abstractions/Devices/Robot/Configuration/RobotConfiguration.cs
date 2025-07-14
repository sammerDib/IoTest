using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.Robot.Configuration
{
    /// <summary>
    /// Class containing Robot parameters.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class RobotConfiguration : DeviceConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the upper arm is enabled or disabled.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public ArmConfiguration UpperArm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the lower arm is enabled or disabled.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public ArmConfiguration LowerArm { get; set; }

        #endregion Properties

        #region Override methods

        protected override void SetDefaults()
        {
            base.SetDefaults();

            UpperArm = new ArmConfiguration();
            LowerArm = new ArmConfiguration();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"UpperArm:{Environment.NewLine}{UpperArm}");
            builder.AppendLine($"LowerArm:{Environment.NewLine}{LowerArm}");

            return builder.ToString();
        }

        #endregion Override methods
    }
}
