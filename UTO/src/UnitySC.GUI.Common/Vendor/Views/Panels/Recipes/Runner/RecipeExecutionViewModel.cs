using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.ProcessingFramework;
using Agileo.ProcessingFramework.Instructions;
using Agileo.Recipes.Components;

using UnitySC.Equipment.Abstractions.Vendor.Devices.RecipeProcessModule;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface;
using UnitySC.GUI.Common.Vendor.ProcessExecution;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Runner;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Recipes.Runner
{
    public class RecipeExecutionViewModel : Notifier
    {
        #region Constructor

        public RecipeExecutionViewModel(ProcessRecipe component, RecipeProcessModule processModule)
        {
            Component = component;
            ProcessModule = processModule;
            if (processModule?.ProcessProgram != null
                && processModule.ProcessProgram.Name == component.Id)
            {
                _program = processModule.ProcessProgram;
            }
            else
            {
                _program = component.ToProgram(App.Instance.RecipeManager.GetRecipes());
            }

            var recipeStepInstructionDictionary = GetRecipeStepInstruction(_program.Instructions);

            if (recipeStepInstructionDictionary.ContainsKey(RecipeSteps.PreProcess))
            {
                FillPreProcessInstructions(recipeStepInstructionDictionary[RecipeSteps.PreProcess]);
            }

            if (recipeStepInstructionDictionary.ContainsKey(RecipeSteps.Process))
            {
                FillProcessInstructions(recipeStepInstructionDictionary[RecipeSteps.Process]);
            }

            if (recipeStepInstructionDictionary.ContainsKey(RecipeSteps.PostProcess))
            {
                FillPostProcessInstructions(recipeStepInstructionDictionary[RecipeSteps.PostProcess]);
            }

            foreach (var subRecipe in component.SubRecipes)
            {
                FillSubRecipe(subRecipe);
            }

            UpdateExecutionState();
            UpdateProgression();

            HasBreakpointFunc = (instruction, _) => HasBreakpoint(instruction);
            ErrorIsVisibleFunc = (isCurrentProgram, errorString)
                => isCurrentProgram && !string.IsNullOrWhiteSpace(errorString)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            _program.InstructionStateChanged += Program_InstructionStateChanged;

            InstructionToIndexFunc = instruction =>
            {
                var index = _preProcessInstructions.IndexOf(instruction);
                if (index > -1)
                {
                    return (index + 1).ToString();
                }

                index = _processInstructions.IndexOf(instruction);
                if (index > -1)
                {
                    return (index + 1).ToString();
                }

                index = _postProcessInstructions.IndexOf(instruction);
                return (index + 1).ToString();
            };
        }

        #endregion

        #region Properties

        public ProcessRecipe Component { get; }

        public RecipeProcessModule ProcessModule { get; }

        private readonly Program _program;

        public string ComponentVersion
            => string.IsNullOrWhiteSpace(Component?.Header?.VersionId)
                ? string.Empty
                : $"({Component.Header.VersionId})";

        public Func<Instruction, string> InstructionToIndexFunc { get; }

        public ObservableCollection<object> InstructionsView { get; } = new();

        private readonly List<Instruction> _preProcessInstructions = new();

        private readonly List<Instruction> _processInstructions = new();

        private readonly List<Instruction> _postProcessInstructions = new();

        private Instruction _selectedInstruction;

        public Instruction SelectedInstruction
        {
            get { return _selectedInstruction; }
            set { SetAndRaiseIfChanged(ref _selectedInstruction, value); }
        }

        private HmiRecipeExecutionState _executionState;

        public HmiRecipeExecutionState ExecutionState
        {
            get { return _executionState; }
            set { SetAndRaiseIfChanged(ref _executionState, value); }
        }

        private int _instructionCount;

        public int InstructionCount
        {
            get { return _instructionCount; }
            set { SetAndRaiseIfChanged(ref _instructionCount, value); }
        }

        private int _currentInstructionIndex;

        public int CurrentInstructionIndex
        {
            get { return _currentInstructionIndex; }
            set { SetAndRaiseIfChanged(ref _currentInstructionIndex, value); }
        }

        private int _percentProgression;

        public int PercentProgression
        {
            get { return _percentProgression; }
            set
            {
                SetAndRaiseIfChanged(ref _percentProgression, value);
                OnPropertyChanged(nameof(HumanizedPercentProgression));
            }
        }

        public string HumanizedPercentProgression => PercentProgression + "%";

        public string StepProgression => $"(Step {CurrentInstructionIndex}/{InstructionCount})";

        /// <summary>
        /// True if the <see cref="Agileo.ProcessingFramework.Program"/> of the <see cref="AppProgramProcessor"/> is the instance of this <see cref="ScenarioExecutionViewModel"/>, otherwise false.
        /// </summary>
        public bool IsCurrentProgram => ReferenceEquals(_program, ProcessModule.ProcessProgram);

        /// <summary>
        /// Accessor to allow viewing of the instructions to opt out if a breakpoint is associated with it.
        /// The second parameter is only used as an event to initiate the evaluation.
        /// </summary>
        public Func<Instruction, bool, bool> HasBreakpointFunc { get; }

        /// <summary>
        /// Accessor to allow the view to opt out if the error must be displayed.
        /// </summary>
        public Func<bool, string, Visibility> ErrorIsVisibleFunc { get; }

        private bool _breakpointsChangedFlag;

        /// <summary>
        /// This property allows the view to reevaluate breakpoints.
        /// </summary>
        public bool BreakpointsChangedFlag
        {
            get { return _breakpointsChangedFlag; }
            set { SetAndRaiseIfChanged(ref _breakpointsChangedFlag, value); }
        }

        private readonly StringBuilder _programErrors = new StringBuilder();
        public string ProgramErrors => _programErrors.ToString();

        #endregion

        #region Commands

        #region Skip Command

        private ICommand _skipInstructionCommand;

        public ICommand SkipInstructionCommand
            => _skipInstructionCommand ??= new DelegateCommand(
                SkipInstructionCommandExecute,
                SkipInstructionCommandCanExecute);

        private bool SkipInstructionCommandCanExecute()
        {
            return (ProcessModule.ProcessProgramProcessor?.State == ProcessorState.Running
                    && ProcessModule.ProgramExecutionState == ProgramExecutionState.Running
                    || ProcessModule.CurrentProgramInstruction?.InstructionState
                    == InstructionState.Failed)
                   && ProcessModule.CurrentProgramInstruction is ISkippableInstruction
                   && ProcessModule.ProcessProgramProcessor?.State != ProcessorState.Complete
                   && IsCurrentProgram;
        }

        private void SkipInstructionCommandExecute()
        {
            ProcessModule.SkipCurrentInstruction();
        }

        #endregion

        #region Jump Command

        private ICommand _jumpInstructionCommand;

        public ICommand JumpInstructionCommand
            => _jumpInstructionCommand ??= new DelegateCommand(
                JumpInstructionCommandExecute,
                JumpInstructionCommandCanExecute);

        private bool JumpInstructionCommandCanExecute()
        {
            return (ProcessModule.ProcessProgramProcessor?.State == ProcessorState.Paused
                    || ProcessModule.CurrentProgramInstruction?.InstructionState
                    == InstructionState.Failed)
                   && SelectedInstruction != null
                   && IsCurrentProgram
                   && ProcessModule.ProcessProgramProcessor?.State != ProcessorState.Complete;
        }

        private void JumpInstructionCommandExecute()
        {
            ProcessModule.JumpToInstruction(SelectedInstruction);
        }

        #endregion

        #region Replay Command

        private ICommand _replayInstructionCommand;

        public ICommand ReplayInstructionCommand
            => _replayInstructionCommand ??= new DelegateCommand(
                ReplayInstructionCommandExecute,
                ReplayInstructionCommandCanExecute);

        private bool ReplayInstructionCommandCanExecute()
        {
            return ProcessModule.ProcessProgramProcessor?.State == ProcessorState.Paused
                   || ProcessModule.CurrentProgramInstruction?.InstructionState
                   == InstructionState.Failed
                   && IsCurrentProgram
                   && ProcessModule?.ProcessProgramProcessor?.State != ProcessorState.Complete;
        }

        private void ReplayInstructionCommandExecute()
        {
            ProcessModule.ReplayCurrentInstruction();
        }

        #endregion

        #endregion

        #region Private Methods

        private void FillPreProcessInstructions(List<Instruction> instructions)
        {
            InstructionsView.Add(
                new RecipeGroupHeader(
                    nameof(RecipePanelResources.RECIPE_LIBRARY_STEP_PRE_PROCESS)));
            foreach (var instructionFound in instructions.Select(instruction => _program.Instructions.Find(
                         i => i.Details.Equals(instruction.Details))))
            {
                InstructionsView.Add(instructionFound);
                _preProcessInstructions.Add(instructionFound);
            }
        }

        private void FillProcessInstructions(List<Instruction> instructions)
        {
            InstructionsView.Add(
                new RecipeGroupHeader(nameof(RecipePanelResources.RECIPE_LIBRARY_STEP_PROCESS)));
            foreach (var instructionFound in instructions.Select(instruction => _program.Instructions.Find(
                         i => i.Details.Equals(instruction.Details))))
            {
                InstructionsView.Add(instructionFound);
                _processInstructions.Add(instructionFound);
            }
        }

        private void FillPostProcessInstructions(List<Instruction> instructions)
        {
            InstructionsView.Add(
                new RecipeGroupHeader(
                    nameof(RecipePanelResources.RECIPE_LIBRARY_STEP_POST_PROCESS)));
            foreach (var instructionFound in instructions.Select(instruction => _program.Instructions.Find(
                         i => i.Details.Equals(instruction.Details))))
            {
                InstructionsView.Add(instructionFound);
                _postProcessInstructions.Add(instructionFound);
            }
        }

        private void FillSubRecipe(RecipeReference subRecipe)
        {
            var foundRecipe =
                App.Instance.RecipeManager.GetRecipes().FirstOrDefault(
                    r => r.Id.Equals(subRecipe.RecipeId));

            if (foundRecipe != null)
            {
                InstructionsView.Add(
                    new RecipeGroupHeader(
                        new LocalizableText(
                            nameof(RecipePanelResources.RECIPE_LIBRARY_SUB_RECIPE),
                            foundRecipe.Id)));

                var recipeStepInstructionDictionary = GetRecipeStepInstruction(foundRecipe.ToProgram(App.Instance.RecipeManager.GetRecipes()).Instructions);

                if (recipeStepInstructionDictionary.ContainsKey(RecipeSteps.PreProcess))
                {
                    FillPreProcessInstructions(recipeStepInstructionDictionary[RecipeSteps.PreProcess]);
                }

                if (recipeStepInstructionDictionary.ContainsKey(RecipeSteps.Process))
                {
                    FillProcessInstructions(recipeStepInstructionDictionary[RecipeSteps.Process]);
                }

                if (recipeStepInstructionDictionary.ContainsKey(RecipeSteps.PostProcess))
                {
                    FillPostProcessInstructions(recipeStepInstructionDictionary[RecipeSteps.PostProcess]);
                }
            }
        }

        private bool HasBreakpoint(Instruction instruction)
        {
            return ProcessModule.ProcessProgramProcessor != null
                   && ProcessModule.ProcessProgramProcessor.Breakpoints.Any(
                       breakpoint => ReferenceEquals(instruction, breakpoint.Instruction));
        }

        
        private static Dictionary<RecipeSteps, List<Instruction>> GetRecipeStepInstruction(List<Instruction> instructions)
        {
            var preProcessIndex = instructions.FindIndex(
                x => x is StepCrossingInstruction<RecipeSteps> {RecipeStep: RecipeSteps.PreProcess});

            var processIndex = instructions.FindIndex(
                x => x is StepCrossingInstruction<RecipeSteps> {RecipeStep: RecipeSteps.Process});

            var postProcessIndex = instructions.FindIndex(
                x => x is StepCrossingInstruction<RecipeSteps> {RecipeStep: RecipeSteps.PostProcess});

            var recipeStepInstructionDictionary = new Dictionary<RecipeSteps, List<Instruction>>();

            if (preProcessIndex != -1)
            {
                var nextStepIndex = postProcessIndex == -1
                    ? instructions.Count - preProcessIndex
                    : postProcessIndex - preProcessIndex;

                var rangeCount = (processIndex == -1
                    ? nextStepIndex
                    : processIndex - preProcessIndex) - 1;

                var preProcessInstructions = instructions.GetRange(
                    preProcessIndex + 1,
                    rangeCount);

                recipeStepInstructionDictionary.Add(RecipeSteps.PreProcess, preProcessInstructions);
            }

            if (processIndex != -1)
            {
                var rangeCount = (postProcessIndex == -1
                                     ? instructions.Count - processIndex
                                     : postProcessIndex - processIndex)
                                 - 1;

                var processInstructions = instructions.GetRange(processIndex+1, rangeCount);
                recipeStepInstructionDictionary.Add(RecipeSteps.Process, processInstructions);
            }

            if (postProcessIndex != -1)
            {
                var rangeCount = (instructions.Count - 1) - postProcessIndex;

                var postProcessInstructions = instructions.GetRange(postProcessIndex + 1, rangeCount);
                recipeStepInstructionDictionary.Add(RecipeSteps.PostProcess, postProcessInstructions);
            }

            return recipeStepInstructionDictionary;
        }


        #endregion

        #region Public Methods

        public void UpdateProgression()
        {
            InstructionCount = _program.Instructions.Count;

            var targetedInstruction = ProcessModule?.ExecutingInstruction ?? ProcessModule?.CurrentProgramInstruction;

            if (ProcessModule!=null && ReferenceEquals(_program, ProcessModule.ProcessProgram) && targetedInstruction != null)
            {
                CurrentInstructionIndex = _program.Instructions.FindIndex(instruction => ReferenceEquals(targetedInstruction, instruction)) + 1;

                var percentProgressionIndex = CurrentInstructionIndex;

                // Decrements the progress index if the current instruction is not executed
                var currentInstructionState = targetedInstruction.InstructionState;
                if (currentInstructionState != InstructionState.Executed && currentInstructionState != InstructionState.Failed)
                {
                    percentProgressionIndex--;
                }

                PercentProgression = (int)(100 * (percentProgressionIndex / (double)InstructionCount));
            }
            else
            {
                CurrentInstructionIndex = 0;
                PercentProgression = 0;
            }

            OnPropertyChanged(nameof(StepProgression));
        }

        public void UpdateExecutionState()
        {
            if (ProcessModule!=null && ReferenceEquals(_program, ProcessModule.ProcessProgram))
            {
                switch (ProcessModule.ProgramExecutionState)
                {
                    case ProgramExecutionState.NotStarted:
                        ExecutionState = HmiRecipeExecutionState.NotStarted;
                        break;
                    case ProgramExecutionState.Running:
                        {
                            switch (ProcessModule.ProcessProgramProcessor.State)
                            {
                                case ProcessorState.Paused:
                                    ExecutionState =
                                        ProcessModule.CurrentProgramInstruction?.InstructionState == InstructionState.Failed
                                            ? HmiRecipeExecutionState.PausedWithError
                                            : HmiRecipeExecutionState.Paused;
                                    break;
                                case ProcessorState.Running:
                                case ProcessorState.Complete:
                                case ProcessorState.Pausing:
                                    ExecutionState = HmiRecipeExecutionState.Running;
                                    break;
                                default:
                                    throw new InvalidEnumArgumentException(nameof(ProcessModule.ProcessProgramProcessor.State),
                                        (int)ProcessModule.ProcessProgramProcessor.State,
                                        typeof(ProcessorState));
                            }
                        }
                        break;
                    case ProgramExecutionState.Finished:
                        ExecutionState =
                            ProcessModule.ProcessProgram.Instructions.Any(
                                inst => inst.InstructionState == InstructionState.Failed)
                                ? HmiRecipeExecutionState.FinishedWithError
                                : HmiRecipeExecutionState.Finished;
                        break;
                    case ProgramExecutionState.Aborted:
                        ExecutionState = HmiRecipeExecutionState.Aborted;
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(ProcessModule.ProgramExecutionState),
                            (int)ProcessModule.ProgramExecutionState,
                            typeof(ProgramExecutionState));
                }
            }
            else
            {
                ExecutionState = HmiRecipeExecutionState.NotStarted;
            }
        }

        #endregion

        #region Event Handlers

        private void Program_InstructionStateChanged(object sender, InstructionStateChangeEventArgs e)
        {
            _programErrors.Clear();
            if (e.InstructionStateChange.State == InstructionState.Failed
                && e.InstructionStateChange.Exception != null
                && !string.IsNullOrEmpty(e.InstructionStateChange.Exception.Message))
            {
                var errors = e.InstructionStateChange.Exception.Message.Split(
                    new[] {"., "},
                    StringSplitOptions.RemoveEmptyEntries);
                _programErrors.Append('[').AppendFormat("{0:hh:mm:ss}", e.InstructionStateChange.Timestamp).Append(']');
                for (var index = 0; index < errors.Length - 1; index++)
                {
                    _programErrors.AppendLine().Append(" • ").Append(errors[index]).Append('.');
                }
                _programErrors.AppendLine().Append(" • ").Append(errors[errors.Length - 1]);
            }

            OnPropertyChanged(nameof(ProgramErrors));
        }

        #endregion
    }
}
