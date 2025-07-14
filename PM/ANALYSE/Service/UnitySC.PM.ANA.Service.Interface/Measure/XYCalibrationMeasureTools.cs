using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [DataContract]
    public class XYCalibrationMeasureTools : MeasureToolsBase
    {
        [DataMember]
        public List<string> CompatibleObjectiveIds { get; set; }

        [DataMember]
        public string RecommendedObjectiveId { get; set; }
    }
}
