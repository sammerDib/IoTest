using System;

using Agileo.EquipmentModeling;
using Agileo.Recipes.Components;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.UserInteraction;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitProcessModuleStatusThreshold;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.UserInteraction;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.WaitProcessModuleStatusThreshold;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.WaitStatusThreshold;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions
{
    public static class RecipeInstructionViewModelFactory
    {
        public static IInstructionEditorViewModel Build<T>(T recipeInstruction,
            Func<DeviceCommand, bool> commandFilter = null) where T : RecipeInstruction
        {
            if (recipeInstruction is DeviceCommandInstruction deviceCommandInstruction)
            {
                return new DeviceCommandEditorViewModel(deviceCommandInstruction, commandFilter);
            }

            if (recipeInstruction is WaitProcessModuleStatusThresholdInstruction waitProcessModuleStatusThresholdInstruction)
            {
                return new WaitProcessModuleStatusThresholdInstructionEditor(waitProcessModuleStatusThresholdInstruction);
            }

            if (recipeInstruction is WaitStatusThresholdInstruction waitStatusThresholdInstruction)
            {
                return new WaitStatusThresholdInstructionEditor(waitStatusThresholdInstruction);
            }

            if (recipeInstruction is UserInteractionInstruction userInteractionInstruction)
            {
                return new UserInteractionEditor(userInteractionInstruction);
            }

            return new GenericRecipeInstructionViewModel<T>(recipeInstruction);
        }
    }

    public abstract class GenericRecipeInstructionViewModel : IInstructionEditorViewModel
    {
        public abstract RecipeInstruction Model { get; }
    }

    /// <summary>
    /// Represents a generic viewModel for editing instructions, it is used in the case where a view can be
    /// directly linked to the instruction model without requiring the behavior of a particular viewModel.
    /// </summary>
    /// <typeparam name="T">Type of instruction</typeparam>
    public class GenericRecipeInstructionViewModel<T> : GenericRecipeInstructionViewModel where T : RecipeInstruction
    {
        public override RecipeInstruction Model => TypedModel;

        public GenericRecipeInstructionViewModel(T model)
        {
            TypedModel = model;
        }

        public T TypedModel { get; }
    }
}
