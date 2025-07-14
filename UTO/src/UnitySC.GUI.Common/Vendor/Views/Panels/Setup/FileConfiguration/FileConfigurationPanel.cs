using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Access;
using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.Recipes.Components;
using Agileo.Recipes.Management.StorageFormats;

using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.FileConfiguration
{
    public class FileConfigurationPanel : SetupNodePanel<FilesBaseConfiguration>
    {

        #region Fields

        private readonly LocalizableText _xmlStorageFormat = new(nameof(FileConfigurationPanelResources.SETUP_FILE_CONFIGURATION_XML_FORMAT));
        private readonly LocalizableText _jsonStorageFormat = new(nameof(FileConfigurationPanelResources.SETUP_FILE_CONFIGURATION_JSON_FORMAT));

        #endregion

        #region Constructors

        static FileConfigurationPanel()
        {
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(FileConfigurationPanelResources)));
        }

        public FileConfigurationPanel()
            : this("Design Time Constructor")
        {
            FileConfigType = FileConfigurationType.Recipe;
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public FileConfigurationPanel(string relativeId, IIcon icon = null, FileConfigurationType fileConfigurationType = FileConfigurationType.Recipe)
            : base(relativeId, icon)
        {
            FileConfigType = fileConfigurationType;
            Groups.Filter.AddEnumFilter(
                nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_ACCESS_LEVEL),
                group => group.AccessLevel);
            Groups.Sort.SetCurrentSorting(nameof(RecipeGroup.Name), ListSortDirection.Ascending);

            AddGroupCommand = new InvisibleBusinessPanelCommand(
                FileConfigType == FileConfigurationType.Recipe
                    ? nameof(FileConfigurationPanelResources
                        .SETUP_GROUPS_DEFINITION_ADD_RECIPE_GROUP)
                    : nameof(FileConfigurationPanelResources
                        .SETUP_GROUPS_DEFINITION_ADD_SCENARIO_GROUP),
                new DelegateCommand(
                    () => ShowGroupEditor(new RecipeGroup(), ModifiedConfigNode.Groups)));
            Commands.Add(AddGroupCommand);

            DeleteGroupCommand = new InvisibleBusinessPanelCommand(
                FileConfigType == FileConfigurationType.Recipe
                    ? nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_DELETE_RECIPE_GROUP)
                : nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_DELETE_SCENARIO_GROUP),
                new DelegateCommand(DeleteGroupCommandExecute, DeleteGroupCommandCanExecute));
            Commands.Add(DeleteGroupCommand);

            EditGroupCommand = new InvisibleBusinessPanelCommand(
                FileConfigType == FileConfigurationType.Recipe
                    ? nameof(FileConfigurationPanelResources
                        .SETUP_GROUPS_DEFINITION_EDIT_RECIPE_GROUP)
                    : nameof(FileConfigurationPanelResources
                        .SETUP_GROUPS_DEFINITION_EDIT_SCENARIO_GROUP),
                new DelegateCommand(
                    () => ShowGroupEditor(SelectedGroup, ModifiedConfigNode.Groups),
                    () => SelectedGroup != null && GroupIsAccessible(SelectedGroup)));
            Commands.Add(EditGroupCommand);
        }

        #endregion Constructors

        #region Commands

        private DelegateCommand _defineFillPathCommand;
        public ICommand DefineFillPathCommand
            => _defineFillPathCommand ??= new DelegateCommand(DefineFillPathExecute);

        private void DefineFillPathExecute()
            => ShowOpenFolderDialog<FileConfigurationPanel>(p => p.FilePath);

        public InvisibleBusinessPanelCommand AddGroupCommand { get; }

        public InvisibleBusinessPanelCommand DeleteGroupCommand { get; }

        public InvisibleBusinessPanelCommand EditGroupCommand { get; }

        #region DeleteGroupCommand

        private bool DeleteGroupCommandCanExecute()
            => SelectedGroup != null && GroupIsAccessible(SelectedGroup);

        private void DeleteGroupCommandExecute()
        {
            var confirmationPopup = new Popup(
                nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_DELETE_GROUP_CONFIRMATION_TITLE),
                new LocalizableText(
                    nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_DELETE_GROUP_CONFIRMATION),
                    SelectedGroup.Name))
            { SeverityLevel = MessageLevel.Warning };

            confirmationPopup.Commands.Add(
                new PopupCommand(nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_CANCEL)));

            confirmationPopup.Commands.Add(
                new PopupCommand(
                    nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_DELETE_CONFIRMATION_DELETE),
                    new DelegateCommand(
                        () =>
                        {
                            ModifiedConfigNode.Groups?.Remove(SelectedGroup);
                            UpdateGroups();
                            SelectedGroup = null;
                        },
                        () => GroupIsAccessible(SelectedGroup))));

            Popups.Show(confirmationPopup);
        }

        #endregion DeletePPGroupCommand

        #endregion Commands

        #region Properties

        #region private
        private FileConfigurationType FileConfigType { get; }

        
        private FilesBaseConfiguration ModifiedConfiguration
        {
            get
            {
                return FileConfigType == FileConfigurationType.Recipe
                    ? ModifiedConfig.RecipeConfiguration
                    : ModifiedConfig.ScenarioConfiguration;
            }
        }
        #endregion

        #region Translate

        public string GroupsManagementTranslate
        {
            get
            {
                return FileConfigType == FileConfigurationType.Recipe
                    ? new LocalizableText(nameof(FileConfigurationPanelResources
                        .SETUP_GROUPS_DEFINITION_RECIPE_GROUPS_MANAGEMENT)).Value
                    : new LocalizableText(nameof(FileConfigurationPanelResources
                        .SETUP_GROUPS_DEFINITION_SCENARIO_GROUPS_MANAGEMENT)).Value;
            }
        }


        public string GroupsExplanationTranslate
        {
            get
            {
                return FileConfigType == FileConfigurationType.Recipe
                    ? new LocalizableText(nameof(FileConfigurationPanelResources
                        .SETUP_GROUPS_DEFINITION_RECIPE_GROUP_EXPLANATION)).Value
                    : new LocalizableText(nameof(FileConfigurationPanelResources
                        .SETUP_GROUPS_DEFINITION_SCENARIO_GROUPS_EXPLANATION)).Value;
            }
        }

        public string FileConfigurationTitleTranslate
        {
            get
            {
                return FileConfigType == FileConfigurationType.Recipe
                    ? new LocalizableText(nameof(FileConfigurationPanelResources
                        .SETUP_RECIPE_FILE_CONFIGURATION)).Value
                    : new LocalizableText(nameof(FileConfigurationPanelResources
                        .SETUP_SCENARIO_FILE_CONFIGURATION)).Value;
            }
        }
        #endregion

        public string FilePath
        {
            get => ModifiedConfiguration.Path;
            set
            {

                if (string.Equals(ModifiedConfiguration.Path, value, StringComparison.Ordinal))
                {
                    return;
                }

                ModifiedConfiguration.Path = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LocalizableText> StorageType
            => new() { _xmlStorageFormat, _jsonStorageFormat };

        public string SelectedStorageType
        {
            get => ModifiedConfiguration.StorageFormat == StorageFormat.XML ? nameof(FileConfigurationPanelResources.SETUP_FILE_CONFIGURATION_XML_FORMAT) : nameof(FileConfigurationPanelResources.SETUP_FILE_CONFIGURATION_JSON_FORMAT);

            set
            {
                var val = value == nameof(FileConfigurationPanelResources.SETUP_FILE_CONFIGURATION_XML_FORMAT)
                    ? StorageFormat.XML
                    : StorageFormat.JSON;


                if (ModifiedConfiguration.StorageFormat == val)
                {
                    return;
                }

                ModifiedConfiguration.StorageFormat = val;
                OnPropertyChanged();
            }
        }

        public string FileExtention
        {
            get => ModifiedConfiguration.FileExtension;
            set
            {

                if (string.Equals(ModifiedConfiguration.FileExtension, value, StringComparison.Ordinal))
                {
                    return;
                }

                ModifiedConfiguration.FileExtension = value;
                OnPropertyChanged();
            }
        }

        public DataTableSource<RecipeGroup> Groups { get; } = new();

        private void UpdateGroups()
            => Groups.Reset(ModifiedConfigNode.Groups ?? Enumerable.Empty<RecipeGroup>());

        private RecipeGroup _selectedGroup;

        public RecipeGroup SelectedGroup
        {
            get => _selectedGroup;
            set => SetAndRaiseIfChanged(ref _selectedGroup, value);
        }

        #endregion Properties

        #region Override

        protected override FilesBaseConfiguration GetNode(ApplicationConfiguration applicationConfiguration)
            => FileConfigType == FileConfigurationType.Recipe
                ? applicationConfiguration?.RecipeConfiguration :
                applicationConfiguration?.ScenarioConfiguration;

        public override void OnSetup()
        {
            base.OnSetup();
            UpdateGroups();
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();
            UpdateGroups();
        }

        protected override void SaveConfig()
        {
            base.SaveConfig();
            if (FileConfigType == FileConfigurationType.Recipe)
                App.Instance.RecipeManager.UpdateGroups(CurrentConfigNode.Groups);
            else
                App.Instance.ScenarioManager.UpdateGroups(CurrentConfigNode.Groups);
        }

        #endregion

        #region Private Methods

        private static bool GroupIsAccessible(RecipeGroup group)
        {
            if (group == null)
            {
                return false;
            }

            var currentUserAccessLevel = App.Instance.AccessRights.CurrentUser?.AccessLevel ?? AccessLevel.Visibility;
            return group.AccessLevel <= currentUserAccessLevel;
        }

        private void ShowGroupEditor(RecipeGroup editedGroup, ICollection<RecipeGroup> existingGroups)
        {
            if (editedGroup == null || existingGroups == null)
            {
                return;
            }

            var isCreation = !existingGroups.Contains(editedGroup);
            string popupTile;
            if (isCreation)
            {
                popupTile = FileConfigType == FileConfigurationType.Recipe
                    ? nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_RECIPE_CREATION_TITLE)
                    : nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_SCENARIO_CREATION_TITLE);
            }
            else
            {
                popupTile = FileConfigType == FileConfigurationType.Recipe
                    ? nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_RECIPE_EDITION_TITLE)
                    : nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_SCENARIO_EDITION_TITLE);
            }

            var popupContent = new GroupsDefinitionEditorPopupContent(editedGroup, existingGroups);
            var groupEditionPopup = new Popup(popupTile)
            {
                Content = popupContent,
                Commands =
                {
                    new PopupCommand(
                        nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_CANCEL)),
                    new PopupCommand(
                        isCreation
                            ? nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_CREATE)
                            : nameof(FileConfigurationPanelResources.SETUP_GROUPS_DEFINITION_EDIT),
                        new DelegateCommand(
                            () =>
                            {
                                if (isCreation)
                                {
                                    existingGroups.Add(editedGroup);
                                }

                                editedGroup.Name = popupContent.GroupName;
                                editedGroup.AccessLevel = popupContent.AccessLevel;
                                UpdateGroups();
                                SelectedGroup = Groups.FirstOrDefault(
                                    g => string.Equals(
                                        g.Name,
                                        editedGroup.Name,
                                        StringComparison.InvariantCultureIgnoreCase));
                            },
                            () => popupContent.CanSave && GroupIsAccessible(editedGroup)))
                }
            };

            Popups.Show(groupEditionPopup);
        }

        #endregion Private Methods
    }
}
