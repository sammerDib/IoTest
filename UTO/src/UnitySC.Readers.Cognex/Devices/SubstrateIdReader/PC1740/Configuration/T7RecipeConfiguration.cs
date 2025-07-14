using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Configuration
{
    /// <summary>
    /// Defines configuration for "single set" T7 recipe.
    /// </summary>
    /// <remarks>Properties coming from Rorze.ini, could need renaming/documentation once purpose is best known.</remarks>
    [Serializable]
    [DataContract(Namespace = "")]
    public class T7RecipeConfiguration
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T7RecipeConfiguration"/> class.
        /// </summary>
        public T7RecipeConfiguration()
        {
            SetDefaults();
        }

        #endregion Constructors

        #region Properties

        [XmlElement(IsNullable = false)]
        [DataMember]
        public double Angle { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember]
        public double Angle8 { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember]
        public DateTime Date { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember]
        public string Name { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"<{GetType().Name}>");
            builder.AppendLine($"Date: {Date}");
            builder.AppendLine($"Name: {Name}");
            builder.AppendLine($"Angle: {Angle}");
            builder.AppendLine($"Angle8: {Angle8}");

            return builder.ToString();
        }

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
            Date   = DateTime.Now;
            Name   = "T7";
            Angle  = 0;
            Angle8 = 0;
        }

        #endregion Methods
    }
}
