using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace UnitySC.GUI.Common.Configuration
{
    /// <summary>
    /// Defines the Equipment.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class EquipmentConfiguration
    {
        #region Configuration

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentConfiguration"/> class.
        /// </summary>
        public EquipmentConfiguration()
        {
            SetDefaults();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path to root folder containing Devices configuration
        /// </summary>
        /// <value>Configuration\Equipments\Devices</value>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public string DeviceConfigFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the path to folder containing Equipment files
        /// </summary>
        /// <value>Configuration\Equipments</value>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public string EquipmentsFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the a value indicating the equipment file to load.
        /// </summary>
        [XmlElement]
        [DataMember]
        public string EquipmentFileName { get; set; }

        /// <summary>
        /// Gets or sets whether low-level communication logs (socket callbacks, etc.) are written.
        /// </summary>
        /// <remarks>This could generate a large amount of traces and should be enabled only temporarily for debugging purpose.</remarks>
        [XmlElement]
        [DataMember]
        public bool IsSocketLogEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the process modules are graphically inverted.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool InvertPmOnUserInterface { get; set; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("<Equipment Configuration>");
            builder.AppendLine($"Equipments folder path: {EquipmentsFolderPath}");
            builder.AppendLine($"Devices config folder path: {DeviceConfigFolderPath}");
            builder.AppendLine($"Equipment file name: {EquipmentFileName}");
            builder.AppendLine($"Is socket log enabled: {IsSocketLogEnabled}");
            builder.AppendLine($"Invert process modules on user interface : {InvertPmOnUserInterface}");
            return builder.ToString();
        }

        #endregion

        #region Private Methods

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            SetDefaults();
        }

        /// <summary>
        /// Sets the default values (called on deserializing and from constructor)
        /// </summary>
        private void SetDefaults()
        {
            DeviceConfigFolderPath = @"Configuration\Equipments\Devices";
            EquipmentsFolderPath = @"Configuration\Equipments";
            EquipmentFileName = "Efem_Slim.equipment";
            IsSocketLogEnabled = false;
            InvertPmOnUserInterface = false;
        }

        #endregion
    }
}
