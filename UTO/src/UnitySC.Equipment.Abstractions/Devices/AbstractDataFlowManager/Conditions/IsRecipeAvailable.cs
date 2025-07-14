using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.Conditions
{
    public class IsRecipeAvailable : CSharpCommandConditionBehavior
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

            if (context.Command.Name.Equals(nameof(IAbstractDataFlowManager.StartRecipe))
                && context.GetArgument("materialRecipe") is MaterialRecipe)
            {
                return;
            }

            context.AddContextError(Messages.RECIPE_NOT_SPECIFIED);
        }
    }
}
