using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Robot.Resources;

using UnitsNet;

using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;

namespace UnitySC.Equipment.Abstractions.Devices.Robot.Conditions
{
    public class IsSpeedValid : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (!context.Command.Name.Equals(nameof(IRobot.SetMotionSpeed)))
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    GenericDeviceMessages.CommandNotSupported,
                    context.Command.Name,
                    nameof(IsSpeedValid)));
                return;
            }

            var arg = context.GetArgument("percentage");
            if (arg is not Ratio percentage)
            {
                context.AddContextError(Messages.ArgumentNotARatio);
                return;
            }

            if (percentage.Percent is < 0 or > 100)
            {
                context.AddContextError(Messages.SpeedNotInRange);
            }
        }
    }
}
