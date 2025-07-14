using System;
using System.IO;
using System.Linq;

using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Deflectometry
{
    public class ComputeUnwrappedPhaseMapForDirectionFlow : FlowComponent<ComputeUnwrappedPhaseMapForDirectionInput, ComputeUnwrappedPhaseMapForDirectionResult, ComputeUnwrappedPhaseMapForDirectionConfiguration>
    {
        public const int MaximumNumberOfSteps = 3;

        public ComputeUnwrappedPhaseMapForDirectionFlow(ComputeUnwrappedPhaseMapForDirectionInput input) : base(input, "ComputeUnwrappedPhaseMapForDirectionFlow")
        {
        }

        protected override void Process()
        {
            SetProgressMessage($"Starting {Input.Side}side {Name}");
            int smallestPeriod = Input.PhaseMaps.Keys.Min();
            var unwrappedPhaseMap = DeflectometryCalculations.MultiPeriodUnwrap(Input.PhaseMaps.Values.ToList(),
                Input.PhaseMaps[smallestPeriod].Mask, Input.Fringe.Periods);

            CheckCancellation();
            Result.UnwrappedPhaseMap = ShouldSubtractMeanPlane()
                ? DeflectometryCalculations.SubstractPlaneFromUnwrapped(unwrappedPhaseMap, Input.PhaseMaps[smallestPeriod].Mask)
                : unwrappedPhaseMap;

            Result.Fringe = Input.Fringe;
            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;

            SaveReportIfNeeded();
            SetProgressMessage($"Successfully computed {Input.Side}side unwrapped phase map for direction {Enum.GetName(typeof(FringesDisplacement), Input.FringesDisplacementDirection)}");
        }

        protected bool ShouldSubtractMeanPlane()
        {
            return Configuration.ProduceUntiltedSlopeMaps && Input.IsNeededForSlopeMaps;
        }

        private void SaveReportIfNeeded()
        {
            if (Configuration.IsAnyReportEnabled())
            {
                CheckCancellation();
                using (var UnwrappedPhaseMapImage = Result.UnwrappedPhaseMap.ConvertToUSPImageMil(false))
                {
                    string direction = Enum.GetName(typeof(FringesDisplacement), Input.FringesDisplacementDirection);
                    UnwrappedPhaseMapImage.Save(Path.Combine(ReportFolder, $"UnwrappedPhaseMap_{direction}.tif"));
                }
            }
        }
    }
}
