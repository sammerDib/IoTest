using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Agileo.Common.Access;
using Agileo.Common.Access.Users;
using Agileo.Common.Localization;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.Recipes.Components;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.GroupSelector;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Recipes
{
    public abstract class BaseRecipeLibraryPanel : BusinessPanel
    {
        public const string UnknownGroupName = "Unknown";
        private bool _lockSelectionChanged;

        #region Constructor

        static BaseRecipeLibraryPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(RecipePanelResources)));
        }

        protected BaseRecipeLibraryPanel()
            : this("RECIPE LIBRARY", new RecipeManager(), PathIcon.RecipeManager)
        {

        }

        protected BaseRecipeLibraryPanel(string relativeId, RecipeManager recipeManager, IIcon icon) : base(relativeId, icon)
        {
            if (IsInDesignMode)
            {
                return;
            }

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
                }, RefreshRecipesList, group => group.Name);

            // Refresh the GroupSelector, select the first group which refresh the recipe list.
            RefreshAll();
        }

        #endregion

        #region Properties

        #region Commons

        protected IAccessManager AccessManager => App.Instance.AccessRights;

        #endregion

        internal RecipeManager RecipeManager { get; }

        public GroupSelector<RecipeGroup> GroupSelector { get; }

        public DataTableSource<ProcessRecipe> Recipes { get; }

        private ProcessRecipe _selectedRecipe;

        public ProcessRecipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (_lockSelectionChanged)
                {
                    return;
                }

                if (SetAndRaiseIfChanged(ref _selectedRecipe, value))
                {
                    OnSelectedRecipeChanged();
                }
            }
        }

        private bool _detailsIsExpanded;

        public bool DetailsIsExpanded
        {
            get => _detailsIsExpanded;
            set => SetAndRaiseIfChanged(ref _detailsIsExpanded, value);
        }

        #endregion

        #region Methods

        #region Private/Protected
        protected virtual void OnSelectedRecipeChanged()
        {
        }

        protected void RefreshRecipesList()
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                Recipes.Reset(RecipeManager.Recipes.Values.Where(GroupIsSelected));
                if (SelectedRecipe == null)
                {
                    SelectedRecipe = Recipes.FirstOrDefault();
                }
            });
        }

        protected bool IsAccessible(string groupName)
        {
            if (IsInDesignMode) return true;
            if (string.IsNullOrWhiteSpace(groupName)) return false;

            var recipeGroupsConfig = App.Instance.Config.RecipeConfiguration.Groups;
            if (recipeGroupsConfig == null) return false;

            var recipeAccess = groupName.Equals(UnknownGroupName)
                ? AccessLevel.Visibility
                : recipeGroupsConfig.FirstOrDefault(grp => grp.Name.Equals(groupName))?.AccessLevel ?? AccessLevel.Visibility;

            var currentUserAccess = AccessManager.CurrentUser.AccessLevel;
            return currentUserAccess >= recipeAccess;
        }

        protected bool IsAccessible(ProcessRecipe recipe) => IsAccessible(recipe?.Header?.GroupName);

        private void RecipeGroupsManager_OnRecipeGroupsChanged(object sender, EventArgs e)
        {
            RefreshAll();
        }

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
        protected void RefreshAll()
        {
            _lockSelectionChanged = false;
            DispatcherHelper.DoInUiThreadAsynchronously(() =>
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

        private void UserChanged(User user, UserEventArgs e)
        {
            RefreshAll();
        }

        #endregion

        #endregion

        #region Overrides of IdentifiableElement

        public override void OnSetup()
        {
            AccessManager.UserLogoff += UserChanged;
            AccessManager.UserLogon += UserChanged;
            RecipeManager.OnRecipeGroupsChanged += RecipeGroupsManager_OnRecipeGroupsChanged;

            base.OnSetup();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                AccessManager.UserLogoff -= UserChanged;
                AccessManager.UserLogon -= UserChanged;
                RecipeManager.OnRecipeGroupsChanged -= RecipeGroupsManager_OnRecipeGroupsChanged;
            }
            base.Dispose(disposing);
        }

        #region Overrides of BusinessPanel

        public override void OnShow()
        {
            _lockSelectionChanged = false;
            base.OnShow();
        }

        public override void OnHide()
        {
            _lockSelectionChanged = true;
            base.OnHide();
        }

        #endregion

        #endregion
    }
}
