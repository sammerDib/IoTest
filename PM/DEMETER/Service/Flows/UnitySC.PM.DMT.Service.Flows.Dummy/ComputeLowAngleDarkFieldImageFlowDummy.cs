using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class ComputeLowAngleDarkFieldImageFlowDummy : ComputeLowAngleDarkFieldImageFlow
    {
        public ComputeLowAngleDarkFieldImageFlowDummy(ComputeLowAngleDarkFieldImageInput input) : base(input)
        {
        }

        protected override void Process()
        {
            SetProgressMessage($"Starting {Input.Side}side {Name} for period {Input.Period}px");
            Result.DarkImage = new ImageData();
            Result.Fringe = Input.Fringe;
            Result.Period = Input.Period;
            SetProgressMessage($"Successfully computed Dark image for period {Input.Period}px");
        }
    }
}
