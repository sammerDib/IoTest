using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Implementation;

namespace UnitySC.PM.EME.TC
{
   public class EMEStatusVariableOperations : StatusVariableOperations<SVName_EME>, IPMStatusVariableOperations
    {
        public void Update_AllTCPMState(TC_PMState state)
        {
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.State)].Value = state.ToString();
            CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.ReadyForProcess)].Value = (state == TC_PMState.Idle);
            CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.RecipeActive)].Value = (state == TC_PMState.Pending_To_Active) || (state == TC_PMState.Active);
            // Update TC
            UpdateSVState();
        }

        public void Update_PMProgressInfo(PMProgressInfo pmProgressInfo)
        {
            Logger.Debug("[Send to TC] Update total recipe progress percentage:" + pmProgressInfo.TotalProgress_Percentage);
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.PMProcessProgress_Percentage)].Value = pmProgressInfo.TotalProgress_Percentage;
            CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.RecipeName)].Value = pmProgressInfo.RecipeName;

            List<StatusVariable> list = new List<StatusVariable>();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.PMProcessProgress_Percentage)]);
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.RecipeName)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }

        public void UpdateSVState()
        {
            if (!FirstGetAllValuesRequestDone) return;
            Task.Run(() =>
            {
                List<String> logList = new List<string>();
                List<StatusVariable> list = new List<StatusVariable>();
                list.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.State)]);
                logList.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.State)].Name + "=" + CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.State)].Value.ToString());
                list.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.ReadyForProcess)]);
                logList.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.ReadyForProcess)].Name + "=" + CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.ReadyForProcess)].Value.ToString());
                list.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.RecipeActive)]);
                logList.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.RecipeActive)].Name + "=" + CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.RecipeActive)].Value.ToString());
                Logger.Debug("[Send to TC] UpdateSVState:" + string.Join(",", logList));
                UtoVidService.SVSetMessage(list);
            });
        }

        public void Update_MaterialState(MaterialPresence materialState)
        {
            List<StatusVariable> list = new List<StatusVariable>();
            Logger.Debug("[Send to TC] Update MaterialState:" + materialState);

            CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.MaterialPresenceState)].Value = materialState.ToString();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.MaterialPresenceState)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }

        public void Update_TransferState(String transferState)
        {
            Logger.Debug("[Send to TC] Update TransferState:" + transferState);
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.TransferState)].Value = transferState;

            List<StatusVariable> list = new List<StatusVariable>();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.TransferState)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }

        public void Update_TransferValidationState(bool state)
        {
            Logger.Debug("[Send to TC] Update TransferValidationState:" + state);
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.TransferValidationState)].Value = state;

            List<StatusVariable> list = new List<StatusVariable>();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.TransferValidationState)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }

        public void Update_OnlyTCPMState(TC_PMState state)
        {
            Logger.Debug("[Send to TC] Update TCPMState:" + state);
            // Update current state
            CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.State)].Value = state;

            List<StatusVariable> list = new List<StatusVariable>();
            list.Add(CurrentVDico[Enum.GetName(typeof(SVName_EME), SVName_EME.State)]);
            Task.Run(() => UtoVidService.SVSetMessage(list));
        }


        protected override VSettings<StatusVariable> GetDefaultSettings()
        {
            VSettings<StatusVariable> newItems = new VSettings<StatusVariable>();
            newItems.VariableList.Add(GetNewStatusVariable(SVName_EME.State, "Get PM state to Toolcommander", "OffLine", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_EME.RecipeName, "Get selected Recipe name to Toolcommander", "", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_EME.ReadyForProcess, "Get state if PM is ready for for Processing", "False", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_EME.RecipeActive, "Get state if recipe is currently running", "False", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_EME.TransferState, "Get PM transfer state", "NotReady", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_EME.MaterialPresenceState, "Material presence state", "False", "None"));
            newItems.VariableList.Add(GetNewStatusVariable(SVName_EME.PMProcessProgress_Percentage, "Percentage of process executed", "0", "%"));
            return newItems;
        }

        public void InitAllSVIDsToDefaultvalues()
        {
            Update_AllTCPMState(TC_PMState.Error);
            Update_TransferState(EnumPMTransferState.NotReady.ToString());
            Update_MaterialState(MaterialPresence.Unknown);
            Update_PMProgressInfo(new PMProgressInfo(EMEPMTCManager.STEP_NBR, ""));
        }

    }
}

