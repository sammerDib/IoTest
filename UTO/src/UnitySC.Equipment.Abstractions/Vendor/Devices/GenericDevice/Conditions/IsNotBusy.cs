using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions
{
    public class IsNotBusy : CSharpCommandConditionBehavior
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

            if (genericDevice.State is OperatingModes.Executing or OperatingModes.Initialization)
            {
                context.AddContextError(Messages.AlreadyBusy);
            }
        }
    }
}
