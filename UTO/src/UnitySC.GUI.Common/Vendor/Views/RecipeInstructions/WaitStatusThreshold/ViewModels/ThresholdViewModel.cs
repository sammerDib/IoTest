using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.WaitStatusThreshold.ViewModels
{
    public abstract class ThresholdViewModel<T> : Notifier, IThresholdViewModel where T : Threshold
    {
        protected ThresholdViewModel(T model)
        {
            Model = model;
        }

        Threshold IThresholdViewModel.Model => Model;

        public T Model { get; }
    }
}
