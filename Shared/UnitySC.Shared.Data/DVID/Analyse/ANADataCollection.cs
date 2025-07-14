using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class ANADataCollection : ModuleDataCollection
    {
        [DataMember]
        public DVIDWaferStatistics WaferStatistics { get; set; }

        [DataMember]
        public DVIDAllDiesStatistics AllDiesStatistics { get; set; }

        [DataMember]
        public DVIDWaferMeasureData WaferMeasureData { get; set; }
    }
}
