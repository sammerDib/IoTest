using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Shared;
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

using static UnitySC.PM.ANA.Service.Core.Recipe.ANARecipeExecutionManager;

using Material = UnitySC.Shared.Data.Material;


namespace UnitySC.PM.ANA.TC
{
    public class AnaPMTCManager : BasePMTCManager<ANARecipe, IANAHandling, RecipeProgressState>, IANARecipeServiceCallback
    {
        #region Members

        private IANARecipeExecutionManager _recipeExecutionManager;
        public ProgressingEventHandler RecipeProgressing;
        public MeasureResultEventHandler MeasureResultChanging;
        private DbRecipeServiceProxy _dbRecipeServiceProxy;

        public const int STEP_NBR = 1;
        public const int STEP_ALL_PROCESS = 0;

        #endregion Members

        #region Constructor

        public AnaPMTCManager()
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
                Logger.Information($"Abort current recipe running : {CurrentRecipe.Name}");
                _recipeExecutionManager.StopRecipe();
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

            PMHandling = ClassLocator.Default.GetInstance<IANAHandling>();
            PMHandling.HandlingManagerCB.Register(PMStateManager);
            PMHandling.Init();
            _recipeExecutionManager = ClassLocator.Default.GetInstance<IANARecipeExecutionManager>();
            _dbRecipeServiceProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
           
            Messenger.Register<RecipeProgress>(this, (r, m) => RecipeProgressChanged(m));
        }

        public override void Init_Status()
        {
            base.Init_Status();
        }

        public override void LoadMaterialOnChuck_pmtcs()
        {
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
                _recipeExecutionManager.NotifyRecipeExecutionFailed();
                return;
            }
            else
                materialName = material.ToString();

            if (material != CurrentMaterial)
            {
                msgErr = $"StartRecipe Failed. Material {materialName} associated to the starting recipe is not the current loaded material {CurrentMaterial.ToString()}.";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                _recipeExecutionManager.NotifyRecipeExecutionFailed();
                return;
            }

            if (!pmRecipeKey.HasValue)
            {
                msgErr = $"StartRecipe Failed. Recipe to process material named {materialName} is unknown";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                _recipeExecutionManager.NotifyRecipeExecutionFailed();
                return;
            }

            Recipe dbRecipe = null;
            ANARecipe anaRecipeToStart = null;
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
                _recipeExecutionManager.NotifyRecipeExecutionFailed();
                return;
            }

            try
            {
                anaRecipeToStart = _recipeExecutionManager.Convert_RecipeToAnaRecipe(dbRecipe);
            }
            catch (Exception exCon)
            {
                msgErr = $"StartRecipe Failed. Recipe <{dbRecipe.Name}> Convert error for <{materialName}>\nMsg={exCon.Message}";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                _recipeExecutionManager.NotifyRecipeExecutionFailed();
                return;
            }

            try
            {
                ANARecipeHelper.UpdateRecipeWithExternalFiles(anaRecipeToStart, null);
            }
            catch (Exception exExtFile)
            {
                msgErr = $"StartRecipe Failed. Recipe <{dbRecipe.Name}> Failed loading external files for <{materialName}>\nMsg={exExtFile.Message}";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                _recipeExecutionManager.NotifyRecipeExecutionFailed();
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
                        _recipeExecutionManager.NotifyRecipeExecutionFailed();
                        return;
                    }
                }
                CurrentRecipe = anaRecipeToStart;

                Logger.Information($"StartRecipe {CurrentRecipe.Name} on material {CurrentMaterial.ToString()}");
                if (CurrentRecipe.IsWaferLessModified)
                {
                    Logger.Information("Recipe has been modified in Wafer Less Mode");

                    var pattern = Consts.WaferLessCommentPattern;
                    // Define the regex with the multiline option to match each line separately
                    Regex regex = new Regex(pattern, RegexOptions.Multiline);

                    // Find the first match
                    Match match = regex.Match(CurrentRecipe.Comment);
                    if (match.Success)
                    {
                        Logger.Information(match.Value);
                    }
                }
                
                var automationInfo = new RemoteProductionInfo() { ProcessedMaterial = material, DFRecipeName = dfRecipeInfo?.Name, ModuleRecipeName = CurrentRecipe?.Name, ModuleStartRecipeTime= DateTime.Now };

                _recipeExecutionManager.Execute(CurrentRecipe, automationInfo);
            }
            else
            {
                msgErr = $"StartRecipe failed. Recipe {CurrentRecipe.Name} is already in progress with material {CurrentMaterial.ToString()}";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                _recipeExecutionManager.NotifyRecipeExecutionFailed();
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
            bool success = SpinWait.SpinUntil(() => PMStateManager.CurrentTransferValidationState.TransferValidationData.MaterialUnclamped, 1500); // Wait unclamp and other signals
            if(success)
                Logger.Debug($"TransferValidation: Unclamp state = {PMStateManager.CurrentTransferValidationState.TransferValidationData.MaterialUnclamped}");
            else
                Logger.Error($"TransferValidation TIMEOUT : Not reliable Unclamp state = {PMStateManager.CurrentTransferValidationState.TransferValidationData.MaterialUnclamped}");
            // clear Recipe selection
            base.UnloadMaterialOnChuck_pmtcs();
        }

        public override void UpdateChuckPositionState_pmtcs()
        {
            base.UpdateChuckPositionState_pmtcs();
        }

        #endregion IPMTCManager

        #region IANARecipeServiceCallback

        public void MeasureResultChanged(UnitySC.Shared.Format.Metro.MeasurePointResult res, string resultFolderPath, DieIndex dieIndex)
        {
            // Nothing to do
        }

        public void RecipeFinished(List<UnitySC.Shared.Format.Metro.MetroResult> results)
        {
            // From editor recipe - Nothing to do
        }

        public void RecipeStarted(ANARecipeWithExecContext startedRecipe)
        {
            // Nothing to do
        }
        public void RecipeProgressChanged(RecipeProgress recipeProgress)
        {
            // Check if progression sent in remote mode
            if (recipeProgress.RemoteInfo == null) 
                return;
            if (!DfSupervisor.IsConnected)
                return;
            var pmProgressInfo = GetRecipeProgressInfo(recipeProgress);
            PMStateManager.UpdatePMProgressInfo(pmProgressInfo);

            switch (recipeProgress.RecipeProgressState)
            {
                case RecipeProgressState.AutoFocusInProgress:
                case RecipeProgressState.AutoLightInProgress:
                case RecipeProgressState.EdgeAlignmentInProgress:
                case RecipeProgressState.MarkAlignmentInProgress:
                case RecipeProgressState.Measuring:
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

                case RecipeProgressState.InPause:
                    break;

                case RecipeProgressState.Canceled:
                case RecipeProgressState.Error:
                    ErrorID errid = ErrorID.RecipeExecutionError_PMError;
                    if (recipeProgress.RecipeProgressState == RecipeProgressState.Canceled)
                        errid = ErrorID.RecipeExecutionCanceled;
                    Task.Run(() =>
                    {
                        DfSupervisor.RecipeAcquisitionComplete(PMIdentity, PMStateManager?.CurrentMaterial, recipeProgress?.RunningRecipeInfo?.Key, "Recipe " + recipeProgress.RecipeProgressState.ToString(), (recipeProgress.RecipeProgressState == RecipeProgressState.Canceled) ? RecipeTerminationState.canceled : RecipeTerminationState.failed);
                        DfSupervisor.RecipeExecutionComplete(PMIdentity, PMStateManager.CurrentMaterial, recipeProgress?.RunningRecipeInfo?.Key, "Recipe " + recipeProgress.RecipeProgressState.ToString(), (recipeProgress.RecipeProgressState == RecipeProgressState.Canceled) ? RecipeTerminationState.canceled : RecipeTerminationState.failed);
                        PMStateManager.UpdatePMProgressInfo(new PMProgressInfo(STEP_NBR, ""));
                        PMStateManager.SetError_GlobalStatus(errid, recipeProgress.Message);
                        PMStateManager.OnPMProcessFinished();
                    });
                    break;

                case RecipeProgressState.Success:
                    Task.Run(() =>
                    {
                        DfSupervisor.RecipeAcquisitionComplete(PMIdentity, PMStateManager?.CurrentMaterial, recipeProgress?.RunningRecipeInfo?.Key, "Recipe succeed", RecipeTerminationState.successfull);
                        DfSupervisor.RecipeExecutionComplete(PMIdentity, PMStateManager?.CurrentMaterial, recipeProgress?.RunningRecipeInfo?.Key, "Recipe succeed", RecipeTerminationState.successfull);
                        PMStateManager.UpdatePMProgressInfo(new PMProgressInfo(STEP_NBR, ""));
                        PMStateManager.OnPMProcessFinished();
                    });
                    break;

                default:
                    break;
            }
        }

        private int _nbPoints = 0;
        #endregion IANARecipeServiceCallback



        private PMProgressInfo GetRecipeProgressInfo(RecipeProgress recipeProgress)
        {
            switch (recipeProgress.RecipeProgressState)
            {
                case RecipeProgressState.AutoFocusInProgress: CurrentPMProgressInfo.StepProgressInfo[STEP_ALL_PROCESS].Percentage = 2; break;
                case RecipeProgressState.AutoLightInProgress: CurrentPMProgressInfo.StepProgressInfo[STEP_ALL_PROCESS].Percentage = 4; break;
                case RecipeProgressState.EdgeAlignmentInProgress: CurrentPMProgressInfo.StepProgressInfo[STEP_ALL_PROCESS].Percentage = 6; break;
                case RecipeProgressState.MarkAlignmentInProgress: CurrentPMProgressInfo.StepProgressInfo[STEP_ALL_PROCESS].Percentage = 8; break;
                case RecipeProgressState.Measuring:
                {
                    if (recipeProgress.NbRemainingPoints == 0)
                        CurrentPMProgressInfo.StepProgressInfo[STEP_ALL_PROCESS].Percentage = 100;
                    else
                    {
                        if (_nbPoints == 0)
                            _nbPoints = recipeProgress.NbRemainingPoints;
                        CurrentPMProgressInfo.StepProgressInfo[STEP_ALL_PROCESS].Percentage = 10 + (_nbPoints - recipeProgress.NbRemainingPoints) * 90 / _nbPoints;
                    }
                    break;
                }
                case RecipeProgressState.Success: CurrentPMProgressInfo.StepProgressInfo[STEP_ALL_PROCESS].Percentage = 100; _nbPoints = 0; break;
                case RecipeProgressState.InPause: break;
                case RecipeProgressState.Canceled: _nbPoints = 0; break;
                case RecipeProgressState.Error: _nbPoints = 0; break;
                default:
                    break;
            }
            CurrentPMProgressInfo.RecipeName = recipeProgress.RunningRecipeInfo.Name;
            return CurrentPMProgressInfo;
        }
    }
}
