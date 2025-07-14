using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    /// <summary>
    /// Base class for messages exchanged with Efem controller.
    /// Aims to encapsulate common treatment so derived classes can focus on their specificness.
    /// </summary>
    public abstract class RorzeCommand : Command
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RorzeCommand"/> class.
        /// </summary>
        /// <param name="commandType"><see cref="Constants.CommandType"/></param>
        /// <param name="commandName"><see cref="Constants.Commands"/></param>
        /// <param name="commandParameters">A list of parameters composed by arguments.</param>
        /// <param name="sender"><inheritdoc cref="sender"/></param>
        /// <param name="eqFacade"><inheritdoc cref="eqFacade"/></param>
        /// <param name="logger">Instance to use to log things.</param>
        protected RorzeCommand(
            char commandType,
            string commandName,
            List<string[]> commandParameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandName, sender, eqFacade)
        {
            Command       = new Message(commandType, commandName, commandParameters);
            commandString = Command.Frame;
            Logger        = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructors

        #region Properties

        public Message Command { get; }

        public ILogger Logger { get; }

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
                if (e.ParsedMessage == null
                    || e.ParsedMessage.CommandName != "????") // Ignore unknown command reply
                {
                    Logger.Error(e);
                }

                return false;
            }

            switch (message.CommandType)
            {
                case Constants.CommandType.Ack:
                    // Ack is receive in response of an order command
                    // message sent:     o<CommandName>:<optional parameters>
                    // message received: a<CommandName>:<optional parameters (same as order)>
                    // Check whether received message applies to this current command (same command name)
                    // 'event' commands (always listening events from HW) are not supposed to treat 'ack' messages (only 'order' command do)
                    if (!message.CommandName.Equals(Command.CommandName)
                        || Command.CommandType == Constants.CommandType.Event)
                        return false;

                    return TreatAck(message);

                case Constants.CommandType.Event: // Format: e<EventName>:<data>
                    // Event can be received in response of an order command (to notify end of command)
                    //     message sent:     o<CommandName>:<optional parameters>
                    //     message received: e<CommandName>:<optional parameters (same as order)>_<Result (success/failure)>
                    // Event can also be received to notify that something happened in the EFEM Controller
                    //     no message sent
                    //     message received: e<CommandName>:<event data>
                    if (!message.CommandName.Equals(Command.CommandName))
                        return false;

                    return TreatEvent(message);

                case Constants.CommandType.Cancel:
                    // Cancel is receive in response of an order command
                    // message sent:     o<CommandName>:<optional parameters>
                    // message received: c<CommandName>:<Level>_<ErrorCode>_<Message>
                    // Check whether received message applies to this current command (same command name)
                    // 'event' commands (always listening events from HW) are not supposed to treat 'cancel' messages (only 'order' command do)
                    if (!message.CommandName.Equals(Command.CommandName)
                        || Command.CommandType == Constants.CommandType.Event)
                        return false;

                    return TreatCancel(message);

                default:
                    // No log here to avoid having all instances adding the same log
                    // (one "unexpected message received" log can be added by the postman when no one has treated the reply)
                    return base.TreatReply(reply, ref macroCommandData);
            }
        }

        public override void SendCommand()
        {
            // 'Event' command are in queue to await for host controller messages
            // but those commands should not be sent
            if (string.IsNullOrWhiteSpace(commandString)
                || commandString.StartsWith(Constants.CommandType.Event.ToString()))
            {
                return;
            }

            base.SendCommand();
        }

        #endregion Command override

        #region Receiving methods

        protected virtual bool TreatAck(Message message) => true;

        /// <summary>
        /// When calling TreatEvent, we know
        /// OR - that the message is an event that indicates completion of an order
        /// OR - that the current listener is listening to the received event (type already checked).
        /// </summary>
        protected virtual bool TreatEvent(Message message) => false;

        protected virtual bool TreatCancel(Message message)
        {
            // TODO Parse received parameters to build the error (Level -> type / ErrorCode -> Code / Message -> Description)
            var error = new Error();

            facade.SendEquipmentEvent((int)EventsToFacade.CmdCompleteWithError, new ErrorOccurredEventArgs(
                error,
                message.CommandName));

            CommandComplete(); // Command cancelled by hardware, we expect nothing else, command is done

            return true;
        }

        #endregion Receiving methods
    }
}
