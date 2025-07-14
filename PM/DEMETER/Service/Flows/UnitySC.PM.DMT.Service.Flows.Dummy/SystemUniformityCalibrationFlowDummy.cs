using UnitySC.PM.DMT.Service.Flows.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class SystemUniformityCalibrationFlowDummy : SystemUniformityCalibrationFlow
    {
        public SystemUniformityCalibrationFlowDummy(SystemUniformityCalibrationInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Result.UnifomityCorrection = new ImageData();
        }
    }
}
