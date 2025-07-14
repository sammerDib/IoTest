using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;


using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.EME.Service.Core.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.TC.PM.Operations.Implementation;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.PM.EME.TC
{
    internal class EMEPMTCManager : BasePMTCManager<EMERecipe, IEMEHandling, ExecutionStatus>, IEMERecipeServiceCallback
    {

        public const int STEP_NBR = 2;
        public const int STEP_ACQU = 0;
        public const int STEP_COMP = 1;

        #region Members

        private RecipeExecution _recipeExecution;
        private RecipeOrchestrator _orchestrator;
        private DbRecipeServiceProxy _dbRecipeServiceProxy;

        #endregion Members

        #region Constructor

        public EMEPMTCManager()
        {
        }

        #endregion Constructor

        #region IPMTCManager

        public override void AbortRecipeExecution_pmtcs()
        {
            if (PMStateManager.CurrentTCPMState.State == TC_PMState.Active)
            {
                // Abort current recipe running
                Logger.Information("-----------------------------------------------------------------------");
                Logger.Information($"Abort current recipe running : {CurrentRecipe?.Name}");
                _orchestrator.Cancel();
            }
            else
            {
                string msg = "Recipe aborted before starting";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeExecutionCanceled, msg);
                if (CurrentRecipe != null) DfSupervisor.RecipeExecutionComplete(PMIdentity, PMStateManager.CurrentMaterial, CurrentRecipe.Key, msg, RecipeTerminationState.canceled);
                PMStateManager.OnPMProcessFinished();
            }
        }

        public override void Init_Services()
        {
            base.Init_Services();

            var pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            PMIdentity = new Identity(pmConfiguration.ToolKey, pmConfiguration.Actor, pmConfiguration.ChamberKey);

            PMHandling = ClassLocator.Default.GetInstance<IEMEHandling>();
            PMHandling.HandlingManagerCB.Register(PMStateManager);
            PMHandling.Init();
            _recipeExecution = ClassLocator.Default.GetInstance<RecipeExecution>();
            _orchestrator = ClassLocator.Default.GetInstance<RecipeOrchestrator>();
            _dbRecipeServiceProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();

            Messenger.Register<RecipeExecutionMessage>(this, (r, m) => ExecutionStatusUpdated(m));
        }

        public override void Init_Status()
        {
            base.Init_Status();
        }

        public override void LoadMaterialOnChuck_pmtcs()
        {
            PMHandling.PMClampMaterial(PMStateManager.CurrentMaterial);
            // => Load action finished, Doing material clamp if needed
            PMHandling.CheckWaferPresenceAndClampOrRelease();
        }

        public override void MoveToLoadingUnloadingPosition_pmtcs(MaterialTypeInfo materialTypeInfo)
        {
            try
            {
                Logger.Information("MoveToLoadingUnloadingPosition - Move to Loading position");
                // Put stage in loading/unloading position
                PMHandling.MoveChuck(ChuckPosition.LoadingUnloadingPosition, materialTypeInfo);
                Logger.Information("MoveToLoadingUnloadingPosition - Open Door");
                PMHandling.MoveSlitDoor(SlitDoorPosition.OpenPosition);
            }
            catch
            {
                string msg = "MoveToLoadingUnloadingPosition - Move to Loading position FAILED";
                PMStateManager.SetError_GlobalStatus(ErrorID.MaterialTransferError, msg);
            }
        }

        public override void MoveToProcessPosition_pmtcs()
        {
            try
            {
                Logger.Information("MoveToProcessPosition - Move to process position");
                // Place stage in process position
                PMHandling.MoveChuck(ChuckPosition.ProcessPosition, PMStateManager.CurrentMaterial);
            }
            catch
            {
                string msg = "MoveToProcessPosition - Move to process position FAILED";
                PMStateManager.SetError_GlobalStatus(ErrorID.MaterialTransferError, msg);
            }
        }

        public override void OnTransferMaterialStarted_pmtcs()
        {
            base.OnTransferMaterialStarted_pmtcs();
        }

        public override void OnTransferMaterialFinished_pmtcs(String failedReason)
        {
            base.OnTransferMaterialFinished_pmtcs(failedReason);
        }

        public override void PMInitialization_pmtcs()
        {
            base.PMInitialization_pmtcs();
        }

        private void UpdateFDCData(List<FDCData> fdcsData)
        {
            if (DfSupervisor != null)
                DfSupervisor.NotifyFDCCollectionChanged(PMIdentity, fdcsData);
        }

        public override void StartRecipeExecution_pmtcs(Guid? pmRecipeKey, DataflowRecipeInfo dfRecipeInfo, Material material)
        {
            Logger.Information("-----------------------------------------------------------------------");

            string msgErr = String.Empty;
            string materialName = "Unknown";

            if (material == null)
            {
                msgErr = $"StartRecipe Failed. Material is unknown";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                ExecutionStatusUpdated(new RecipeExecutionMessage() { Status = ExecutionStatus.Failed, ErrorMessage = msgErr, RecipeKey = pmRecipeKey });
                return;
            }
            else
                materialName = material.ToString();

            if (material != CurrentMaterial)
            {
                msgErr = $"StartRecipe Failed. Material {materialName} associated to the starting recipe is not the current loaded material {CurrentMaterial.ToString()}.";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                ExecutionStatusUpdated(new RecipeExecutionMessage() { Status = ExecutionStatus.Failed, ErrorMessage = msgErr, RecipeKey = pmRecipeKey });
                return;
            }

            if (!pmRecipeKey.HasValue)
            {
                msgErr = $"StartRecipe Failed. Recipe to process material named {materialName} is unknown";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                ExecutionStatusUpdated(new RecipeExecutionMessage() { Status = ExecutionStatus.Failed, ErrorMessage = msgErr, RecipeKey = pmRecipeKey });
                return;
            }

            UnitySC.DataAccess.Dto.Recipe dbRecipe = null;
            EMERecipe emeRecipeToStart = null;
            try
            {
                dbRecipe = _dbRecipeServiceProxy.GetLastRecipeWithProductAndStep(pmRecipeKey.Value);
                if (dbRecipe == null)
                    throw new Exception($"DB recipe is null");
            }
            catch (Exception exDb)
            {
                msgErr = $"StartRecipe Failed. could not retreive Recipe <{pmRecipeKey.Value.ToString()}> from DB for <{materialName}>\nMsg={exDb.Message}";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                ExecutionStatusUpdated(new RecipeExecutionMessage() { Status = ExecutionStatus.Failed, ErrorMessage = msgErr, RecipeKey = pmRecipeKey });
                return;
            }

            try
            {
                emeRecipeToStart = _recipeExecution.Convert_RecipeToEMERecipe(dbRecipe);
            }
            catch (Exception exCon)
            {
                msgErr = $"StartRecipe Failed. Recipe <{dbRecipe.Name}> Convert error for <{materialName}>\nMsg={exCon.Message}";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                ExecutionStatusUpdated(new RecipeExecutionMessage() { Status = ExecutionStatus.Failed, ErrorMessage = msgErr, RecipeKey = pmRecipeKey });
                return;
            }

            if ((PMStateManager.TCPMState != TC_PMState.Active) && (PMStateManager.TCPMState != TC_PMState.Pending_To_Active))
            {
                Logger.Debug("Waiting chuck in process position to start recipe [timeout 1 minute]");
                if (!PMHandling.IsChuckInProcessPosition)
                {
                    DateTime startTime = DateTime.Now;
                    bool timeout = false;
                    // Check
                    do
                    {
                        Thread.Sleep(500);
                        timeout = (DateTime.Now.Subtract(startTime).TotalSeconds > 60);
                    } while ((!PMHandling.IsChuckInProcessPosition) && (!timeout));
                    if (timeout)
                    {
                        msgErr = $"StartRecipe failed. Chuck is not in process position";
                        PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                        ExecutionStatusUpdated(new RecipeExecutionMessage() { Status = ExecutionStatus.Failed, ErrorMessage = msgErr, RecipeKey = pmRecipeKey });
                        return;
                    }
                }
                CurrentRecipe = emeRecipeToStart;

                Logger.Information($"StartRecipe {CurrentRecipe.Name} on material {CurrentMaterial.ToString()}");
                

                var automationInfo = new RemoteProductionInfo() { ProcessedMaterial = material, DFRecipeName = dfRecipeInfo?.Name, ModuleRecipeName = CurrentRecipe?.Name, ModuleStartRecipeTime = DateTime.Now };
                var builder = ClassLocator.Default.GetInstance<RecipeAdapterBuilder>();
                var adaptedRecipe = builder.ValidateAndBuild(CurrentRecipe,null, automationInfo);                
                _orchestrator.Start(adaptedRecipe, PMIdentity, automationInfo);
            }
            else
            {
                msgErr = $"StartRecipe failed. Recipe {CurrentRecipe.Name} is already in progress with material {CurrentMaterial.ToString()}";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                ExecutionStatusUpdated(new RecipeExecutionMessage() { Status = ExecutionStatus.Failed, ErrorMessage = msgErr, RecipeKey = pmRecipeKey });
            }
        }

        public override void StartRecipeRequest_pmtcs(Material currentMaterial)
        {
            base.StartRecipeRequest_pmtcs(currentMaterial);
        }

        public override void UnloadMaterialOnChuck_pmtcs()
        {
            // => Pick/Place action, Do material unclamp if needed and clear Recipe selection
            // Unclamp if material present or not
            PMHandling.PMUnclampMaterial(PMStateManager.CurrentMaterial);
            // clear Recipe selection
            base.UnloadMaterialOnChuck_pmtcs();
        }

        public override void UpdateChuckPositionState_pmtcs()
        {
            base.UpdateChuckPositionState_pmtcs();
        }

        #endregion IPMTCManager

        #region IANARecipeServiceCallback

        public void ExecutionStatusUpdated(RecipeExecutionMessage recipeProgress)
        {
            if (!DfSupervisor.IsConnected)
                return;

            if (recipeProgress?.CurrentRemoteProductionInfo == null)
            {
                return;
            }

            var pmProgressInfo = GetPMProgressInfo(recipeProgress);
            PMStateManager.UpdatePMProgressInfo(pmProgressInfo);

            switch (recipeProgress.Status)
            {
                case ExecutionStatus.Executing:
                    // Fire ProcessStarted
                    if (PMStateManager.CurrentPMProcessingState.State == PMProcessingStates.Idle)
                    {
                        PMStateManager.OnPMProcessing();
                        Task.Run(() =>
                        {
                            // Inform Dataflow: PM recipe is started
                            DfSupervisor.RecipeStarted(PMIdentity, PMStateManager.CurrentMaterial);
                        });
                    }
                    break;
                case ExecutionStatus.Canceled:
                case ExecutionStatus.Failed:
                    ErrorID errid = ErrorID.RecipeExecutionError_PMError;
                    if (recipeProgress.Status == ExecutionStatus.Canceled)
                        errid = ErrorID.RecipeExecutionCanceled;
                    Task.Run(() =>
                    {
                        DfSupervisor.RecipeExecutionComplete(PMIdentity, PMStateManager.CurrentMaterial, recipeProgress?.RecipeKey, "Recipe " + recipeProgress.Status.ToString(), (recipeProgress.Status == ExecutionStatus.Canceled) ? RecipeTerminationState.canceled : RecipeTerminationState.failed);
                        PMStateManager.UpdatePMProgressInfo(new PMProgressInfo(STEP_NBR, ""));
                        PMStateManager.SetError_GlobalStatus(errid, recipeProgress.ErrorMessage);
                        PMStateManager.OnPMProcessFinished();
                    });
                    break;

                case ExecutionStatus.Finished:
                    Task.Run(() =>
                    {
                        DfSupervisor.RecipeExecutionComplete(PMIdentity, PMStateManager?.CurrentMaterial, recipeProgress?.RecipeKey, "Recipe succeed", RecipeTerminationState.successfull);
                        PMStateManager.UpdatePMProgressInfo(new PMProgressInfo(STEP_NBR, ""));
                        PMStateManager.OnPMProcessFinished();
                    });
                    break;

                case ExecutionStatus.NotExecuted:
                default:
                    break;
            }
        }

        private int _nbPoints = 0;
        #endregion IANARecipeServiceCallback

        protected PMProgressInfo GetPMProgressInfo(RecipeExecutionMessage recipeProgress)
        {
            switch (recipeProgress.Status)
            {
                case ExecutionStatus.NotExecuted:
                    CurrentPMProgressInfo.StepProgressInfo[STEP_ACQU].Percentage = 0;
                    break;
                case ExecutionStatus.Executing:
                    SetPMProgressInfo_Step(STEP_ACQU, recipeProgress);
                    break;

                case ExecutionStatus.Finished:
                    CurrentPMProgressInfo.StepProgressInfo[STEP_ACQU].Percentage = 100;
                    break;

                case ExecutionStatus.Failed:
                case ExecutionStatus.Canceled:
                default:
                    break;
            }
            CurrentPMProgressInfo.RecipeName = recipeProgress.CurrentRemoteProductionInfo.ModuleRecipeName;
            return CurrentPMProgressInfo;
        }
        protected void SetPMProgressInfo_Step(int stepIndex, RecipeExecutionMessage recipeProgress)
        {
            if (recipeProgress.TotalImages == 0)
                CurrentPMProgressInfo.StepProgressInfo[stepIndex].Percentage = 100;
            else
                CurrentPMProgressInfo.StepProgressInfo[stepIndex].Percentage = recipeProgress.ImageIndex * 100 / recipeProgress.TotalImages;
            CurrentPMProgressInfo.StepProgressInfo[stepIndex].SubstID = recipeProgress.CurrentRemoteProductionInfo.ProcessedMaterial.SubstrateID;
        }   
    }
}

