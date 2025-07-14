using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Agileo.Common.Access;
using Agileo.Common.Access.Users;
using Agileo.GUI.Components;
using Agileo.Recipes.Components;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.GroupSelector;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Recipes
{
    public class BaseRecipeLibraryForDataTable : Notifier, IDisposable
    {
        public const string UnknownGroupName = "Unknown";

        #region Constructor

        public BaseRecipeLibraryForDataTable(RecipeManager recipeManager)
        {
            RecipeManager = recipeManager ?? throw new ArgumentNullException(nameof(recipeManager));

            Recipes = new DataTableSource<ProcessRecipe>();
            Recipes.Sort.SetCurrentSorting(nameof(ProcessRecipe.Id), ListSortDirection.Ascending);
            Recipes.Search.AddSearchDefinition(new InvariantText("Name"), component => component.Id);

            GroupSelector = new GroupSelector<RecipeGroup>(
                () =>
                {
                    var groups = RecipeManager.RecipeGroups.Where(grp => IsAccessible(grp.Name)).ToList();
                    var unknownGroupNeeded = RecipeManager.Recipes.Values.Any(recipe => !GroupExists(recipe));
                    if (unknownGroupNeeded)
                    {
                        groups.Add(new RecipeGroup(UnknownGroupName, AccessLevel.Visibility));
                    }

                    return groups;
                },
                RefreshRecipesList,
                group => group.Name);

            AccessManager.UserLogoff += UserChanged;
            AccessManager.UserLogon += UserChanged;
            RecipeManager.OnRecipeGroupsChanged += RecipeGroupsManager_OnRecipeGroupsChanged;

            // Refresh the GroupSelector, select the first group which refresh the recipe list.
            RefreshAll();
        }

        #endregion

        #region Properties

        internal RecipeManager RecipeManager { get; }
        protected IAccessManager AccessManager => App.Instance.AccessRights;

        public DataTableSource<ProcessRecipe> Recipes { get; }

        private ProcessRecipe _selectedRecipe;

        public ProcessRecipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedRecipe, value))
                {
                    OnSelectedRecipeChanged();
                }
            }
        }

        public GroupSelector<RecipeGroup> GroupSelector { get; }

        #endregion

        #region Methods

        protected void RefreshRecipesList()
            => Application.Current.Dispatcher?.Invoke(
                () =>
                {
                    Recipes.Reset(RecipeManager.Recipes.Values.Where(GroupIsSelected));
                    if (SelectedRecipe == null)
                    {
                        SelectedRecipe = Recipes.FirstOrDefault();
                    }
                });

        protected virtual void OnSelectedRecipeChanged()
        {
        }

        protected bool IsAccessible(string groupName)
        {
            if (IsInDesignMode)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(groupName))
            {
                return false;
            }

            var recipeGroupsConfig = App.Instance.Config.RecipeConfiguration.Groups;
            if (recipeGroupsConfig == null)
            {
                return false;
            }

            var recipeAccess = groupName.Equals(UnknownGroupName)
                ? AccessLevel.Visibility
                : recipeGroupsConfig.FirstOrDefault(grp => grp.Name.Equals(groupName))?.AccessLevel
                  ?? AccessLevel.Visibility;

            var currentUserAccess = AccessManager.CurrentUser.AccessLevel;
            return currentUserAccess >= recipeAccess;
        }

        protected void RefreshAll()
        {
            DispatcherHelper.DoInUiThreadAsynchronously(
                () =>
                {
                    GroupSelector.Refresh();

                    if (GroupSelector.SelectedGroups.Count != 0)
                    {
                        return;
                    }

                    var firstGroup = GroupSelector.Groups.FirstOrDefault();
                    if (firstGroup != null)
                    {
                        GroupSelector.Select(firstGroup);
                    }
                });
        }

        #endregion

        #region Private methods

        private bool GroupIsSelected(ProcessRecipe recipe)
        {
            if (!GroupExists(recipe))
            {
                return GroupSelector.SelectedGroups.Any(group => group.Name == UnknownGroupName);
            }

            return GroupSelector.SelectedGroups.Any(group => group.Name == recipe.Header.GroupName);
        }

        private bool GroupExists(ProcessRecipe recipe)
        {
            var groupName = recipe.Header.GroupName;

            return RecipeManager.RecipeGroups.Any(group => group.Name == groupName);
        }

        private void UserChanged(User user, UserEventArgs e) => RefreshAll();

        private void RecipeGroupsManager_OnRecipeGroupsChanged(object sender, EventArgs e) => RefreshAll();

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            AccessManager.UserLogoff -= UserChanged;
            AccessManager.UserLogon -= UserChanged;
            RecipeManager.OnRecipeGroupsChanged -= RecipeGroupsManager_OnRecipeGroupsChanged;
        }

        #endregion
    }
}
