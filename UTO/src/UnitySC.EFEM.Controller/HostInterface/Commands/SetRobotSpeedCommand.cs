using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Robot;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the set robot speed command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.2.8 oSSPD
    /// </summary>
    public class SetRobotSpeedCommand : RobotCommand
    {
        private readonly HostDriver _hostDriver;

        public SetRobotSpeedCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager,
            HostDriver hostDriver)
            : base(Constants.Commands.SetRobotSpeed, sender, eqFacade, logger, equipmentManager)
        {
            _hostDriver = hostDriver;
        }

        protected override bool TreatOrder(Message message)
        {
            if (App.EfemAppInstance.ControlState == ControlState.Local)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InMaintenanceMode]);
                return true;
            }

            // Too many/few parameters (we expect only one parameter with one argument).
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check parameter validity
            if (message.CommandParameters[0][0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check that parameter is in range
            var speedAsChar = message.CommandParameters[0][0][0];
            if (!Constants.RobotSpeeds.ContainsKey(speedAsChar))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Actually check if command can be executed
            if (!EquipmentManager.Robot.CanExecute(
                nameof(IRobot.SetMotionSpeed),
                out CommandContext context,
                Constants.RobotSpeeds[speedAsChar]) && !_hostDriver.IsRobotSequenceForOcrReadingInProgress)
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
                    if (CancelIfIsSpeedValidFailed(message, error))
                    {
                        return true;
                    }

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
                    SendCancellation(message, Constants.Errors[ErrorCode.RobotError]);
                    return true;
                }
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Start command asynchronously and send completion event when done
            Task.Factory.StartNew(
                    () =>
                    {
                        lock (_hostDriver.LockRobotCommand)
                        {
                            EquipmentManager.Robot.SetMotionSpeed(
                                Constants.RobotSpeeds[speedAsChar]);
                        }
                    })
                .ContinueWith(antecedent => SendCommandResult(antecedent));

            return true;
        }
    }
}
