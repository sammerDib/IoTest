using System.Globalization;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Conditions
{
    public class IsInUnclampPosition : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not LoadPort loadPort)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotALoadPort,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (loadPort.PhysicalState != LoadPortState.Unclamped
                && loadPort.PhysicalState != LoadPortState.Clamped
                && loadPort.PhysicalState != LoadPortState.Undocked)
            {
                context.AddContextError(Messages.CarrierNotInUnclampPosition);
            }
        }
    }
}
