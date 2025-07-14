using UnitySC.PM.DMT.Service.Flows.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class CurvatureDynamicsCalibrationFlowDummy : CurvatureDynamicsCalibrationFlow
    {
        public CurvatureDynamicsCalibrationFlowDummy(CurvatureDynamicsCalibrationInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Result.CurvatureDynamicsCoefficient = 0.005f;
        }
    }
}
