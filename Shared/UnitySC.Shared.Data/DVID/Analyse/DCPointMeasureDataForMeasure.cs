using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class DCPointMeasureDataForMeasure
    {
        [DataMember]
        public string MeasureName { get; set; }
        [DataMember]
        public List<DCPointMeasureData> WaferMeasuresDataForMeasure { get; set; }


    }
}
