using System.Collections.ObjectModel;
using System.Globalization;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Devices.Controller.Resources;

using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.Equipment.Devices.Controller
{
    public class CheckSourceSlotNotEmpty : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            var controller = context.Device as Controller;
            if (controller == null)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.InvalidDeviceType,
                    nameof(Controller),
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (!(context.GetArgument("loadPort") is ILoadPort))
            {
                context.AddContextError(Messages.IncorrectLoadPortSelected);
                return;
            }

            var expectedLoadPort = (Abstractions.Devices.LoadPort.LoadPort)context.GetArgument("loadPort");

            if (expectedLoadPort == null)
            {
                context.AddContextError(Messages.LoadPortNotAvailable);
                return;
            }

            if (expectedLoadPort.CarrierPresence != CassettePresence.Correctly || expectedLoadPort.Carrier == null)
            {
                context.AddContextError(Messages.CarrierNotCorrectlyPlaced);
                return;
            }

            var expectedSourceSlot = (byte)context.GetArgument("sourceSlot");

            if (expectedSourceSlot <= 0)
            {
                context.AddContextError(Messages.InvalidSourceSlot1);
                return;
            }

            if (expectedSourceSlot > expectedLoadPort.Carrier.Capacity)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.InvalidSourceSlot2,
                    expectedLoadPort.Carrier.Capacity));
                return;
            }

            Collection<SlotState> mappingTable;

            try
            {
                mappingTable = expectedLoadPort.Carrier.MappingTable;
            }
            catch
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.InvalidMapping,
                    expectedLoadPort.Name));
                return;
            }

            if (mappingTable[expectedSourceSlot - 1] != SlotState.HasWafer)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.SlotEmpty,
                    expectedSourceSlot));
            }
        }
    }
}
