using System;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.DVID
{
    [DataContract]
    public class ModuleDataCollection
    {
        [DataMember]
        public int SlotID { get; set; }

        [DataMember]
        public int LoadportID { get; set; }

        [DataMember]
        public String LotID { get; set; }

        [DataMember]
        public String ControlJobID { get; set; }

        [DataMember]
        public String ProcessJobID { get; set; }

        [DataMember]
        public DateTime ProcessStartTime { get; set; }

        [DataMember]
        public DateTime ProcessEndTime { get; set; }

        [DataMember]
        public String CarrierID { get; set; }

        [DataMember]
        public String SubstrateID { get; set; }

        [DataMember]
        public String AcquiredID { get; set; }

        [DataMember]
        public String RecipeID { get; set; }
    }
}
