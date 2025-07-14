using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    public class IsCarrierUndocked : CSharpCommandConditionBehavior
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

            if (loadPort.IsDocked)
            {
                context.AddContextError(Messages.CarrierMustBeUndocked);
            }
        }
    }
}
