using System;
using System.IO;

using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Deflectometry
{
    public class ComputeRawCurvatureMapForPeriodAndDirectionFlow : FlowComponent<
        ComputeRawCurvatureMapForPeriodAndDirectionInput, ComputeRawCurvatureMapForPeriodAndDirectionResult,
        ComputeRawCurvatureMapForPeriodAndDirectionConfiguration>
    {
        public const int MaximumNumberOfSteps = 3;

        public ComputeRawCurvatureMapForPeriodAndDirectionFlow(ComputeRawCurvatureMapForPeriodAndDirectionInput input) : base(input, "ComputeCurvatureMapForPeriodAndDirectionFlow")
        {
        }

        protected override void Process()
        {
            string directionString = Enum.GetName(typeof(FringesDisplacement), Input.FringesDisplacementDirection);
            SetProgressMessage($"Starting {Input.Side}side {Name} for period {Input.Period}px and direction {directionString}");
            CheckCancellation();
            Result.RawCurvatureMap =
                DeflectometryCalculations.ComputeCurvatureMap(Input.PhaseMapAndMask, Input.PhaseMapAndMask.Mask,
                                                              Input.Period, Input.FringesDisplacementDirection);
            Result.Mask = Input.PhaseMapAndMask.Mask;
            Result.Fringe = Input.Fringe;
            Result.Period = Input.Period;
            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;
            SetProgressMessage($"Successfully computed raw curvature map for period {Input.Period}px and direction {directionString}");
            if (!Configuration.IsAnyReportEnabled())
            {
                return;
            }

            CheckCancellation();
            using (var reportRawCurvatureMapImage = Result.RawCurvatureMap.ConvertToUSPImageMil(false))
            {
                reportRawCurvatureMapImage.Save(Path.Combine(ReportFolder, $"C{directionString}_{Input.Period}px_raw.tif"));
            }
        }
    }
}
