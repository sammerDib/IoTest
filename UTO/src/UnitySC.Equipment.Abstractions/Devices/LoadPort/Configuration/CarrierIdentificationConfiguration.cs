using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.Common.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration
{
    /// <summary>
    /// Class containing Load Port Carrier Identification parameters.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class CarrierIdentificationConfiguration
    {
        #region Properties

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool ByPassReadId { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public CarrierIDAcquisitionType CarrierIdAcquisition { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public CarrierTagLocation CarrierTagLocation { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string DefaultCarrierId { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public byte MaxNumberOfRetry { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int CarrierIdStartIndex { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int CarrierIdStopIndex { get; set; }
        #endregion

        #region Constructor

        public CarrierIdentificationConfiguration()
        {
            SetDefaults();
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<CarrierIdentificationConfiguration>");
            builder.AppendLine($"{nameof(ByPassReadId)} : {ByPassReadId}");
            builder.AppendLine($"{nameof(CarrierIdAcquisition)} : {CarrierIdAcquisition}");
            builder.AppendLine($"{nameof(CarrierTagLocation)} : {CarrierTagLocation}");
            builder.AppendLine($"{nameof(DefaultCarrierId)} : {DefaultCarrierId}");
            builder.AppendLine($"{nameof(CarrierIdStartIndex)} : {CarrierIdStartIndex}");
            builder.AppendLine($"{nameof(CarrierIdStopIndex)} : {CarrierIdStopIndex}");
            builder.AppendLine($"{nameof(MaxNumberOfRetry)} : {MaxNumberOfRetry}");

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
            ByPassReadId = false;
            CarrierIdAcquisition = CarrierIDAcquisitionType.Generate;
            CarrierTagLocation = CarrierTagLocation.Clamped;
            DefaultCarrierId = "None";
            MaxNumberOfRetry = 3;
            CarrierIdStartIndex = -1;
            CarrierIdStopIndex = -1;
        }

        #endregion
    }
}
