using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class DVIDWaferMeasureData
    {
        [DataMember]
        public string Name { get; set; } //PointsMeasurement 


        [DataMember]
        public List<DCPointMeasureDataForMeasure> WaferMeasuresData { get; set; }
    }
}
