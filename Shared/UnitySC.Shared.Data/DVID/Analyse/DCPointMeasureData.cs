using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    [KnownType(typeof(DCDataDouble))]
    [KnownType(typeof(DCDataInt))]
    [KnownType(typeof(DCDataDoubleWithDescription))]
    [KnownType(typeof(DCDataIntWithDescription))]
    public class DCPointMeasureData
    {
        [DataMember]
        public double CoordinateX { get; set; }

        [DataMember]
        public double CoordinateY { get; set; }

        [DataMember]
        public int DieColumnIndex { get; set; }

        [DataMember]
        public int DieRowIndex { get; set; }

        [DataMember]
        public int SiteId { get; set; }


        [DataMember]
        public List<DCData> PointMeasuresData { get; set; }
    }
}
