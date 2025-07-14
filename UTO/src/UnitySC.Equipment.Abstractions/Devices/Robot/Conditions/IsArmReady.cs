using System;
using System.Globalization;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Robot.Resources;

using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;

namespace UnitySC.Equipment.Abstractions.Devices.Robot.Conditions
{
    public class IsArmReady : CSharpCommandConditionBehavior
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
                case nameof(IRobot.Pick):
                case nameof(IRobot.Place):
                    {
                        var arg = context.GetArgument("arm");
                        if (arg is not RobotArm arm)
                        {
                            context.AddContextError(Messages.ArgumentNotAnArm);
                            return;
                        }

                        var isPick = context.Command.Name.Equals(nameof(IRobot.Pick), StringComparison.Ordinal);
                        var error = CheckIfArmReady(robot, arm, isPick);
                        if (!string.IsNullOrEmpty(error))
                        {
                            context.AddContextError(error);
                        }
                    }
                    break;

                case nameof(IRobot.Transfer):
                    {
                        var pickArg = context.GetArgument("pickArm");
                        if (pickArg is not RobotArm pickArm)
                        {
                            context.AddContextError(Messages.ArgumentNotAnArm);
                            return;
                        }

                        var placeArg = context.GetArgument("placeArm");
                        if (placeArg is not RobotArm placeArm)
                        {
                            context.AddContextError(Messages.ArgumentNotAnArm);
                            return;
                        }

                        var error = CheckIfArmReady(robot, pickArm, true);
                        if (!string.IsNullOrEmpty(error))
                        {
                            context.AddContextError(error);
                        }

                        error = CheckIfArmReady(robot, placeArm, false);
                        if (!string.IsNullOrEmpty(error))
                        {
                            context.AddContextError(error);
                        }
                    }
                    break;

                case nameof(IRobot.Swap):
                    {
                        var pickArg = context.GetArgument("pickArm");
                        if (pickArg is not RobotArm pickArm)
                        {
                            context.AddContextError(Messages.ArgumentNotAnArm);
                            return;
                        }

                        var error = CheckIfArmReady(robot, pickArm, true);
                        if (!string.IsNullOrEmpty(error))
                        {
                            context.AddContextError(error);
                        }

                        var placeArm = pickArm == RobotArm.Arm1
                            ? RobotArm.Arm2
                            : RobotArm.Arm1;

                        error = CheckIfArmReady(robot, placeArm, false);
                        if (!string.IsNullOrEmpty(error))
                        {
                            context.AddContextError(error);
                        }
                    }
                    break;

                default:
                    context.AddContextError(string.Format(
                        CultureInfo.InvariantCulture,
                        GenericDeviceMessages.CommandNotSupported,
                        context.Command.Name,
                        nameof(IsArmReady)));
                    break;
            }
        }

        private string CheckIfArmReady(Robot robot, RobotArm arm, bool isPick)
        {
            bool substratePresence;
            switch (arm)
            {
                case RobotArm.Arm1:

                    if (robot.UpperArmSubstrateDetectionError)
                    {
                        return string.Format(CultureInfo.InvariantCulture, Messages.ArmHasPresenceDetectionError, arm);
                    }

                    substratePresence = robot.UpperArmLocation.Material != null;
                    break;

                case RobotArm.Arm2:

                    if (robot.LowerArmSubstrateDetectionError)
                    {
                        return string.Format(CultureInfo.InvariantCulture, Messages.ArmHasPresenceDetectionError, arm);
                    }


                    substratePresence = robot.LowerArmLocation.Material != null;
                    break;

                default:
                    return string.Format(CultureInfo.InvariantCulture, Messages.ArmIsInvalid, arm);
            }

            // If substrate will be picked, it should not be present on arm.
            if (isPick)
            {
                return !substratePresence
                    ? string.Empty
                    : string.Format(CultureInfo.InvariantCulture, Messages.ArmAlreadyHaveMaterial, arm);
            }

            // Otherwise, it should be present.
            return substratePresence
                ? string.Empty
                : string.Format(CultureInfo.InvariantCulture, Messages.ArmHaveNoMaterial, arm);
        }
    }
}
