using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.PM.DMT.Service.Implementation.Execution;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.TC.PM.Operations.Implementation;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.PM.DMT.TC
{
    public class DMTPMTCManager : BasePMTCManager<DMTRecipe, IDMTHandling, DMTRecipeState>, IDisposable
    {
        private readonly RecipeExecution _recipeExecution;
        private readonly IDMTRecipeService _recipeService;

        public const int STEP_NBR = 2;
        public const int STEP_ACQU = 0;
        public const int STEP_COMP = 1;

        public DMTPMTCManager(RecipeExecution recipeExecution, IDMTRecipeService recipeService)
        {
            _recipeExecution = recipeExecution;
            _recipeService = recipeService;
            CurrentPMProgressInfo = new PMProgressInfo(STEP_NBR,"");
        }

        public void Dispose()
        {
            _recipeExecution.ProgressChanged -= RecipeExecution_Progress;
            //TODO _recipeExecution.Dispose();
        }

        #region DMTRecipeService Callback

        private void RecipeExecution_Progress(object sender, RecipeStatus recipeStatus)
        {
            // TODO FDS Check with Christophe if it is OK
            if (recipeStatus?.CurrentRemoteProductionInfo == null)
            {
                return;
            }

            var pmProgressInfo = GetPMProgressInfo(recipeStatus, recipeStatus.CurrentRemoteProductionInfo.ProcessedMaterial);
            PMStateManager.UpdatePMProgressInfo(pmProgressInfo);

            if (!DfSupervisor.IsConnected)
                return;
            CurrentRecipeState = recipeStatus.State;

            switch (recipeStatus.State)
            {
                case DMTRecipeState.Preparing:
                    PMStateManager.OnPMPreparing();
                    break;
                
                case DMTRecipeState.Executing:
                    if (recipeStatus.Step == DMTRecipeExecutionStep.Acquisition)
                    {
                        // Fire ProcessStarted
                        if (PMStateManager.CurrentPMProcessingState.State == PMProcessingStates.Idle)
                        {
                            PMStateManager.OnPMProcessing();
                            Task.Run(() =>
                            {
                                // Inform Dataflow: PM recipe is started
                                DfSupervisor.RecipeStarted(PMIdentity, recipeStatus.CurrentRemoteProductionInfo.ProcessedMaterial);
                            });
                        }
                    }
                    break;

                case DMTRecipeState.Aborted:
                case DMTRecipeState.Failed:
                    var errid = ErrorID.RecipeExecutionError_PMError;
                    if (recipeStatus.State == DMTRecipeState.Aborted)
                        errid = ErrorID.RecipeExecutionCanceled;
                    if (PMStateManager.CurrentPMProcessingState.State == PMProcessingStates.Processing)
                        PMStateManager.OnPMProcessFinished();
                    Task.Run(() =>
                    {
                        DfSupervisor.RecipeExecutionComplete(PMIdentity, recipeStatus.CurrentRemoteProductionInfo.ProcessedMaterial,
                                                             recipeStatus.RecipeKey, "Recipe " + recipeStatus.CurrentStep,
                                                             recipeStatus.State == DMTRecipeState.Aborted
                                                                 ? RecipeTerminationState.canceled
                                                                 : RecipeTerminationState.failed);
                        PMStateManager.SetError_GlobalStatus(errid, recipeStatus.Message);
                        PMStateManager.UpdatePMProgressInfo(new PMProgressInfo(STEP_NBR, ""));
                    });
                    break;

                case DMTRecipeState.AcquisitionComplete:
                    if (PMStateManager.CurrentPMProcessingState.State == PMProcessingStates.Processing)
                    {
                        PMStateManager.OnPMProcessFinished();
                    }
                    Task.Run(() =>
                    {
                        DfSupervisor.RecipeAcquisitionComplete(PMIdentity, recipeStatus.CurrentRemoteProductionInfo.ProcessedMaterial,
                                                             recipeStatus.RecipeKey, "Acquisition succeed",
                                                             RecipeTerminationState.successfull);
                    });
                    break;

                case DMTRecipeState.ExecutionComplete:
                    Task.Run(() =>
                    {
                        if (PMStateManager.CurrentPMProcessingState.State == PMProcessingStates.Processing)
                        {
                            PMStateManager.OnPMProcessFinished();
                        }
                        DfSupervisor.RecipeExecutionComplete(PMIdentity, recipeStatus.CurrentRemoteProductionInfo.ProcessedMaterial,
                                                             recipeStatus.RecipeKey, "Recipe succeed",
                                                             RecipeTerminationState.successfull);
                        PMStateManager.UpdatePMProgressInfo(new PMProgressInfo(STEP_NBR,""));
                    });
                    break;
            }
        }

        #endregion DMTRecipeService Callback

        private int _totalAcqSteps;
        private int _totalCalc;

        protected PMProgressInfo GetPMProgressInfo(RecipeStatus status, Material material)
        {
            switch (status.State)
            {
                case DMTRecipeState.Executing:
                    switch (status.Step)
                    {
                        case DMTRecipeExecutionStep.Acquisition:
                                SetPMProgressInfo_Step(STEP_ACQU, status, material);
                            break;

                        case DMTRecipeExecutionStep.Computation:
                            SetPMProgressInfo_Step(STEP_COMP, status, material);
                            break;

                        default:
                            break;
                    }
                    break;

                case DMTRecipeState.AcquisitionComplete:
                    CurrentPMProgressInfo.StepProgressInfo[STEP_ACQU].Percentage = 100;
                    break;

                case DMTRecipeState.ExecutionComplete:
                    CurrentPMProgressInfo.StepProgressInfo[STEP_ACQU].Percentage = 100;
                    CurrentPMProgressInfo.StepProgressInfo[STEP_COMP].Percentage = 100;                           
                    break;

                case DMTRecipeState.Failed:
                case DMTRecipeState.Aborted:
                default:
                    break;
            }
            CurrentPMProgressInfo.RecipeName = status.CurrentRemoteProductionInfo.ModuleRecipeName;
            return CurrentPMProgressInfo;
        }

        protected void SetPMProgressInfo_Step(int stepIndex, RecipeStatus acquisitionStepStatus, Material material)
        {
            if (acquisitionStepStatus.TotalSteps == 0)
                CurrentPMProgressInfo.StepProgressInfo[stepIndex].Percentage = 100;
            else
                CurrentPMProgressInfo.StepProgressInfo[stepIndex].Percentage = acquisitionStepStatus.CurrentStep * 100 / acquisitionStepStatus.TotalSteps;
            CurrentPMProgressInfo.StepProgressInfo[stepIndex].SubstID = material.SubstrateID;
        }

        private void StartRecipeExecution_Failed(string errorMessage)
        {
            var status = new RecipeStatus();
            status.Message = errorMessage;
            status.State = DMTRecipeState.Failed;
            RecipeExecution_Progress(null, status);
        }

        public List<TCPMRecipe> GetRecipesList()
        {
            var recipesList = _recipeService.GetTCRecipeList();
            var tcRecipesList = new List<TCPMRecipe>();

            if (recipesList.Result != null)
            {
                tcRecipesList.AddRange(recipesList.Result);
            }

            return tcRecipesList;
        }

        #region IPMTCService

        public override void AbortRecipeExecution_pmtcs()
        {
            if (PMStateManager.CurrentTCPMState.State == TC_PMState.Active)
            {
                // Abort current recipe running
                Logger.Information("-----------------------------------------------------------------------");
                Logger.Information($"Abort current recipe running : {CurrentRecipe.Name}");
                _recipeExecution.AbortExecution();
            }
            else
            {
                string msg = "Recipe aborted before starting";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeExecutionCanceled, msg);
                if (CurrentRecipe != null)
                    DfSupervisor.RecipeExecutionComplete(PMIdentity, PMStateManager.CurrentMaterial, CurrentRecipe.Key,
                                                         msg, RecipeTerminationState.canceled);
                PMStateManager.OnPMProcessFinished();
            }
        }

        public override void Init_Services()
        {
            base.Init_Services();

            // Identity update
            var pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            PMIdentity = new Identity(pmConfiguration.ToolKey, pmConfiguration.Actor, pmConfiguration.ChamberKey);

            // Handling for PM in mode remote
            PMHandling = ClassLocator.Default.GetInstance<IDMTHandling>();
            PMHandling.HandlingManagerCB.Register(PMStateManager);
            PMHandling.Init();

            // Acquisition progression set up
            _recipeExecution.ProgressChanged += RecipeExecution_Progress;
        }

        public override void Init_Status()
        {
            base.Init_Status();
        }

        public override void LoadMaterialOnChuck_pmtcs()
        {
            base.LoadMaterialOnChuck_pmtcs();
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
                Logger.Information("MoveToProcessPosition - Close Door");
                PMHandling.MoveSlitDoor(SlitDoorPosition.ClosePosition);

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

        public override void OnTransferMaterialFinished_pmtcs(string failedReason)
        {
            base.OnTransferMaterialFinished_pmtcs(failedReason);
        }

        public override void PMInitialization_pmtcs()
        {
            base.PMInitialization_pmtcs();
        }

        public override void StartRecipeExecution_pmtcs(
            Guid? pmRecipeKey, DataflowRecipeInfo dfRecipeInfo, Material material)
        {
            string materialName = "Unknown";
            if (material != null)
                materialName = material.ToString();

            if (pmRecipeKey == null)
            {
                string msgErr = $"StartRecipe Failed. Recipe to process material named {materialName} is unknown";
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                //Update Progression Process failed
                StartRecipeExecution_Failed(msgErr);
                return;
            }

            var dmtRecipeToStart = _recipeService.GetLastRecipeWithProductAndStep((Guid)pmRecipeKey).Result;

            if (dmtRecipeToStart is null)
            {
                string msgErr = "StartRecipe Failed. Error while loading the recipe from the database";
                Logger.Error(msgErr);
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                StartRecipeExecution_Failed(msgErr);
                return;
            }

            if (material == null)
            {
                string msgErr = $"StartRecipe Failed. {dmtRecipeToStart.Name} recipe on material {materialName}";
                Logger.Error(msgErr);
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                StartRecipeExecution_Failed(msgErr);
                return;
            }

            if (material != CurrentMaterial)
            {
                string msgErr = $"StartRecipe Failed. Current material loaded is not material {materialName} expected.";
                Logger.Error(msgErr);
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                StartRecipeExecution_Failed(msgErr);
                return;
            }

            if (PMStateManager.TCPMState != TC_PMState.Active &&
                PMStateManager.TCPMState != TC_PMState.Pending_To_Active)
            {
                Logger.Debug("Waiting chuck in process position to start recipe [timeout 1 minute]");
                if (!PMHandling.IsChuckInProcessPosition)
                {
                    var startTime = DateTime.Now;
                    bool timeout = false;
                    // Check
                    do
                    {
                        Thread.Sleep(500);
                        timeout = DateTime.Now.Subtract(startTime).TotalSeconds > 60;
                    } while (!PMHandling.IsChuckInProcessPosition && !timeout);

                    if (timeout)
                    {
                        string msgErr =
                            $"StartRecipe {dmtRecipeToStart.Name} rejected on material {materialName}. Chuck is not in process position";
                        Logger.Error(msgErr);
                        PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                        StartRecipeExecution_Failed(msgErr);
                        return;
                    }
                }

                CurrentRecipe = dmtRecipeToStart;

                Logger.Information("-----------------------------------------------------------------------");
                Logger.Information($"StartRecipe {CurrentRecipe.Name} on material {CurrentMaterial}");

                // Execute recipe
                var remoteProductionInfo = new RemoteProductionInfo()
                {
                    ProcessedMaterial = material,
                    ModuleRecipeName = CurrentRecipe.Name,
                    DFRecipeName = dfRecipeInfo.Name,
                    ModuleStartRecipeTime = DateTime.Now
                };

                _recipeExecution.StartRecipe(CurrentRecipe, remoteProductionInfo, true, string.Empty);
            }
            else
            {
                string msgErr =
                    $"StartRecipe {dmtRecipeToStart.Name} rejected on material {materialName}. Recipe {CurrentRecipe.Name} already in progress on material {PMStateManager.CurrentMaterial}";
                Logger.Error(msgErr);
                PMStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMStart, msgErr);
                StartRecipeExecution_Failed(msgErr);
            }
        }

        public override void StartRecipeRequest_pmtcs(Material currentMaterial)
        {
            while (_recipeExecution.IsRecipeRunning)
            {
                Task.Run(() => Task.Delay(500)).Wait();
            }

            base.StartRecipeRequest_pmtcs(currentMaterial);
        }

        public override void UnloadMaterialOnChuck_pmtcs()
        {
            // => Pick/Place action, Do material unclamp if needed and clear Recipe selection
            // No Unclamp on PSD
            // clear Recipe selection
            base.UnloadMaterialOnChuck_pmtcs();
        }

        public override void UpdateChuckPositionState_pmtcs()
        {
            base.UpdateChuckPositionState_pmtcs();
        }

        #endregion IPMTCService
    }
}
