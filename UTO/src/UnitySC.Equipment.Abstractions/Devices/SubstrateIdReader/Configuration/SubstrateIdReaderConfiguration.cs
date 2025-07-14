using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Configuration
{
    /// <summary>
    /// Defines Substrate Id reader configuration.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class SubstrateIdReaderConfiguration : DeviceConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string ImagePath { get; set; }

        #endregion Properties

        #region Override methods

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"Image Path: {ImagePath}");

            return builder.ToString();
        }

        /// <summary>
        /// Sets the default values (called on deserializing and from constructor)
        /// </summary>
        protected override void SetDefaults()
        {
            ImagePath = @"./EFEM/SubstrateIdReader/Resources/Image/ReadFail.bmp";
        }

        #endregion Override methods
    }
}
