using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class DVIDWaferStatistics
    {
        [DataMember]
        public string Name { get; set; } //Gloabal Wafer Statistics

        [DataMember]
        public List<DCWaferStatisticsForMeasure> WaferStatistics { get; set; }
    }
}
