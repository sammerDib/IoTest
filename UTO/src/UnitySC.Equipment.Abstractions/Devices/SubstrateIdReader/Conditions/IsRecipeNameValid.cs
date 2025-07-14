using System.Globalization;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Conditions
{
    public class IsRecipeNameValid : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not SubstrateIdReader reader)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.DeviceNotASubstrateIdReader,
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            var recipeNameArg = context.GetArgument("recipeName");
            if (recipeNameArg is not string recipeName)
            {
                context.AddContextError(Messages.ArgumentNotAnRecipeName);
                return;
            }

            if (!reader.Recipes.Any(r=>r.Name == recipeName && r.IsStored))
            {
                context.AddContextError(string.Format(CultureInfo.InvariantCulture, Messages.RecipeNameIsInvalid, recipeName));
                return;
            }
        }
    }
}
