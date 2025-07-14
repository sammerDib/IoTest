using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface.Converters;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Robot;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the place wafer command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.2.5 oUNLD
    /// </summary>
    public class PlaceCommand : RobotCommand
    {
        private readonly HostDriver _hostDriver;

        public PlaceCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager,
            HostDriver hostDriver)
            : base(Constants.Commands.Unload, sender, eqFacade, logger, equipmentManager)
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

            if (!byte.TryParse(message.CommandParameters[0][2], out byte placeSlot))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Get command parameters in correct format
            var robotArm = RobotArmConverter.ToRobotArm(arm);
            var destination = StageConverter.ToMaterialLocationContainer(stage, EquipmentManager);
            var effectorType = arm == Constants.Arm.Upper
                ? EquipmentManager.Robot.Configuration.UpperArm.EffectorType
                : EquipmentManager.Robot.Configuration.LowerArm.EffectorType;
            var armLocation = arm == Constants.Arm.Upper
                ? EquipmentManager.Robot.UpperArmLocation
                : EquipmentManager.Robot.LowerArmLocation;
            var size = armLocation.Substrate?.MaterialDimension ?? SampleDimension.NoDimension;
            var materialType = armLocation.Wafer.MaterialType;

            // If we're placing on Aligner, we need to prepare it (cancel chuck, move pins...)
            // So we need to check if this command can be done
            if (stage == Constants.Stage.Aligner
                && !EquipmentManager.Aligner.CanExecute(nameof(IAligner.PrepareTransfer), out _, effectorType, size, materialType))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.RobotError]);
                return true;
            }

            //If we're picking in LoadPort1 and if LoadPort1 is a SMIF LoadPort
            //We need to move to the place slot
            //So we need to check if this command can be done
            if (stage == Constants.Stage.LP1
                && EquipmentManager.LoadPort1 is RE201
                && !EquipmentManager.LoadPort1.CanExecute(nameof(IRE201.GoToSlot), out _, placeSlot))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.RobotError]);
            }

            //If we're picking in LoadPort2 and if LoadPort2 is a SMIF LoadPort
            //We need to move to the place slot
            //So we need to check if this command can be done
            if (stage == Constants.Stage.LP2
                && EquipmentManager.LoadPort2 is RE201
                && !EquipmentManager.LoadPort2.CanExecute(nameof(IRE201.GoToSlot), out _, placeSlot))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.RobotError]);
            }

            // Always need to check that robot can execute the place command
            if (!EquipmentManager.Robot.CanExecute(
                nameof(IRobot.Place),
                out CommandContext context,
                robotArm,
                destination,
                placeSlot) && !_hostDriver.IsRobotSequenceForOcrReadingInProgress)
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
                    if (CancelIfIsArmEffectorValidFailed(message, error, robotArm, effectorType, size)
                        || CancelIfIsArmReadyFailed(message, error, robotArm, false)
                        || CancelIfIsArmEnabledFailed(message, error, robotArm)
                        || CancelIfIsLocationReadyFailed(message, error, placeSlot)
                        || CancelIfIsIdleFailed(message, error))
                    {
                        return true;
                    }

                    // Ignore Aligner Interlock
                    // (aligner will be commanded before robot, interlock is checked again when starting robot command)
                    if (stage == Constants.Stage.Aligner)
                    {
                        shouldBypassCancellation = true;
                        break;
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

            Task.Factory.StartNew(() =>
                {
                    lock (_hostDriver.LockRobotCommand)
                    {
                        // If the destination is the aligner, we must prepare it (move pins to avoid material damage...).
                        if (stage == Constants.Stage.Aligner)
                        {
                            var tasksToWait = new List<Task>();

                            tasksToWait.Add(EquipmentManager.Robot.GoToSpecifiedLocationAsync(destination as IExtendedMaterialLocationContainer,
                                placeSlot, robotArm, false));

                            tasksToWait.Add(
                                EquipmentManager.Aligner.PrepareTransferAsync(
                                    effectorType,
                                    robotArm == RobotArm.Arm1
                                        ? EquipmentManager.Robot.UpperArmWaferDimension
                                        : EquipmentManager.Robot.LowerArmWaferDimension,
                                    robotArm == RobotArm.Arm1
                                        ? EquipmentManager.Robot.UpperArmLocation.Wafer.MaterialType
                                        : EquipmentManager.Robot.LowerArmLocation.Wafer.MaterialType));

                            Task.WaitAll(tasksToWait.ToArray());
                        }
                        else if (stage == Constants.Stage.LP1)
                        {
                            if (EquipmentManager.LoadPort1 is RE201 smifLoadPort)
                            {
                                var tasksToWait = new List<Task>();
                                tasksToWait.Add(EquipmentManager.Robot.GoToSpecifiedLocationAsync(destination as IExtendedMaterialLocationContainer,
                                    1, robotArm, true));

                                tasksToWait.Add(smifLoadPort.GoToSlotAsync(placeSlot));

                                //In case of SMIF Load port, robot will always access to slot 1
                                //So we do a particular case by setting the slot to maxValue
                                placeSlot = byte.MaxValue;

                                Task.WaitAll(tasksToWait.ToArray());
                            }
                        }
                        else if (stage == Constants.Stage.LP2)
                        {
                            if (EquipmentManager.LoadPort2 is RE201 smifLoadPort)
                            {
                                var tasksToWait = new List<Task>();
                                tasksToWait.Add(EquipmentManager.Robot.GoToSpecifiedLocationAsync(destination as IExtendedMaterialLocationContainer,
                                    1, robotArm, true));

                                tasksToWait.Add(smifLoadPort.GoToSlotAsync(placeSlot));

                                //In case of SMIF Load port, robot will always access to slot 1
                                //So we do a particular case by setting the slot to maxValue
                                placeSlot = byte.MaxValue;

                                Task.WaitAll(tasksToWait.ToArray());
                            }
                        }

                        EquipmentManager.Robot.Place(robotArm, destination as IExtendedMaterialLocationContainer, placeSlot);
                    }
                })
                .ContinueWith(antecedent => SendCommandResult(antecedent));

            return true;
        }
    }
}
