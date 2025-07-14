using System;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public interface IPMStatusVariableOperations : IStatusVariableOperations
    {
        void Update_AllTCPMState(TC_PMState state);
        void Update_TransferState(String transferState);
        void Update_PMProgressInfo(PMProgressInfo pmProgressInfo);
        void Update_MaterialState(MaterialPresence materialPresence);
        void Update_OnlyTCPMState(TC_PMState state);
        void Update_TransferValidationState(bool state);
        void InitAllSVIDsToDefaultvalues();
    }
}
