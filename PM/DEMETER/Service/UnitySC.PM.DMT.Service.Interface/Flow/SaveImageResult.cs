using System;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class SaveImageResult : IFlowResult
    {
        public string SavePath { get; set; }
        
        public Side ImageSide { get; set; }
        
        public string ImageName { get; set; }
        
        public FlowStatus Status { get; set; }
    }
}
