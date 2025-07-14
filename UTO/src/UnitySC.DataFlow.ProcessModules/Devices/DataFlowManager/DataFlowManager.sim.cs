using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.DataAccess.Dto;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager
{
    public partial class DataFlowManager
    {
        private List<bool> _jobsInProgress;
        private string _currentRecipe;

        private const string RecipeNameWithTwoPmNoAlignment =
            "Recipe with 2 PM, no alignment between PM";

        private const string RecipeNameWithTwoPmWithAlignment =
            "Recipe with 2 PM, with alignment between PM";

        private const string RecipeNameWithPm1Only = "Recipe with PM1 only";
        private const string RecipeNameWithPm2Only = "Recipe with PM2 only";

        #region Commands

        protected override void InternalSimulateStartRecipe(
            MaterialRecipe materialRecipe,
            string processJobId,
            Tempomat tempomat)
        {
            _currentRecipe = materialRecipe.Recipe.Name;
            var jobProgram = new UTOJobProgram();
            var processModules = this.GetEquipment().AllDevices<UnityProcessModule>();
            jobProgram.PMItems = new List<PMItem>();
            switch (processModules.Count())
            {
                case 1:
                    jobProgram.PMItems.Add(
                        new PMItem() { OrientationAngle = 90, PMType = ActorType.ANALYSE });
                    break;
                case 2:
                    switch (_currentRecipe)
                    {
                        case RecipeNameWithTwoPmWithAlignment:
                            jobProgram.PMItems.Add(
                                new PMItem() { OrientationAngle = 90, PMType = ActorType.ANALYSE });
                            jobProgram.PMItems.Add(
                                new PMItem() { OrientationAngle = 45, PMType = ActorType.DEMETER });
                            break;
                        case RecipeNameWithPm1Only:
                            jobProgram.PMItems.Add(
                                new PMItem() { OrientationAngle = 90, PMType = ActorType.ANALYSE });
                            break;
                        case RecipeNameWithPm2Only:
                            jobProgram.PMItems.Add(
                                new PMItem() { OrientationAngle = 90, PMType = ActorType.DEMETER });
                            break;
                        default:
                            jobProgram.PMItems.Add(
                                new PMItem() { OrientationAngle = 90, PMType = ActorType.ANALYSE });
                            jobProgram.PMItems.Add(
                                new PMItem() { OrientationAngle = 90, PMType = ActorType.DEMETER });
                            break;
                    }

                    break;
            }

            foreach (var wafer in materialRecipe.Wafers)
            {
                wafer.JobProgram = jobProgram;
            }
        }

        protected override void InternalSimulateAbortRecipe(string jobId, Tempomat tempomat)
        {
            foreach (var processModule in this.GetEquipment().AllDevices<UnityProcessModule>())
            {
                if (processModule.ProcessModuleState == ProcessModuleState.Active
                    || processModule.ProcessModuleState == ProcessModuleState.Error)
                {
                    processModule.AbortRecipe();
                }
            }
        }

        protected override void InternalSimulateStartJobOnMaterial(
            DataflowRecipeInfo recipe,
            Wafer wafer,
            Tempomat tempomat)
        {
            //Do nothing in simulation mode
        }

        protected override void InternalSimulateGetAvailableRecipes(Tempomat tempomat)
        {
            AvailableRecipes = new List<DataflowRecipeInfo>();
            var recipeId = 1;
            for (var iProductId = 1; iProductId <= 3; iProductId++)
            {
                for (var iStepId = 1; iStepId <= 4; iStepId++)
                {
                    if (iProductId == 1 && iStepId == 1)
                    {
                        AvailableRecipes.Add(
                            new DataflowRecipeInfo()
                            {
                                CreatedDate = DateTime.Now,
                                Id = recipeId,
                                IdGuid = Guid.NewGuid(),
                                Name = RecipeNameWithTwoPmNoAlignment,
                                ProductId = iProductId,
                                StepId = iStepId
                            });
                        AvailableRecipes.Add(
                            new DataflowRecipeInfo()
                            {
                                CreatedDate = DateTime.Now,
                                Id = recipeId,
                                IdGuid = Guid.NewGuid(),
                                Name = RecipeNameWithTwoPmWithAlignment,
                                ProductId = iProductId,
                                StepId = iStepId
                            });
                        AvailableRecipes.Add(
                            new DataflowRecipeInfo()
                            {
                                CreatedDate = DateTime.Now,
                                Id = recipeId,
                                IdGuid = Guid.NewGuid(),
                                Name = RecipeNameWithPm1Only,
                                ProductId = iProductId,
                                StepId = iStepId
                            });
                        AvailableRecipes.Add(
                            new DataflowRecipeInfo()
                            {
                                CreatedDate = DateTime.Now,
                                Id = recipeId,
                                IdGuid = Guid.NewGuid(),
                                Name = RecipeNameWithPm2Only,
                                ProductId = iProductId,
                                StepId = iStepId
                            });
                    }

                    AvailableRecipes.Add(
                        new DataflowRecipeInfo()
                        {
                            CreatedDate = DateTime.Now,
                            Id = recipeId,
                            IdGuid = Guid.NewGuid(),
                            Name = $"Recipe{recipeId}",
                            ProductId = iProductId,
                            StepId = iStepId
                        });
                    recipeId++;
                }
            }
        }

        protected override void InternalSimulateStartCommunication(Tempomat tempomat)
        {
            base.InternalSimulateStartCommunication(tempomat);

            //Need to register alarms only the first time we are connected
            if (!_firstConnection)
            {
                return;
            }

            InternalSimulateGetAvailableRecipes(tempomat);
            _firstConnection = false;
        }

        #endregion

        #region Event Handlers

        private void ProcessModule_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not UnityProcessModule processModule)
            {
                return;
            }

            if (e.Execution.Context.Command.Name == nameof(IUnityProcessModule.StartRecipe)
                && e.NewState == ExecutionState.Success
                && processModule.Location.Wafer != null)
            {
                Logger.Info($"Recipe started on process module {processModule.InstanceId}");
                OnProcessModuleRecipeStarted(
                    new ProcessModuleRecipeEventArgs(processModule, _currentRecipe));
            }
        }

        private void ProcessModule_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not UnityProcessModule processModule)
            {
                return;
            }

            if (e.PropertyName.Equals(nameof(UnityProcessModule.ProcessModuleState))
                && processModule.ProcessModuleState == ProcessModuleState.Idle
                && _jobsInProgress[processModule.InstanceId - 1])
            {
                _jobsInProgress[processModule.InstanceId - 1] = false;
                Logger.Info($"Recipe finished on process module {processModule.InstanceId}");
                OnProcessModuleAcquisitionCompleted(
                    new ProcessModuleRecipeEventArgs(processModule, _currentRecipe));
                OnProcessModuleRecipeCompleted(
                    new ProcessModuleRecipeEventArgs(processModule, _currentRecipe));
            }
        }

        #endregion

        #region Private Methods

        private void SetUpSimulatedMode()
        {
            _jobsInProgress = new List<bool>();
            foreach (var processModule in this.GetEquipment().AllDevices<UnityProcessModule>())
            {
                _jobsInProgress.Add(false);
                processModule.CommandExecutionStateChanged +=
                    ProcessModule_CommandExecutionStateChanged;
                processModule.PropertyChanged += ProcessModule_PropertyChanged;
            }
        }

        private void DisposeSimulatedMode()
        {
            foreach (var processModule in this.GetEquipment().AllDevices<UnityProcessModule>())
            {
                processModule.CommandExecutionStateChanged -=
                    ProcessModule_CommandExecutionStateChanged;
                processModule.PropertyChanged -= ProcessModule_PropertyChanged;
            }
        }

        #endregion
    }
}
