using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Data;
using UnitySC.DataAccess.Dto;

namespace UnitySC.PM.HLS.Service.Interface.Recipe
{
    [DataContract]
    public class HLSRecipe : PmRecipe, IHLSRecipe
    {
        [DataMember]
        public string FileVersion { get; set; } = "1.0.0";

        [DataMember]
        [XmlIgnore]
        public Step Step { get; set; }
    }
}
