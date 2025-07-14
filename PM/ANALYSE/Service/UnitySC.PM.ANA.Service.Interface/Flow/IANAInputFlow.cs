using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Flow
{
    /// <summary>
    /// Flow input dedicated to ANALYSE flows
    /// </summary>
    public interface IANAInputFlow : IFlowInput
    {
        /// <summary>
        /// Machine context to apply before running the flow. No context given means that the flow
        /// will run with the current machine context (current objective, position, ...).
        /// </summary>
        ANAContextBase InitialContext { get; set; }
    }
}
