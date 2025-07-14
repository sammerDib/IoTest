using System.Runtime.Serialization;

using UnitySC.Shared.Data;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class KlarfSettingsData
    {
        [DataMember]
        public DefectBins RoughBins { get; set; }

        [DataMember]
        public SizeBins SizeBins { get; set; }
    }
}