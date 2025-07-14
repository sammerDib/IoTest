using System;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class GlobalTopoSystemCalibrationResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        public ExtrinsicCameraCalibration ExtrinsicCamera;
        public ExtrinsicScreenCalibration ExtrinsicScreen;
        public ExtrinsicSystemCalibration ExtrinsicSystem;
    }

    [Serializable]
    public class ExtrinsicCameraCalibration
    {
        public Matrix<double> RWaferToCamera;

        public double[] TWaferToCamera;

        public ExtrinsicCameraCalibration()
        { }

        public ExtrinsicCameraCalibration(Matrix<double> rWaferToCamera, double[] tWaferToCamera)
        {
            RWaferToCamera = rWaferToCamera;
            TWaferToCamera = tWaferToCamera;
        }
    }

    [Serializable]
    public class ExtrinsicScreenCalibration
    {
        public Matrix<double> RScreenToWafer;

        public double[] TScreenToWafer;

        public ExtrinsicScreenCalibration()
        { }

        public ExtrinsicScreenCalibration(Matrix<double> rScreenToWafer, double[] tScreenToWafer)
        {
            RScreenToWafer = rScreenToWafer;
            TScreenToWafer = tScreenToWafer;
        }
    }

    [Serializable]
    public class ExtrinsicSystemCalibration
    {
        public Matrix<double> RScreenToCamera;

        public double[] TScreenToCamera;

        public ExtrinsicSystemCalibration()
        { }

        public ExtrinsicSystemCalibration(Matrix<double> rScreenToCamera, double[] tScreenToCamera)
        {
            RScreenToCamera = rScreenToCamera;
            TScreenToCamera = tScreenToCamera;
        }
    }
}
