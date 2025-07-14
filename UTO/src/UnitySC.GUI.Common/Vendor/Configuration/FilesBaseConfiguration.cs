using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.Recipes.Components;
using Agileo.Recipes.Management.StorageFormats;

namespace UnitySC.GUI.Common.Vendor.Configuration
{
    /// <summary>
    /// Define the configuration of file element (Recipe, Scenario)
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class FilesBaseConfiguration
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesBaseConfiguration"/> class.
        /// </summary>
        protected FilesBaseConfiguration()
        {
            Init();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>\Manual\UM.pdf</value>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string Path { get; set; }


        /// <summary>
        /// Gets or sets a value indicating the machine type.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string FileExtension { get; set; }


        /// <summary>
        /// Gets or sets a value indicating the file storage format.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(IsRequired = true)]
        public StorageFormat StorageFormat { get; set; }


        [XmlElement(IsNullable = false)]
        [DataMember(IsRequired = true)]
        public List<RecipeGroup> Groups { get; set; } = new();

        #endregion

        #region Private Methods

        #region Overrides

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Path: {Path}");
            builder.AppendLine($"File Extension: {FileExtension}");
            builder.AppendLine($"StorageFormat: {StorageFormat}");
            builder.AppendLine("<GroupsConfiguration>");
            Groups.ForEach(grp =>
            {
                builder.Append("Group: ")
                    .Append(grp.Name)
                    .Append(" (")
                    .Append(grp.AccessLevel)
                    .Append(')')
                    .AppendLine();
            });
            return builder.ToString();
        }

        #endregion

        private void Init()
        {
            SetDefaults();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context) => SetDefaults();

        /// <summary>
        /// Sets the default values (called on deserializing and from constructor)
        /// </summary>
        protected abstract void SetDefaults();

        #endregion
    }
}
