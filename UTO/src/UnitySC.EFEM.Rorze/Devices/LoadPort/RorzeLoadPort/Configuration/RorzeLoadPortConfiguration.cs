using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.Common.Configuration;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Configuration
{
    /// <summary>
    /// Class containing Rorze Load Port parameters.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class RorzeLoadPortConfiguration : LoadPortConfiguration
    {
        /// <summary>
        /// If set to true, do not use <see cref="CarrierIdStartPage"/> and <see cref="CarrierIdStopPage"/>.
        /// </summary>
        public bool UseDefaultPageIntervalForReading { get; set; }

        /// <summary>
        /// Start page for carrier ID when reading <see cref="CarrierIDAcquisitionType.TagReader"/>.
        /// </summary>
        public uint CarrierIdStartPage { get; set; }

        /// <summary>
        /// Stop page for carrier ID when reading <see cref="CarrierIDAcquisitionType.TagReader"/>.
        /// </summary>
        public uint CarrierIdStopPage { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public CommunicationConfiguration CommunicationConfig { get; set; }

        protected override void SetDefaults()
        {
            UseDefaultPageIntervalForReading = true;
            CarrierIdStartPage               = 1;
            CarrierIdStopPage                = 2;
            CommunicationConfig              = new CommunicationConfiguration();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"Use default page interval for reading: {UseDefaultPageIntervalForReading}");
            builder.AppendLine($"Start carrier ID page: {CarrierIdStartPage}");
            builder.AppendLine($"Stop carrier ID page: {CarrierIdStopPage}");
            builder.Append(CommunicationConfig);

            return builder.ToString();
        }
    }
}
