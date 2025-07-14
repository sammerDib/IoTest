using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class RorzeSubCommand : RorzeCommand
    {
        #region Fields

        private readonly IReadOnlyDictionary<string, DevicePart> _deviceParts;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor used for creating sub-commands.
        /// </summary>
        /// <param name="commandType"><see cref="RorzeConstants.CommandTypeAbb"/></param>
        /// <param name="deviceType">The type of the device as described in RORZE documentation</param>
        /// <param name="deviceId">The ID of the device as described in RORZE documentation</param>
        /// <param name="devicePart">The motion axis or data table of the device on which the command should be sent.</param>
        /// <param name="commandName"><see cref="RorzeConstants.Commands"/></param>
        /// <param name="useParenthesesArgumentSeparator">
        /// Determine if commands argument should be surrounded by parentheses or by brackets.
        /// When using parentheses, all arguments would be surrounded by global parentheses and separated by commas: (p1, p2, ...).
        /// When using brackets, each argument would be surrounded by it's own brackets: [p1][p2]....
        /// </param>
        /// <param name="deviceParts">Device parts</param>
        /// <param name="sender"><inheritdoc cref="sender"/></param>
        /// <param name="eqFacade"><inheritdoc cref="eqFacade"/></param>
        /// <param name="suffix">A suffix that could be appended to the message frame.</param>
        /// <param name="commandParameters">A list of parameters.</param>
        protected RorzeSubCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            string devicePart,
            string commandName,
            bool useParenthesesArgumentSeparator,
            IReadOnlyDictionary<string, DevicePart> deviceParts,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            string suffix = "",
            params string[] commandParameters)
            : base(
                CreateMessage(commandType, deviceType, deviceId, devicePart, commandName, useParenthesesArgumentSeparator, deviceParts, suffix, commandParameters),
                sender,
                eqFacade)
        {
            _deviceParts = deviceParts;
        }

        #endregion Constructors

        #region Properties

        public new SubCommandMessage Command => (SubCommandMessage)base.Command;

        protected ILogger Logger => Agileo.Common.Logging.Logger.GetLogger(nameof(RorzeCommand));

        #endregion Properties

        #region Command override

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            if (reply.Length <= RorzeConstants.EndOfFrame.Length)
            {
                Logger.Warning("Empty string was received.");
                return false;
            }

            // Do not try to parse classic RorzeMessage
            if (reply.Count(character => character == '.') != 2)
            {
                return false;
            }

            try
            {
                // Parse message
                var message = new SubCommandMessage(reply, _deviceParts);

                // Check whether received message applies to this current command (same device)
                if (!message.DeviceType.Equals(Command.DeviceType)
                    || !message.DeviceId.Equals(Command.DeviceId)
                    || !message.DevicePart.Equals(Command.DevicePart))
                    return false;

                switch (message.CommandType)
                {
                    case RorzeConstants.CommandTypeAbb.Ack:
                        // Ack is received in response of an order command
                        // message sent:     o<deviceType><deviceId>.<CommandName>
                        // message received: a<deviceType><deviceId>.<CommandName>:<optional data>
                        // Check whether received message applies to this current command (same command name)
                        // 'event' commands (always listening events from HW) are not supposed to treat 'ack' messages (only 'order' command do)
                        if (!message.Name.Equals(Command.Name)
                            || Command.CommandType == RorzeConstants.CommandTypeAbb.Event)
                            return false;

                        return TreatAck(message);

                    case RorzeConstants.CommandTypeAbb.Event: //Format: e<deviceType><deviceId>.<EventName>:<statuses>
                        return TreatEvent(message);

                    case RorzeConstants.CommandTypeAbb.Cancel:
                        // Cancel is received in response of an invalid order command (syntax error, command context error, etc.)
                        // message sent:     o<deviceType><deviceId>.<CommandName>
                        // message received: c<deviceType><deviceId>.<CommandName>:<cancel code>
                        // Check whether received message applies to this current command (same command name)
                        if (!message.Name.Equals(Command.Name))
                            return false;

                        return TreatCancel(message);

                    case RorzeConstants.CommandTypeAbb.Nak:
                        // Nack is received in response of a not recognized order command (command name not recognized by HW)
                        // message sent:     o<deviceType><deviceId>.<CommandName>
                        // message received: n<deviceType><deviceId>.<CommandName>:<cancel code>
                        // Check whether received message applies to this current command (same command name)
                        if (!message.Name.Equals(Command.Name))
                            return false;

                        return TreatNack(message);

                    default:
                        Logger.Error($"Command type not recognized: {message.CommandType}.");
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error occurred while treating data from the equipment. Received frame: \"{reply}\"");
            }

            return base.TreatReply(reply, ref macroCommandData);
        }

        #endregion Command override

        #region Static Methods

        private static SubCommandMessage CreateMessage(
            char commandType,
            string deviceType,
            byte deviceId,
            string devicePart,
            string commandName,
            bool useParenthesesArgumentSeparator,
            IReadOnlyDictionary<string, DevicePart> deviceParts,
            string suffix = "",
            params string[] commandParameters)
        {
            return new SubCommandMessage(
                commandType, deviceType, deviceId, devicePart,
                commandName, commandParameters, deviceParts, useParenthesesArgumentSeparator, suffix);
        }

        #endregion Static Methods
    }
}
