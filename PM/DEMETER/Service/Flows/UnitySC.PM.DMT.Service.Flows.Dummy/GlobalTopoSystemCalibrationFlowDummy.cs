using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class GlobalTopoSystemCalibrationFlowDummy : GlobalTopoSystemCalibrationFlow
    {
        public GlobalTopoSystemCalibrationFlowDummy(GlobalTopoSystemCalibrationInput input, DMTHardwareManager hardwareManager)
            : base(input, hardwareManager)
        {
        }

        protected override void Process()
        {
            var rWaferToCamera = new Matrix<double>(3, 3);
            rWaferToCamera.Values = new double[3][]
            {
                new double[3] { 0.998, 0.049, -0.023 },
                new double[3] { 0.036, -0.918, -0.394 },
                new double[3] { -0.041, 0.392,  -0.918 },
            };
            var tWaferToCamera = new double[]
            {
                23.355,
                -24.296,
                514.426,
            };

            var rScreenToWafer = new Matrix<double>(3, 3);
            rScreenToWafer.Values = new double[3][]
            {
                new double[3] { -0.998, -0.041, 0.042 },
                new double[3] { -0.055, 0.909, -0.412 },
                new double[3] { -0.021, -0.413, -0.910 },
            };
            var tScreenToWafer = new double[]
            {
                -79.945,
                120.736,
                274.782,
            };

            var rScreenToCamera = new Matrix<double>(3, 3);
            rScreenToCamera.Values = new double[3][]
            {
                new double[3] { -0.999, 0.013, 0.044 },
                new double[3] { 0.023, -0.673, 0.738 },
                new double[3] { 0.039, 0.739, 0.672 },
            };
            var tScreenToCamera = new double[]
            {
                -56.983,
                -246.458,
                312.772,
            };

            Result.ExtrinsicCamera = new ExtrinsicCameraCalibration(rWaferToCamera, tWaferToCamera);
            Result.ExtrinsicScreen = new ExtrinsicScreenCalibration(rScreenToWafer, tScreenToWafer);
            Result.ExtrinsicSystem = new ExtrinsicSystemCalibration(rScreenToCamera, tScreenToCamera);
        }
    }
}
