using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class DCDataInt : DCData
    {
        [DataMember]
        public int Value { get; set; }
    }

    [DataContract]
    public class DCDataIntWithDescription : DCDataInt
    {
        [DataMember]
        public string Description { get; set; }

    }
}
