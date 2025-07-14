using System;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class GlobalTopoCameraCalibrationResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        public Matrix<double> CameraIntrinsic;

        public double[] Distortion;

        public double RMS;
    }
}
