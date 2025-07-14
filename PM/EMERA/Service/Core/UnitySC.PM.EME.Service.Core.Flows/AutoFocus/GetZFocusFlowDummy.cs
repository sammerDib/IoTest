using System.Threading;

using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;

namespace UnitySC.PM.EME.Service.Core.Flows.AutoFocus
{
    public class GetZFocusFlowDummy : GetZFocusFlow
    {
        public GetZFocusFlowDummy(GetZFocusInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(3000);
            CheckCancellation();
            Result = GetZFocusResult.Success(42.0);
        }
    }
}
