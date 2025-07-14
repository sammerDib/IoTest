using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class WaferResultData
    {
        [DataMember]
        public int SlotId { get; set; }

        [DataMember]
        public string WaferName { get; set; }

        [DataMember]
        public ResultItem ResultItem { get; set; }

        [DataMember]
        public ActorType ActorType { get; set; }
    }
}
