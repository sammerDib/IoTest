using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public interface IUTOPMOperations : IUTOBaseOperations, IPMStateManagerCB
    {
        IMaterialOperations MaterialOperations { get; }
        IPMStatusVariableOperations SVOperations { get; }
        IUTOPMServiceCB UTOService { get; }
    }
}
