using System.Threading;

using UnitySC.PM.EME.Service.Interface.Algo;

namespace UnitySC.PM.EME.Service.Core.Flows.AutoExposure
{
    public class AutoExposureFlowDummy : AutoExposureFlow
    {
        public AutoExposureFlowDummy(AutoExposureInput input) : base(input, null)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            CheckCancellation();

            Result = new AutoExposureResult
            {
                Brightness = 0.73,
                ExposureTime = 155,
            };
        }
    }
}
