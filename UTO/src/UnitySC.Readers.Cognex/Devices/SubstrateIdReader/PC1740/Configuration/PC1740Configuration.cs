using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Configuration;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Configuration
{
    /// <summary>
    /// Defines configuration parameters for a cognex PC-1740 substrate ID reader.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    // ReSharper disable once InconsistentNaming
    public class PC1740Configuration : SubstrateIdReaderConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the recipe folder path.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember]
        public string RecipeFolderPath { get; set; }

        /// <summary>
        /// Gets or sets whether only one T7 recipe is used.
        /// </summary>
        /// <remarks>
        /// When <see langword="true"/> T7 recipe comes from <see cref="T7Recipe"/>.
        /// When <see langword="false"/> recipes present in <see cref="RecipeFolderPath"/> are used.
        /// </remarks>
        [XmlElement(IsNullable = false)]
        [DataMember]
        public bool UseOnlyOneT7 { get; set; }

        /// <summary>
        /// Gets or sets the recipe to use when <see cref="UseOnlyOneT7"/> is <see langword="true"/>.
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public T7RecipeConfiguration T7Recipe { get; set; }

        /// <summary>
        /// Gets or sets the communication configuration.
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public CommunicationConfiguration CommunicationConfig { get; set; }

        #endregion Properties

        #region Override methods

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"Recipe Folder Path: {RecipeFolderPath}");
            builder.AppendLine($"Use Only One T7: {UseOnlyOneT7}");
            builder.AppendLine(T7Recipe?.ToString());
            builder.Append(CommunicationConfig);

            return builder.ToString();
        }

        /// <summary>
        /// Sets the default values (called on deserializing and from constructor)
        /// </summary>
        protected override void SetDefaults()
        {
            base.SetDefaults();

            RecipeFolderPath    = @"./EFEM/SubstrateIdReader/Resources/OCRecipe";
            UseOnlyOneT7        = false;
            T7Recipe            = null;
            CommunicationConfig = new CommunicationConfiguration();
        }

        #endregion Override methods
    }
}
