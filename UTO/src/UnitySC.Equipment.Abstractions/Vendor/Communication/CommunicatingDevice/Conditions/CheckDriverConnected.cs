using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice.Resources;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice.Conditions
{
    public class CheckDriverConnected : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not CommunicatingDevice communicatingDevice)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotACommunicatingDevice,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (!communicatingDevice.IsConnected)
            {
                context.AddContextError(Messages.CommunicationNotEnabled);
            }
        }
    }
}
