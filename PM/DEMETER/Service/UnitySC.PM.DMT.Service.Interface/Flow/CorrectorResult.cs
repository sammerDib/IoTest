using System;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class CorrectorResult : IFlowResult
    {
        public Angle WaferAngle;

        public Length WaferXShift;

        public Length WaferYShift;
        public FlowStatus Status { get; set; }
    }
}
