using System.Threading;

using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class AFLiseFlowDummy : AFLiseFlow
    {
        public AFLiseFlowDummy(AFLiseInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(100);
            Result.ZPosition = 13; // This value is used in tests, do not change it
            Result.QualityScore = 0.9;
            Logger.Information($"{LogHeader} Z position {Result.ZPosition} mm");
        }
    }
}
