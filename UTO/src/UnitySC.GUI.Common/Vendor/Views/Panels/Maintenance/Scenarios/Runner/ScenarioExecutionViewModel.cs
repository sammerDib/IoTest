using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.ProcessingFramework;
using Agileo.ProcessingFramework.Instructions;

using UnitySC.GUI.Common.Vendor.ProcessExecution;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Runner
{
    public class ScenarioExecutionViewModel : Notifier
    {
        public SequenceRecipe Component { get; }

        public AppProgramProcessor ProgramProcessor { get; }

        public Program Program { get; }

        public ScenarioExecutionViewModel(SequenceRecipe component, AppProgramProcessor programProcessor)
        {
            Component = component;
            ProgramProcessor = programProcessor;
            if (programProcessor.CurrentProgram != null && programProcessor.CurrentProgram.Name == component.Id)
            {
                Program = programProcessor.CurrentProgram;
            }
            else
            {
                Program = component.ToProgram(App.Instance.ScenarioManager.GetRecipes());
            }
            Instructions = Program.Instructions;
            UpdateExecutionState();
            UpdateProgression();

            HasBreakpointFunc = (instruction, b) => HasBreakpoint(instruction);
            ErrorIsVisibleFunc = (isCurrentProgram, errorString) => isCurrentProgram && !string.IsNullOrWhiteSpace(errorString) ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Properties

        public string ComponentVersion => string.IsNullOrWhiteSpace(Component?.Header?.VersionId) ? string.Empty : $"({Component.Header.VersionId})";

        public List<Instruction> Instructions { get; }

        private Instruction _selectedInstruction;

        public Instruction SelectedInstruction
        {
            get { return _selectedInstruction; }
            set { SetAndRaiseIfChanged(ref _selectedInstruction, value); }
        }

        private HmiScenarioExecutionState _executionState;

        public HmiScenarioExecutionState ExecutionState
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
        public bool IsCurrentProgram => ReferenceEquals(Program, ProgramProcessor.CurrentProgram);

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

        #endregion

        #region Commands
        
        private ICommand _skipInstructionCommand;

        public ICommand SkipInstructionCommand => _skipInstructionCommand ?? (_skipInstructionCommand = new DelegateCommand(SkipInstructionCommandExecute, SkipInstructionCommandCanExecute));

        private bool SkipInstructionCommandCanExecute()
        {
            if (!IsCurrentProgram) return false;
            return ProgramProcessor.State == ProcessorState.Running
                   && ProgramProcessor.ProgramExecutionState == ProgramExecutionState.Running
                   && ProgramProcessor.ExecutingInstruction is ISkippableInstruction;
        }

        private void SkipInstructionCommandExecute()
        {
            ProgramProcessor.SkipCurrentInstruction();
        }

        private ICommand _jumpInstructionCommand;

        public ICommand JumpInstructionCommand => _jumpInstructionCommand ?? (_jumpInstructionCommand = new DelegateCommand(JumpInstructionCommandExecute, JumpInstructionCommandCanExecute));

        private bool JumpInstructionCommandCanExecute()
        {
            if (!IsCurrentProgram) return false;
            if (ProgramProcessor.State != ProcessorState.Paused) return false;
            return SelectedInstruction != null;
        }

        private void JumpInstructionCommandExecute()
        {
            ProgramProcessor.JumpToInstruction(SelectedInstruction);
        }

        private ICommand _replayInstructionCommand;

        public ICommand ReplayInstructionCommand => _replayInstructionCommand ?? (_replayInstructionCommand = new DelegateCommand(ReplayInstructionCommandExecute, ReplayInstructionCommandCanExecute));

        private bool ReplayInstructionCommandCanExecute()
        {
            if (!IsCurrentProgram) return false;
            if (ProgramProcessor.State != ProcessorState.Paused) return false;
            return true;
        }

        private void ReplayInstructionCommandExecute()
        {
            ProgramProcessor.ReplayCurrentInstruction();
        }

        private ICommand _breakPointCommand;

        public ICommand BreakPointCommand => _breakPointCommand ?? (_breakPointCommand = new DelegateCommand(BreakPointCommandExecute, BreakPointCommandCanExecute));

        private bool BreakPointCommandCanExecute()
        {
            if (!IsCurrentProgram) return false;
            if (ProgramProcessor.State == ProcessorState.Complete) return false;
            if (SelectedInstruction == null) return false;
            return !HasBreakpoint(SelectedInstruction);
        }

        private void BreakPointCommandExecute()
        {
            if (HasBreakpoint(SelectedInstruction))
            {
                ProgramProcessor.RemoveBreakPoint(SelectedInstruction);
            }
            else
            {
                ProgramProcessor.AddBreakPoint(SelectedInstruction);
            }

            BreakpointsChangedFlag = !BreakpointsChangedFlag;
        }

        private bool HasBreakpoint(Instruction instruction)
        {
            return ProgramProcessor.Breakpoints.Any(breakpoint => ReferenceEquals(instruction, breakpoint.Instruction));
        }

        #endregion

        #region Public Methods

        public void UpdateProgression()
        {
            InstructionCount = Program.Instructions.Count;

            var targetedInstruction = ProgramProcessor.ExecutingInstruction ?? ProgramProcessor.CurrentInstruction;

            if (ReferenceEquals(Program, ProgramProcessor.CurrentProgram) && targetedInstruction != null)
            {
                CurrentInstructionIndex = Program.Instructions.FindIndex(instruction => ReferenceEquals(targetedInstruction, instruction)) + 1;

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
            if (ReferenceEquals(Program, ProgramProcessor.CurrentProgram))
            {
                switch (ProgramProcessor.ProgramExecutionState)
                {
                    case ProgramExecutionState.NotStarted:
                        ExecutionState = HmiScenarioExecutionState.NotStarted;
                        break;
                    case ProgramExecutionState.Running:
                        {
                            switch (ProgramProcessor.State)
                            {
                                case ProcessorState.Paused:
                                    ExecutionState =
                                        ProgramProcessor.CurrentInstruction?.InstructionState == InstructionState.Failed
                                            ? HmiScenarioExecutionState.PausedWithError
                                            : HmiScenarioExecutionState.Paused;
                                    break;
                                case ProcessorState.Running:
                                case ProcessorState.Complete:
                                case ProcessorState.Pausing:
                                    ExecutionState = HmiScenarioExecutionState.Running;
                                    break;
                                default:
                                    throw new InvalidEnumArgumentException(
                                        nameof(ProgramProcessor.State),
                                        (int)ProgramProcessor.State,
                                        typeof(ProcessorState));
                            }
                        }
                        break;
                    case ProgramExecutionState.Finished:
                        ExecutionState =
                            ProgramProcessor.CurrentProgram.Instructions.Any(
                                inst => inst.InstructionState == InstructionState.Failed)
                                ? HmiScenarioExecutionState.FinishedWithError
                                : HmiScenarioExecutionState.Finished;
                        break;
                    case ProgramExecutionState.Aborted:
                        ExecutionState = HmiScenarioExecutionState.Aborted;
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(ProgramProcessor.ProgramExecutionState),
                            (int)ProgramProcessor.ProgramExecutionState,
                            typeof(ProgramExecutionState));
                }
            }
            else
            {
                ExecutionState = HmiScenarioExecutionState.NotStarted;
            }
        }

        #endregion
    }
}
