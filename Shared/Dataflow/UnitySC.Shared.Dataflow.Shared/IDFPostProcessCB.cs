using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.Dataflow.Shared
{
    public interface IDFPostProcessCB
    {
        void DFPostProcessStarted(Identity identity, Side side, String processJobID, Guid material);
        void DFPostProcessComplete(Identity identity, Side side, String processJobID, Guid material, DataflowRecipeStatus status);
    }
}
