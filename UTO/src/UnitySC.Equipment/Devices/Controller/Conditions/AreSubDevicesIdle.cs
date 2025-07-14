using System.Globalization;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.Equipment.Devices.Controller.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class AreSubDevicesIdle : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not Controller controller)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.InvalidDeviceType,
                    nameof(Controller),
                    context.Device?.GetType().Name ?? "null"));
                return;
            }



            foreach (var notIdleDevice in controller.GetDevices<Abstractions.Devices.DriveableProcessModule.DriveableProcessModule>()
                         .Where(d => d.ProcessModuleState != ProcessModuleState.Idle))
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotIdle,
                    notIdleDevice.Name));
            }

            foreach (var notIdleDevice in controller.GetDevices<GenericDevice>()
                         .Where(d => d.State != OperatingModes.Idle && d is IMaterialLocationContainer))
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotIdle,
                    notIdleDevice.Name));
            }
        }
    }
}
