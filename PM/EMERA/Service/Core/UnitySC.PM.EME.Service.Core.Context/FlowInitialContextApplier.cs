using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;

namespace UnitySC.PM.EME.Service.Core.Context
{
    /// <summary>
    /// Is responsible for applying the initial context of a flow. Contexts are given by the inputs.
    /// </summary>
    public class FlowInitialContextApplier : ContextApplier<IEMEInputFlow>
    {
        private readonly IContextManager _contextManager;

        public FlowInitialContextApplier(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public void ApplyInitialContext(IEMEInputFlow flowInput)
        {
            _contextManager.Apply(flowInput.InitialContext);
        }
    }
}
