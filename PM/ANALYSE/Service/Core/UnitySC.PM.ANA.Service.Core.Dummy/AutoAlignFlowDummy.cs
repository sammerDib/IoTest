using System.Threading;

using UnitySC.PM.ANA.Service.Core.AdvancedFlow;
using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class AutoAlignFlowDummy : AutoAlignFlow
    {
        public AutoAlignFlowDummy(AutoAlignInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
        }
    }
}
