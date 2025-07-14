using Agileo.Recipes.Components;

using Humanizer;

namespace UnitySC.GUI.Common.Vendor.Recipes
{
    public class PreProcess : RecipeStep
    {
        public PreProcess() : base(nameof(PreProcess).Humanize())
        {
        }

        protected override RecipeStep CloneStep()
        {
            return new PreProcess();
        }
    }

    public class Process : RecipeStep
    {
        public Process() : base(nameof(Process).Humanize())
        {
        }

        protected override RecipeStep CloneStep()
        {
            return new Process();
        }
    }

    public class PostProcess : RecipeStep
    {
        public PostProcess() : base(nameof(PostProcess).Humanize())
        {
        }

        protected override RecipeStep CloneStep()
        {
            return new PostProcess();
        }
    }
}
