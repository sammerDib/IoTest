using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Access;
using Agileo.Common.Localization;
using Agileo.Recipes.Components;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Vendor.Views.Panels.Recipes;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.FileConfiguration
{
    public class GroupsDefinitionEditorPopupContent : NotifyDataError
    {
        private readonly RecipeGroup _groupInEdition;
        private string _groupName;
        private AccessLevel _accessLevel;
        private ObservableCollection<AccessLevel> _availableAccessLevels;

        static GroupsDefinitionEditorPopupContent()
        {
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(FileConfigurationPanelResources)));
        }

        public GroupsDefinitionEditorPopupContent()
        {
            DispatcherHelper.ThrowExceptionIfIsNotInDesignMode();

            _groupInEdition = new RecipeGroup("Process", AccessLevel.Level4);
            AvailableAccessLevels = new ObservableCollection<AccessLevel>(
                new[]
                {
                    AccessLevel.Visibility,
                    AccessLevel.Level1,
                    AccessLevel.Level2,
                    AccessLevel.Level3,
                    AccessLevel.Level4
                });
            GroupName = _groupInEdition.Name;
            AccessLevel = _groupInEdition.AccessLevel;
            Rules.Add(new DelegateRule(nameof(GroupName), () => "Invalid group name"));
            ApplyRules();
        }

        public GroupsDefinitionEditorPopupContent(RecipeGroup groupInEdition, IEnumerable<RecipeGroup> existingGroups)
        {
            _groupInEdition =
                groupInEdition ?? throw new ArgumentNullException(nameof(groupInEdition));
            existingGroups ??= Enumerable.Empty<RecipeGroup>();

            UpdateAvailableAccessLevels();
            GroupName = groupInEdition.Name;
            AccessLevel = groupInEdition.AccessLevel;

            Rules.Add(
                new DelegateRule(nameof(GroupName),
                    () =>
                    {
                        // Check if group name is not empty
                        if (string.IsNullOrWhiteSpace(GroupName))
                        {
                            return LocalizationManager.GetString(
                                nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_EDITOR_NAME_CANNOT_BE_EMPTY));
                        }

                        // Check if group name is not equals to Unknown
                        if (string.Equals(
                                GroupName,
                                BaseRecipeLibraryPanel.UnknownGroupName,
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            return LocalizationManager.GetString(
                                nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_EDITOR_NAME_UNAVAILABLE));
                        }

                        // Check if group name is not already used
                        if (existingGroups.Any(
                                group => string.Equals(group.Name, GroupName, StringComparison.InvariantCultureIgnoreCase))
                            && !string.Equals(_groupInEdition.Name, GroupName))
                        {
                            return LocalizationManager.GetString(
                                nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_EDITOR_NAME_ALREADY_EXISTS));
                        }

                        return string.Empty;
                    }));
            ApplyRules();

            App.Instance.AccessRights.UserLogon += (_, _) => UpdateAvailableAccessLevels();
            App.Instance.AccessRights.UserLogoff += (_, _) => UpdateAvailableAccessLevels();
        }

        public string GroupName
        {
            get => _groupName;
            set => SetAndRaiseIfChanged(ref _groupName, value);
        }

        public AccessLevel AccessLevel
        {
            get => _accessLevel;
            set => SetAndRaiseIfChanged(ref _accessLevel, value);
        }

        public ObservableCollection<AccessLevel> AvailableAccessLevels
        {
            get => _availableAccessLevels;
            private set => SetAndRaiseIfChanged(ref _availableAccessLevels, value);
        }

        public bool CanSave
            => !HasErrors
               && (!string.Equals(GroupName, _groupInEdition.Name) || AccessLevel != _groupInEdition.AccessLevel);

        private void UpdateAvailableAccessLevels()
        {
            var currentUserAccessLevel = App.Instance.AccessRights.CurrentUser?.AccessLevel ?? AccessLevel.Visibility;
            AvailableAccessLevels = new ObservableCollection<AccessLevel>(
                Enum.GetValues(typeof(AccessLevel))
                    .Cast<AccessLevel>()
                    .Where(level => level <= currentUserAccessLevel));

            if (!AvailableAccessLevels.Contains(AccessLevel))
            {
                AccessLevel = AvailableAccessLevels.LastOrDefault();
            }
        }
    }
}
