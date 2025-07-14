using System;

using Agileo.GUI.Components;
using Agileo.Recipes.Components;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions
{
    public abstract class InstructionEditorViewModel<T> : Notifier, IInstructionEditorViewModel where T : RecipeInstruction
    {
        protected InstructionEditorViewModel(T model)
        {
            if (IsInDesignMode) return;
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        RecipeInstruction IInstructionEditorViewModel.Model => Model;

        public T Model { get; }
    }
}
