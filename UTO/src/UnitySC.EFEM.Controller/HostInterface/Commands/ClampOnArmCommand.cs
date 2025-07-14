using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Controller.HostInterface.Converters;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Robot;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the clamp/unclamp wafer command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.2.4 oRBCP
    /// </summary>
    public class ClampOnArmCommand : RobotCommand
    {
        private readonly HostDriver _hostDriver;

        public ClampOnArmCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager,
            HostDriver hostDriver)
            : base(Constants.Commands.ClampOnArm, sender, eqFacade, logger, equipmentManager)
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

            // Check number of parameters
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 2)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check first parameter (syntax only)
            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][0], out Constants.Arm arm))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check first parameter (range validity)
            // According to original EFEM, must send cRBCP:1_FF05_Abnormal Range of Parameter
            if (arm == Constants.Arm.Both)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check second parameter (syntax only)
            if (!uint.TryParse(message.CommandParameters[0][1], out uint isClampAsUint))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalParameter]);
                return true;
            }

            // Check second parameter (range validity)
            if (isClampAsUint > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            var isClamp  = isClampAsUint == 1;
            var robotArm = RobotArmConverter.ToRobotArm(arm);

            // Check if command can be executed
            if (!EquipmentManager.Robot.CanExecute(
                isClamp ? nameof(IRobot.Clamp) : nameof(IRobot.Unclamp),
                out CommandContext context,
                robotArm) && !_hostDriver.IsRobotSequenceForOcrReadingInProgress)
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
                        if (isClamp)
                        {
                            EquipmentManager.Robot.Clamp(robotArm);
                        }
                        else
                        {
                            EquipmentManager.Robot.Unclamp(robotArm);
                        }
                    }
                })
                .ContinueWith(antecedent => SendCommandResult(antecedent));

            return true;
        }
    }
}
