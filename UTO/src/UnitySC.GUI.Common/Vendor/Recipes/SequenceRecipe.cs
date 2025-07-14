using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Agileo.Recipes.Components;
using Agileo.Recipes.Components.Services;
using Agileo.Recipes.Components.Validation;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.UserInformation;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.UserInteraction;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.Wait;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;
using UnitySC.GUI.Common.Vendor.Recipes.ValidationRules;

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
    public class SequenceRecipe : RecipeComponent
    {
        public SequenceRecipe()
        {
        }

        private SequenceRecipe(RecipeComponent other) : base(other)
        {
        }

        public override object Clone() => new SequenceRecipe(this);

        public override List<RecipeInstruction> DefineDefaultInstructions() => new List<RecipeInstruction>();

        public override List<IRecipeComponentValidationRule> DefineValidationRules()
        {
            return new List<IRecipeComponentValidationRule>
            {
                new ScenarioNameMustBeUniqueValidationRule(),
                new EquipmentMustContainDevicesValidationRule(),
                new RecipeComponentValidationRule()
            };
        }

        public override List<RecipeStep> DefineDefaultSteps() => new List<RecipeStep>();

        public override List<RecipeReference> DefineDefaultSubRecipes() => new List<RecipeReference>();
    }
}
