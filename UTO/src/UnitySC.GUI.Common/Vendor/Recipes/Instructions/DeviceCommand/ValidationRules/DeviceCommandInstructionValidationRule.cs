using Agileo.Common.Localization;
using Agileo.Recipes.Components;
using Agileo.Recipes.Components.Services;

using UnitySC.GUI.Common.Vendor.Recipes.Resources;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.ValidationRules
{
    public class DeviceCommandInstructionValidationRule : IRecipeInstructionValidationRule
    {
        public string Validate(RecipeInstruction recipeInstruction)
        {
            var instruction = recipeInstruction as DeviceCommandInstruction;
            return string.IsNullOrWhiteSpace(instruction?.CommandName)
                ? LocalizationManager.GetString(nameof(RecipeValidationResources.SELECT_DEVICE_COMMAND))
                : string.Empty;
        }
    }
}
