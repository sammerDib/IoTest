using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Operations.Interface;

namespace UnitySC.Dataflow.Operations.Interface.UTODF
{
    public interface IDFStatusVariableOperations : IStatusVariableOperations
    {
        void Update_DataflowState(TC_DataflowStatus dataflowState);
    }
}
