using System.Linq;

using Agileo.Common.Localization;
using Agileo.Recipes.Components;
using Agileo.Recipes.Components.Services;

using UnitySC.GUI.Common.Vendor.Recipes.Resources;

namespace UnitySC.GUI.Common.Vendor.Recipes.ValidationRules
{
    public class ScenarioNameMustBeUniqueValidationRule : IRecipeComponentValidationRule
    {
        public string Validate(RecipeComponent recipeComponent)
        {
            var scenarioManager = App.Instance.ScenarioManager;
            if (scenarioManager == null) return string.Empty;

            var scenariosNotInEdition = scenarioManager.Recipes.Where(r => !r.Value.IsInEdition).Select(p => p.Key);

            if (scenariosNotInEdition.ToList().Exists(scenarioId => scenarioId == recipeComponent.Id))
            {
                return LocalizationManager.GetString(nameof(RecipeValidationResources.SCENARIO_ALREADY_EXISTS), recipeComponent.Id);
            }

            return string.Empty;
        }
    }
}
