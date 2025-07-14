using System;
using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Units;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public interface IPMTCManager
    {
        #region Properties
        Material CurrentMaterial { get; }
        Guid? CurrentRecipeKey { get; }
        Identity PMIdentity { get; }
        IHandling Handling { get; }
        #endregion

        #region Methodes
        void AbortRecipeExecution_pmtcs();
        void Init_Services();
        void Init_Status();
        void LoadMaterialOnChuck_pmtcs();
        void MoveToLoadingUnloadingPosition_pmtcs(MaterialTypeInfo materialTypeInfo);
        void MoveToProcessPosition_pmtcs();
        void OnTransferMaterialFinished_pmtcs(string failedReason);
        void OnTransferMaterialStarted_pmtcs();
        void PMInitialization_pmtcs();
        void RequestAllFDCsUpdate();
        void StartRecipeExecution_pmtcs(Guid? pmRecipeKey, DataflowRecipeInfo dfRecipeInfo, Material material);
        void StartRecipeRequest_pmtcs(Material material);
        void UnloadMaterialOnChuck_pmtcs();
        void UpdateChuckPositionState_pmtcs();
        List<Length> GetMaterialDiametersSupported();
        void SetMaterialPresenceWithoutSensorPresence(Length slotSize, MaterialPresence presence);
        #endregion
    }
}
