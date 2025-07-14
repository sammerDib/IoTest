using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.Resources;
using UnitySC.Equipment.Abstractions.Material;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.Conditions
{
    public class WaferPresentInRunningRecipe : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not AbstractDataFlowManager)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.INVALID_DEVICE_TYPE,
                    nameof(AbstractDataFlowManager),
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (context.GetArgument("wafer") is not Wafer)
            {
                context.AddContextError(Messages.WAFER_NOT_SPECIFIED);
            }
        }
    }
}
