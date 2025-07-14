using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Implementation;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Dataflow.Operations.Interface.UTODF;

namespace UnitySC.Dataflow.Operations.Implementation.UTODF
{
    public class DFStatusVariableOperations : StatusVariableOperations<SVName_DF>, IDFStatusVariableOperations
    {

        public void Update_DataflowState(TC_DataflowStatus dataflowState)
        {
            Logger.Debug("[Send to TC] Update DataflowState:" + dataflowState);
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_DF), SVName_DF.DataflowState)].Value = dataflowState;

            List<StatusVariable> list = new List<StatusVariable>();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DF), SVName_DF.DataflowState)]);
            UtoVidService.SVSetMessage(list);
        }


        protected override VSettings<StatusVariable> GetDefaultSettings()
        {
            VSettings<StatusVariable> newItems = new VSettings<StatusVariable>();
                newItems.VariableList.Add(GetNewStatusVariable(SVName_DF.DataflowState, "Get Dataflow state", "Maintenance", "None"));
            return newItems;
        }
    }
}
