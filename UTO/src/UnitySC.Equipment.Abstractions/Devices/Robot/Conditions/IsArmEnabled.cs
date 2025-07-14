using System.Globalization;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Robot.Resources;

using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;

namespace UnitySC.Equipment.Abstractions.Devices.Robot.Conditions
{
    public class IsArmEnabled : CSharpCommandConditionBehavior
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
                case nameof(IRobot.GoToSpecifiedLocation):
                case nameof(IRobot.GoToTransferLocation):
                case nameof(IRobot.ExtendArm):
                    {
                        var arg = context.GetArgument("arm");
                        if (arg is not RobotArm arm)
                        {
                            context.AddContextError(Messages.ArgumentNotAnArm);
                            return;
                        }

                        var error = CheckIfArmEnabled(robot, arm);
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

                        var error = CheckIfArmEnabled(robot, pickArm);
                        if (!string.IsNullOrEmpty(error))
                        {
                            context.AddContextError(error);
                        }

                        error = CheckIfArmEnabled(robot, placeArm);
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

                        var error = CheckIfArmEnabled(robot, pickArm);
                        if (!string.IsNullOrEmpty(error))
                        {
                            context.AddContextError(error);
                        }

                        var placeArm = pickArm == RobotArm.Arm1
                            ? RobotArm.Arm2
                            : RobotArm.Arm1;

                        error = CheckIfArmEnabled(robot, placeArm);
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
                        nameof(IsArmEnabled)));
                    break;
            }
        }

        private string CheckIfArmEnabled(Robot robot, RobotArm arm)
        {
            bool isArmEnabled;
            switch (arm)
            {
                case RobotArm.Arm1:
                    isArmEnabled = robot.Configuration.UpperArm.IsEnabled;
                    break;

                case RobotArm.Arm2:
                    isArmEnabled = robot.Configuration.LowerArm.IsEnabled;
                    break;

                default:
                    return string.Format(CultureInfo.InvariantCulture, Messages.ArmIsInvalid, arm);
            }

            return isArmEnabled
                ? string.Empty
                : string.Format(CultureInfo.InvariantCulture, Messages.ArmIsDisabled, arm);
        }
    }
}
