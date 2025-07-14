using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Devices.Controller.Resources;

namespace UnitySC.Equipment.Devices.Controller
{
    public class CheckProcessModuleEmpty : CSharpCommandConditionBehavior
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

            if (context.GetArgument("processModule") is not Abstractions.Devices.ProcessModule.ProcessModule
                expectedProcessModule)
            {
                context.AddContextError(Messages.IncorrectProcessModuleSelected);
                return;
            }

            if (expectedProcessModule.Location.Material != null)
            {
                context.AddContextError(Messages.ProcessModuleNotEmpty);
            }
        }
    }
}
