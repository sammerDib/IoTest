using System;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.ProcessingFramework;
using Agileo.Recipes.Management;

using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericController;
using UnitySC.Equipment.Abstractions.Vendor.Devices.RecipeProcessModule;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.ProcessExecution;
using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Runner.ExecutionConfiguration;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Recipes.Runner
{
    public class RecipeRunnerPanel : BaseRecipeLibraryPanel
    {
        #region Fields

        private readonly ILogger _logger;
        private const string RecipeRunnerTracerName = "Recipe Runner";
        private readonly RecipeProcessModule _processModule;
        private static EquipmentManager EquipmentManager
            => App.Instance.EquipmentManager;
        private AppProgramProcessor AppProgramProcessor => App.Instance.SequenceProgramProcessor;

        #endregion

        #region Ctor

        public RecipeRunnerPanel()
            : this(
                nameof(L10N.BP_SCENARIO_SEQUENCER),
                new RecipeManager(),
                null,
                PathIcon.PlayDocument)
        {
        }

        public RecipeRunnerPanel(
            string relativeId,
            RecipeManager recipeManager,
            RecipeProcessModule processModule,
            IIcon icon = null)
            : base(relativeId, recipeManager, icon)
        {
            _logger = App.Instance.GetLogger(RecipeRunnerTracerName);
            _processModule = processModule;

            StartPanelCommand = new BusinessPanelCommand(
                nameof(RecipePanelResources.RECIPE_START),
                StartCommand,
                PathIcon.Play);
            Commands.Add(StartPanelCommand);

            ResumePanelCommand = new BusinessPanelCommand(
                nameof(RecipePanelResources.RECIPE_RESUME),
                ResumeCommand,
                PathIcon.Resume);
            Commands.Add(ResumePanelCommand);

            PausePanelCommand = new BusinessPanelCommand(
                nameof(RecipePanelResources.RECIPE_PAUSE),
                PauseScenarioCommand,
                PathIcon.Pause);
            Commands.Add(PausePanelCommand);

            StopPanelCommand = new BusinessPanelCommand(
                nameof(RecipePanelResources.RECIPE_STOP),
                StopCommand,
                PathIcon.Stop);
            Commands.Add(StopPanelCommand);

            AbortPanelCommand = new BusinessPanelCommand(
                nameof(RecipePanelResources.RECIPE_ABORT),
                AbortCommand,
                PathIcon.Private);
            Commands.Add(AbortPanelCommand);

            StepModePanelCommand = new BusinessPanelToggleCommand(
                nameof(RecipePanelResources.RECIPE_STEP_MODE),
                new BusinessPanelCommand(
                    nameof(RecipePanelResources.RECIPE_STEP_MODE_ON),
                    StepModeCommand,
                    PathIcon.FilledCircle),
                new BusinessPanelCommand(
                    nameof(RecipePanelResources.RECIPE_STEP_MODE_OFF),
                    StepModeCommand,
                    PathIcon.EmptyDot));
            Commands.Add(StepModePanelCommand);

            LoopCommand = new BusinessPanelCheckToggleCommand(
                nameof(RecipePanelResources.RECIPE_EXECUTION_CONFIGURATION),
                ConfigureExecutionCommand,
                ConfigureExecutionCommand,
                PathIcon.Infinity);
            Commands.Add(LoopCommand);
        }

        #endregion

        #region Properties

        public BusinessPanelCommand AbortPanelCommand { get; }

        public BusinessPanelCommand StopPanelCommand { get; }

        public BusinessPanelCommand PausePanelCommand { get; }

        public BusinessPanelCommand ResumePanelCommand { get; }

        public BusinessPanelCommand StartPanelCommand { get; }

        public BusinessPanelToggleCommand StepModePanelCommand { get; }

        public BusinessPanelCheckToggleCommand LoopCommand { get; }

        private RecipeExecutionViewModel _recipeExecutionViewModel;

        public RecipeExecutionViewModel RecipeExecutionViewModel
        {
            get => _recipeExecutionViewModel;
            set => SetAndRaiseIfChanged(ref _recipeExecutionViewModel, value);
        }

        private int _currentExecutionNumber;

        public int CurrentExecutionNumber
        {
            get => _currentExecutionNumber;
            set
            {
                if (SetAndRaiseIfChanged(ref _currentExecutionNumber, value))
                {
                    OnPropertyChanged(nameof(CycleProgression));
                }
            }
        }

        private int _requiredExecutionNumber;

        public int RequiredExecutionNumber
        {
            get => _requiredExecutionNumber;
            set
            {
                if (SetAndRaiseIfChanged(ref _requiredExecutionNumber, value))
                {
                    OnPropertyChanged(nameof(CycleProgression));
                }
            }
        }

        public string CycleProgression
        {
            get
            {
                if (RequiredExecutionNumber == -1)
                {
                    return $"(Cycle {CurrentExecutionNumber})";
                }

                return RequiredExecutionNumber > 0
                    ? $"(Cycle {CurrentExecutionNumber}/{RequiredExecutionNumber})"
                    : string.Empty;
            }
        }

        public bool IsStepModeActive { get; set; }

        private static bool JobNotRunning
        {
            get
            {
                var ctrl = EquipmentManager.Equipment.AllDevices<GenericController>().FirstOrDefault();
                return ctrl == null ||
                       ctrl.CurrentActivity == null ||
                       (!ctrl.CurrentActivity.IsStarted && ctrl.State == OperatingModes.Idle);
            }
        }

        private bool ScenarioNotRunning
            => AppProgramProcessor.State == ProcessorState.Complete
               || AppProgramProcessor.CurrentProgram == null;

        #endregion

        #region Commands

        #region Start Command

        private DelegateCommand _runScenarioCommand;

        public ICommand StartCommand
            => _runScenarioCommand ??= new DelegateCommand(StartExecute, StartCanExecute);

        private bool StartCanExecute()
        {
            if (RecipeExecutionViewModel == null)
            {
                return false;
            }

            return JobNotRunning
                && ScenarioNotRunning
                && (_processModule.ProcessProgramProcessor?.State == ProcessorState.Complete
                   || (_processModule.ProcessProgram == null
                       && _processModule.State == OperatingModes.Idle));
        }

        private void StartExecute()
        {
            DetailsIsExpanded = true;
            CurrentExecutionNumber = 0;
            InternalStart();
        }

        private void InternalStart()
        {
            if (RecipeExecutionViewModel == null)
            {
                return;
            }

            try
            {
                var component = RecipeExecutionViewModel.Component;
                var program = component.ToProgram(App.Instance.RecipeManager.GetRecipes());

                foreach (var programInstruction in program.Instructions
                             .OfType<IUserInterfaceInstruction>())
                {
                    programInstruction.TargetedBusinessPanel = this;
                }

                _processModule.SetupProcessProgram(program);

                if (IsStepModeActive)
                {
                    _processModule.BreakpointProcess(true);
                }

                // [TLa] The program is not persistent so it is necessary to create a new viewer in case the program needs to be restarted.
                RecipeExecutionViewModel = new RecipeExecutionViewModel(component, _processModule);

                ++CurrentExecutionNumber;

                _processModule.StartProcessExecution();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                var msg = new UserMessage(
                    MessageLevel.Error,
                    nameof(RecipePanelResources.RECIPE_START_ERROR));
                msg.CanUserCloseMessage = true;
                Messages.Show(msg);
            }
        }

        #endregion

        #region Resume Command

        private ICommand _resumeCommand;

        public ICommand ResumeCommand
            => _resumeCommand
               ?? (_resumeCommand = new DelegateCommand(
                   ResumeCommandExecute,
                   ResumeCommandCanExecute));

        private bool ResumeCommandCanExecute()
        {
            return JobNotRunning
                   && ScenarioNotRunning
                   && _processModule.ProcessProgramProcessor?.State == ProcessorState.Paused
                   && _processModule.ProgramExecutionState == ProgramExecutionState.Running;
        }

        private void ResumeCommandExecute()
        {
            _processModule.StartProcessExecution();
        }

        #endregion

        #region Stop Command

        private ICommand _stopCommand;

        public ICommand StopCommand
            => _stopCommand
               ?? (_stopCommand = new DelegateCommand(StopCommandExecute, StopCommandCanExecute));

        private bool StopCommandCanExecute()
        {
            return _processModule.ProcessProgram != null
                   && _processModule.ProgramExecutionState == ProgramExecutionState.Running
                   && !_processModule.StopRequested;
        }

        private void StopCommandExecute()
        {
            _processModule.StopProcessExecution();
        }

        #endregion

        #region Abort Command

        private DelegateCommand _abortCommand;

        public ICommand AbortCommand
            => _abortCommand
               ?? (_abortCommand = new DelegateCommand(
                   AbortCommandExecute,
                   AbortCommandCanExecute));

        private bool AbortCommandCanExecute()
        {
            return _processModule.ProcessProgram != null
                   && _processModule.ProgramExecutionState == ProgramExecutionState.Running;
        }

        private void AbortCommandExecute()
        {
            _processModule.AbortProcessExecution();
        }

        #endregion

        #region Loop Commands

        private DelegateCommand _configureExecutionCommand;

        public ICommand ConfigureExecutionCommand
            => _configureExecutionCommand
               ?? (_configureExecutionCommand = new DelegateCommand(ConfigureExecutionExecute));

        private void ConfigureExecutionExecute()
        {
            var content = new ExecutionConfigurationPopup
            {
                NumberOfExecution = RequiredExecutionNumber < 0
                    ? 1
                    : RequiredExecutionNumber,
                LoopModeEnabled = RequiredExecutionNumber == -1
            };
            var popup = new Popup(nameof(RecipePanelResources.RECIPE_EXECUTION_CONFIGURATION))
            {
                Content = content
            };
            popup.Commands.Add(
                new PopupCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_CANCEL),
                    new DelegateCommand(UpdateConfigureExecutionCommandCheckState)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(RecipePanelResources.RECIPE_LIBRARY_APPLY),
                    new DelegateCommand(
                        () =>
                        {
                            RequiredExecutionNumber = content.LoopModeEnabled
                                ? -1
                                : content.NumberOfExecution;
                            UpdateConfigureExecutionCommandCheckState();
                        },
                        () => content.LoopModeEnabled || content.NumberOfExecution >= 1)));

            Popups.Show(popup);
        }

        private void UpdateConfigureExecutionCommandCheckState()
        {
            LoopCommand.IsChecked = RequiredExecutionNumber == -1 || RequiredExecutionNumber > 1;
        }

        #endregion

        #region Pause Command

        private DelegateCommand _pauseScenarioCommand;

        public ICommand PauseScenarioCommand
            => _pauseScenarioCommand
               ?? (_pauseScenarioCommand = new DelegateCommand(
                   PauseScenarioCommandExecute,
                   PauseScenarioCommandCanExecute));

        private bool PauseScenarioCommandCanExecute()
        {
            return _processModule.ProcessProgram != null
                   && _processModule.ProcessProgramProcessor?.State == ProcessorState.Running
                   && _processModule.ProgramExecutionState == ProgramExecutionState.Running;
        }

        private void PauseScenarioCommandExecute()
        {
            _processModule.BreakProcessExecution("Pause");
        }

        #endregion

        #region Step Mode Command

        private ICommand _stepModeCommand;

        public ICommand StepModeCommand
            => _stepModeCommand
               ?? (_stepModeCommand = new DelegateCommand(
                   StepModeCommandExecute,
                   StepModeCommandCanExecute));

        private bool StepModeCommandCanExecute()
        {
            return RecipeExecutionViewModel != null;
        }

        private void StepModeCommandExecute()
        {
            IsStepModeActive = !IsStepModeActive;

            if (!RecipeExecutionViewModel.IsCurrentProgram
                || RecipeExecutionViewModel.ProcessModule.ProcessProgramProcessor?.State
                == ProcessorState.Complete)
            {
                return;
            }

            RecipeExecutionViewModel.ProcessModule.BreakpointProcess(
                IsStepModeActive);
            RecipeExecutionViewModel.BreakpointsChangedFlag =
                !RecipeExecutionViewModel.BreakpointsChangedFlag;
        }

        #endregion

        #endregion

        #region Handlers

        private void ProcessorStateChanged(object sender, ProcessorState processorState)
        {
            UpdateCommandsVisibility();

            RecipeExecutionViewModel?.UpdateExecutionState();

            if (processorState != ProcessorState.Complete)
            {
                return;
            }

            if ((RequiredExecutionNumber == -1 || RequiredExecutionNumber > CurrentExecutionNumber)
                && !_processModule.AbortRequested
                && !_processModule.StopRequested)
            {
                InternalStart();
            }
            else
            {
                RequiredExecutionNumber = 0;
                CurrentExecutionNumber = 0;
                UpdateConfigureExecutionCommandCheckState();
            }
        }

        private void ProgramExecutionStateChanged(
            object sender,
            ProgramExecutionState programExecutionState)
        {
            RecipeExecutionViewModel?.UpdateExecutionState();
        }

        private void OnRecipesChanged(object sender, RecipeEventArgs e)
        {
            var selectedId = SelectedRecipe?.Id;
            RefreshAll();
            if (selectedId != null)
            {
                SelectedRecipe =
                    Recipes.SourceView.SingleOrDefault(
                        component => component.Id.Equals(selectedId));
            }
        }

        private void CurrentInstructionStateChanged(
            object sender,
            InstructionStateChangeEventArgs e)
        {
            RecipeExecutionViewModel?.UpdateProgression();
        }

        private void CurrentProgramInstructionChanged(object sender, EventArgs e)
        {
            RecipeExecutionViewModel?.UpdateProgression();
        }

        #endregion

        private void UpdateCommandsVisibility()
        {
            StartPanelCommand.IsVisible =
                _processModule.ProgramExecutionState != ProgramExecutionState.Running;
            ResumePanelCommand.IsVisible =
                _processModule.ProgramExecutionState == ProgramExecutionState.Running
                && (_processModule.ProcessProgramProcessor?.State == ProcessorState.Pausing
                    || _processModule.ProcessProgramProcessor.State == ProcessorState.Paused);
            PausePanelCommand.IsVisible =
                _processModule.ProgramExecutionState == ProgramExecutionState.Running
                && _processModule.ProcessProgramProcessor?.State == ProcessorState.Running;
            StopPanelCommand.IsVisible =
                _processModule.ProgramExecutionState == ProgramExecutionState.Running
                && _processModule.ProcessProgramProcessor?.State != ProcessorState.Complete;
            AbortPanelCommand.IsVisible =
                _processModule.ProgramExecutionState == ProgramExecutionState.Running
                && _processModule.ProcessProgramProcessor?.State != ProcessorState.Complete;
        }

        protected override void OnSelectedRecipeChanged()
        {
            RecipeExecutionViewModel = SelectedRecipe == null
                ? null
                : new RecipeExecutionViewModel(SelectedRecipe, _processModule);
        }

        #region Overrides of BaseScenarioLibraryPanel

        public override void OnSetup()
        {
            base.OnSetup();

            UpdateCommandsVisibility();

            _processModule.ProcessorStateChanged += ProcessorStateChanged;
            _processModule.ProgramExecutionStateChanged += ProgramExecutionStateChanged;

            _processModule.ProgramCurrentInstructionStateChanged += CurrentInstructionStateChanged;
            _processModule.CurrentProgramInstructionChanged += CurrentProgramInstructionChanged;
            RecipeManager.OnRecipeCreated += OnRecipesChanged;
            RecipeManager.OnRecipeDeleted += OnRecipesChanged;
            RecipeManager.OnRecipeModified += OnRecipesChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _processModule.ProcessorStateChanged -= ProcessorStateChanged;
                _processModule.ProgramExecutionStateChanged -= ProgramExecutionStateChanged;

                _processModule.ProgramCurrentInstructionStateChanged -=
                    CurrentInstructionStateChanged;
                _processModule.CurrentProgramInstructionChanged -= CurrentProgramInstructionChanged;
                RecipeManager.OnRecipeCreated -= OnRecipesChanged;
                RecipeManager.OnRecipeDeleted -= OnRecipesChanged;
                RecipeManager.OnRecipeModified -= OnRecipesChanged;
            }

            base.Dispose(disposing);
        }

        public override void OnShow()
        {
            RefreshRecipesList();
            base.OnShow();
        }

        #endregion
    }
}
