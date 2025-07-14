using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Recipes.Components;
using Agileo.Recipes.Management;
using Agileo.Recipes.Management.StorageStrategies;

namespace UnitySC.GUI.Common.Vendor.Recipes
{
    public class RecipeManager : RecipeManager<ProcessRecipe>
    {
        public RecipeManager()
        {
        }

        public RecipeManager(RecipeStorageStrategy<ProcessRecipe> storageStrategy)
            : base(storageStrategy)
        {
        }

        public event EventHandler<EventArgs> OnRecipeGroupsChanged;

        public void UpdateGroups(IEnumerable<RecipeGroup> groups)
        {
            if (RecipeGroups == null)
            {
                RecipeGroups = groups.ToList();
            }
            else
            {
                RecipeGroups.Clear();
                RecipeGroups.AddRange(groups);
            }

            OnRecipeGroupsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
