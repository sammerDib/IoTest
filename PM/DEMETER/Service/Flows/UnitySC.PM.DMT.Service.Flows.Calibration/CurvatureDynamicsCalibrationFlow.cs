using System;

using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Calibration
{
    public class CurvatureDynamicsCalibrationFlow : FlowComponent<CurvatureDynamicsCalibrationInput,
        CurvatureDynamicsCalibrationResult, CurvatureDynamicsCalibrationConfiguration>
    {
        public CurvatureDynamicsCalibrationFlow(CurvatureDynamicsCalibrationInput input) :
            base(input, "CurvatureDynamicsCalibration")
        {
        }

        protected override void Process()
        {
            float calibrationCoefficient =
                PhaseShiftingDeflectometry.CalibrateCurvatureDynamics(Input.XRawCurvatureMap, Input.YRawCurvatureMap,
                                                                      Input.CurvatureMapMask);
            if (float.IsNaN(calibrationCoefficient) || calibrationCoefficient <= 0f)
            {
                throw new Exception("Unable to perform curvature dynamics calibration");
            }

            Result.CurvatureDynamicsCoefficient = calibrationCoefficient;
        }
    }
}
