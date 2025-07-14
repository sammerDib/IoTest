using System;

using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class ComputePhaseMapAndMaskForPeriodAndDirectionFlowDummy : ComputePhaseMapAndMaskForPeriodAndDirectionFlow
    {
        public ComputePhaseMapAndMaskForPeriodAndDirectionFlowDummy(ComputePhaseMapAndMaskForPeriodAndDirectionInput input)
            : base(input)
        {
        }

        protected override void Process()
        {
            string directionString = Enum.GetName(typeof(FringesDisplacement), Input.FringesDisplacementDirection);
            SetProgressMessage($"Starting {Input.Side}side {Name} for period {Input.Period}px and direction {directionString}");

            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;
            Result.Period = Input.Period;
            Result.Fringe = Input.Fringe;
            Result.PsdResult = new PSDResult()
            {
                Amplitude = new ImageData(),
                Background = new ImageData(),
                Dark = new ImageData(),
                Mask = new ImageData(),
                WrappedPhaseMap = new ImageData(),
                WrappedPhaseMap2 = new ImageData()
            };
        }
    }
}
