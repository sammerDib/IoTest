using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    [KnownType(typeof(DCDataDouble))]
    [KnownType(typeof(DCDataInt))]
    [KnownType(typeof(DCDataDoubleWithDescription))]
    [KnownType(typeof(DCDataIntWithDescription))]
    public class DCDieStatistics
    {
        [DataMember]
        public int ColumnIndex { get; set; }

        [DataMember]
        public int RowIndex { get; set; }

        [DataMember]
        public List<DCData> DieStatistics { get; set; }
    }
}
