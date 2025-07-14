using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Agileo.Common.Localization;
using Agileo.Recipes.Components;
using Agileo.Recipes.Components.Services;
using Agileo.Recipes.Components.Validation;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.UserInformation;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.UserInteraction;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.Wait;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;
using UnitySC.GUI.Common.Vendor.Recipes.Resources;
using UnitySC.GUI.Common.Vendor.Recipes.ValidationRules;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios;

namespace UnitySC.GUI.Common.Vendor.Recipes
{
    [Serializable]
    [XmlInclude(typeof(PreProcess))]
    [XmlInclude(typeof(Process))]
    [XmlInclude(typeof(PostProcess))]
    [XmlInclude(typeof(DeviceCommandInstruction))]
    [XmlInclude(typeof(WaitStatusThresholdInstruction))]
    [XmlInclude(typeof(UserInformationInstruction))]
    [XmlInclude(typeof(UserInteractionInstruction))]
    [XmlInclude(typeof(WaitForTimeInstruction))]
    [XmlRoot(nameof(RecipeComponent))]
    public class ProcessRecipe : RecipeComponent
    {
        static ProcessRecipe()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(RecipeValidationResources)));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ScenarioResources)));
        }

        public ProcessRecipe()
        { }

        private ProcessRecipe(RecipeComponent other)
            : base(other)
        { }

        public override object Clone()
        {
            return new ProcessRecipe(this);
        }

        public override List<RecipeInstruction> DefineDefaultInstructions()
        {
            return new List<RecipeInstruction>();
        }

        public override List<IRecipeComponentValidationRule> DefineValidationRules()
        {
            return new List<IRecipeComponentValidationRule>
            {
                new RecipeNameMustBeUniqueValidationRule(),
                new RecipeComponentValidationRule(),
            };
        }

        public override List<RecipeStep> DefineDefaultSteps()
        {
            return new List<RecipeStep>();
        }

        public override List<RecipeReference> DefineDefaultSubRecipes()
        {
            return new List<RecipeReference>();
        }
    }
}
