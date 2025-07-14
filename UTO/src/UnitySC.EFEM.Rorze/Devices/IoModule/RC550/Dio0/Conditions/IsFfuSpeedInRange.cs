using Agileo.EquipmentModeling;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Resources;

using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Conditions
{
    public class IsFfuSpeedInRange : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (!context.Command.Name.Equals(nameof(IDio0.SetFfuSpeed)))
            {
                context.AddContextError(string.Format(
                    GenericDeviceMessages.CommandNotSupported,
                    context.Command.Name,
                    nameof(IsFfuSpeedInRange)));
                return;
            }

            var arg = context.GetArgument("setPoint");
            if (arg is not RotationalSpeed speed)
            {
                context.AddContextError(Messages.ArgumentNotARotationalSpeed);
                return;
            }

            if (speed.RevolutionsPerMinute is not 0 and (< 300 or > 1650))
            {
                context.AddContextError(Messages.RotationalSpeedNotInRange);
            }
        }
    }
}
