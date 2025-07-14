using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.DMT.TC;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.Shared.Operations.Implementation
{
    public class DMTStatusVariableOperations : StatusVariableOperations<SVName_DMT>, IPMStatusVariableOperations
    {
        public void Update_AllTCPMState(TC_PMState state)
        {
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.State)].Value = state.ToString();
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.ReadyForProcess)].Value = (state == TC_PMState.Idle);
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.RecipeActive)].Value = (state == TC_PMState.Pending_To_Active) || (state == TC_PMState.Active);
            // Update TC
            UpdateSVState();
        }

        public void Update_PMProgressInfo(PMProgressInfo pmProgressInfo)
        {
            Logger.Debug("[Send to TC] Update total recipe progress percentage:" + pmProgressInfo.TotalProgress_Percentage);
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.PMAcquisitionProgress_Percentage)].Value = pmProgressInfo.StepProgressInfo[DMTPMTCManager.STEP_ACQU].Percentage;
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.PMAcquisitionProgress_SubstID)].Value = pmProgressInfo.StepProgressInfo[DMTPMTCManager.STEP_ACQU].SubstID;
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.PMComputationProgress_Percentage)].Value = pmProgressInfo.StepProgressInfo[DMTPMTCManager.STEP_COMP].Percentage;
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.PMComputationProgress_SubstID)].Value = pmProgressInfo.StepProgressInfo[DMTPMTCManager.STEP_COMP].SubstID;
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.RecipeName)].Value = pmProgressInfo.RecipeName;

            List<StatusVariable> list = new List<StatusVariable>();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.PMAcquisitionProgress_Percentage)]);
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.PMComputationProgress_Percentage)]);
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.PMAcquisitionProgress_SubstID)]);
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.PMComputationProgress_SubstID)]);
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.RecipeName)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }

        public void UpdateSVState()
        {
            if (!FirstGetAllValuesRequestDone) return;
            Task.Run(() =>
            {
                List<String> logList = new List<string>();
                List<StatusVariable> list = new List<StatusVariable>();
                list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.State)]);
                logList.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.State)].Name + "=" + CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.State)].Value.ToString());
                list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.ReadyForProcess)]);
                logList.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.ReadyForProcess)].Name + "=" + CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.ReadyForProcess)].Value.ToString());
                list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.RecipeActive)]);
                logList.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.RecipeActive)].Name + "=" + CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.RecipeActive)].Value.ToString());
                Logger.Debug("[Send to TC] UpdateSVState:" + string.Join(",", logList));
                UtoVidService.SVSetMessage(list);
            });
        }

        public void Update_MaterialState(MaterialPresence materialState)
        {
            List<StatusVariable> list = new List<StatusVariable>();
            Logger.Debug($"[Send to TC] Update MaterialState: wafer is {materialState}");

            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.MaterialPresenceState)].Value = materialState.ToString();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.MaterialPresenceState)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }

        public void Update_TransferState(String transferState)
        {
            Logger.Debug("[Send to TC] Update TransferState:" + transferState);
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.TransferState)].Value = transferState;

            List<StatusVariable> list = new List<StatusVariable>();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.TransferState)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }

        public void Update_OnlyTCPMState(TC_PMState state)
        {
            Logger.Debug("[Send to TC] Update TCPMState:" + state);
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.State)].Value = state;

            List<StatusVariable> list = new List<StatusVariable>();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.State)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }

        public void Update_TransferValidationState(bool state)
        {
            Logger.Debug("[Send to TC] Update TransferValidationState:" + state);
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.TransferValidationState)].Value = state;

            List<StatusVariable> list = new List<StatusVariable>();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_DMT), SVName_DMT.TransferValidationState)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }

        protected override VSettings<StatusVariable> GetDefaultSettings()
        {
            VSettings<StatusVariable> newItems = new VSettings<StatusVariable>();
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.State, "Get PM state to Toolcommander", "OffLine", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.RecipeName, "Get selected Recipe name to Toolcommander", "", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.ReadyForProcess, "Get state if PM is ready for for Processing", "False", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.RecipeActive, "Get state if recipe is currently running", "False", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.TransferState, "Get PM transfer state", "NotReady", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.MaterialPresenceState, "Material presence state", "False", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.PMAcquisitionProgress_Percentage, "Percentage of recipe executed", "0", "%"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.PMComputationProgress_Percentage, "Percentage of recipe computing executed", "0", "%"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.PMAcquisitionProgress_SubstID, "Substrate ID of material that its acquisition is in progress", "", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_DMT.PMComputationProgress_SubstID, "Substrate ID of material that its computing is in progress", "", "None"));
            return newItems;
        }

        public void InitAllSVIDsToDefaultvalues()
        {
            Update_AllTCPMState(TC_PMState.Error);
            Update_TransferState(EnumPMTransferState.NotReady.ToString());
            Update_MaterialState(MaterialPresence.Unknown);
            Update_PMProgressInfo(new PMProgressInfo(DMTPMTCManager.STEP_NBR, ""));
        }
    }
}
