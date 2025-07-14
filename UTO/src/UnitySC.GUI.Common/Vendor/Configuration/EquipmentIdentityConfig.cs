using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace UnitySC.GUI.Common.Vendor.Configuration
{
    /// <summary>
    /// Define MDLN and SOFTREV
    /// </summary>
    [DataContract(Namespace = "")]
    [Serializable]
    public class EquipmentIdentityConfig
    {
        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentIdentityConfig"/> class.
        /// </summary>
        public EquipmentIdentityConfig()
        {
            SetDefaults();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the MDLN.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string MDLN { get; set; }

        /// <summary>
        /// Gets or sets the SOFTREV.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string SOFTREV { get; set; }


        /// <summary>
        /// Gets or sets the EqpSerialNum.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string EqpSerialNum { get; set; }


        /// <summary>
        /// Gets or sets the E30EquipmentSupplier.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string E30EquipmentSupplier { get; set; }
        #endregion

        #region Overrides

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("<EquipmentIdentityConfig>");
            builder.Append("MDLN: ").AppendLine(MDLN);
            builder.Append("SOFTREV: ").AppendLine(SOFTREV);
            builder.Append("EqpSerialNum: ").AppendLine(EqpSerialNum);
            builder.Append("E30EquipmentSupplier: ").AppendLine(E30EquipmentSupplier);

            return builder.ToString();
        }

        #endregion

        #region Private methods

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context) => SetDefaults();

        /// <summary>
        /// Sets the default values (called on deserializing and from constructor)
        /// </summary>
        private void SetDefaults()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            MDLN = assembly.GetName().Name;
            SOFTREV = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "0.0.0.0";
            EqpSerialNum = "V0.0.0";
            E30EquipmentSupplier = "Unity-SC";
        }

        #endregion
    }
}
