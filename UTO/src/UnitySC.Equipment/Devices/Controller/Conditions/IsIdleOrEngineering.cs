using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class IsIdleOrEngineering : CSharpCommandConditionBehavior
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

            if (controller.State != OperatingModes.Idle
                && controller.State != OperatingModes.Engineering)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.NotEngineeringOrIdle,
                    controller.Name ?? "null"));
            }
        }
    }
}
