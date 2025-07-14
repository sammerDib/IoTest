using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

using Agileo.MessageDataBus;
using Agileo.SemiDefinitions;

using Timer = System.Timers.Timer;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication
{
    /// <summary>
    /// A driver based on message data bus that implements a protocol.
    /// </summary>
    public abstract class ProtocolizedDriver : MessageDatabusDriver
    {

        // Just a class to keep track of the defined command.
        // private usage
        public class CommandDefinition
        {
            public string Name { get; set; }
            public ushort Id { get; set; }
            public ExternalGroup ParameterGroup { get; set; }
            public List<CommandStep> CommandSteps { get; } = new List<CommandStep>();

            public CommandDefinition AddCommandStep(string stepName, ushort stepValue)
            {
                if (string.IsNullOrEmpty(stepName)) throw new ArgumentException("Step name cannot be null or empty", nameof(stepName));
                if (ExistsStep(stepName, stepValue)) throw new ArgumentException($"Step {stepName} has already been defined");
                if (stepValue == (ushort)HandshakeState.Complete)
                    throw new ArgumentException(
                        $"Adding step for step {stepName} and value {stepValue} is not possible because it is corresponding to existing value for {nameof(HandshakeState.Complete)}.");
                if (stepValue == (ushort)HandshakeState.Executing)
                    throw new ArgumentException(
                        $"Adding step for step {stepName} and value {stepValue} is not possible because it is corresponding to existing value for {nameof(HandshakeState.Executing)}.");
                if (stepValue == (ushort)HandshakeState.DoneOnAutomata)
                    throw new ArgumentException(
                        $"Adding step for step {stepName} and value {stepValue} is not possible because it is corresponding to existing value for {nameof(HandshakeState.DoneOnAutomata)}.");
                if (stepValue == (ushort)HandshakeState.Error)
                    throw new ArgumentException(
                        $"Adding step for step {stepName} and value {stepValue} is not possible because it is corresponding to existing value for {nameof(HandshakeState.Error)}.");
                if (stepValue >= 10)
                    throw new ArgumentException(
                        $"Adding step for step {stepName} and value {stepValue} is not possible because it is greather than 10. Step value must be between 4 and 9 included.");

                CommandSteps.Add(new CommandStep { Name = stepName, Value = stepValue });

                return this;
            }

            bool ExistsStep(string stepName, ushort stepValue)
            {
                return CommandSteps.Exists(x => x.Name == stepName || x.Value == stepValue);
            }
        }

        public class CommandStep
        {
            public string Name { get; set; }
            public ushort Value { get; set; }
        }

        class Handshake
        {
            public CommandDefinition CommandDefinition { get; set; }
            public ushort Error { get; set; }
        }

        enum HandshakeState
        {
            Complete = 0,
            Executing = 1,
            DoneOnAutomata = 2,
            Error = 3,
        }

        #region Fields

        private Timer _heartBeatTimer;
        private Timer _heartBeatTimeoutTimer;
        #endregion Fields

        protected ProtocolizedDriver(string category, byte port, string mdbDriverName)
            : base(category, port, mdbDriverName)
        { }

        private bool _disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (CommandStatusTag != null) CommandStatusTag.ValueChanged -= CommandStatus_ValueChanged;
                if (IncomingHeartBeatTag != null)
                    IncomingHeartBeatTag.ValueChanged -= IncomingHeartBeatTag_ValueChanged;

                _heartBeatTimer?.Stop();
                _heartBeatTimeoutTimer?.Stop();
                if (_heartBeatTimer != null) _heartBeatTimer.Elapsed -= HeartBeatTimer_Elapsed;
                if (_heartBeatTimeoutTimer != null) _heartBeatTimeoutTimer.Elapsed -= HeartBeatTimeoutTimer_Elapsed;
            }

            _disposed = true;
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override void Configure(MessageDataBus tags, int timing)
        {
            base.Configure(tags, timing);

            SetProtocolVariables();
            if (CommandTag == null) throw new InvalidOperationException($"You have to set property {nameof(CommandTag)} in method {nameof(SetProtocolVariables)}");
            if (CommandStatusTag == null) throw new InvalidOperationException($"You have to set property {nameof(CommandStatusTag)} in method {nameof(SetProtocolVariables)}");
            if (CommandErrorTag == null) throw new InvalidOperationException($"You have to set property {nameof(CommandErrorTag)} in method {nameof(SetProtocolVariables)}");

            bool isHeartBeatEnabled = SetHeartBeatVariables();
            if (isHeartBeatEnabled)
            {
                if (IncomingHeartBeatTag == null) throw new InvalidOperationException($"You have to set property {nameof(IncomingHeartBeatTag)} in method {nameof(SetHeartBeatVariables)}");
                if (OutcomingHeartBeatTag == null) throw new InvalidOperationException($"You have to set property {nameof(OutcomingHeartBeatTag)} in method {nameof(SetHeartBeatVariables)}");
                if (HeartBeatCadence <= 0) throw new InvalidOperationException($"You have to set property {nameof(HeartBeatCadence)} in method {nameof(SetHeartBeatVariables)}");
                if (HeartBeatTimeout <= 0) throw new InvalidOperationException($"You have to set property {nameof(HeartBeatTimeout)} in method {nameof(SetHeartBeatVariables)}");

                _heartBeatTimer = new Timer(HeartBeatCadence);
                _heartBeatTimer.Elapsed += HeartBeatTimer_Elapsed;
                _heartBeatTimer.AutoReset = false;

                _heartBeatTimeoutTimer = new Timer(HeartBeatTimeout);
                _heartBeatTimeoutTimer.Elapsed += HeartBeatTimeoutTimer_Elapsed;
                _heartBeatTimeoutTimer.AutoReset = false;

                _heartBeatTimer.Start();
                _heartBeatTimeoutTimer.Start();

                IncomingHeartBeatTag.ValueChanged += IncomingHeartBeatTag_ValueChanged;
            }

            CommandStatusTag.ValueChanged += CommandStatus_ValueChanged;

            DefineCommands();
        }

        private readonly List<CommandDefinition> _commandDefinitions = new List<CommandDefinition>();
        bool ExistsCommand(string commandName)
        {
            return _commandDefinitions.Exists(x => x.Name == commandName);
        }

        /// <summary>
        /// As integrator, call API DefineCommand to register command in the protocol.
        /// </summary>
        protected abstract void DefineCommands();

        /// <summary>
        /// Register a command to the protocol.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="commandAsUShort">The value of the command that will be written on the communication link</param>
        /// <param name="parameterGroup">The group of parameters that will be written, can be null</param>
        protected CommandDefinition DefineCommand(string commandName, ushort commandAsUShort, ExternalGroup parameterGroup = null)
        {
            // pre conditions
            if (string.IsNullOrEmpty(commandName)) throw new ArgumentException("Command name cannot be null or empty", nameof(commandName));
            if (ExistsCommand(commandName)) throw new ArgumentException($"Command {commandName} has already been defined");
            if (commandAsUShort == 0) throw new ArgumentException("Command cannot be equal to 0", nameof(commandAsUShort));

            CommandDefinition commandDefinition = new CommandDefinition
            {
                Name = commandName,
                Id = commandAsUShort,
                ParameterGroup = parameterGroup
            };

            _commandDefinitions.Add(commandDefinition);

            return commandDefinition;
        }

        #region protocol
        /// <summary>
        /// As integrator, set properties CommandTag, CommandStatusTag and CommandErrorTag.
        /// </summary>
        protected abstract void SetProtocolVariables();

        protected Tag<ushort> CommandTag { get; set; }
        private ushort CommandAsUShort => CommandTag.Value;

        protected Tag<ushort> CommandStatusTag { get; set; }

        protected Tag<ushort> CommandErrorTag { get; set; }

        /// <summary>
        /// This will execute the protocol for the given command.
        /// </summary>
        /// <param name="command">The name of the command to write</param>
        /// <exception cref="InvalidOperationException">If a command is already being executed</exception>
        protected void WriteCommand(string command)
        {
            // pre conditions
            if (CommandAsUShort != 0) throw new InvalidOperationException($"Cannot write command {command} because there is already one command being executed");

            CommandDefinition cd = _commandDefinitions.FirstOrDefault(x => x.Name == command);
            if (cd == null) throw new ArgumentException($"No command with id {command} found");

            WriteCommand(cd);
        }

        private Handshake _handshake;

        private void WriteCommand(CommandDefinition cd)
        {
            _handshake = new Handshake { CommandDefinition = cd };
            cd.ParameterGroup?.SyncWriteOnDriver();
            CommandTag.Value = cd.Id;
        }

        private void CommandStatus_ValueChanged(object sender, ValueChangedEventArgs<ushort> e)
        {
            if (_handshake == null)
            {
                //No need to raise exception if the current value is 0 because it means no command active
                if (e.Value == 0) return;

                throw new InvalidOperationException("Why does command status change? No command active.");
            }

            CommandDefinition cd = _handshake.CommandDefinition;
            ushort command = cd.Id;

            ushort commandStatus = e.Value;
            if (commandStatus != 0)
            {
                if ((ushort)Math.Floor((double)e.Value / 10) != command) throw new InvalidOperationException("Command status not for current command");
                commandStatus = (ushort)(e.Value - (command * 10));
            }

            switch (commandStatus)
            {
                case (ushort)HandshakeState.Complete:
                    CommandExecutionEventArgs args;
                    if (_handshake.Error == 0)
                    {
                        args = CommandExecutionEventArgs.Ok(_handshake.CommandDefinition.Id, _handshake.CommandDefinition.Name);
                        _handshake = null;
                        OnCommandDone(args);
                    }
                    else
                    {
                        args = CommandExecutionEventArgs.Failed(_handshake.CommandDefinition.Id,
                            _handshake.CommandDefinition.Name,
                            _handshake.Error.ToString());
                        _handshake = null;
                        OnCommandFailed(args);
                    }
                    break;
                case (ushort)HandshakeState.Executing:
                    OnCommandStepChanged(nameof(HandshakeState.Executing), commandStatus);
                    break;
                case (ushort)HandshakeState.DoneOnAutomata:
                    CommandTag.Value = 0;
                    OnCommandStepChanged(nameof(HandshakeState.DoneOnAutomata), commandStatus);
                    break;
                case (ushort)HandshakeState.Error:
                    CommandTag.Value = 0;
                    _handshake.Error = CommandErrorTag.Value;
                    OnCommandStepChanged(nameof(HandshakeState.Error), commandStatus);
                    break;
                default:
                    CommandStep step = cd.CommandSteps.FirstOrDefault(x => x.Value == commandStatus);
                    if (step != null)
                    {
                        OnCommandStepChanged(step);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                    break;
            }
        }
        #endregion

        public void DiscardOpenTransaction(string senderName)
        {
            if (_handshake != null)
            {
                CommandExecutionEventArgs args = CommandExecutionEventArgs.Failed(
                    _handshake.CommandDefinition.Id,
                    _handshake.CommandDefinition.Name,
                    $"Command {_handshake.CommandDefinition.Name} discarded by {senderName}"
                );

                _handshake = null;
                CommandTag.Value = 0;
                OnCommandFailed(args);
            }
        }

        public override void Abort()
        {
            if (_handshake == null) throw new InvalidOperationException("No command is active. No need to send abort on the protocol.");

            CommandTag.Value = ushort.MaxValue;
        }

        public bool IsConnected { get; private set; }

        public bool CommandInProgress => _handshake != null;

        public event EventHandler<CommandStepChangedEventArgs> CommandStepChanged;

        private void OnCommandStepChanged(CommandStep commandStep)
        {
            CommandStepChanged?.Invoke(this, new CommandStepChangedEventArgs(commandStep));
        }

        private void OnCommandStepChanged(string name, ushort value)
        {
            CommandStepChanged?.Invoke(this, new CommandStepChangedEventArgs(name, value));
        }

        #region HeartBeat

        /// <summary>
        /// As integrator, set properties IncomingHeartBeatTag and OutcomingHeartBeatTag.
        /// </summary>
        /// <returns>true = Heart Beat Enabled, false = Heart Beat Disabled</returns>
        protected abstract bool SetHeartBeatVariables();

        protected Tag<bool> IncomingHeartBeatTag { get; set; }
        protected Tag<bool> OutcomingHeartBeatTag { get; set; }
        protected int HeartBeatCadence { get; set; }
        protected int HeartBeatTimeout { get; set; }

        private void HeartBeatTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsConnected)
            {
                IsConnected = false;
                OnCommunicationClosed(new EventArgs());
            }
        }

        private void HeartBeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OutcomingHeartBeatTag.Value = !OutcomingHeartBeatTag.Value;

            _heartBeatTimer.Start();
        }

        private void IncomingHeartBeatTag_ValueChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            _heartBeatTimeoutTimer.Stop();

            if (!IsConnected)
            {
                IsConnected = true;
                OnCommunicationEstablished(new EventArgs());
            }

            _heartBeatTimeoutTimer.Start();
        }
        #endregion HeartBeat

        public virtual void Execute(Action protocolAction)
        {
            Semaphore s = new Semaphore(0, 1);
            ErrorDescription errors = new ErrorDescription(ErrorCode.Ok, string.Empty);
            string commandName;

            EventHandler<CommandExecutionEventArgs> done = (sender, args) =>
                {
                    switch (args.Status)
                    {
                        case CommandStatusCode.Error:
                            commandName = args.Name;
                            errors.ErrorText = $"Command {commandName} in error : {string.Join("\r\n", args.Errors)}";
                            errors.ErrorCode = ErrorCode.Ok;
                            s.Release();
                            break;
                        case CommandStatusCode.Ok:
                            s.Release();
                            break;
                    }
                }

                ;
            CommandDone += done;
            CommandFailed += done;
            try
            {
                protocolAction();
                s.WaitOne();
            }
            finally
            {
                CommandDone -= done;
                CommandFailed -= done;
            }
        }

    }
}
