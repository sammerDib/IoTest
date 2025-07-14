using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class WaferAcquisitionResult
    {
        [DataMember]
        public int SlotId { get; set; }

        [DataMember]
        public string PathName { get; set; }

        [DataMember]
        public ResultAcqItem AcqItem { get; set; }

        [DataMember]
        public ActorType ActorType { get; set; }
    }
}
