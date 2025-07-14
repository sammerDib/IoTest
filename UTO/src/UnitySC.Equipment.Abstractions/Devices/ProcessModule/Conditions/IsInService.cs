using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.ProcessModule.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.ProcessModule.Conditions
{
    public class IsInService : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not ProcessModule processModule)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotAProcessModule,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (processModule.IsOutOfService)
            {
                context.AddContextError(Messages.IsOutOfService);
            }
        }
    }
}
