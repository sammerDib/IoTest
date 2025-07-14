using System;
using System.IO;
using System.Linq;
using System.Windows;

using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Recipes.Management.StorageStrategies;

using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.Scenarios;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Library
{
    public class ScenarioLibraryPanel : BaseScenarioLibraryPanel
    {
        private const string ScenarioLibraryTracerName = "Scenario Library";
        private readonly ILogger _logger;

        #region Constructors

        public ScenarioLibraryPanel()
            : this(nameof(L10N.BP_SCENARIO_LIBRARY), new ScenarioManager(), PathIcon.RecipeManager)
        {
        }

        /// <param name="relativeId"></param>
        /// <param name="scenarioManager"></param>
        /// <param name="icon"></param>
        /// <inheritdoc />
        public ScenarioLibraryPanel(string relativeId, ScenarioManager scenarioManager, IIcon icon)
            : base(relativeId, scenarioManager, icon)
        {
            _logger = App.Instance.GetLogger(ScenarioLibraryTracerName);

            // Invisible commands
            AddScenarioCommand = new InvisibleBusinessPanelCommand(
                nameof(ScenarioResources.SCENARIO_ADD),
                new DelegateCommand(AddScenario, () => ScenarioManager.RecipeGroups.Any(grp => IsAccessible(grp.Name))),
                PathIcon.Add);
            Commands.Add(AddScenarioCommand);

            EditScenarioCommand = new InvisibleBusinessPanelCommand(
                nameof(ScenarioResources.SCENARIO_EDIT),
                new DelegateCommand(DisplayScenarioEditor, () => IsAccessible(SelectedScenario)),
                PathIcon.Edit);
            Commands.Add(EditScenarioCommand);

            DeleteScenarioCommand = new InvisibleBusinessPanelCommand(
                nameof(ScenarioResources.SCENARIO_DELETE),
                new DelegateCommand(DeleteScenario, () => IsAccessible(SelectedScenario)),
                PathIcon.Delete);
            Commands.Add(DeleteScenarioCommand);

            DuplicateScenarioCommand = new InvisibleBusinessPanelCommand(
                nameof(ScenarioResources.SCENARIO_DUPLICATE),
                new DelegateCommand(CloneScenario, () => IsAccessible(SelectedScenario)),
                PathIcon.Duplicate);
            Commands.Add(DuplicateScenarioCommand);

            // Edition commands
            SaveScenarioCommand = new BusinessPanelCommand(
                nameof(SetupPanelResources.SETUP_SAVE_CONFIG_COMMAND),
                new DelegateCommand(SaveScenarioCommandExecute, SaveScenarioCommandCanExecute),
                PathIcon.Save) { IsVisible = false };
            Commands.Add(SaveScenarioCommand);

            CancelCommand = new BusinessPanelCommand(
                nameof(SetupPanelResources.SETUP_UNDO_CHANGES_COMMAND),
                new DelegateCommand(CancelScenarioEditionExecute),
                PathIcon.Undo) { IsVisible = false };
            Commands.Add(CancelCommand);

            DisplayScenarioDetails();
        }

        public InvisibleBusinessPanelCommand AddScenarioCommand { get; }

        public InvisibleBusinessPanelCommand EditScenarioCommand { get; }

        public InvisibleBusinessPanelCommand DeleteScenarioCommand { get; }

        public InvisibleBusinessPanelCommand DuplicateScenarioCommand { get; }

        public BusinessPanelCommand CancelCommand { get; }

        public BusinessPanelCommand SaveScenarioCommand { get; }

        #endregion Constructors

        #region Properties

        private ScenarioDetailsViewModel _scenarioDetailsViewModel;

        public ScenarioDetailsViewModel ScenarioDetailsViewModel
        {
            get { return _scenarioDetailsViewModel; }
            set
            {
                _scenarioDetailsViewModel?.Dispose();
                _scenarioDetailsViewModel = value;
                OnPropertyChanged();
                UpdateCommandButtonVisibility();
            }
        }

        public bool IsEditing => ScenarioDetailsViewModel != null && ScenarioDetailsViewModel.IsEditing;

        #endregion

        #region Commands

        #region AddCommand

        private void AddScenario()
        {
            var newScenarioName = NamingStrategy.GetIncrementedName(
                LocalizationManager.GetString(nameof(ScenarioResources.SCENARIO_NEW_SCENARIO)),
                ScenarioManager.Recipes.Keys);
            var newScenario = (SequenceRecipe)ScenarioManager.Create(newScenarioName);

            // Associate the first selected group (or the first accessible) to the scenario
            var firstGroup = GroupSelector.SelectedGroups.FirstOrDefault()
                             ?? ScenarioManager.RecipeGroups.FirstOrDefault(grp => IsAccessible(grp.Name));
            if (GroupSelector.SelectedGroups.Count == 0)
            {
                GroupSelector.Select(firstGroup);
            }

            newScenario.Header.GroupName = firstGroup?.Name;
            newScenario.IsInEdition = true;

            SaveScenario(newScenario, true);
            SelectScenario(newScenarioName, true);
            DisplayScenarioEditor(true);
        }

        #endregion

        #region DeleteCommand

        private void DeleteScenario()
        {
            if (!DataTableSource.Contains(SelectedScenario)) return;

            var popup =
                new Popup(new LocalizableText(nameof(ScenarioResources.SCENARIO_DELETE_CONFIRMATION_TITLE),
                    SelectedScenario.Id))
                {
                    Commands =
                    {
                        new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                        new PopupCommand(nameof(ScenarioResources.SCENARIO_DELETE), new DelegateCommand(() =>
                        {
                            try
                            {
                                Application.Current.Dispatcher?.Invoke(delegate
                                {
                                    var selectedScenario = SelectedScenario;

                                    ScenarioManager.Delete(selectedScenario);
                                    DataTableSource.Remove(selectedScenario);

                                    Messages.Show(new UserMessage(MessageLevel.Success,
                                        new LocalizableText(nameof(ScenarioResources.SCENARIO_DELETED),
                                            selectedScenario.Id)) {SecondsDuration = 5});

                                    _logger.Info(
                                        "Scenario '{ScenarioId}' has been deleted by {UserName}.",
                                        selectedScenario.Id,
                                        AccessManager.CurrentUser?.Name);
                                });
                            }
                            catch (Exception e)
                            {
                                Messages.Show(new UserMessage(MessageLevel.Error,
                                    nameof(ScenarioResources.SCENARIO_ERROR_DELETION)) {CanUserCloseMessage = true});
                                _logger.Error(e, "An exception occurred while deleting the scenario '{ScenarioId}'", SelectedScenario.Id);
                            }
                        }))
                    }
                };
            Popups.Show(popup);
        }

        #endregion

        #region CloneCommand

        private void CloneScenario()
        {
            var scenarioClone = ScenarioManager.Clone(SelectedScenario.Id);
            scenarioClone.Id = NamingStrategy.GetCloneName(scenarioClone.Id, ScenarioManager.Recipes.Keys);
            ScenarioManager.Save(scenarioClone);
            RefreshScenariosList();

            SelectScenario(scenarioClone.Id);
        }

        #endregion CloneCommand

        #region SaveCommand

        private bool SaveScenarioCommandCanExecute()
        {
            return ScenarioDetailsViewModel != null && ScenarioDetailsViewModel.CanBeSaved();
        }

        private void SaveScenarioCommandExecute()
        {
            var saved = SaveScenario(ScenarioDetailsViewModel.Scenario);
            if (!saved) return;

            try
            {
                var savedScenario = ScenarioDetailsViewModel.Scenario;
                DisplayScenarioDetails();

                if (ScenarioManager.StorageStrategy is OnDiskRecipeStorageStrategy<SequenceRecipe> storageStrategy)
                {
                    var filePath = Path.Combine(
                        ".",
                        "Configuration",
                        "Scenarios",
                        savedScenario.Id + "." + storageStrategy.FileExtension);
                    var userMessage = OpenFileDirectory.GetUserMessage(
                        new LocalizableText(nameof(ScenarioResources.SCENARIO_SAVED), savedScenario.Id), filePath);
                    Messages.Show(userMessage);
                }
            }
            catch (Exception e)
            {
                Messages.Show(new UserMessage(MessageLevel.Error,
                    new LocalizableText(nameof(ScenarioResources.SCENARIO_ERROR_SAVING)))
                { CanUserCloseMessage = true });

                _logger.Error(
                    e,
                    "An exception occurred after saving the scenario '{ScenarioId}'",
                    ScenarioDetailsViewModel.Scenario.Id);
            }
        }

        #endregion

        #region CancelCommand

        private void CancelScenarioEditionExecute()
        {
            if (!ScenarioDetailsViewModel.SaveIsRequired())
            {
                CancelScenarioEdition();
            }
            else
            {
                var popup =
                    new Popup(nameof(ScenarioResources.SCENARIO_DISCARD_CHANGES),
                        nameof(ScenarioResources.SCENARIO_DISCARD_CHANGES_DESCRIPTION))
                    {
                        Commands =
                        {
                            new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                            new PopupCommand(nameof(ScenarioResources.SCENARIO_DISCARD),
                                new DelegateCommand(CancelScenarioEdition))
                        }
                    };
                Popups.Show(popup);
            }
        }

        private void CancelScenarioEdition()
        {
            Messages.HideAll();

            var editingScenario = DataTableSource.SingleOrDefault(scenario => scenario.IsInEdition);
            if (editingScenario != null) editingScenario.IsInEdition = false;
            if (ScenarioDetailsViewModel.IsNew)
            {
                RefreshScenariosList();
                Messages.Show(new UserMessage(MessageLevel.Error,
                        new LocalizableText(nameof(ScenarioResources.SCENARIO_LIBRARY_CREATION_CANCELED)))
                    { CanUserCloseMessage = true });
            }
            DisplayScenarioDetails();
            DetailsIsExpanded = false;
        }

        #endregion

        #endregion Commands

        #region Private

        /// <summary>
        /// It is necessary to recover the instance from the scenarioManager
        /// because during a save, the instance of the SequenceRecipe changes.
        /// </summary>
        private void SelectScenario(string scenarioId, bool isNew = false)
        {
            if (isNew)
            {
                SelectedScenario = DataTableSource.Single(recipe => recipe.Id.Equals(scenarioId));
                return;
            }
            SelectedScenario = ScenarioManager.Recipes.Values.Single(recipe => recipe.Id.Equals(scenarioId));
        }

        #region Overrides of BaseScenarioLibraryPanel

        protected override void OnSelectedScenarioChanged()
        {
            if (!IsEditing)
            {
                DisplayScenarioDetails();
            }
        }

        #endregion

        private void DisplayScenarioDetails()
        {
            ScenarioDetailsViewModel = SelectedScenario != null
                ? new ScenarioDetailsViewModel(SelectedScenario, false, this)
                : null;
        }

        internal void DisplayScenarioEditor()
        {
            DisplayScenarioEditor(false);
        }

        internal void DisplayScenarioEditor(bool isNew)
        {

            SelectedScenario.IsInEdition = true;

            if (isNew)
            {
                ScenarioDetailsViewModel = new ScenarioDetailsViewModel(SelectedScenario, true, this, true);
                return;
            }

            // Set scenario IsInEdition property in order to enable scenario name validation during edition
            RefreshScenariosList();
            var clonedScenario = (SequenceRecipe)ScenarioManager.Clone(SelectedScenario.Id);
            ScenarioDetailsViewModel = new ScenarioDetailsViewModel(clonedScenario, true, this);
        }

        private bool SaveScenario(SequenceRecipe scenarioToSave, bool isNew = false)
        {
            Messages.HideAll();

            scenarioToSave.Header.ModificationDate = DateTime.Now;
            scenarioToSave.Header.Author = AccessManager.CurrentUser.Name;
            scenarioToSave.IsInEdition = false;

            var relatedDataTableScenario = DataTableSource.SingleOrDefault(scenario => scenario.IsInEdition);
            try
            {
                // Remove old scenario from system and disk
                if (relatedDataTableScenario != null && ScenarioManager.Contains(relatedDataTableScenario.Id))
                {
                    ScenarioManager.Delete(relatedDataTableScenario);
                    DataTableSource.Remove(relatedDataTableScenario);
                }

                // Add scenario on system and create file on disk
                if (!DataTableSource.Contains(scenarioToSave))
                {
                    DataTableSource.Add(scenarioToSave);
                }

                var saved = true;
                if (!isNew)
                {
                    saved = ScenarioManager.Save(scenarioToSave);
                    RefreshScenariosList();
                }

                SelectedScenario = null;
                SelectedScenario =
                    DataTableSource.SourceView.SingleOrDefault(scenario => scenario.Id == scenarioToSave.Id);

                if (!saved) throw new InvalidOperationException("Error during scenario saving.");
            }
            catch (Exception e)
            {
                Messages.Show(new UserMessage(MessageLevel.Error,
                    new LocalizableText(nameof(ScenarioResources.SCENARIO_ERROR_SAVING)))
                { CanUserCloseMessage = true });

                _logger.Error(e, "An exception occurred while saving the scenario '{ScenarioId}'", scenarioToSave.Id);
                return false;
            }

            return true;
        }

        private void UpdateCommandButtonVisibility()
        {
            // Ignore if the constructor has not finished his work.
            if (SaveScenarioCommand == null) return;

            if (ScenarioDetailsViewModel != null && ScenarioDetailsViewModel.IsEditing)
            {
                SaveScenarioCommand.IsVisible = true;
                CancelCommand.IsVisible = true;
                DetailsIsExpanded = true;
            }
            else
            {
                SaveScenarioCommand.IsVisible = false;
                CancelCommand.IsVisible = false;
            }

            OnPropertyChanged(nameof(IsEditing));
        }

        #endregion
    }
}
