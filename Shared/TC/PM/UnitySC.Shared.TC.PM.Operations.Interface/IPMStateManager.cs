using System.Collections.Generic;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public interface IPMStateManagerCB : ICommunicationOperationsCB
    {
        void OnPMStateChanged(TC_PMState state);

        void OnTransferStateChanged(EnumPMTransferState state);
        void OnTransferValidationStateChanged(bool validated);

        void OnMaterialChanged_pmsm(Material material);

        void FireEvent(int eventName);

        void NotifyMaterialNeedTransfer();
    }

    public interface IPMStateManager : IPMHandlingStatesChangedCB, IMaterialOperationsCB
    {
        Material CurrentMaterial { get; set; }
        EnumPMTransferState TransferState { get; }
        TC_PMState TCPMState { get; }
        PMGlobalStates? CurrentPMGlobalState { get; }
        bool CurrentTransferValidated { get; }

        bool IsPMAccessGranted();

        bool RequestPMReservation();

        void UpdateChuckPositionState();

        bool PMInitialization();

        void UpdatePMProgressInfo(PMProgressInfo recipeProgressInfo);

        void SetError_GlobalStatus(ErrorID errorID, string msgError);
        void LoadMaterial(Material material);
        Material UnloadMaterial(); 
        List<Length> MaterialDimensionsSupported { get; }
        void OnRequestMaterialTransfer_UpdateMaterialDimensionValidation(bool validated);
    }
}
