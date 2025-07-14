using UnitySC.PM.DMT.Service.Flows.Corrector;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class CorrectorFlowDummy : CorrectorFlow
    {
        public CorrectorFlowDummy(CorrectorInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Result.WaferAngle = 0.Degrees();
            Result.WaferXShift = 0.Micrometers();
            Result.WaferYShift = 0.Micrometers();
        }
    }
}
