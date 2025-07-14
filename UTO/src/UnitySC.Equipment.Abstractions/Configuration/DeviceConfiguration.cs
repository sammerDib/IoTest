using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.Common.Configuration;

namespace UnitySC.Equipment.Abstractions.Configuration
{
    /// <summary>
    /// Base class for any device configuration.
    /// Used to streamline code needed by IConfiguration and serialization.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "", Name = "Configuration")]
    [XmlRoot("Configuration", IsNullable = false)]
    public abstract class DeviceConfiguration : IConfiguration
    {
        #region Properties

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int InitializationTimeout { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="DeviceConfiguration"/>.
        /// </summary>
        protected DeviceConfiguration()
        {
            SetDefaultsInternal();
        }

        #endregion Constructor

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

            builder.AppendLine($"<{GetType().Name}>");
            builder.AppendLine($"InitializationTimeout: {InitializationTimeout}s");

            return builder.ToString();
        }

        #endregion Overrides

        #region IConfiguration

        /// <summary>
        /// Validates configuration data and returns information about data conflicts.
        /// </summary>
        /// <returns>String information of wrong configuration data.</returns>
        public virtual string ValidatedParameters()
        {
            return string.Empty;
        }

        /// <summary>
        /// Analyzes configuration data and returns information about data conflicts
        /// Mandatory to a proper work of equipment.
        /// Alarm will be generated in case of any conflict in this case.
        /// </summary>
        /// <returns>String information of wrong configuration data.</returns>
        public virtual string ValidatingParameters()
        {
            return string.Empty;
        }

        #endregion IConfiguration

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
            InitializationTimeout = 60;
        }

        #endregion Default values
    }
}
