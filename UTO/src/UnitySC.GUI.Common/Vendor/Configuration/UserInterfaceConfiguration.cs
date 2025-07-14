using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace UnitySC.GUI.Common.Vendor.Configuration
{
    /// <summary>
    /// Defines the User Interface Configuration
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class UserInterfaceConfiguration
    {
        #region Constructor

        public UserInterfaceConfiguration()
        {
            SetDefaults();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the application theme folder path.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string ThemeFolder { get; set; }

        /// <summary>
        /// Gets or sets the application theme.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string Theme { get; set; }

        /// <summary>
        /// Gets or sets the application font scale.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public double FontScale { get; set; }

        /// <summary>
        /// Gets or sets the application global scale.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public double GlobalScale { get; set; }

        #endregion

        #region Private Methods

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            SetDefaults();
        }

        protected void SetDefaults()
        {
            ThemeFolder = "Configuration/Themes/";
            Theme = "Light";
            FontScale = 1.0;
            GlobalScale = 1.0;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("<UserInterface Configuration>");
            builder.AppendLine($"ThemeFolder: {ThemeFolder}");
            builder.AppendLine($"Theme: {Theme}");
            builder.AppendLine($"Font scale: {FontScale}");
            builder.AppendLine($"Button scale: {GlobalScale}");
            return builder.ToString();
        }

        #endregion
    }
}
