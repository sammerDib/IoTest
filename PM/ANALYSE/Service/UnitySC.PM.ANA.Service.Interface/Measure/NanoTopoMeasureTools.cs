using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    public enum NanoTopoAcquisitionResolution
    {
        [XmlEnum(Name = "Low")]
        Low,

        [XmlEnum(Name = "Medium")]
        Medium,

        [XmlEnum(Name = "High")]
        High
    }

    [DataContract]
    public class NanoTopoMeasureTools : MeasureToolsBase
    {
        [DataMember]
        public List<string> OrderedAlgoNames { get; set; }

        [DataMember]
        public List<string> CompatibleObjectives { get; set; }

        [DataMember]
        public bool PostProcessingIsAvailable { get; set; }
    }
}
