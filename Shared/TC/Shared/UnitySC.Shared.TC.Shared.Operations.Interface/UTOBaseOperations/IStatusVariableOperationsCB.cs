using System.Collections.Generic;

using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Operations.Interface
{
    public interface IStatusVariableOperationsCB
    {
        void SVStateChanged(Dictionary<string, StatusVariable> svStatesDico);
    }
}
