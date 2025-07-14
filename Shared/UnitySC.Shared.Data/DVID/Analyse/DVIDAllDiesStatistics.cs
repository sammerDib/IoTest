using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class DVIDAllDiesStatistics
    {
        [DataMember]
        public string Name { get; set; } //Dies statistics 


        [DataMember]
        public List<DCDiesStatisticsForMeasure> DiesStatistics { get; set; }


    }
}
