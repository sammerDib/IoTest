using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Alignment;
using UnitySC.PM.ANA.Service.Interface.Recipe.AlignmentMarks;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Interface.Recipe.WaferMap;
using UnitySC.PM.Shared.Data;

namespace UnitySC.PM.ANA.Service.Interface.Recipe
{
    [DataContract]
    public class ANARecipe : PmRecipe, IANARecipe
    {
        public static string CurrentFileVersion = "1.0.3";

        [DataMember]
        [XmlAttribute]
        public string FileVersion { get; set; } = CurrentFileVersion;

        [DataMember]
        public List<MeasurePoint> Points { get; set; }

        [DataMember]
        public List<MeasureSettingsBase> Measures { get; set; }

        [DataMember]
        public List<DieIndex> Dies { get; set; }

        [DataMember]
        public AlignmentSettings Alignment { get; set; }

        [DataMember]
        public ExecutionSettings Execution { get; set; }

        [DataMember]
        [XmlIgnore]
        public Step Step { get; set; }

        [DataMember]
        public bool IsWaferMapSkipped { get; set; }

        [DataMember]
        public WaferMapSettings WaferMap { get; set; }

        [DataMember]
        public bool IsAlignmentMarksSkipped { get; set; }

        [DataMember]
        public AlignmentMarksSettings AlignmentMarks { get; set; }

        [DataMember]
        public bool IsWaferLessModified { get; set; }

        [XmlIgnore]
        public bool WaferHasDies => WaferMap != null;
    }
}
