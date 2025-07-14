using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    public class IsManualCarrierTypeEnabled : CSharpCommandConditionBehavior
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

            if (!loadPort.Configuration.IsManualCarrierType)
            {
                context.AddContextError(Messages.ManualCarrierTypeNotEnabled);
            }
        }
    }
}
