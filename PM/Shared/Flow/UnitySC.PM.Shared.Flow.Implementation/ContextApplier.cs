using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.Shared.Flow.Implementation
{
    /// <summary>
    /// Apply initial context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ContextApplier<in T> where T : IFlowInput
    {
        void ApplyInitialContext(T flowInput);
    }
}
