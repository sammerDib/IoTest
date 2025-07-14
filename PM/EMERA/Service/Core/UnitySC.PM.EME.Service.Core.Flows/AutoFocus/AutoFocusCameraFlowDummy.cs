using System.Threading;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;

namespace UnitySC.PM.EME.Service.Core.Flows.AutoFocus
{
    public class AutoFocusCameraFlowDummy : AutoFocusCameraFlow
    {
        public AutoFocusCameraFlowDummy(AutoFocusCameraInput input, IEmeraCamera camera, ImageOperators imageOperatorsLib = null) : base(input, camera)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            Result.SensorDistance = 8000;
            Logger.Information($"{LogHeader} sensor distance {Result.SensorDistance}");
        }
    }
}
