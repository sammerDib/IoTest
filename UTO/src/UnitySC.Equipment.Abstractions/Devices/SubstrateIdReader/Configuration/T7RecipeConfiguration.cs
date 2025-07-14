using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitsNet;

namespace UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Configuration
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
            SetDefaultsInternal();
        }

        #endregion Constructors

        #region Properties

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public Angle Angle { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public Angle Angle8 { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public DateTime Date { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        #endregion Properties

        #region Overrides

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Date: {Date}");
            builder.AppendLine($"Name: {Name}");
            builder.AppendLine($"Angle: {Angle}");
            builder.AppendLine($"Angle8: {Angle8}");

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
            Date   = DateTime.Now;
            Name   = "T7";
            Angle  = Angle.FromDegrees(0);
            Angle8 = Angle.FromDegrees(0);
        }

        #endregion Methods
    }
}
