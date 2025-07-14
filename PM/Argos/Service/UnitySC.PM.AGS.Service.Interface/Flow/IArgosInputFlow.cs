using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.AGS.Service.Interface.Flow
{
    public interface IArgosInputFlow : IFlowInput
    {
        ArgosContextBase InitialContext { get; }
    }
}
