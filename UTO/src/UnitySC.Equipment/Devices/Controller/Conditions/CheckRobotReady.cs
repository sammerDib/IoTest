using System.Globalization;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Devices.Controller.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class CheckRobotReady : CSharpCommandConditionBehavior
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

            if (robot.State != OperatingModes.Idle)
            {
                context.AddContextError(Messages.RobotNotIdle);
            }
        }
    }
}
