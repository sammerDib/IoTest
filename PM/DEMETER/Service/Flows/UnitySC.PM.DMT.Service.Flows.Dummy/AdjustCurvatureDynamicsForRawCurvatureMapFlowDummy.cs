using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class AdjustCurvatureDynamicsForRawCurvatureMapFlowDummy : AdjustCurvatureDynamicsForRawCurvatureMapFlow
    {
        public AdjustCurvatureDynamicsForRawCurvatureMapFlowDummy(AdjustCurvatureDynamicsForRawCurvatureMapInput input)
            : base(input)
        {
        }

        protected override void Process()
        {
            SetProgressMessage($"Starting {Input.Side}side {Name} for direction {Input.FringesDisplacementDirection}");

            Result.CurvatureMap = new ImageData();
            Result.Fringe = Input.Fringe;
            Result.Period = Input.Period;
            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;

            SetProgressMessage($"Successfully adjusted the curvature dynamics for direction {Input.FringesDisplacementDirection}");
        }
    }
}
