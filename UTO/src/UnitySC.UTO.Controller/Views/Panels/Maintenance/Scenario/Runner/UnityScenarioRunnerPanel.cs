using System;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Logging;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.ProcessingFramework;
using Agileo.Recipes.Management;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.ProcessExecution;
using UnitySC.GUI.Common.Vendor.Scenarios;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Runner;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Runner.ExecutionConfiguration;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions;

namespace UnitySC.UTO.Controller.Views.Panels.Maintenance.Scenario.Runner
{
    public class UnityScenarioRunnerPanel : BaseScenarioLibraryPanel
    {
        #region Fields

        private readonly ILogger _logger;
        private const string ScenarioRunnerTracerName = "Scenario Runner";

        private AppProgramProcessor AppProgramProcessor => GUI.Common.App.Instance.SequenceProgramProcessor;

        #endregion

        #region Constructors

        static UnityScenarioRunnerPanel()
        {
            DataTemplateGenerator.Create(typeof(UnityScenarioRunnerPanel), typeof(UnityScenarioRunnerPanelView));
        }

        public UnityScenarioRunnerPanel()
            : this(nameof(L10N.BP_SCENARIO_SEQUENCER), new ScenarioManager(), PathIcon.PlayDocument)
        {
        }

        public UnityScenarioRunnerPanel(string relativeId, ScenarioManager scenarioManager, IIcon icon = null)
            : base(relativeId, scenarioManager, icon)
        {
            _logger = GUI.Common.App.Instance.GetLogger(ScenarioRunnerTracerName);

            StartPanelCommand = new BusinessPanelCommand(
                nameof(ScenarioResources.SCENARIO_START),
                StartCommand,
                PathIcon.Play);
            Commands.Add(StartPanelCommand);

            ResumePanelCommand = new BusinessPanelCommand(
                nameof(ScenarioResources.SCENARIO_RESUME),
                ResumeCommand,
                PathIcon.Resume);
            Commands.Add(ResumePanelCommand);

            PausePanelCommand = new BusinessPanelCommand(
                nameof(ScenarioResources.SCENARIO_PAUSE),
                PauseScenarioCommand,
                PathIcon.Pause);
            Commands.Add(PausePanelCommand);

            StopPanelCommand = new BusinessPanelCommand(
                nameof(ScenarioResources.SCENARIO_STOP),
                StopCommand,
                PathIcon.Stop);
            Commands.Add(StopPanelCommand);

            AbortPanelCommand = new BusinessPanelCommand(
                nameof(ScenarioResources.SCENARIO_ABORT),
                AbortCommand,
                PathIcon.Private);
            Commands.Add(AbortPanelCommand);

            LoopCommand = new BusinessPanelCheckToggleCommand(
                nameof(ScenarioResources.SCENARIO_EXECUTION_CONFIGURATION),
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

        public BusinessPanelCheckToggleCommand LoopCommand { get; }

        private ScenarioExecutionViewModel _scenarioExecutionViewModel;

        public ScenarioExecutionViewModel ScenarioExecutionViewModel
        {
            get => _scenarioExecutionViewModel;
            set => SetAndRaiseIfChanged(ref _scenarioExecutionViewModel, value);
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

        public bool IsMaintenanceMode
            => App.ControllerInstance.ControllerEquipmentManager.Controller.State
               == OperatingModes.Maintenance;

        #endregion

        #region Commands

        #region Start Command

        private DelegateCommand _runScenarioCommand;

        public ICommand StartCommand
            => _runScenarioCommand ??= new DelegateCommand(StartExecute, StartCanExecute);

        private bool StartCanExecute()
        {
            if (ScenarioExecutionViewModel == null) return false;
            return (AppProgramProcessor.State == ProcessorState.Complete || AppProgramProcessor.CurrentProgram == null) && IsMaintenanceMode;
        }

        private void StartExecute()
        {
            DetailsIsExpanded = true;
            CurrentExecutionNumber = 0;
            InternalStart();
        }

        private void InternalStart()
        {
            if (ScenarioExecutionViewModel == null) return;
            try
            {
                var component = ScenarioExecutionViewModel.Component;
                AppProgramProcessor.LoadProgram(component.ToProgram(ScenarioManager.GetRecipes()));

                // [TLa] The program is not persistent so it is necessary to create a new viewer in case the program needs to be restarted.
                ScenarioExecutionViewModel = new ScenarioExecutionViewModel(component, AppProgramProcessor);

                ++CurrentExecutionNumber;

                AppProgramProcessor.RunProgram();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        #endregion

        #region Resume Command

        private ICommand _resumeCommand;

        public ICommand ResumeCommand
            => _resumeCommand ??= new DelegateCommand(
                ResumeCommandExecute,
                ResumeCommandCanExecute);

        private bool ResumeCommandCanExecute()
            => AppProgramProcessor.State == ProcessorState.Paused
               && AppProgramProcessor.ProgramExecutionState == ProgramExecutionState.Running
               && IsMaintenanceMode;

        private void ResumeCommandExecute() => AppProgramProcessor.RunProgram();

        #endregion

        #region Stop Command

        private ICommand _stopCommand;

        public ICommand StopCommand
            => _stopCommand ??= new DelegateCommand(StopCommandExecute, StopCommandCanExecute);

        private bool StopCommandCanExecute() =>
            AppProgramProcessor.CurrentProgram != null &&
            AppProgramProcessor.ProgramExecutionState == ProgramExecutionState.Running &&
            !AppProgramProcessor.StopRequested && IsMaintenanceMode;

        private void StopCommandExecute() => AppProgramProcessor.StopProgramExecution();

        #endregion

        #region Abort Command

        private DelegateCommand _abortCommand;

        public ICommand AbortCommand
            => _abortCommand ??= new DelegateCommand(
                AbortCommandExecute,
                AbortCommandCanExecute);

        private bool AbortCommandCanExecute()
            => AppProgramProcessor.CurrentProgram != null
               && AppProgramProcessor.ProgramExecutionState == ProgramExecutionState.Running
               && IsMaintenanceMode;

        private void AbortCommandExecute() => AppProgramProcessor.AbortProgramExecution();

        #endregion

        #region Loop Commands

        private DelegateCommand _configureExecutionCommand;

        public ICommand ConfigureExecutionCommand
            => _configureExecutionCommand ??= new DelegateCommand(ConfigureExecutionExecute, ConfigureExecutionCanExecute);

        private void ConfigureExecutionExecute()
        {
            var content = new ExecutionConfigurationPopup
            {
                NumberOfExecution = RequiredExecutionNumber < 0 ? 1 : RequiredExecutionNumber,
                LoopModeEnabled = RequiredExecutionNumber == -1
            };
            var popup = new Popup(nameof(ScenarioResources.SCENARIO_EXECUTION_CONFIGURATION))
            {
                Content = content
            };
            popup.Commands.Add(
                new PopupCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_CANCEL),
                    new DelegateCommand(UpdateConfigureExecutionCommandCheckState)));
            popup.Commands.Add(new PopupCommand(nameof(ScenarioResources.SCENARIO_APPLY), new DelegateCommand(
                () =>
                {
                    RequiredExecutionNumber = content.LoopModeEnabled ? -1 : content.NumberOfExecution;
                    UpdateConfigureExecutionCommandCheckState();
                },
                () => content.LoopModeEnabled || content.NumberOfExecution >= 1)));

            Popups.Show(popup);
        }

        private bool ConfigureExecutionCanExecute()
        {
            return IsMaintenanceMode;
        }

        private void UpdateConfigureExecutionCommandCheckState()
        {
            LoopCommand.IsChecked = RequiredExecutionNumber == -1 || RequiredExecutionNumber > 1;
        }

        #endregion

        #region Pause Command

        private DelegateCommand _pauseScenarioCommand;

        public ICommand PauseScenarioCommand
            => _pauseScenarioCommand ??= new DelegateCommand(
                PauseScenarioCommandExecute,
                PauseScenarioCommandCanExecute);

        private bool PauseScenarioCommandCanExecute()
        {
            return AppProgramProcessor.CurrentProgram != null
                   && AppProgramProcessor.State == ProcessorState.Running
                   && AppProgramProcessor.ProgramExecutionState == ProgramExecutionState.Running
                   && IsMaintenanceMode;
        }

        private void PauseScenarioCommandExecute()
        {
            AppProgramProcessor.PauseProgramExecution();
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnSetup()
        {
            base.OnSetup();

            UpdateCommandsVisibility();

            AppProgramProcessor.Setup(this);

            AppProgramProcessor.ProcessorStateChanged += ProcessorStateChanged;
            AppProgramProcessor.ProgramExecutionStateChanged += ProgramExecutionStateChanged;

            AppProgramProcessor.ProgramCurrentInstructionStateChanged += CurrentInstructionStateChanged;
            AppProgramProcessor.CurrentProgramInstructionChanged += CurrentProgramInstructionChanged;
            ScenarioManager.OnRecipeCreated += OnRecipesChanged;
            ScenarioManager.OnRecipeDeleted += OnRecipesChanged;
            ScenarioManager.OnRecipeModified += OnRecipesChanged;

            App.ControllerInstance.ControllerEquipmentManager.Controller.StatusValueChanged +=
                Controller_StatusValueChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.ControllerInstance.ControllerEquipmentManager.Controller.StatusValueChanged -=
                    Controller_StatusValueChanged;

                AppProgramProcessor.ProcessorStateChanged -= ProcessorStateChanged;
                AppProgramProcessor.ProgramExecutionStateChanged -= ProgramExecutionStateChanged;

                AppProgramProcessor.ProgramCurrentInstructionStateChanged -= CurrentInstructionStateChanged;
                AppProgramProcessor.CurrentProgramInstructionChanged -= CurrentProgramInstructionChanged;
                ScenarioManager.OnRecipeCreated -= OnRecipesChanged;
                ScenarioManager.OnRecipeDeleted -= OnRecipesChanged;
                ScenarioManager.OnRecipeModified -= OnRecipesChanged;
            }

            base.Dispose(disposing);
        }

        public override void OnShow()
        {
            RefreshScenariosList();
            base.OnShow();
        }

        #endregion

        #region Event Handlers

        private void ProcessorStateChanged(object sender, ProcessorState processorState)
        {
            UpdateCommandsVisibility();

            ScenarioExecutionViewModel?.UpdateExecutionState();

            if (processorState != ProcessorState.Complete)
            {
                return;
            }

            if ((RequiredExecutionNumber == -1 || RequiredExecutionNumber > CurrentExecutionNumber)
                && !AppProgramProcessor.AbortRequested
                && !AppProgramProcessor.StopRequested)
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

        private void ProgramExecutionStateChanged(object sender, ProgramExecutionState programExecutionState)
        {
            ScenarioExecutionViewModel?.UpdateExecutionState();
        }

        private void OnRecipesChanged(object sender, RecipeEventArgs e)
        {
            var selectedId = SelectedScenario?.Id;
            RefreshAll();
            if (selectedId != null)
            {
                SelectedScenario =
                    DataTableSource.SourceView.SingleOrDefault(
                        component => component.Id.Equals(selectedId));
            }
        }

        private void CurrentInstructionStateChanged(object sender, InstructionStateChangeEventArgs e)
        {
            ScenarioExecutionViewModel?.UpdateProgression();
        }

        private void CurrentProgramInstructionChanged(object sender, EventArgs e)
        {
            ScenarioExecutionViewModel?.UpdateProgression();
        }

        private void Controller_StatusValueChanged(object sender, Agileo.EquipmentModeling.StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(Equipment.Abstractions.Devices.Controller.Controller.State))
            {
                OnPropertyChanged(nameof(IsMaintenanceMode));
            }
        }

        #endregion

        #region Private Methods

        private void UpdateCommandsVisibility()
        {
            StartPanelCommand.IsVisible = AppProgramProcessor.ProgramExecutionState
                                          != ProgramExecutionState.Running;
            ResumePanelCommand.IsVisible =
                AppProgramProcessor.ProgramExecutionState == ProgramExecutionState.Running
                && (AppProgramProcessor.State == ProcessorState.Pausing
                    || AppProgramProcessor.State == ProcessorState.Paused);
            PausePanelCommand.IsVisible =
                AppProgramProcessor.ProgramExecutionState == ProgramExecutionState.Running
                && AppProgramProcessor.State == ProcessorState.Running;
            StopPanelCommand.IsVisible =
                AppProgramProcessor.ProgramExecutionState == ProgramExecutionState.Running
                && AppProgramProcessor.State != ProcessorState.Complete;
            AbortPanelCommand.IsVisible =
                AppProgramProcessor.ProgramExecutionState == ProgramExecutionState.Running
                && AppProgramProcessor.State != ProcessorState.Complete;
        }

        protected override void OnSelectedScenarioChanged()
        {
            ScenarioExecutionViewModel = SelectedScenario == null
                ? null
                : new ScenarioExecutionViewModel(SelectedScenario, AppProgramProcessor);
        }

        #endregion
    }
}
