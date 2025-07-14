using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions
{
    public class IsMaintenanceOrIdle : CSharpCommandConditionBehavior
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

            //We only check this conditions if there is no activity in progress to allow low level commands to launched in Maintenance & Idle mode
            //If an activity is running, the device state will be Executing but low level commands will be launched by the activity
            if (genericDevice.State != OperatingModes.Maintenance
                && genericDevice.State != OperatingModes.Idle
                && genericDevice.CurrentActivity == null)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.NotMaintenanceOrIdle,
                    genericDevice.Name ?? "null"));
            }
        }
    }
}
