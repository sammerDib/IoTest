using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Resources;

using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;
using System.Globalization;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Enum;

namespace UnitySC.Equipment.Abstractions.Devices.Ffu.Conditions
{
    public class IsFfuSpeedValid : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not Ffu ffu)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotAFfu,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (!context.Command.Name.Equals(nameof(IFfu.SetFfuSpeed)))
            {
                context.AddContextError(string.Format(
                    GenericDeviceMessages.CommandNotSupported,
                    context.Command.Name,
                    nameof(IsFfuSpeedValid)));
                return;
            }

            var setPoint = (double)context.GetArgument("setPoint");
            var unit = (FfuSpeedUnit)context.GetArgument("unit");
            var errors = ffu.IsFfuSpeedValid(setPoint, unit);

            if (!string.IsNullOrEmpty(errors))
            {
                context.AddContextError(errors);
            }
        }
    }
}
