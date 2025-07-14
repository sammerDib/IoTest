using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using Agileo.Common.Communication;
using Agileo.Common.Tracing;
using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver;
using UnitySC.EFEM.Rorze.Drivers.RC530;
using UnitySC.EFEM.Rorze.Drivers.RC550;
using UnitySC.Equipment.Abstractions.Drivers.Common.Enums;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public abstract class RorzeCommand : Command
    {
        #region Constructors

        /// <summary>
        /// Constructor used when sending initial command.
        /// </summary>
        /// <param name="commandType"><see cref="RorzeConstants.CommandTypeAbb"/></param>
        /// <param name="deviceType">The type of the device as described in RORZE documentation</param>
        /// <param name="deviceId">The ID of the device as described in RORZE documentation</param>
        /// <param name="commandName"><see cref="RorzeConstants.Commands"/></param>
        /// <param name="commandParameters">A list of parameters.</param>
        /// <param name="sender"><inheritdoc cref="sender"/></param>
        /// <param name="eqFacade"><inheritdoc cref="eqFacade"/></param>
        protected RorzeCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            string commandName,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandName, sender, eqFacade)
        {
            Command = new RorzeMessage(commandType, deviceType, deviceId, commandName,
                new ReadOnlyDictionary<string, DevicePart>(new Dictionary<string, DevicePart>()), commandParameters);
            commandString = Command.Frame;
        }

        /// <summary>
        /// Constructor to be used by inherited classes that need a <see cref="Command"/> of a type derived from <see cref="RorzeMessage"/>.
        /// </summary>
        protected RorzeCommand(
            RorzeMessage command,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(command.Name, sender, eqFacade)
        {
            Command       = command;
            commandString = Command.Frame;
        }

        #endregion Constructors

        #region Properties

        public RorzeMessage Command { get; }

        #endregion Properties

        #region Command override

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            if (reply.Length <= RorzeConstants.EndOfFrame.Length)
            {
                TraceManager.Instance(nameof(RorzeCommand))
                    .Trace(
                        TraceLevelType.Warning,
                        "Empty string was received.");
                return false;
            }

            if (!CheckReplyParsing(reply))
            {
                return false;
            }

            try
            {
                // Parse message.
                var message = new RorzeMessage(reply,
                    new ReadOnlyDictionary<string, DevicePart>(new Dictionary<string, DevicePart>()));

                // Check whether received message applies to this current command (same device)
                if (!message.DeviceType.Equals(Command.DeviceType)
                    || !message.DeviceId.Equals(Command.DeviceId))
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
                        TraceManager.Instance(nameof(RorzeCommand))
                            .Trace(
                                TraceLevelType.Error,
                                $"Command type not recognized: {message.CommandType}.");
                        break;
                }
            }
            catch (Exception e)
            {
                TraceManager.Instance()
                    .Trace(
                        TraceLevelType.Error,
                        e,
                        $"Error occurred while treating data from the equipment. Received frame: \"{reply}\"");
            }

            return base.TreatReply(reply, ref macroCommandData);
        }

        public override void SendCommand()
        {
            // An event command is used only for listening for the equipment. It does not have to send message to equipment when added to queue.
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Event)
            {
                // Connection command is listen only: do nothing here
                return;
            }

            base.SendCommand();
        }

        #endregion Command override

        #region Virtual methods

        protected virtual bool CheckReplyParsing(string reply)
        {
            // Do not try to parse RorzeSubCommandMessage
            return reply.Count(character => character == '.') == 1;
        }

        #endregion

        #region Abstract methods

        protected virtual bool TreatAck(RorzeMessage message) => true;

        protected virtual bool TreatEvent(RorzeMessage message) => false;

        protected virtual bool TreatCancel(RorzeMessage message)
        {
            var cancelCode = GetCancelCode(message);
            string cancelReason;

            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.Robot:
                    cancelReason = RorzeRobotCancelCodeInterpreter.CancelCodeToString[cancelCode];
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    cancelReason = RorzeAlignerCancelCodeInterpreter.CancelCodeToString[cancelCode];
                    break;

                case RorzeConstants.DeviceTypeAbb.IO:
                    cancelReason = message.DeviceId != 0
                        ? RorzeRC530CancelCodeInterpreter.CancelCodeToString[cancelCode]
                        : RorzeRC550CancelCodeInterpreter.CancelCodeToString[cancelCode];
                    break;

                default:
                    cancelReason =
                        $"Cancel code is {cancelCode} and could not be interpreted for device {message.DeviceType}.";
                    TraceManager.Instance(nameof(RorzeCommand))
                        .Trace(
                            TraceLevelType.Error,
                            $"Cancel code is {cancelCode} and could not be interpreted for device {message.DeviceType}.");
                    break;
            }

            // TODO: Error instance should come from a static dictionary (not instantiated here)
            facade.SendEquipmentEvent((int)CommandEvents.CmdCompleteWithError, new ErrorOccurredEventArgs(
                new Error
                {
                    Code        = $"{GetCancelCode(message):X4}",
                    Cause       = "Received command cannot be executed",
                    Description = cancelReason,
                    Type        = RorzeConstants.CommandTypeAbb.Cancel.ToString()
                },
                message.Name,
                message.DeviceType,
                message.DeviceId));

            CommandComplete(); // Command cancelled by hardware, we expect nothing else, command is done

            return true;
        }

        protected virtual bool TreatNack(RorzeMessage message)
        {
            facade.SendEquipmentEvent((int)CommandEvents.CmdNotRecognized, new ErrorOccurredEventArgs(
                new Error
                {
                    Cause       = $"The physical device does not recognize the command {message.Name}",
                    Description = $"NAK received - Command name \"{Command.Name}\" is not recognized by {message.DeviceType + message.DeviceId}.",
                    Type        = RorzeConstants.CommandTypeAbb.Nak.ToString()
                },
                message.Name,
                message.DeviceType,
                message.DeviceId));

            CommandComplete();

            return true;
        }

        #endregion Abstract methods

        protected static int GetCancelCode(RorzeMessage message)
        {
            string codeString = message.Data.Replace(":", string.Empty);
            if (!int.TryParse(codeString, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture,
                out var cancelCode))
            {
                throw new ArgumentException($@"Unexpected cancel code: {codeString}.", nameof(message));
            }

            return cancelCode;
        }
    }
}
