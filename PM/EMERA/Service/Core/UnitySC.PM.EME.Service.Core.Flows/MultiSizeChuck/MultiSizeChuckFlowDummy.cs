using System.Threading;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.MultiSizeChuck
{
    public class MultiSizeChuckFlowDummy : MultiSizeChuckFlow
    {
        public MultiSizeChuckFlowDummy(MultiSizeChuckInput input, IEmeraCamera camera) : base(input, camera)
        {
        }
        protected override void Process()
        {
            Thread.Sleep(1000);             
            Result.ShiftX = 0.1.Millimeters();
            Result.ShiftY = 0.23.Millimeters();
            Result.Status.State = FlowState.Success;
            Logger.Information($"ShiftX {Result.ShiftX}");
            Logger.Information($"ShiftY {Result.ShiftY}");
        }
    }
}
