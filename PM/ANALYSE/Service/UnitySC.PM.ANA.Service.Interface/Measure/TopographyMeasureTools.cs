using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [DataContract]
    public class TopographyMeasureTools : MeasureToolsBase
    {
        [DataMember]
        public List<string> CompatibleObjectives { get; set; }


        [DataMember]
        public bool PostProcessingIsAvailable { get; set; }
    }
}
