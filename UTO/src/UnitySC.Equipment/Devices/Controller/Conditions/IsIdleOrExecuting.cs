using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class IsIdleOrExecuting : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not Controller controller)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotAGenericDevice,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            //We only check this conditions if there is no activity in progress to allow low level commands to launched in Maintenance
            //If an activity is running, the device state will be Executing but low level commands will be launched by the activity
            if (controller.State != OperatingModes.Idle
                && controller.State != OperatingModes.Executing)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Messages.NotIdleOrExecuting,
                    controller.Name ?? "null"));
            }
        }
    }
}
