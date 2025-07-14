using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Devices.Efem.Configuration;

namespace UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem.Configuration
{
    public class BrooksEfemConfiguration : EfemConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of Brooks client.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksClientName { get; set; }

        /// <summary>
        /// Gets or sets the name of efem in Brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksEfemName { get; set; }

        /// <summary>
        /// Gets or sets the name of Dio in Brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksDioName { get; set; }

        /// <summary>
        /// Gets or sets the name of the load port 1 location in brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksLoadPort1LocationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the load port 1 location in brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksLoadPort2LocationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the load port 1 location in brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksLoadPort3LocationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the load port 1 location in brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksLoadPort4LocationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the process module A location in brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksProcessModuleALocationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the process module B location in brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksProcessModuleBLocationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the process module C location in brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksProcessModuleCLocationName { get; set; }

        /// <summary>
        /// Gets or sets the name of air node signal for DIO.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string AirNodeSignal { get; set; }


        /// <summary>
        /// Gets or sets the name of pressure node signal for DIO.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string PressureNodeSignal { get; set; }


        /// <summary>
        /// Gets or sets the name of door sensor 1 node signal for DIO.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string DoorSensor1NodeSignal { get; set; }


        /// <summary>
        /// Gets or sets the name of door sensor 2 node signal for DIO.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string DoorSensor2NodeSignal { get; set; }


        /// <summary>
        /// Gets or sets the name of interlock sensor 1 node signal for DIO.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string InterlockSensor1NodeSignal { get; set; }


        /// <summary>
        /// Gets or sets the name of interlock sensor 2 node signal for DIO.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string InterlockSensor2NodeSignal { get; set; }

        #endregion Properties

        #region Override methods

        protected override void SetDefaults()
        {
            base.SetDefaults();

            BrooksClientName = "CTC";
            BrooksEfemName = "EFEM";
            BrooksDioName = "EFEM.Device.DIO";
            BrooksLoadPort1LocationName = "EFEM.PortA";
            BrooksLoadPort2LocationName = "EFEM.PortB";
            BrooksLoadPort3LocationName = "EFEM.PortC";
            BrooksLoadPort4LocationName = "EFEM.PortD";
            BrooksProcessModuleALocationName = "EFEM.PortU";
            BrooksProcessModuleBLocationName = "EFEM.PortV";
            BrooksProcessModuleCLocationName = "EFEM.PortW";
            AirNodeSignal = "VBE AR SIC";
            PressureNodeSignal = "VBE PRES1";
            DoorSensor1NodeSignal = "VBE EFEM DR CLSD-1";
            DoorSensor2NodeSignal = "VBE EFEM DR CLSD-2";
            InterlockSensor1NodeSignal = "INTLK1 SW IN CH1";
            InterlockSensor2NodeSignal = "INTLK1 SW IN CH2";
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.AppendLine($"{nameof(BrooksClientName)}: {BrooksClientName}");
            builder.AppendLine($"{nameof(BrooksEfemName)}: {BrooksEfemName}");
            builder.AppendLine($"{nameof(BrooksDioName)}: {BrooksDioName}");
            builder.AppendLine($"{nameof(BrooksLoadPort1LocationName)}: {BrooksLoadPort1LocationName}");
            builder.AppendLine($"{nameof(BrooksLoadPort2LocationName)}: {BrooksLoadPort2LocationName}");
            builder.AppendLine($"{nameof(BrooksLoadPort3LocationName)}: {BrooksLoadPort3LocationName}");
            builder.AppendLine($"{nameof(BrooksLoadPort4LocationName)}: {BrooksLoadPort4LocationName}");
            builder.AppendLine($"{nameof(BrooksProcessModuleALocationName)}: {BrooksProcessModuleALocationName}");
            builder.AppendLine($"{nameof(BrooksProcessModuleBLocationName)}: {BrooksProcessModuleBLocationName}");
            builder.AppendLine($"{nameof(BrooksProcessModuleCLocationName)}: {BrooksProcessModuleCLocationName}");
            builder.AppendLine($"{nameof(AirNodeSignal)}: {AirNodeSignal}");
            builder.AppendLine($"{nameof(PressureNodeSignal)}: {PressureNodeSignal}");
            builder.AppendLine($"{nameof(DoorSensor1NodeSignal)}: {DoorSensor1NodeSignal}");
            builder.AppendLine($"{nameof(DoorSensor2NodeSignal)}: {DoorSensor2NodeSignal}");
            builder.AppendLine($"{nameof(InterlockSensor1NodeSignal)}: {InterlockSensor1NodeSignal}");
            builder.AppendLine($"{nameof(InterlockSensor2NodeSignal)}: {InterlockSensor2NodeSignal}");

            return builder.ToString();
        }

        #endregion Override methods
    }
}
