using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Devices.Controller.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class CheckProcessModuleReady : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not Controller)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.InvalidDeviceType,
                    nameof(Controller),
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (context.GetArgument("processModule") is not Abstractions.Devices.DriveableProcessModule.DriveableProcessModule
                expectedProcessModule)
            {
                context.AddContextError(Messages.IncorrectProcessModuleSelected);
                return;
            }

            if (expectedProcessModule.ProcessModuleState != ProcessModuleState.Idle)
            {
                context.AddContextError(Messages.ProcessModuleNotIdle);
            }
        }
    }
}
