using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.ProcessingFramework;
using Agileo.ProcessingFramework.Instructions;
using Agileo.Recipes.Components;

using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions
{
    public static class RecipeExtensions
    {
        public static Program ToProgram(this RecipeComponent recipe, IEnumerable<RecipeComponent> availableRecipes)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            if (string.IsNullOrEmpty(recipe.Id))
            {
                throw new ArgumentException(@"Recipe Id is not defined", nameof(recipe));
            }

            var program = new Program(recipe.Id);

            program.Instructions.AddRange(GenerateProgramInstructions(recipe, availableRecipes));

            return program;
        }

        private static IEnumerable<Instruction> GenerateProgramInstructions(
            RecipeComponent recipe,
            IEnumerable<RecipeComponent> availableRecipes)
        {
            var instructions = new List<Instruction>();

            // Steps
            foreach (var recipeStep in recipe.Steps)
            {
                switch (recipeStep)
                {
                    case PreProcess:
                        instructions.Add(
                            new StepCrossingInstruction<RecipeSteps>(RecipeSteps.PreProcess)
                            {
                                ExecutorId = recipeStep.Id, Modifier = ExecutionModifier.Sync
                            });
                        break;

                    case Process:
                        instructions.Add(
                            new StepCrossingInstruction<RecipeSteps>(RecipeSteps.Process)
                            {
                                ExecutorId = recipeStep.Id, Modifier = ExecutionModifier.Sync
                            });
                        break;

                    case PostProcess:
                        instructions.Add(
                            new StepCrossingInstruction<RecipeSteps>(RecipeSteps.PostProcess)
                            {
                                ExecutorId = recipeStep.Id, Modifier = ExecutionModifier.Sync
                            });
                        break;

                    default:
                        instructions.Add(
                            new StepCrossingInstruction<RecipeSteps>(RecipeSteps.Undefined)
                            {
                                ExecutorId = recipeStep.Id, Modifier = ExecutionModifier.Sync
                            });
                        break;
                }

                instructions.AddRange(
                    recipeStep.Instructions.OfType<BaseRecipeInstruction>()
                        .Select(instruction => instruction.ToProcessingInstruction()));
            }

            // Instructions
            instructions.AddRange(
                recipe.Instructions.OfType<BaseRecipeInstruction>()
                    .Select(instruction => instruction.ToProcessingInstruction()));

            foreach (var subRecipe in recipe.SubRecipes)
            {
                var foundRecipe =
                    availableRecipes.FirstOrDefault(r => r.Id.Equals(subRecipe.RecipeId));

                if (foundRecipe != null)
                {
                    instructions.AddRange(
                        GenerateProgramInstructions(foundRecipe, availableRecipes));
                }
            }

            return instructions;
        }
    }
}
