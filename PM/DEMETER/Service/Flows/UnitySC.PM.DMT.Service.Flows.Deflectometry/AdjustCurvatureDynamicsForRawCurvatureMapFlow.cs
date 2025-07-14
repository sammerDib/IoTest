using System;
using System.IO;

using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Deflectometry
{
    public class AdjustCurvatureDynamicsForRawCurvatureMapFlow : FlowComponent<
        AdjustCurvatureDynamicsForRawCurvatureMapInput, AdjustCurvatureDynamicsForRawCurvatureMapResult,
        AdjustCurvatureDynamicsForRawCurvatureMapConfiguration>
    {
        public const int MaximumNumberOfSteps = 3;

        public AdjustCurvatureDynamicsForRawCurvatureMapFlow(AdjustCurvatureDynamicsForRawCurvatureMapInput input) :
            base(input, "AdjustCurvatureDynamicsForRawCurvatureMap")
        {
        }

        protected override void Process()
        {
            SetProgressMessage($"Starting {Input.Side}side {Name} for direction {Input.FringesDisplacementDirection}");
            float calibrationDynamicsCoefficient = Input.CurvatureDynamicsCalibrationCoefficient == 0f
                ? Configuration.DefaultCurvatureDynamicsCoefficient
                : Input.CurvatureDynamicsCalibrationCoefficient;
            CheckCancellation();
            var adjustedCurvatureMap =
                PhaseShiftingDeflectometry.ApplyDynamicCalibration(Input.RawCurvatureMap, Input.Mask,
                                                                   calibrationDynamicsCoefficient,
                                                                   Configuration.TargetBackgroundLevel,
                                                                   Input.DynamicsCoefficient);
            Result.CurvatureMap = adjustedCurvatureMap;
            Result.Fringe = Input.Fringe;
            Result.Period = Input.Period;
            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;

            SaveReportIfNeeded();
            CheckCancellation();
            SetProgressMessage($"Successfully adjusted the curvature dynamics for direction {Input.FringesDisplacementDirection}");
        }

        private void SaveReportIfNeeded()
        {
            if (Configuration.IsAnyReportEnabled())
            {
                CheckCancellation();
                using (var CurvatureMapImage = Result.CurvatureMap.ConvertToUSPImageMil(false))
                {
                    string direction = Enum.GetName(typeof(FringesDisplacement), Input.FringesDisplacementDirection);
                    CurvatureMapImage.Save(Path.Combine(ReportFolder, $"CurvatureMap_{Input.Period}px_{direction}.tif"));
                }
            }
        }
    }
}
