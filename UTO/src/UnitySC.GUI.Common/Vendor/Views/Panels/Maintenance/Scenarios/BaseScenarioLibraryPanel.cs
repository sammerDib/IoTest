using System;
using System.ComponentModel;
using System.Linq;

using Agileo.Common.Access;
using Agileo.Common.Access.Users;
using Agileo.Common.Localization;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.Recipes.Components;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.Scenarios;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.GroupSelector;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios
{
    public abstract class BaseScenarioLibraryPanel : BusinessPanel
    {
        public const string UnknownGroupName = "Unknown";
        private bool _lockSelectionChanged;

        static BaseScenarioLibraryPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ScenarioResources)));
        }

        protected BaseScenarioLibraryPanel(string relativeId, ScenarioManager scenarioManager, IIcon icon = null)
            : base(relativeId, icon)
        {
            ScenarioManager = scenarioManager;

            #region Sorts

            DataTableSource.Sort.Add(
                new SortDefinition<SequenceRecipe>(
                    $"{nameof(SequenceRecipe.Header.GroupName)}.{nameof(SequenceRecipe.Header.GroupName)}",
                    scenario => scenario.Header.GroupName));
            DataTableSource.Sort.Add(
                new SortDefinition<SequenceRecipe>(nameof(SequenceRecipe.Id), scenario => scenario.Id));
            DataTableSource.Sort.Add(
                new SortDefinition<SequenceRecipe>(nameof(SequenceRecipe.Header.Author), scenario => scenario.Header.Author));
            DataTableSource.Sort.Add(
                new SortDefinition<SequenceRecipe>(
                    nameof(SequenceRecipe.Header.CreateDate),
                    scenario => scenario.Header.CreateDate));
            DataTableSource.Sort.Add(
                new SortDefinition<SequenceRecipe>(
                    nameof(SequenceRecipe.Header.ModificationDate),
                    scenario => scenario.Header.ModificationDate));
            DataTableSource.Sort.SetCurrentSorting(nameof(SequenceRecipe.Id), ListSortDirection.Ascending);

            #endregion

            DataTableSource.Search.AddSearchDefinition(new InvariantText("Name"), component => component.Id);

            GroupSelector = new GroupSelector<RecipeGroup>(
                () =>
                {
                    var groups = ScenarioManager.RecipeGroups.Where(grp => IsAccessible(grp.Name)).ToList();
                    var unknownGroupNeeded = ScenarioManager.Recipes.Values.Any(recipe => !GroupExists(recipe));
                    if (unknownGroupNeeded)
                    {
                        groups.Add(new RecipeGroup(UnknownGroupName, AccessLevel.Visibility));
                    }

                    return groups;
                },
                RefreshScenariosList,
                group => group.Name);

            RefreshAll();
        }

        #region Properties

        public GroupSelector<RecipeGroup> GroupSelector { get; }

        protected IAccessManager AccessManager => App.Instance.AccessRights;

        protected ScenarioManager ScenarioManager { get; }

        public DataTableSource<SequenceRecipe> DataTableSource { get; } = new DataTableSource<SequenceRecipe>();

        private SequenceRecipe _selectedScenario;

        public SequenceRecipe SelectedScenario
        {
            get { return _selectedScenario; }
            set
            {
                if (_lockSelectionChanged)
                {
                    return;
                }


                if (SetAndRaiseIfChanged(ref _selectedScenario, value))
                {
                    OnSelectedScenarioChanged();
                }
            }
        }

        private bool _detailsIsExpanded;

        public bool DetailsIsExpanded
        {
            get { return _detailsIsExpanded; }
            set { SetAndRaiseIfChanged(ref _detailsIsExpanded, value); }
        }

        #endregion

        #region Privates

        private bool GroupExists(SequenceRecipe scenario)
        {
            var groupName = scenario.Header.GroupName;
            return ScenarioManager.RecipeGroups.Any(group => group.Name == groupName);
        }

        protected virtual void OnSelectedScenarioChanged()
        {
        }

        private bool GroupIsSelected(SequenceRecipe scenario)
        {
            if (!GroupExists(scenario))
            {
                return GroupSelector.SelectedGroups.Any(group => group.Name == UnknownGroupName);
            }
            return GroupSelector.SelectedGroups.Any(group => group.Name == scenario.Header.GroupName);
        }

        protected bool IsAccessible(string groupName)
        {
            if (IsInDesignMode) return true;
            if (string.IsNullOrWhiteSpace(groupName)) return false;

            var scenarioGroupsConfig = App.Instance.Config.ScenarioConfiguration.Groups;
            if (scenarioGroupsConfig == null) return false;

            var scenarioAccess = groupName.Equals(UnknownGroupName)
                ? AccessLevel.Visibility
                : scenarioGroupsConfig.FirstOrDefault(grp => grp.Name.Equals(groupName))?.AccessLevel ?? AccessLevel.Visibility;

            var currentUserAccess = AccessManager.CurrentUser.AccessLevel;
            return currentUserAccess >= scenarioAccess;
        }

        protected bool IsAccessible(SequenceRecipe scenario) => IsAccessible(scenario?.Header?.GroupName);

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

        protected void RefreshScenariosList()
        {
            DispatcherHelper.DoInUiThreadAsynchronously(() =>
            {
                DataTableSource.Reset(ScenarioManager.Recipes.Values.Where(GroupIsSelected));
                if (SelectedScenario == null)
                {
                    SelectedScenario = DataTableSource.FirstOrDefault();
                }
            });
        }

        private void UserChanged(User user, UserEventArgs e)
        {
            RefreshAll();
        }

        private void ScenarioManager_OnGroupsChanged(object sender, EventArgs e)
        {
            RefreshAll();
        }

        #endregion

        #region Overrides of IdentifiableElement

        public override void OnSetup()
        {
            AccessManager.UserLogoff += UserChanged;
            AccessManager.UserLogon += UserChanged;
            ScenarioManager.OnGroupsChanged += ScenarioManager_OnGroupsChanged;

            base.OnSetup();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                AccessManager.UserLogoff -= UserChanged;
                AccessManager.UserLogon -= UserChanged;
                ScenarioManager.OnGroupsChanged -= ScenarioManager_OnGroupsChanged;
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
