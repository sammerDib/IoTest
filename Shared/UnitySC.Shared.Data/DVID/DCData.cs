using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class DCData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Unit { get; set; }

        [DataMember]
        public bool IsMeasured { get; set; }
    }
}
