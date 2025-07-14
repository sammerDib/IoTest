using System;
using System.Collections.Generic;
using System.Globalization;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Ffu;
using UnitySC.Equipment.Abstractions.Devices.LightTower;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.Robot;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the set system time command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.5.4 oTIME
    /// </summary>
    public class SetTimeCommand : BaseCommand
    {
        public SetTimeCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.SetTime, sender, eqFacade, logger, equipmentManager)
        {
        }

        public SetTimeCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            // Check number of parameters
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check first parameter (syntax only)
            if (message.CommandParameters[0][0].Length != 14)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check first parameter validity
            if (!DateTime.TryParseExact(
                message.CommandParameters[0][0],
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out DateTime dateValue))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check if command can be executed

            // Robot
            bool result = CheckCanExecuteInError(EquipmentManager.Robot, message, nameof(IRobot.SetDateAndTime));
            if (result)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.RobotError]);
                return true;
            }

            // Aligner
            result = CheckCanExecuteInError(EquipmentManager.Aligner, message, nameof(IAligner.SetDateAndTime));
            if (result)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AlignerError]);
                return true;
            }

            // LoadPorts
            foreach (var loadPort in EquipmentManager.LoadPorts.Values)
            {
                result = CheckCanExecuteInError(loadPort, message, nameof(ILoadPort.SetDateAndTime));
                if (result)
                {
                    SendCancellation(message, Constants.Errors[ErrorCode.LoadPortError]);
                    return true;
                }
            }

            // LightTower
            result = CheckCanExecuteInError(EquipmentManager.LightTower, message, nameof(ILightTower.SetDateAndTime));
            if (result)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.ErrorOccurredState]);
                return true;
            }

            // FFU
            result = CheckCanExecuteInError(EquipmentManager.Ffu, message, nameof(IFfu.SetDateAndTime));
            if (result)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.ErrorOccurredState]);
                return true;
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Start command asynchronously (no completion event)
            TimeUpdater.SetTime(dateValue);
            EquipmentManager.Robot.SetDateAndTimeAsync();
            EquipmentManager.Aligner.SetDateAndTimeAsync();

            foreach (var loadPort in EquipmentManager.LoadPorts.Values)
            {
                loadPort.SetDateAndTimeAsync();
            }

            EquipmentManager.LightTower.SetDateAndTimeAsync();
            EquipmentManager.Ffu.SetDateAndTimeAsync();

            return true;
        }

        // ReSharper disable once UnusedParameter.Local message parameter not used now but could be useful (e.g. to send cancellation)
        private bool CheckCanExecuteInError(Device device, Message message, string commandName)
        {
            if (!device.CanExecute(commandName, out CommandContext context))
            {
                bool shouldBypassCancellation = false;
                foreach (var error in context?.Errors ?? new List<string>())
                {
                    // Log the error so we can determine later why Host command is cancelled
                    if (!string.IsNullOrEmpty(error))
                    {
                        Logger.Error(error);
                    }

                    // Check for preconditions that have a dedicated error code
                    /* None for now */

                    // Special case for preconditions that can be ignored:
                    // meaning we don't want to send a cancellation message but a command ended in error message
                    // (this is to mimic behavior of previous EFEM Controller)
                    if (IsCommunicatingFailed(error))
                    {
                        shouldBypassCancellation = true;
                        break;
                    }
                }

                // Send default cancellation code in case we don't know any better
                if (!shouldBypassCancellation)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
