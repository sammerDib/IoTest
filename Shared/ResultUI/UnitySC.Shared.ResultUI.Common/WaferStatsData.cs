using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.Enum;

namespace UnitySC.Shared.ResultUI.Common
{
    public class WaferStatsData
    {
        public int SlotId { get; set; }
        public ResultState State { get; set; }
        public ResultItemValue ResultValue { get; set; }
    }
}
