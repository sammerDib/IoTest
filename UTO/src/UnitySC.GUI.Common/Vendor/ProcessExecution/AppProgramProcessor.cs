using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.ProcessingFramework;
using Agileo.ProcessingFramework.Instructions;

using UnitsNet;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution
{
    public class AppProgramProcessor : Notifier, IDisposable
    {
        #region Fields

        private readonly StringBuilder _programErrors = new();

        private readonly ProgramProcessor _programProcessor;
        private readonly ILogger _logger;
        private readonly Stopwatch _waitInstructionStopWatch;
        private BusinessPanel _targetedBusinessPanel;

        #endregion

        public event EventHandler ProgramLoaded;

        public event EventHandler CurrentProgramInstructionChanged;

        public event EventHandler<InstructionStateChangeEventArgs> ProgramCurrentInstructionStateChanged;

        public event EventHandler<ProgramExecutionState> ProgramExecutionStateChanged;

        public event EventHandler<ProcessorState> ProcessorStateChanged;

        public event EventHandler OnDisposed;

        public AppProgramProcessor(ITracer tracer)
        {
            if (tracer == null)
            {
                throw new ArgumentNullException(nameof(tracer));
            }

            _logger = Logger.GetLogger(nameof(AppProgramProcessor), tracer);

            AbortRequested = false;
            StopRequested = false;
            _programProcessor =
                new ProgramProcessor(
                    "ProcessExecutor",
                    new Agileo.ProcessingFramework.Logging.Logger(_logger.Name))
                {
                    ExecutionContextFactory = new ProcessModuleExecutionFactory()
                };

            _programProcessor.PropertyChanged += ProgramProcessor_PropertyChanged;

            _waitInstructionStopWatch = new Stopwatch();

            ProgramExecutionState = ProgramExecutionState.NotStarted;
        }

        #region Properties

        private bool _stopRequested;

        public bool StopRequested
        {
            get { return _stopRequested; }
            private set { SetAndRaiseIfChanged(ref _stopRequested, value); }
        }

        private bool _abortRequested;

        public bool AbortRequested
        {
            get { return _abortRequested; }
            private set { SetAndRaiseIfChanged(ref _abortRequested, value); }
        }

        public ProcessorState State => _programProcessor.State;

        private ProgramExecutionState _programExecutionState;

        public ProgramExecutionState ProgramExecutionState
        {
            get { return _programExecutionState; }
            set
            {
                if (_programExecutionState == value) return;
                _programExecutionState = value;
                ProgramExecutionStateChanged?.Invoke(this, ProgramExecutionState);
                OnPropertyChanged(nameof(ProgramExecutionState));
            }
        }

        private Program _currentProgram;

        public Program CurrentProgram
        {
            get { return _currentProgram; }
            private set
            {
                if (_currentProgram == value) return;

                if (_currentProgram != null)
                {
                    _currentProgram.InstructionStateChanged -= CurrentProgram_InstructionStateChanged;
                }

                _currentProgram = value;

                if (_currentProgram != null)
                {
                    _currentProgram.InstructionStateChanged += CurrentProgram_InstructionStateChanged;
                }

                ProgramLoaded?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged(nameof(CurrentProgram));
            }
        }

        public Instruction CurrentInstruction => _programProcessor?.CurrentInstruction;

        public Instruction ExecutingInstruction =>
            CurrentProgram?.Instructions.SingleOrDefault(instruction =>
                instruction.InstructionState == InstructionState.Executing);

        public string ProgramError => _programErrors.ToString();

        #endregion

        public void Setup(BusinessPanel targetedBusinessPanel)
        {
            _targetedBusinessPanel = targetedBusinessPanel;
        }

        public bool LoadProgram(Program program)
        {
            try
            {
                AbortRequested = false;
                StopRequested = false;

                foreach (var programInstruction in program.Instructions.OfType<IUserInterfaceInstruction>())
                {
                    if (_targetedBusinessPanel == null)
                    {
                        throw new InvalidOperationException(
                            $"Business panel not specified to display user interface instruction included in program {program.Name}.");
                    }

                    programInstruction.TargetedBusinessPanel = _targetedBusinessPanel;
                }

                foreach (var programInstruction in program.Instructions.OfType<WaitProcessInstruction>())
                {
                    programInstruction.AppProgramProcessor = this;
                }

                _programProcessor.SetupProgram(
                    program,
                    program.Instructions.Select(x => x.InstructionState).ToArray(),
                    _programProcessor.ExecutionContext);

                // Define Predicate for Breakpoint -> add disabled breakpoint for all instructions
                _programProcessor.BreakOn(x => true, false);
                CurrentProgram = program;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }

            return true;
        }

        public void RunProgram()
        {
            if (!CheckProgramConditionsAreOk()) return;
            _programProcessor.Run();
            ProgramExecutionState = ProgramExecutionState.Running;
        }

        public void BreakpointProcess(bool isActive)
        {
            if (_programProcessor == null)
            {
                return;
            }

            foreach (var breakpoint in _programProcessor.Breakpoints)
            {
                breakpoint.IsActive = isActive;
            }
        }

        private void CurrentProgram_InstructionStateChanged(object sender, InstructionStateChangeEventArgs e)
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
                    _programErrors.AppendLine().Append(" â¢ ").Append(errors[index]).Append('.');
                }
                _programErrors.AppendLine().Append(" â¢ ").Append(errors[errors.Length - 1]);
            }

            OnPropertyChanged(nameof(ProgramError));

            ProgramCurrentInstructionStateChanged?.Invoke(sender, e);
        }

        public void PauseProgramExecution()
        {
            if (!CheckProgramConditionsAreOk()) return;

            _programProcessor.Break("cause");
        }

        public void AbortProgramExecution()
        {
            if (!CheckProgramConditionsAreOk()) return;

            AbortRequested = true;
            var abortable = _programProcessor?.CurrentInstruction as IInstructionAbortable;
            abortable?.AbortInstruction();
            SpinWait.SpinUntil(() => _programProcessor.State == ProcessorState.Paused, 5000);
            _programProcessor?.Abort();

            var instruction = ExecutingInstruction as IInstructionAbortable;
            instruction?.AbortInstruction();
        }

        public void StopProgramExecution()
        {
            if (!CheckProgramConditionsAreOk()) return;

            StopRequested = true;
            if (_programProcessor.State == ProcessorState.Paused)
            {
                _programProcessor.Abort();
            }
            else
            {
                _programProcessor.Break("Stop");
            }
        }
        
        #region Private

        private void ReleaseProgramProcessor()
        {
            _programProcessor.PropertyChanged -= ProgramProcessor_PropertyChanged;
        }

        private void ProgramProcessor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_programProcessor.CurrentInstruction):
                    CurrentProgramInstructionChanged?.Invoke(sender, e);
                    break;

                case nameof(_programProcessor.State):
                    switch (_programProcessor.State)
                    {
                        case ProcessorState.Complete:
                            ProgramExecutionState = AbortRequested
                                ? ProgramExecutionState.Aborted
                                : ProgramExecutionState.Finished;
                            break;
                        case ProcessorState.Paused:
                            if (StopRequested)
                            {
                                _programProcessor.Abort();
                            }

                            break;
                    }

                    ProcessorStateChanged?.Invoke(this, State);
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }

        private bool CheckProgramConditionsAreOk()
        {
            if (CurrentProgram == null)
            {
                _logger.Error("No program is loaded.");
                return false;
            }

            return true;
        }

        #endregion

        public void SetWaitInstructionElapsedTimer(bool isEnable)
        {
            if (isEnable)
            {
                _waitInstructionStopWatch.Restart();
            }
            else
            {
                _waitInstructionStopWatch.Stop();
                WaitInstructionElapsedTime = Duration.FromSeconds(_waitInstructionStopWatch.Elapsed.Seconds);
            }
        }

        private Duration _waitInstructionElapsedTime;

        public Duration WaitInstructionElapsedTime
        {
            get { return _waitInstructionElapsedTime; }
            set
            {
                if (_waitInstructionElapsedTime == value) return;
                _waitInstructionElapsedTime = value;
                OnPropertyChanged(nameof(WaitInstructionElapsedTime));
            }
        }

        public void SkipCurrentInstruction()
        {
            (ExecutingInstruction as ISkippableInstruction)?.Skip();
        }

        public void JumpToInstruction(Instruction instruction)
        {
            if (!(instruction is AwaitInstruction))
            {
                _programProcessor?.Jump(instruction);
                var index = _currentProgram.Instructions.IndexOf(instruction);
                for (var i = index; i < _currentProgram.Instructions.Count - 1; i++)
                {
                    var instruct = _currentProgram.Instructions[i];
                    _currentProgram.SetInstructionState(instruct, InstructionState.Waiting);
                }
            }
        }

        public void ReplayCurrentInstruction()
        {
            _programProcessor.Jump(_programProcessor.CurrentInstruction);
            _currentProgram.SetInstructionState(_programProcessor.CurrentInstruction, InstructionState.Waiting);
            RunProgram();
        }

        public IReadOnlyCollection<Breakpoint> Breakpoints => _programProcessor.Breakpoints;

        public void AddBreakPoint(Instruction instruction)
        {
            _programProcessor.BreakOn(i => ReferenceEquals(i, instruction), true);
        }

        public void RemoveBreakPoint(Instruction instruction)
        {
            _programProcessor.BreakOn(i => ReferenceEquals(i, instruction), false);
        }

        #region IDisposable

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_programProcessor != null)
                    {
                        ReleaseProgramProcessor();
                        _programProcessor.Dispose();
                        CurrentProgram?.Dispose();
                        ProgramExecutionState = ProgramExecutionState.NotStarted;
                        OnPropertyChanged(nameof(CurrentProgram));
                        OnDisposed?.Invoke(this, EventArgs.Empty);
                    }

                    if (_waitInstructionStopWatch.IsRunning)
                    {
                        _waitInstructionStopWatch.Reset();
                    }
                }
                
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
