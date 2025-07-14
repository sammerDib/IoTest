using System.Threading;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.PixelSize
{
    public class PixelSizeComputationFlowDummy : PixelSizeComputationFlow
    {
        public PixelSizeComputationFlowDummy(PixelSizeComputationInput input, IEmeraCamera camera) : base(input, camera)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            Result.PixelSize = 2.Micrometers();
            Logger.Information($"Pixel Size {Result.PixelSize}");
        }
    }
}
