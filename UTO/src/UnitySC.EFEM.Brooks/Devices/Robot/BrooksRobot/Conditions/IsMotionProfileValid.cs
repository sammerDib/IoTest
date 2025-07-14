using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.Resources;

namespace UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.Conditions
{
    public class IsMotionProfileValid : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not BrooksRobot robot)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotABrooksRobot,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (!context.Command.Name.Equals(nameof(IBrooksRobot.SetMotionProfile)))
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.CommandNotSupported,
                    context.Command.Name,
                    nameof(IsMotionProfileValid)));
                return;
            }

            var arg = context.GetArgument("motionProfile").ToString();
            if (robot.MotionProfiles.Count == 0)
            {
                context.AddContextError(Messages.MotionProfilesEmpty);
                return;
            }

            if (!robot.MotionProfiles.Contains(arg))
            {
                context.AddContextError(Messages.UnknownMotionProfile);
            }
        }
    }
}
