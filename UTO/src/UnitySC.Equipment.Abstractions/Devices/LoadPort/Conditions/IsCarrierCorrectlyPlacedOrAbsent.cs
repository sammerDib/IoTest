using System.Globalization;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    public class IsCarrierCorrectlyPlacedOrAbsent : CSharpCommandConditionBehavior
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

            if (loadPort.CarrierPresence != CassettePresence.Correctly
                && loadPort.CarrierPresence != CassettePresence.Absent)
            {
                context.AddContextError(Messages.CarrierMustBePlacedOrAbsent);
            }
        }
    }
}
