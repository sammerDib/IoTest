using System.Threading;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;

namespace UnitySC.PM.EME.Service.Core.Flows.Vignetting
{
    public class VignettingFlowDummy : VignettingFlow
    {
        public VignettingFlowDummy(VignettingInput input, IEmeraCamera camera) : base(input, camera)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            Result.FilterWheelPosition = 20.0;
            Logger.Information($"{LogHeader} Filter wheel position {Result.FilterWheelPosition} mm");
        }
    }
}
