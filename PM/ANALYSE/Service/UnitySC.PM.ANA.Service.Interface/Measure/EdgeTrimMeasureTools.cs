using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [DataContract]
    public class EdgeTrimMeasureTools : MeasureToolsBase
    {
        [DataMember]
        public List<ProbeWithObjectivesMaterial> Probes { get; set; }
    }
}
