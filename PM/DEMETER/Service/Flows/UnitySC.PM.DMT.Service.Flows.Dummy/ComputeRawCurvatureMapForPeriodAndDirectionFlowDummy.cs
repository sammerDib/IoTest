using System;

using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class ComputeRawCurvatureMapForPeriodAndDirectionFlowDummy : ComputeRawCurvatureMapForPeriodAndDirectionFlow
    {
        public ComputeRawCurvatureMapForPeriodAndDirectionFlowDummy(ComputeRawCurvatureMapForPeriodAndDirectionInput input)
            : base(input)
        {
        }

        protected override void Process()
        {
            string directionString = Enum.GetName(typeof(FringesDisplacement), Input.FringesDisplacementDirection);
            SetProgressMessage($"Starting {Input.Side}side {Name} for period {Input.Period}px and direction {directionString}");
            Result.RawCurvatureMap = new ImageData();
            Result.Mask = Input.PhaseMapAndMask.Mask;
            Result.Fringe = Input.Fringe;
            Result.Period = Input.Period;
            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;
            SetProgressMessage($"Successfully computed raw curvature map for period {Input.Period}px and direction {directionString}");
        }
    }
}
