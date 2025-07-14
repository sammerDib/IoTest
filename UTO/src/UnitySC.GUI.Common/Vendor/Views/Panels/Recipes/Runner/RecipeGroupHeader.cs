using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Recipes.Runner
{
    public class RecipeGroupHeader
    {
        public RecipeGroupHeader(LocalizableText groupName)
        {
            GroupName = groupName;
        }

        public LocalizableText GroupName { get; }
    }
}
