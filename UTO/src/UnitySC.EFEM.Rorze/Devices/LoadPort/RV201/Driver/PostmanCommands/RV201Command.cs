using Agileo.Common.Communication;
using Agileo.Common.Tracing;
using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.PostmanCommands
{
    public class RV201Command : RorzeCommand
    {
        #region Constructors

        protected RV201Command(
            char commandType,
            string deviceType,
            byte deviceId,
            string commandName,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, deviceType, deviceId, commandName, sender, eqFacade, commandParameters)
        {
        }

        /// <summary>
        /// Constructor to be used by inherited classes that need a <see cref="Command"/> of a type derived from <see cref="RorzeMessage"/>.
        /// </summary>
        protected RV201Command(
            RorzeMessage command,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(command, sender, eqFacade)
        {
        }

        #endregion

        #region Overrides

        protected override bool TreatCancel(RorzeMessage message)
        {
            var cancelCode = GetCancelCode(message);
            string cancelReason;

            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.LoadPort:
                    cancelReason = RorzeLoadPortCancelCodeInterpreter.CancelCodeToString[cancelCode];
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

            facade.SendEquipmentEvent((int)CommandEvents.CmdCompleteWithError, new ErrorOccurredEventArgs(
                new Error
                {
                    Code = $"{GetCancelCode(message):X4}",
                    Cause = "Received command cannot be executed",
                    Description = cancelReason,
                    Type = RorzeConstants.CommandTypeAbb.Cancel.ToString()
                },
                message.Name,
                message.DeviceType,
                message.DeviceId));

            CommandComplete(); // Command cancelled by hardware, we expect nothing else, command is done

            return true;
        }

        #endregion
    }
}
