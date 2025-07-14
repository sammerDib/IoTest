using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    public class SaveMaskResult : IFlowResult
    {
        public FlowStatus Status { get; set; }
        
        public string SavePath { get; set; }
        
        public Side MaskSide { get; set; }
        
        public string MaskFileName { get; set; }
    }
}
