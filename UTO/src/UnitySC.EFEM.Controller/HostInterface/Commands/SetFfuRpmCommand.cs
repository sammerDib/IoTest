using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.EquipmentModeling;

using UnitsNet;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Ffu;
using UnitySC.Equipment.Abstractions.Devices.Ffu.Enum;

using FfuMessages = UnitySC.Equipment.Abstractions.Devices.Ffu.Resources.Messages;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the FFU set speed command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.5.2 oFFUS
    /// </summary>
    public class SetFfuRpmCommand : BaseCommand
    {
        public SetFfuRpmCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.SetFfuRpm, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            if (App.EfemAppInstance.ControlState == ControlState.Local)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InMaintenanceMode]);
                return true;
            }

            // Check device owning the command exists
            if (EquipmentManager.Ffu == null)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.FfuAbnormal]);
                return true;
            }

            // Check number of parameters
            if (message.CommandParameters.Count <= 0)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            if (message.CommandParameters.Count > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check number of parameter's argument
            if (message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Get parameter
            if (!uint.TryParse(message.CommandParameters[0][0], out uint speedRpm))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            var speed = RotationalSpeed.FromRevolutionsPerMinute(speedRpm);

            // Check if command can be executed
            if (!EquipmentManager.Ffu.CanExecute(nameof(IFfu.SetFfuSpeed), out CommandContext context, speed.RevolutionsPerMinute, FfuSpeedUnit.Rpm))
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
                    if (CancelIfIsFfuSpeedValidFailed(message, error))
                    {
                        return true;
                    }

                    // Special case for preconditions that can be ignored:
                    // meaning we don't want to send a cancellation message but a command ended in error message
                    // (this is to mimic behavior of previous EFEM Controller)
                    if (IsCommunicatingFailed(error))
                    {
                        shouldBypassCancellation = true;
                    }
                }

                // Send default cancellation code in case we don't know any better
                if (!shouldBypassCancellation)
                {
                    SendCancellation(message, Constants.Errors[ErrorCode.FfuAbnormal]);
                    return true;
                }
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Start command asynchronously and send completion event when done
            EquipmentManager.Ffu.SetFfuSpeedAsync(speed.RevolutionsPerMinute, FfuSpeedUnit.Rpm).ContinueWith(antecedent => SendCommandResult(antecedent));
            return true;
        }

        protected override Error GetCommandError(Task completedTask, int deviceId = -1)
        {
            return Constants.Errors[ErrorCode.FfuAbnormal];
        }

        private bool CancelIfIsFfuSpeedValidFailed(Message message, string error)
        {
            if (error.Contains(FfuMessages.ArgumentNotARotationalSpeed)
                || error.Contains(FfuMessages.RotationalSpeedNotInRange))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            return false;
        }
    }
}
