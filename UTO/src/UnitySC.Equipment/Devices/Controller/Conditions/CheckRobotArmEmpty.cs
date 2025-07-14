using System.Globalization;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Devices.Controller.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class CheckRobotArmEmpty : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            var controller = context.Device as Controller;
            if (controller == null)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.InvalidDeviceType,
                    nameof(Controller),
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            var robot = controller.AllDevices<Abstractions.Devices.Robot.Robot>().FirstOrDefault();

            if (robot == null)
            {
                context.AddContextError(Messages.RobotNotAvailable);
                return;
            }

            var expectedRobotArm = (RobotArm)context.GetArgument("robotArm");

            switch (expectedRobotArm)
            {
                case RobotArm.Arm1:
                    if (robot.UpperArmLocation.Material != null)
                    {
                        context.AddContextError(Messages.RobotArm1NotEmpty);
                    }

                    break;
                case RobotArm.Arm2:
                    if (robot.LowerArmLocation.Material != null)
                    {
                        context.AddContextError(Messages.RobotArm2NotEmpty);
                    }

                    break;
                case RobotArm.Undefined:
                    context.AddContextError(Messages.RobotArmNotDefined);
                    break;
                default:
                    context.AddContextError(Messages.RobotArmNotDefined);
                    break;
            }
        }
    }
}
