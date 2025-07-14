using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [DataContract]
    public class ThicknessMeasureToolsForLayer : MeasureToolsBase
    {
        [DataMember]
        public string NameLayerToMeasure { get; set; }

        [DataMember]
        public List<ProbeWithObjectivesMaterial> UpProbes { get; set; }

        [DataMember]
        public List<ProbeWithObjectivesMaterial> DownProbes { get; set; }

        [DataMember]
        public List<DualProbeWithObjectivesMaterial> DualProbes { get; set; }
    }

    [DataContract]
    public class ThicknessMeasureTools : MeasureToolsBase
    {
        [DataMember]
        public List<ThicknessMeasureToolsForLayer> MeasureToolsForLayers { get; set; }
    }
}
