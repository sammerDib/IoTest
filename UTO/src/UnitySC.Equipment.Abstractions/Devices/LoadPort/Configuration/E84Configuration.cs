using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.Common.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration
{
    /// <summary>
    /// Class containing Load Port E84 parameters.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class E84Configuration
    {

        #region Properties

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int Tp1 { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int Tp2 { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int Tp3 { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int Tp4 { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int Tp5 { get; set; }

        #endregion

        #region Constructor

        public E84Configuration()
        {
            SetDefaults();
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<E84Configuration>");
            builder.AppendLine($"{nameof(Tp1)} : {Tp1}");
            builder.AppendLine($"{nameof(Tp2)} : {Tp2}");
            builder.AppendLine($"{nameof(Tp3)} : {Tp3}");
            builder.AppendLine($"{nameof(Tp4)} : {Tp4}");
            builder.AppendLine($"{nameof(Tp5)} : {Tp5}");

            return builder.ToString();
        }

        #endregion

        #region Private Methods

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            Tp1 = 2;
            Tp2 = 2;
            Tp3 = 60;
            Tp4 = 60;
            Tp5 = 2;
        }

        #endregion
    }
}
