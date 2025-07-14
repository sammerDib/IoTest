using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.Resources;
using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.Conditions
{
    public class IsRecipeRunning : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not DriveableProcessModule processModule)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotAProcessModule,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (processModule.ProcessModuleState is not ProcessModuleState.Active and not ProcessModuleState.Error)
            {
                context.AddContextError(Messages.ProcessModuleStateInvalid);
            }
        }
    }
}
