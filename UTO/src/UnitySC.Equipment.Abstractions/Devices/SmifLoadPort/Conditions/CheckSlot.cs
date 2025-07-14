using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.SmifLoadPort
{
    public class CheckSlot : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not SmifLoadPort loadPort)
            {
                context.AddContextError(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.DeviceNotALoadPort,
                        context.Device?.GetType().Name ?? "null"));
                return;
            }

            var slot = (byte)context.GetArgument("slot");
            if (slot < 1 || slot > loadPort.Carrier?.Capacity)
            {
                context.AddContextError(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.InvalidSlotSelection,
                        loadPort.Carrier.Capacity));
            }
        }
    }
}
