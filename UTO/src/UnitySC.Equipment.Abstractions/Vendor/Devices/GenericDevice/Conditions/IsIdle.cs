using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions
{
    public class IsIdle : CSharpCommandConditionBehavior
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

            if (genericDevice.State != OperatingModes.Idle)
            {
                context.AddContextError(Messages.NotIdle);
            }
        }
    }
}
