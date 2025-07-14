using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Devices.ProcessModule.Configuration;

namespace UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class ToolControlProcessModuleConfiguration : ProcessModuleConfiguration
    {
        #region Properties

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string ModuleId { get; set; }

        #endregion
    }
}
