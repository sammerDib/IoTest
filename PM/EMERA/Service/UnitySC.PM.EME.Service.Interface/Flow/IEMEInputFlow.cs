using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Flow
{
    /// <summary>
    /// Flow input dedicated to EMERA flows
    /// </summary>
    public interface IEMEInputFlow : IFlowInput
    {
        /// <summary>
        /// Machine context to apply before running the flow. No context given means that the flow
        /// will run with the current machine context (current objective, position, ...).
        /// </summary>
        /// 
        EMEContextBase InitialContext { get; set; }
    }
}
