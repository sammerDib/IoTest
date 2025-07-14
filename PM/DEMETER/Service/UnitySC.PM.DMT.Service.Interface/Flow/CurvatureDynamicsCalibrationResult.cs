using System;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class CurvatureDynamicsCalibrationResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        public float CurvatureDynamicsCoefficient;
    }
}
