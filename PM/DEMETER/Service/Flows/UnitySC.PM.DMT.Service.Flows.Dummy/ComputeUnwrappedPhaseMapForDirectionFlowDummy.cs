using System;

using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class ComputeUnwrappedPhaseMapForDirectionFlowDummy : ComputeUnwrappedPhaseMapForDirectionFlow
    {
        public ComputeUnwrappedPhaseMapForDirectionFlowDummy(ComputeUnwrappedPhaseMapForDirectionInput input) : base(input)
        {
        }

        protected override void Process()
        {
            SetProgressMessage($"Starting {Input.Side}side {Name}");

            Result.UnwrappedPhaseMap = new ImageData();
            Result.Fringe = Input.Fringe;
            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;

            SetProgressMessage($"Successfully computed {Input.Side}side unwrapped phase map for direction {Enum.GetName(typeof(FringesDisplacement), Input.FringesDisplacementDirection)}");
        }
    }
}
