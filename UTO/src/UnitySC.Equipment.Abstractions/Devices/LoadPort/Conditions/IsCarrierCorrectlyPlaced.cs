using System.Globalization;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Conditions
{
    public class IsCarrierCorrectlyPlaced : CSharpCommandConditionBehavior
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

            if (loadPort.Carrier == null || loadPort.CarrierPresence != CassettePresence.Correctly)
            {
                context.AddContextError(Messages.CarrierNotCorrectlyPlaced);
            }
        }
    }
}
