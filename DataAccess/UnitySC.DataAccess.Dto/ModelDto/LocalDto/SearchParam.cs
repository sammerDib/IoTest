using System;
using System.Runtime.Serialization;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class SearchParam
    {
        [DataMember]
        public int? ToolId { get; set; }

        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public int? ProductId { get; set; }

        [DataMember]
        public string LotName { get; set; }

        [DataMember]
        public string RecipeName { get; set; }

        [DataMember]
        public ActorType? ActorType { get; set; }

        [DataMember]
        public ResultState? ResultState { get; set; }

        [DataMember]
        public string WaferName { get; set; }

        [DataMember]
        public ResultFilterTag? ResultFilterTag { get; set; }
    }
}
