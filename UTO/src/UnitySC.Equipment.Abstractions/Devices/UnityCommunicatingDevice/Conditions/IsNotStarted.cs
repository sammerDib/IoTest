using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions
{
    public class IsNotStarted : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not UnityCommunicatingDevice communicatingDevice)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotACommunicatingDevice,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (communicatingDevice.IsCommunicationStarted)
            {
                context.AddContextError(Messages.CommunicationAlreadyStarted);
            }
        }
    }
}
