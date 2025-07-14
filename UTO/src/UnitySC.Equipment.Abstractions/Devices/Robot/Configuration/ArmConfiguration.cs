using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.Robot.Configuration
{
    /// <summary>
    /// Class containing Robot's arm parameters.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class ArmConfiguration
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="ArmConfiguration"/>.
        /// </summary>
        public ArmConfiguration()
        {
            SetDefaultsInternal();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the arm is enabled or disabled.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The type of end-effector installed on this arm.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public EffectorType EffectorType { get; set; }

        /// <summary>
        /// List of supported substrate sizes on this arm.
        /// </summary>
        [XmlArray(ElementName = "SupportedSubstrateSizes", IsNullable = false)]
        [XmlArrayItem(ElementName = "SampleDimension")]
        [DataMember(EmitDefaultValue = false)]
        public List<SampleDimension> SupportedSubstrateSizes { get; set; }

        /// <summary>
        /// List of supported substrate types on this arm.
        /// </summary>
        [XmlArray(ElementName = "SupportedSubstrateTypes", IsNullable = false)]
        [XmlArrayItem(ElementName = "SubstrateType")]
        [DataMember(EmitDefaultValue = false)]
        public List<SubstrateType> SupportedSubstrateTypes { get; set; }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"IsEnabled: {IsEnabled}");
            builder.AppendLine($"EffectorType: {EffectorType}");
            builder.AppendLine($"Substrate sizes: {string.Join(", ", SupportedSubstrateSizes)}");
            builder.AppendLine($"Substrate types: {string.Join(", ", SupportedSubstrateTypes)}");

            return builder.ToString();
        }

        #endregion Overrides

        #region Default values

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            SetDefaultsInternal();
        }

        private void SetDefaultsInternal()
        {
            SetDefaults();
        }

        /// <summary>
        /// Sets the default values (called on deserializing and from constructor)
        /// </summary>
        protected virtual void SetDefaults()
        {
            IsEnabled               = true;
            EffectorType            = EffectorType.VacuumI;
            SupportedSubstrateSizes = new List<SampleDimension>();
            SupportedSubstrateTypes = new List<SubstrateType>();
        }

        #endregion Default values
    }
}
