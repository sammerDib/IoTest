using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.Drivers;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;

using CommunicatingDeviceMessages = UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Resources.Messages;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Base class for messages exchanged with Host controller.
    /// Aims to encapsulate common treatment so derived classes can focus on their specificness.
    /// </summary>
    public abstract class BaseCommand : Command
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="BaseCommand"/> class.
        /// </summary>
        /// <param name="commandName">Name of the expected order command (<see cref="Constants.Commands"/>)</param>
        /// <param name="sender"><inheritdoc cref="sender"/></param>
        /// <param name="eqFacade"><inheritdoc cref="eqFacade"/></param>
        /// <param name="logger">Instance to use to log things.</param>
        /// <param name="equipmentManager">The equipment manager</param>
        /// <remarks>Constructor intended for command that are received from Host and that we should listen to.</remarks>
        protected BaseCommand(
            string commandName,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(commandName, sender, eqFacade)
        {
            EquipmentManager = equipmentManager;

            CommandType = Constants.CommandType.Order;
            CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
            Logger      = logger ?? throw new ArgumentNullException(nameof(logger));

            // By default nothing to send, command string is updated when sending reply is needed
            commandString = string.Empty;
        }

        protected BaseCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message.CommandName, sender, eqFacade)
        {
            EquipmentManager = equipmentManager;

            CommandType   = message.CommandType;
            Logger        = logger ?? throw new ArgumentNullException(nameof(logger));
            commandString = message.Frame;
        }

        #endregion Constructors

        #region Properties

        public string CommandName { get; }

        public char CommandType { get; }

        public ILogger Logger { get; }

        protected EfemEquipmentManager EquipmentManager;

        #endregion Properties

        #region Command override

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            // Parse the received reply
            Message message;
            try
            {
                message = new Message(reply);
            }
            catch (MessageParseException e)
            {
                // We already have communication log, so this trace might not be that useful
                Logger.Tracer.Trace(Logger.Name, TraceLevelType.Debug, e, "Failed to parse received message.");

                SendCancellation(e.ParsedMessage, e.Error);
                return true;
            }

            // Only orders from Host are expected to be received
            switch (message.CommandType)
            {
                case Constants.CommandType.Order:
                    // Order is received
                    // message received: o<CommandName>:<Parameters (as P1_P2_Pn)>
                    // Check whether received message applies to this current command (same command name)
                    if (!message.CommandName.Equals(CommandName))
                        return false;

                    return TreatOrder(message);

                default:
                    // No log here to avoid having all instances adding the same log
                    // (one "unexpected message received" log can be added by the postman when no one has treated the reply)
                    return base.TreatReply(reply, ref macroCommandData);
            }
        }

        public override void SendCommand()
        {
            // 'Order' command are in queue to await for host controller messages
            // but those commands should not be sent
            if (string.IsNullOrWhiteSpace(commandString)
                || commandString.StartsWith(Constants.CommandType.Order.ToString()))
            {
                return;
            }

            base.SendCommand();

            // Consider an event command completed immediately after sending it if it was not an order.
            // Orders commands are always listening for HOST, they are never completed.
            if (commandString.StartsWith(Constants.CommandType.Event.ToString())
                && CommandType == Constants.CommandType.Event)
            {
                CommandComplete();
            }
        }

        #endregion Command override

        #region Receiving methods

        protected abstract bool TreatOrder(Message message);

        #endregion Receiving methods

        #region Sending methods

        protected void SendAcknowledge(Message receivedMessage)
        {
            Send(new Message(
                Constants.CommandType.Ack,
                receivedMessage.CommandName,
                receivedMessage.CommandParameters));
        }

        protected void SendCancellation(Message receivedMessage, Error error)
        {
            // Build command parameter arguments
            // From RTI EFEM Protocol v2.26_210503.pdf:
            // If Server can’t accept the order command, Server will cancel, and almost all Cancel Command format is cCOMMAND:Level_ErrorCode_Message,
            var parameterArgs = new List<string>
            {
                error.Type,
                error.Code,
                error.Description
            };

            string commandName = receivedMessage?.CommandName ?? "????";

            Send(new Message(
                Constants.CommandType.Cancel,
                commandName,
                new List<string[]> { parameterArgs.ToArray() }));
        }

        protected void SendCompletion(Func<List<string[]>> parametersPredicate)
        {
            Send(new Message(
                Constants.CommandType.Event,
                CommandName,
                parametersPredicate()));
        }

        protected void Send(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            commandString = message.Frame;
            SendCommand();
        }

        protected void SendCommandResult(Task completedTask, int deviceId = -1)
        {
            if (!completedTask.IsCompleted)
                throw new InvalidOperationException(
                    $"{nameof(SendCommandResult)} - The provided {nameof(Task)} must be completed before to send result. "
                    + $"Current task status is \"{completedTask.Status}\".");

            switch (completedTask.Status)
            {
                case TaskStatus.RanToCompletion:
                    // Send completion response (i.e. command is successfully done)
                    SendCompletion(() =>
                    {
                        var parameterArgs = new List<string>();

                        if (deviceId != -1)
                            parameterArgs.Add(deviceId.ToString());

                        parameterArgs.AddRange(GetResultArguments());

                        return new List<string[]> { parameterArgs.ToArray() };
                    });
                    break;

                case TaskStatus.Faulted:
                    // Send completion response (i.e. command is in error)
                    SendCompletion(() =>
                    {
                        var parameterArgs = new List<string>();

                        if (deviceId != -1)
                            parameterArgs.Add(deviceId.ToString());

                        parameterArgs.AddRange(GetResultArguments(GetCommandError(completedTask, deviceId)));

                        return new List<string[]> { parameterArgs.ToArray() };
                    });
                    break;
            }
        }

        protected void SendCommandResult(ExecutionState executionState, int deviceId = -1)
        {
            if (executionState != ExecutionState.Success && executionState != ExecutionState.Failed)
                throw new InvalidOperationException(
                    $"{nameof(SendCommandResult)} - The provided {nameof(executionState)} must be completed before to send result. "
                    + $"Current execution state is \"{executionState}\".");

            switch (executionState)
            {
                case ExecutionState.Success:
                    // Send completion response (i.e. command is successfully done)
                    SendCompletion(() =>
                    {
                        var parameterArgs = new List<string>();

                        if (deviceId != -1)
                            parameterArgs.Add(deviceId.ToString());

                        parameterArgs.AddRange(GetResultArguments());

                        return new List<string[]> { parameterArgs.ToArray() };
                    });
                    break;

                case ExecutionState.Failed:
                    // Send completion response (i.e. command is in error)
                    SendCompletion(() =>
                    {
                        var parameterArgs = new List<string>();

                        if (deviceId != -1)
                            parameterArgs.Add(deviceId.ToString());

                        parameterArgs.AddRange(GetResultArguments(GetCommandError(null, deviceId)));

                        return new List<string[]> { parameterArgs.ToArray() };
                    });
                    break;
            }
        }

        #endregion Sending methods

        #region Other methods

        protected virtual List<string> GetResultArguments(Error error = null)
        {
            // Build command parameter arguments
            var parameterArgs = new List<string>
            {
                // Add result
                ((int)(error == null
                    ? Constants.EventResult.Success
                    : Constants.EventResult.Error)).ToString()
            };

            if (error == null)
                return parameterArgs;

            // Add error level (if defined)
            if (!string.IsNullOrWhiteSpace(error.Type))
            {
                parameterArgs.Add(error.Type);
            }

            // Add error code (if defined)
            if (!string.IsNullOrWhiteSpace(error.Code))
            {
                parameterArgs.Add(error.Code);
            }

            // Add error message (if defined)
            if (!string.IsNullOrWhiteSpace(error.Description))
            {
                parameterArgs.Add(error.Description);
            }

            return parameterArgs;
        }

        protected virtual Error GetCommandError(Task completedTask, int deviceId = -1)
        {
            // Might need to be overriden by derived command class in order to have more context to determine relevant error to send

            Error error;
            switch (deviceId)
            {
                case (int)Constants.Unit.Robot:
                    error = Constants.Errors[ErrorCode.RobotError];
                    break;

                case >= (int)Constants.Unit.LP1 and <= (int)Constants.Unit.LP4:
                    error = Constants.Errors[ErrorCode.LoadPortError];
                    break;

                case (int)Constants.Unit.Aligner:
                    error = Constants.Errors[ErrorCode.AlignerError];
                    break;

                default:
                    error = Constants.Errors[ErrorCode.ErrorOccurredState];
                    break;
            }

            return error;
        }

        internal bool IsCommunicatingFailed(string error)
        {
            return error.Contains(CommunicatingDeviceMessages.NotCommunicating);
        }

        #endregion Other methods
    }
}
