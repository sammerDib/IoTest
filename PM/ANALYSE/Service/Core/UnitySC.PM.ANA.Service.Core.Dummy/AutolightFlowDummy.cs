using System.Threading;

using UnitySC.PM.ANA.Service.Core.Autolight;
using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class AutolightFlowDummy : AutolightFlow
    {
        public AutolightFlowDummy(AutolightInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            Result.LightPower = 10;
            Result.QualityScore = 0.9;
            Logger.Information($"{LogHeader} Light power {Result.LightPower}");
        }
    }
}
