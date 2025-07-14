using System.Threading;

using UnitySC.PM.ANA.Service.Core.AutofocusV2;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class AFV2CameraFlowDummy : AFV2CameraFlow
    {
        public AFV2CameraFlowDummy(AFCameraInput input, ImageOperators imageOperatorsLib = null) : base(input)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            Result.ZPosition = 1.2;
            Result.QualityScore = 0.9;
            Logger.Information($"{LogHeader} Z position {Result.ZPosition} mm");
        }
    }
}
