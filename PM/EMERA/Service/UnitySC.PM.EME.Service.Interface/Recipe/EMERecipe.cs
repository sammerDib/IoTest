using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnitySC.DataAccess.Dto;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.PM.Shared.Data;

namespace UnitySC.PM.EME.Service.Interface.Recipe
{
    [DataContract]
    public class EMERecipe : PmRecipe, IEMERecipe
    {
        [DataMember]
        [XmlAttribute]
        public string FileVersion { get; set; } = "1.0.0";

        [DataMember]
        public List<Acquisition> Acquisitions { get; set; }

        [DataMember]
        public ExecutionSettings Execution { get; set; }

        [DataMember]
        [XmlIgnore]
        public Step Step { get; set; }

        [DataMember] 
        public bool IsSaveResultsEnabled { get; set; }
    }
}
