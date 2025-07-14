using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    /// <summary>
    /// Define a command that would match to any message to send. It should be used only for test.
    /// </summary>
    internal class CustomCommand : Command
    {
        /// <summary>
        /// Create a command that would allow to send the given message. 
        /// </summary>
        /// <param name="message">The customized message that would be send.</param>
        /// <param name="sender"><inheritdoc cref="sender"/></param>
        /// <param name="eqFacade"><inheritdoc cref="eqFacade"/></param>
        /// <param name="logger">Instance to use to log things.</param>
        public CustomCommand(
            string message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger) : base(sender, eqFacade)
        {
            commandString = message;
            Logger        = logger;
        }

        public ILogger Logger { get; }

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

                CommandComplete();
                return true;
            }

            // To consider that a command is ended, we have 2 possibilities: we received an ACK or we received a Cancellation token from the EFEM Controller.
            // We do not have as much information about expected reply as in RorzeCommand, so we can not wait for an event to end. 
            switch (message.CommandType)
            {
                case Constants.CommandType.Ack:
                    CommandComplete();
                    return true;
                case Constants.CommandType.Cancel:
                    CommandComplete();
                    return true;
                default:
                    return base.TreatReply(reply, ref macroCommandData);
            }
        }
    }
}
