using System;
using System.Globalization;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Robot.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Robot.Resources;
using UnitySC.Equipment.Abstractions.Enums;

using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;

namespace UnitySC.Equipment.Abstractions.Devices.Robot.Conditions
{
    /// <summary>
    /// While the EFEM controller will manage only <see cref="EffectorType.VacuumI"/>,
    /// this condition will always be in error when another type of effector is given.
    /// When the EFEM controller would manage other types of robot arm effector, modify or delete this precondition.
    /// </summary>
    public class IsArmEffectorValid : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not Robot robot)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotARobot,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            switch (context.Command.Name)
            {
                case nameof(IRobot.Swap):
                    var swapArg = context.GetArgument("pickArm");
                    if (swapArg is not RobotArm pickArm)
                    {
                        context.AddContextError(Messages.ArgumentNotAnArm);
                        return;
                    }

                    swapArg = context.GetArgument("sourceDevice");
                    if (swapArg is not IExtendedMaterialLocationContainer swapLocationContainer)
                    {
                        context.AddContextError(Messages.ArgumentNotAnIExtendedMaterialLocationContainer);
                        return;
                    }

                    swapArg = context.GetArgument("sourceSlot");
                    if (swapArg is not byte swapSlot)
                    {
                        context.AddContextError(Messages.ArgumentNotASlot);
                        return;
                    }

                    var swapSize = swapLocationContainer.GetMaterialDimension(swapSlot);
                    var swapError = CheckIfArmEffectorValid(robot, pickArm, swapSize, true);
                    break;

                case nameof(IRobot.Transfer):
                    break;

                case nameof(IRobot.Pick):
                case nameof(IRobot.Place):
                    var isPick = context.Command.Name.Equals(nameof(IRobot.Pick), StringComparison.Ordinal);
                    var arg = context.GetArgument("arm");
                    if (arg is not RobotArm arm)
                    {
                        context.AddContextError(Messages.ArgumentNotAnArm);
                        return;
                    }

                    arg = context.GetArgument(isPick ? "sourceDevice" : "destinationDevice");
                    if (arg is not IExtendedMaterialLocationContainer locationContainer)
                    {
                        context.AddContextError(Messages.ArgumentNotAnIExtendedMaterialLocationContainer);
                        return;
                    }

                    arg = context.GetArgument(isPick ? "sourceSlot" : "destinationSlot");
                    if (arg is not byte slot)
                    {
                        context.AddContextError(Messages.ArgumentNotASlot);
                        return;
                    }

                    SampleDimension size = SampleDimension.NoDimension;
                    if (isPick)
                    {
                        size = locationContainer.GetMaterialDimension(slot);
                    }
                    else
                    {
                        switch (arm)
                        {
                            case RobotArm.Arm1:
                                size = robot.UpperArmLocation?.Substrate?.MaterialDimension
                                       ?? SampleDimension.NoDimension;
                                break;

                            case RobotArm.Arm2:
                                size = robot.LowerArmLocation?.Substrate?.MaterialDimension
                                       ?? SampleDimension.NoDimension;
                                break;
                        }
                    }

                    var error = CheckIfArmEffectorValid(robot, arm, size, isPick);
                    if (!string.IsNullOrEmpty(error))
                    {
                        context.AddContextError(error);
                    }

                    break;

                default:
                    context.AddContextError(string.Format(
                        CultureInfo.InvariantCulture,
                        GenericDeviceMessages.CommandNotSupported,
                        context.Command.Name,
                        nameof(IsArmEffectorValid)));
                    return;
            }
        }

        protected virtual string CheckIfArmEffectorValid(Robot robot, RobotArm arm, SampleDimension size, bool isPick)
        {
            ArmConfiguration armConfig;
            switch (arm)
            {
                case RobotArm.Arm1:
                    armConfig = robot.Configuration.UpperArm;
                    break;

                case RobotArm.Arm2:
                    armConfig = robot.Configuration.LowerArm;
                    break;

                default:
                    return string.Format(CultureInfo.InvariantCulture, Messages.ArmIsInvalid, arm);
            }

            // For now we support only one kind of end-effector, this will change in the future
            if (armConfig.EffectorType != EffectorType.VacuumI
                && armConfig.EffectorType != EffectorType.VacuumU)
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.EffectorNotSupported,
                    armConfig.EffectorType);
            }

            if (!armConfig.SupportedSubstrateSizes.Contains(size))
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.SizeNotSupported,
                    size,
                    armConfig.EffectorType);
            }

            return string.Empty;
        }
    }
}
