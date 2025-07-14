using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    [XmlInclude(typeof(LineMotObjectivesSelectorConfig))]
    [XmlInclude(typeof(SingleObjectiveSelectorConfig))]
    [DataContract]
    public abstract class ObjectivesSelectorConfigBase : IDeviceConfiguration
    {
        [DataMember]
        public bool IsSimulated { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public DeviceLogLevel LogLevel { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        /// <summary>List of the objectives available for the current objective selector</summary>
        public List<ObjectiveConfig> Objectives { get; set; }

        public ObjectiveConfig MainObjective => Objectives.FirstOrDefault(_ => _.IsMainObjective);

        public ObjectiveConfig FindObjective(string objectiveID)
        {
            return Objectives.Find(objective => objective.DeviceID == objectiveID);
        }
    }
}
