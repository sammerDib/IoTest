using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Conditions
{
    public class IsCarrierIdSupported : CSharpCommandConditionBehavior
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

            if (!loadPort.Configuration.IsCarrierIdSupported)
            {
                context.AddContextError(Messages.CarrierIdNotSupported);
            }
        }
    }
}
