using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class DCDiesStatisticsForMeasure
    {
        [DataMember]
        public string MeasureName { get; set; }


        [DataMember]
        public List<DCDieStatistics> DiesStatisticsForMeasure { get; set; }


    }
}
