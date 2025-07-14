using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface.Converters;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.Robot;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the prepare pick wafer command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.2.6 oWGET
    /// </summary>
    public class PreparePickCommand : RobotCommand
    {
        private readonly HostDriver _hostDriver;

        public PreparePickCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager,
            HostDriver hostDriver)
            : base(Constants.Commands.PreparePick, sender, eqFacade, logger, equipmentManager)
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
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 3)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check first parameter validity
            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][0], out Constants.Arm arm))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check second parameter
            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][1], out Constants.Stage stage))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check third parameter (syntax only)
            if (message.CommandParameters[0][2].Length != 2)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (!byte.TryParse(message.CommandParameters[0][2], out byte slot))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check if stage is a loadPort
            if (stage >= Constants.Stage.LP1 && stage <= Constants.Stage.LP4)
            {
                // Get the loadport
                EquipmentManager.LoadPorts.TryGetValue(
                    Constants.ToLoadPortId((Constants.Unit)stage),
                    out LoadPort loadPort);

                // Error case Carrier not loaded
                if (loadPort != null && loadPort.PhysicalState != LoadPortState.Open)
                {
                    SendCancellation(message, Constants.Errors[ErrorCode.CarrierNotLoad]);
                    return true;
                }

                // Error case Carrier not present
                if (loadPort != null && loadPort.CarrierPresence == CassettePresence.Absent)
                {
                    SendCancellation(message, Constants.Errors[ErrorCode.CarrierNotPresent]);
                    return true;
                }
            }

            // Get command parameters in correct format
            var source                  = StageConverter.ToMaterialLocationContainer(stage, EquipmentManager);
            var robotArm                = RobotArmConverter.ToRobotArm(arm);
            const bool isPickUpPosition = true;

            // Actually check if command can be executed
            if (!EquipmentManager.Robot.CanExecute(
                nameof(IRobot.GoToSpecifiedLocation),
                out CommandContext context,
                source,
                slot,
                robotArm,
                isPickUpPosition) && !_hostDriver.IsRobotSequenceForOcrReadingInProgress)
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
                    if (CancelIfIsIdleFailed(message, error)
                        || CancelIfIsNotBusyFailed(message, error)
                        || CancelIfIsArmEnabledFailed(message, error, robotArm))
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
                        EquipmentManager.Robot.GoToSpecifiedLocation(
                            source,
                            slot,
                            robotArm,
                            isPickUpPosition);
                    }
                })
                .ContinueWith(antecedent => SendCommandResult(antecedent));


            return true;
        }
    }
}
