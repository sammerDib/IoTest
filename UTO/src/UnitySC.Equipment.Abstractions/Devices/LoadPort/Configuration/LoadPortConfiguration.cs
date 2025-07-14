using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration
{
    /// <summary>
    /// Class containing LoadPort parameters.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class LoadPortConfiguration : DeviceConfiguration
    {
        #region Properties

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public CarrierIdentificationConfiguration CarrierIdentificationConfig { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public CassetteType CassetteType { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool IsInService { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool IsCarrierIdSupported { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool IsMappingSupported { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool IsE84Enabled { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public HandOffType HandOffType { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public byte AutoHandOffTimeout { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public E84Configuration E84Configuration { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool IsManualCarrierType { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public List<CarrierType> CarrierTypes { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool CloseDoorAfterRobotAction { get; set; }

        #endregion Properties

        #region Override methods

        protected override void SetDefaults()
        {
            base.SetDefaults();

            CarrierIdentificationConfig = new CarrierIdentificationConfiguration();
            CassetteType         = CassetteType.Foup;
            IsInService          = true;
            IsCarrierIdSupported = true;
            IsMappingSupported   = true;
            IsE84Enabled         = false;
            HandOffType = HandOffType.Manual;
            AutoHandOffTimeout = 5;
            E84Configuration = new E84Configuration();
            IsManualCarrierType = false;
            CarrierTypes = new List<CarrierType>(16);
            CloseDoorAfterRobotAction = false;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"CassetteType: {CassetteType}");
            builder.AppendLine($"IsInService: {IsInService}");
            builder.AppendLine($"IsCarrierIdSupported: {IsCarrierIdSupported}");
            builder.AppendLine($"CarrierIdentificationConfig : {CarrierIdentificationConfig}");
            builder.AppendLine($"IsMappingSupported: {IsMappingSupported}");
            builder.AppendLine($"IsE84Enabled: {IsE84Enabled}");
            builder.AppendLine($"HandOffType: {HandOffType}");
            builder.AppendLine($"AutoHandOffTimeout: {AutoHandOffTimeout}");
            builder.AppendLine($"E84Configuration : {E84Configuration}");
            builder.AppendLine($"IsManualCarrierType: {IsManualCarrierType}");
            builder.AppendLine($"CloseDoorAfterRobotAction: {CloseDoorAfterRobotAction}");

            return builder.ToString();
        }

        #endregion Override methods
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class CarrierType
    {
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public uint Id { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public MaterialType MaterialType { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }
    }
}
