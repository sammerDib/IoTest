using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.ProcessingFramework;
using Agileo.ProcessingFramework.Instructions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface;

using Logger = Agileo.ProcessingFramework.Logging.Logger;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.RecipeProcessModule
{
    public partial class RecipeProcessModule
    {
        #region Abstract Methods

        protected abstract void SetProcessModuleInstruction(Instruction instruction);

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (ProcessProgramProcessor != null)
            {
                if (ProcessProgramProcessor.CurrentInstruction is IDisposable)
                {
                    ((IDisposable)ProcessProgramProcessor?.CurrentInstruction)?.Dispose();
                }

                if (ProcessProgramProcessor is { State: ProcessorState.Running })
                {
                    ProcessProgramProcessor.Abort();
                }

                if (ProcessProgramProcessor != null)
                {
                    ProcessProgramProcessor.PropertyChanged -=
                        ProcessProgramProcessor_PropertyChanged;
                    ProcessProgramProcessor.Dispose();
                }
            }

            ProcessProgram?.Dispose();
            RecipeElapsedTimeInfo?.Dispose();
        }

        #endregion

        #region Fields

        private readonly ProcessModuleExecutionFactory _executionContextFactory = new();
        private RecipeSteps _currentRecipeStep;
        private ILogger _userLogger;

        #endregion

        #region Events

        public event EventHandler ProgramLoaded;
        public event EventHandler CurrentProgramInstructionChanged;

        public event EventHandler<InstructionStateChangeEventArgs>
            ProgramCurrentInstructionStateChanged;

        public event EventHandler<ProgramExecutionState> ProgramExecutionStateChanged;
        public event EventHandler<ProcessorState> ProcessorStateChanged;
        public event EventHandler<RecipeProgressionInfoEventArgs> RecipeProgressionChanged;

        #endregion

        #region Properties

        public ProgramProcessor ProcessProgramProcessor { get; private set; }

        public Instruction CurrentProgramInstruction => ProcessProgramProcessor?.CurrentInstruction;
        public bool? IsInError => ProcessProgramProcessor?.IsInError;

        public Instruction ExecutingInstruction
            => ProcessProgram?.Instructions.SingleOrDefault(
                instruction => instruction.InstructionState == InstructionState.Executing);

        private Program _processProgram;

        public Program ProcessProgram
        {
            get => _processProgram;
            private set
            {
                if (_processProgram == value)
                {
                    return;
                }

                if (_processProgram != null)
                {
                    _processProgram.InstructionStateChanged -=
                        CurrentProgram_InstructionStateChanged;
                }

                _processProgram = value;
                if (_processProgram != null)
                {
                    _processProgram.InstructionStateChanged +=
                        CurrentProgram_InstructionStateChanged;
                }

                ProgramLoaded?.Invoke(this, null);
                OnPropertyChanged(nameof(ProcessProgram));
            }
        }

        public ProcessModuleExecutionContext ProcessModuleExecutionContext { get; private set; }

        public bool StopRequested { get; private set; }

        public bool AbortRequested { get; private set; }

        public Dictionary<RecipeSteps, RecipeProgressionInfo> RecipeProgressionInfos { get; } =
            new();

        public RecipeElapsedTimeInfo RecipeElapsedTimeInfo { get; private set; }

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    _userLogger = Agileo.Common.Logging.Logger.GetLogger("User");
                    IsProgramLoaded = false;
                    break;
                case SetupPhase.SettingUp:
                    RecipeElapsedTimeInfo = new RecipeElapsedTimeInfo();
                    ClearJobInformation();
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region Event Handlers

        private void ProcessProgramProcessor_PropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ProcessProgramProcessor.CurrentInstruction):
                    if (ProcessProgramProcessor.CurrentInstruction is
                        StepCrossingInstruction<RecipeSteps> stepCrossingInstruction)
                    {
                        HandleCurrentStepChanged(stepCrossingInstruction);
                        return;
                    }

                    if (ProcessProgramProcessor.CurrentInstruction != null)
                    {
                        HandleCurrentInstructionChanged();
                    }

                    OnRecipeProgressionChanged();
                    CurrentProgramInstructionChanged?.Invoke(sender, e);
                    break;
                case nameof(ProcessProgramProcessor.State):
                    switch (ProcessProgramProcessor.State)
                    {
                        case ProcessorState.Complete:
                            UpdateProgramExecutionState(
                                AbortRequested
                                    ? ProgramExecutionState.Aborted
                                    : ProgramExecutionState.Finished);
                            break;
                        case ProcessorState.Paused:
                            if (StopRequested)
                            {
                                ProcessProgramProcessor.Abort();
                            }

                            break;
                    }

                    UpdateProcessorState(ProcessProgramProcessor.State);
                    break;
            }
        }

        private void CurrentProgram_InstructionStateChanged(
            object sender,
            InstructionStateChangeEventArgs e)
        {
            if (e.InstructionStateChange.State == InstructionState.Failed
                && _currentRecipeStep != RecipeSteps.Undefined)
            {
                if (e.InstructionStateChange.Exception != null
                    && !string.IsNullOrEmpty(e.InstructionStateChange.Exception.Message)
                    && RecipeProgressionInfos[_currentRecipeStep] != null)
                {
                    RecipeProgressionInfos[_currentRecipeStep].ProgramError.Clear();
                    RecipeProgressionInfos[_currentRecipeStep]
                        .ProgramError.Append("Process Program message : ");
                    RecipeProgressionInfos[_currentRecipeStep]
                        .ProgramError.Append(e.InstructionStateChange.Exception.Message);
                    RecipeProgressionInfos[_currentRecipeStep]
                        .ProgramError.AppendLine(
                            $" - Time : {e.InstructionStateChange.Timestamp:hh:mm:ss}");
                    RecipeProgressionInfos[_currentRecipeStep]
                        .ProgramError.Append(
                            $"Instruction Name : {e.InstructionStateChange.Instruction.Name}");
                    RecipeProgressionInfos[_currentRecipeStep]
                        .ProgramError.Append(
                            $" - Instruction Details : {e.InstructionStateChange.Instruction.Details}");
                }
            }
            else if (_currentRecipeStep != RecipeSteps.Undefined)
            {
                RecipeProgressionInfos[_currentRecipeStep].ProgramError.Clear();
            }

            OnRecipeProgressionChanged();
            ProgramCurrentInstructionStateChanged?.Invoke(sender, e);
        }

        #endregion

        #region Public Methods

        public void AbortProcessExecution()
        {
            //special case for IInstructionAbortable
            AbortRequested = true;
            var abortable = CurrentProgramInstruction as IInstructionAbortable;
            abortable?.AbortInstruction();
            SpinWait.SpinUntil(() => ProcessProgramProcessor.State == ProcessorState.Paused, 5000);
            ProcessProgramProcessor?.Abort();
            UpdateProgramExecutionState(ProgramExecutionState.Aborted);
        }

        public void JumpToInstruction(Instruction instruction)
        {
            if (!(instruction is AwaitInstruction))
            {
                ProcessProgramProcessor?.Jump(instruction);
                var index = ProcessProgram.Instructions.IndexOf(instruction);
                for (var i = index; i < ProcessProgram.Instructions.Count - 1; i++)
                {
                    var instruct = ProcessProgram.Instructions[i];
                    ProcessProgram.SetInstructionState(instruct, InstructionState.Waiting);
                }
            }
        }

        public void ReplayCurrentInstruction()
        {
            ProcessProgramProcessor.Jump(ProcessProgramProcessor.CurrentInstruction);
            ProcessProgram.SetInstructionState(
                ProcessProgramProcessor.CurrentInstruction,
                InstructionState.Waiting);
            ProcessProgramProcessor.Run();
            UpdateProgramExecutionState(ProgramExecutionState.Running);
        }

        public void BreakProcessExecution(string cause)
        {
            if (ProcessorState == ProcessorState.Pausing)
            {
                return;
            }

            ProcessProgramProcessor?.Break(cause);
            _userLogger.Info($"{Name}: Recipe {ProcessProgram.Name} is pausing.");
        }

        public void StopProcessExecution()
        {
            StopRequested = true;
            if (ProcessProgramProcessor.State == ProcessorState.Paused)
            {
                ProcessProgramProcessor.Abort();
            }
            else
            {
                ProcessProgramProcessor.Break("Stop");
            }
        }

        public void BreakpointProcess(bool isActive)
        {
            foreach (var breakpoint in ProcessProgramProcessor.Breakpoints)
            {
                breakpoint.IsActive = !breakpoint.IsActive;
            }
        }

        public void StartProcessExecution()
        {
            ClearJobInformation();
            var recipeStep = RecipeSteps.Undefined;
            foreach (var instruction in ProcessProgram.Instructions)
            {
                if (instruction is StepCrossingInstruction<RecipeSteps> stepCrossingInstruction)
                {
                    recipeStep = stepCrossingInstruction.RecipeStep;
                    if (!RecipeProgressionInfos.ContainsKey(recipeStep))
                    {
                        RecipeProgressionInfos.Add(recipeStep, new RecipeProgressionInfo());
                    }
                }
                else if (RecipeProgressionInfos.ContainsKey(recipeStep))
                {
                    if (RecipeProgressionInfos[recipeStep].Instructions == null)
                    {
                        RecipeProgressionInfos[recipeStep] = new RecipeProgressionInfo();
                    }

                    RecipeProgressionInfos[recipeStep].Instructions.Add(instruction);
                    RecipeProgressionInfos[recipeStep].IsStepExpected = true;
                    RecipeProgressionInfos[recipeStep].CurrentProgressionMaximum =
                        RecipeProgressionInfos[recipeStep].Instructions.Count;
                    RecipeProgressionInfos[recipeStep].CurrentProgressionValue = 0;
                    RecipeProgressionInfos[recipeStep].StepProgressionPercent = 0;
                }
            }

            OnRecipeProgressionChanged();
            ProcessProgramProcessor?.Run();
            UpdateProgramExecutionState(ProgramExecutionState.Running);
            _userLogger.Info($"{Name}: Recipe {ProcessProgram.Name} has been started.");
        }

        public void SetupProcessProgram(Program program)
        {
            CreateProcessProgramProcessor(program);
            var instructionsState =
                ProcessProgram.Instructions.Select(x => x.InstructionState).ToArray();
            ProcessProgramProcessor?.SetupProgram(
                ProcessProgram,
                instructionsState,
                ProcessProgramProcessor.ExecutionContext);
            ProcessModuleExecutionContext =
                (ProcessModuleExecutionContext)ProcessProgramProcessor?.ExecutionContext;
            IsProgramLoaded = true;
            ProgramLoaded?.Invoke(this, new EventArgs());
            _userLogger.Info($"{Name}: Recipe {program.Name} has been loaded.");
        }

        public void CleanProcessProgram()
        {
            DisposeProcessProgramProcessor();
            IsProgramLoaded = false;
            Logger.Debug("Clean Program done");
            _userLogger.Info($"{Name}: Recipe {ProcessProgram?.Name} has been unloaded.");
        }

        public void SkipCurrentInstruction()
        {
            (ProcessProgramProcessor.CurrentInstruction as ISkippableInstruction)?.Skip();
        }

        public void AddBreakPoint(Instruction instruction)
        {
            ProcessProgramProcessor?.BreakOn(i => ReferenceEquals(i, instruction), true);
        }

        #endregion

        #region Private Methods

        private void DisposeProcessProgramProcessor()
        {
            if (ProcessProgramProcessor != null)
            {
                ProcessProgramProcessor.PropertyChanged -= ProcessProgramProcessor_PropertyChanged;
                ProcessProgramProcessor.Dispose();
                ProcessProgramProcessor = null;
                ProcessProgram.Dispose();
                OnPropertyChanged(nameof(ProcessProgram));
                UpdateProgramExecutionState(ProgramExecutionState.NotStarted);
                UpdateProcessorState(ProcessorState.Paused);
            }
        }

        private void CreateProcessProgramProcessor(Program program)
        {
            DisposeProcessProgramProcessor();
            foreach (var instruction in program.Instructions)
            {
                instruction.ExecutorId = Name;
                SetProcessModuleInstruction(instruction);
            }

            AbortRequested = false;
            StopRequested = false;
            ProcessProgram = program;
            var log = new Logger($"{Name}_Processor");
            ProcessProgramProcessor = new ProgramProcessor($"{Name}_Processor", log)
            {
                ExecutionContextFactory = _executionContextFactory
            };
            ProcessProgramProcessor.PropertyChanged += ProcessProgramProcessor_PropertyChanged;
            UpdateProgramExecutionState(ProgramExecutionState.NotStarted);
        }

        private void UpdateProgramExecutionState(ProgramExecutionState programExecutionState)
        {
            ProgramExecutionState = programExecutionState;
            ProgramExecutionStateChanged?.Invoke(this, ProgramExecutionState);
            switch (programExecutionState)
            {
                case ProgramExecutionState.NotStarted:
                case ProgramExecutionState.Finished:
                case ProgramExecutionState.Aborted:
                    SetState(OperatingModes.Idle);
                    break;
                case ProgramExecutionState.Running:
                    SetState(OperatingModes.Executing);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(programExecutionState),
                        programExecutionState,
                        null);
            }
        }

        private void UpdateProcessorState(ProcessorState processorState)
        {
            ProcessorState = processorState;
            ProcessorStateChanged?.Invoke(this, ProcessorState);
        }

        private void ClearJobInformation()
        {
            RecipeProgressionInfos.Clear();
            RecipeElapsedTimeInfo?.ClearTimerInfo();
            _currentRecipeStep = RecipeSteps.Undefined;
        }

        private void OnRecipeProgressionChanged()
        {
            RecipeProgressionChanged?.Invoke(
                this,
                new RecipeProgressionInfoEventArgs
                {
                    RecipeProgressionInfo = RecipeProgressionInfos,
                    CurrentStep = _currentRecipeStep,
                    IsProgramLoaded = IsProgramLoaded,
                    Program = ProcessProgram
                });
        }

        private void HandleCurrentStepChanged(
            StepCrossingInstruction<RecipeSteps> stepCrossingInstruction)
        {
            RecipeElapsedTimeInfo.IsTimerInstructionRequired = false;
            if (RecipeProgressionInfos.ContainsKey(_currentRecipeStep))
            {
                RecipeProgressionInfos[_currentRecipeStep].StepElapsedTime =
                    RecipeElapsedTimeInfo.TimeElapsedInStep;
            }

            _currentRecipeStep = stepCrossingInstruction.RecipeStep;
            if (_currentRecipeStep != RecipeSteps.Undefined)
            {
                if (RecipeProgressionInfos.ContainsKey(_currentRecipeStep))
                {
                    RecipeProgressionInfos[_currentRecipeStep].CurrentInformation = string.Empty;
                    if (RecipeProgressionInfos[_currentRecipeStep].Instructions != null)
                    {
                        RecipeProgressionInfos[_currentRecipeStep].ProgressionInfo = "0 / "
                            + RecipeProgressionInfos[_currentRecipeStep].Instructions.Count;
                    }
                }

                RecipeElapsedTimeInfo.TimeElapsedInStep = Duration.Zero;
                RecipeElapsedTimeInfo.IsTimerStepRequired = true;
                RecipeElapsedTimeInfo.IsTimerJobRequired = true;
            }
            else
            {
                RecipeElapsedTimeInfo.IsTimerInstructionRequired = false;
                RecipeElapsedTimeInfo.IsTimerStepRequired = false;
                RecipeElapsedTimeInfo.IsTimerJobRequired = false;
            }

            foreach (var jobInfo in RecipeProgressionInfos.Values.Where(
                         jobInfo => jobInfo.Instructions.Count
                                    == (int)jobInfo.CurrentProgressionValue))
            {
                var dr = jobInfo.StepElapsedTime.ToTimeSpan();
                jobInfo.IsStepDone = true;
                jobInfo.ProgressionInfo = string.Concat(
                    $"{dr.Hours:00}:{dr.Minutes:00}:{dr.Seconds:00}",
                    "   [",
                    jobInfo.CurrentProgressionValue,
                    " / ",
                    jobInfo.Instructions.Count,
                    "]");
            }
        }

        private void HandleCurrentInstructionChanged()
        {
            RecipeElapsedTimeInfo.TimeElapsedInInstruction = Duration.Zero;
            RecipeElapsedTimeInfo.IsTimerInstructionRequired = true;
            if (!RecipeProgressionInfos.ContainsKey(_currentRecipeStep))
            {
                return;
            }

            if (CurrentProgramInstruction is StepCrossingInstruction<RecipeSteps>)
            {
                return;
            }

            if (RecipeProgressionInfos[_currentRecipeStep].Instructions != null)
            {
                RecipeProgressionInfos[_currentRecipeStep].CurrentProgressionValue =
                    RecipeProgressionInfos[_currentRecipeStep]
                        .Instructions.IndexOf(CurrentProgramInstruction)
                    + 1;
            }

            RecipeProgressionInfos[_currentRecipeStep].CurrentInformation =
                CurrentProgramInstruction.Name + " : " + CurrentProgramInstruction.Details;
            RecipeProgressionInfos[_currentRecipeStep].CurrentInstruction =
                CurrentProgramInstruction;
            if (RecipeProgressionInfos[_currentRecipeStep]?.Instructions != null)
            {
                RecipeProgressionInfos[_currentRecipeStep].ProgressionInfo =
                    CurrentProgramInstruction.Name
                    + " ["
                    + RecipeProgressionInfos[_currentRecipeStep].CurrentProgressionValue
                    + " / "
                    + RecipeProgressionInfos[_currentRecipeStep].Instructions.Count
                    + "]";
                RecipeProgressionInfos[_currentRecipeStep].StepProgressionPercent =
                    (int)(RecipeProgressionInfos[_currentRecipeStep].CurrentProgressionValue
                          * 100
                          / RecipeProgressionInfos[_currentRecipeStep].Instructions.Count);
            }
        }

        #endregion
    }
}
