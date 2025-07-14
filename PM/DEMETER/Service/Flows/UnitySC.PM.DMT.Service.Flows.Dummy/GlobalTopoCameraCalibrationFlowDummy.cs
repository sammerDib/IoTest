using UnitySC.PM.DMT.Service.Flows.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class GlobalTopoCameraCalibrationFlowDummy : GlobalTopoCameraCalibrationFlow
    {
        public GlobalTopoCameraCalibrationFlowDummy(GlobalTopoCameraCalibrationInput input) : base(input)
        {
        }

        protected override void Process()
        {
            var distorsion = new double[]
            {
                -0.00248191480662082,
                -0.0069393527875920881,
                -0.0010726104110706756,
                -0.00012820043046718769,
                0.27030728795620629,
            };

            var camIntrinsic = new Matrix<double>(3, 3);
            camIntrinsic.Values = new double[3][]
            {
                new double[3] { 18171.5, 0, 7146.9 },
                new double[3] { 0, 18367.3, 5294.7 },
                new double[3] { 0, 0, 1 },
            };

            Result.RMS = 0.068;
            Result.Distortion = distorsion;
            Result.CameraIntrinsic = camIntrinsic;
        }
    }
}
