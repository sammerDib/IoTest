using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.ProcessModule.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class ProcessModuleConfiguration : DeviceConfiguration
    {
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool IsOutOfService { get; set; }
    }
}
