using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;

namespace UnitySC.PM.ANA.Service.Core.Context
{
    /// <summary>
    /// Is responsible for applying the initial context of a flow. Contexts are given by the inputs.
    /// </summary>
    public class FlowInitialContextApplier : ContextApplier<IANAInputFlow>
    {
        private readonly IContextManager _contextManager;

        public FlowInitialContextApplier(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public void ApplyInitialContext(IANAInputFlow flowInput)
        {
            _contextManager.Apply(flowInput.InitialContext);
        }
    }
}
