using Agileo.ProcessingFramework;

namespace UnitySC.Equipment.Abstractions.Vendor.ProcessExecution
{
    public class ProcessModuleExecutionFactory : IExecutionContextFactory
    {
        public IExecutionContext NewExecutionContext(Program program)
        {
            return new ProcessModuleExecutionContext();
        }
    }
}
