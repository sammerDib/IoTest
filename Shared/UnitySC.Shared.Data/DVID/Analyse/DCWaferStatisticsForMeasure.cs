using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    [KnownType(typeof(DCDataDouble))]
    [KnownType(typeof(DCDataInt))]
    [KnownType(typeof(DCDataDoubleWithDescription))]
    [KnownType(typeof(DCDataIntWithDescription))]
    public class DCWaferStatisticsForMeasure
    {
        [DataMember]
        public string MeasureName { get; set; }

        [DataMember]
        public List<DCData> WaferStatisticsForMeasure { get; set; }
    }
}
