using System.Linq;

using Agileo.Common.Localization;
using Agileo.Recipes.Components;
using Agileo.Recipes.Components.Services;

using UnitySC.GUI.Common.Vendor.Recipes.Resources;

namespace UnitySC.GUI.Common.Vendor.Recipes.ValidationRules
{
    public class RecipeNameMustBeUniqueValidationRule : IRecipeComponentValidationRule
    {
        public string Validate(RecipeComponent recipeComponent)
        {
            var recipeManager = App.Instance.RecipeManager;
            if (recipeManager == null) return string.Empty;

            var recipesNotInEdition = recipeManager.Recipes.Where(r => !r.Value.IsInEdition).Select(p => p.Key);

            if (recipesNotInEdition.ToList().Exists(recipeId => recipeId == recipeComponent.Id))
            {
                return LocalizationManager.GetString(nameof(RecipeValidationResources.RECIPE_ALREADY_EXISTS), recipeComponent.Id);
            }

            return string.Empty;
        }
    }
}
