using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions
{
    public class IsCurrentActivityNull : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not GenericDevice genericDevice)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotAGenericDevice,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (genericDevice.CurrentActivity != null)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.ActivityAlreadyRunning,
                    genericDevice.CurrentActivity.Id));
            }
        }
    }
}
