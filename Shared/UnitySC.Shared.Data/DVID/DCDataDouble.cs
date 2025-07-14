using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class DCDataDouble : DCData
    {
        [DataMember]
        public double Value { get; set; }
    }

    [DataContract]
    public class DCDataDoubleWithDescription : DCDataDouble
    {
        [DataMember]
        public string Description { get; set; }
    }
}
