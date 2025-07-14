using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class ComputeNanoTopoFlowDummy : ComputeNanoTopoFlow
    {
        public ComputeNanoTopoFlowDummy(ComputeNanoTopoInput input, ICalibrationManager calibrationManager) : base(input, calibrationManager)
        {
        }
    }
}
