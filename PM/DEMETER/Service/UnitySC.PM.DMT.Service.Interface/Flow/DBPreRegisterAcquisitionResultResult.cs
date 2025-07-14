using System;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class DBPreRegisterAcquisitionResultResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        public OutPreRegisterAcquisition OutPreRegister;
    }
}
