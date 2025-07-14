using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.Recipes.Components;
using Agileo.Recipes.Management;
using Agileo.Recipes.Management.StorageStrategies;

using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.Recipes.Resources;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios;

namespace UnitySC.GUI.Common.Vendor.Scenarios
{
    public class ScenarioManager : RecipeManager<SequenceRecipe>
    {
        static ScenarioManager()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(RecipeValidationResources)));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ScenarioResources)));
        }

        public ScenarioManager()
        {
        }

        public ScenarioManager(RecipeStorageStrategy<SequenceRecipe> storageStrategy)
            : base(storageStrategy)
        {
        }

        public event EventHandler<EventArgs> OnGroupsChanged;

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

            OnGroupsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
