using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.Aligner.Configuration
{
    /// <summary>
    /// Class containing Aligner parameters.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class AlignerConfiguration : DeviceConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the offset (in degrees) that should be applied to each alignment.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public double AlignOffset { get; set; }

        #endregion Properties

        #region Override methods

        protected override void SetDefaults()
        {
            base.SetDefaults();

            AlignOffset = 0;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"AlignOffset: {AlignOffset:F2}°");

            return builder.ToString();
        }

        #endregion Override methods
    }
}
