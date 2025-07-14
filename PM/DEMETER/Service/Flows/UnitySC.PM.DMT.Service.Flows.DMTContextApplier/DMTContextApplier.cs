using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Flows.DMTContextApplier
{
    public class DMTContextApplier<TInput> : ContextApplier<TInput> where TInput : IFlowInput
    {
        public void ApplyInitialContext(TInput flowInput)
        {
        }
    }
}
