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

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Recipes.Library
{
    public class RecipeLibraryPanel : BaseRecipeLibraryPanel
    {
        private const string RecipeLibraryTracerName = "Recipe Library";
        private readonly ILogger _logger;

        public RecipeLibraryPanel(string relativeId, RecipeManager recipeManager, IIcon icon = null)
            : base(relativeId, recipeManager, icon)
        {
            _logger = App.Instance.GetLogger(RecipeLibraryTracerName);

            // Invisible commands
            AddRecipeCommand = new InvisibleBusinessPanelCommand(
                nameof(RecipePanelResources.RECIPE_MANAGER_ADD_RECIPE),
                new DelegateCommand(AddRecipe, () => RecipeManager.RecipeGroups.Any(grp => IsAccessible(grp.Name))),
                PathIcon.Add);
            Commands.Add(AddRecipeCommand);

            EditRecipeCommand = new InvisibleBusinessPanelCommand(
                nameof(RecipePanelResources.RECIPE_MANAGER_EDIT_RECIPE),
                new DelegateCommand(DisplayRecipeEditor, () => IsAccessible(SelectedRecipe)),
                PathIcon.Edit);
            Commands.Add(EditRecipeCommand);

            DeleteRecipeCommand = new InvisibleBusinessPanelCommand(
                nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE),
                new DelegateCommand(DeleteRecipe, () => IsAccessible(SelectedRecipe)),
                PathIcon.Delete);
            Commands.Add(DeleteRecipeCommand);

            DuplicateRecipeCommand = new InvisibleBusinessPanelCommand(
                nameof(RecipePanelResources.RECIPE_MANAGER_CLONE_RECIPE),
                new DelegateCommand(CloneRecipe, () => IsAccessible(SelectedRecipe)),
                PathIcon.Duplicate);
            Commands.Add(DuplicateRecipeCommand);

            // Edition commands
            SaveRecipeCommand = new BusinessPanelCommand(
                nameof(RecipePanelResources.SAVE_COMMAND),
                new DelegateCommand(SaveRecipeCommandExecute, SaveRecipeCommandCanExecute),
                PathIcon.Save) { IsVisible = false };
            Commands.Add(SaveRecipeCommand);

            CancelCommand = new BusinessPanelCommand(
                nameof(RecipePanelResources.UNDO_COMMAND),
                new DelegateCommand(CancelRecipeEditionExecute),
                PathIcon.Undo) { IsVisible = false };
            Commands.Add(CancelCommand);

            DisplayRecipeDetails();
        }

        public InvisibleBusinessPanelCommand AddRecipeCommand { get; }

        public InvisibleBusinessPanelCommand EditRecipeCommand { get; }

        public InvisibleBusinessPanelCommand DeleteRecipeCommand { get; }

        public InvisibleBusinessPanelCommand DuplicateRecipeCommand { get; }

        public BusinessPanelCommand CancelCommand { get; }

        public BusinessPanelCommand SaveRecipeCommand { get; }

        #region Properties

        private RecipeDetailsViewModel _recipeDetailsViewModel;

        public RecipeDetailsViewModel RecipeDetailsViewModel
        {
            get => _recipeDetailsViewModel;
            set
            {
                _recipeDetailsViewModel?.Dispose();
                _recipeDetailsViewModel = value;
                OnPropertyChanged();
                UpdateCommandButtonVisibility();
            }
        }

        public bool IsEditing => RecipeDetailsViewModel != null && RecipeDetailsViewModel.IsEditing;

        #endregion

        #region Commands

        #region AddCommand

        private void AddRecipe()
        {
            var newRecipeName = NamingStrategy.GetIncrementedName(
                LocalizationManager.GetString(nameof(RecipePanelResources.RECIPE_LIBRARY_NEW_RECIPE)),
                RecipeManager.Recipes.Keys);
            var newRecipe = (ProcessRecipe)RecipeManager.Create(newRecipeName);

            var firstGroup = GroupSelector.SelectedGroups.FirstOrDefault()
                             ?? RecipeManager.RecipeGroups.FirstOrDefault(grp => IsAccessible(grp.Name));
            if (GroupSelector.SelectedGroups.Count == 0)
            {
                GroupSelector.Select(firstGroup);
            }

            newRecipe.Header.GroupName = firstGroup?.Name;
            newRecipe.IsInEdition = true;

            SaveRecipe(newRecipe, true);
            SelectRecipe(newRecipeName, true);
            DisplayRecipeEditor(true);
        }

        #endregion

        #region DeleteCommand

        private void DeleteRecipe()
        {
            if (!Recipes.Contains(SelectedRecipe)) return;

            var popup =
                new Popup(nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE_RECIPE_TITLE), new LocalizableText(nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE_RECIPE_DESCRIPTION), SelectedRecipe.Id))
                {
                    Commands =
                    {
                        new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                        new PopupCommand(nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE), new DelegateCommand(() =>
                        {
                            try
                            {
                                Application.Current.Dispatcher?.Invoke(delegate
                                {
                                    var selectedRecipe = SelectedRecipe;

                                    RecipeManager.Delete(selectedRecipe);
                                    Recipes.Remove(selectedRecipe);

                                    Messages.Show(new UserMessage(MessageLevel.Success,
                                        new LocalizableText(nameof(RecipePanelResources.RECIPE_LIBRARY_DELETED),
                                            selectedRecipe.Id)) {SecondsDuration = 5});

                                    _logger.Info(
                                        "Recipe '{RecipeId}' has been deleted by {UserName}.",
                                        selectedRecipe.Id,
                                        AccessManager.CurrentUser?.Name);
                                });
                            }
                            catch (Exception e)
                            {
                                Messages.Show(new UserMessage(MessageLevel.Error,
                                    nameof(RecipePanelResources.RECIPE_LIBRARY_ERROR_DELETION)) {CanUserCloseMessage = true});

                                _logger.Error(e, "An exception occurred while deleting the recipe '{RecipeId}'", SelectedRecipe.Id);
                            }
                        }))
                    }
                };
            Popups.Show(popup);
        }

        #endregion

        #region CloneCommand

        private void CloneRecipe()
        {
            var recipeClone = RecipeManager.Clone(SelectedRecipe.Id);
            recipeClone.Id = NamingStrategy.GetCloneName(recipeClone.Id, RecipeManager.Recipes.Keys);
            RecipeManager.Save(recipeClone);
            RefreshRecipesList();

            SelectRecipe(recipeClone.Id);
        }

        #endregion CloneCommand

        #region SaveCommand

        private bool SaveRecipeCommandCanExecute()
        {
            return RecipeDetailsViewModel != null && RecipeDetailsViewModel.CanBeSaved();
        }

        private void SaveRecipeCommandExecute()
        {
            var saved = SaveRecipe(RecipeDetailsViewModel.Recipe);
            if (!saved) return;

            try
            {
                var savedRecipe = RecipeDetailsViewModel.Recipe;
                DisplayRecipeDetails();

                if (RecipeManager.StorageStrategy is OnDiskRecipeStorageStrategy<ProcessRecipe> storageStrategy)
                {
                    var filePath = Path.Combine(
                        ".",
                        "Configuration",
                        "Recipes",
                        savedRecipe.Id + "." + storageStrategy.FileExtension);
                    var userMessage = OpenFileDirectory.GetUserMessage(
                        new LocalizableText(nameof(RecipePanelResources.RECIPE_LIBRARY_SAVED), savedRecipe.Id), filePath);
                    Messages.Show(userMessage);
                }
            }
            catch (Exception e)
            {
                Messages.Show(new UserMessage(MessageLevel.Error,
                        new LocalizableText(nameof(RecipePanelResources.RECIPE_LIBRARY_ERROR_SAVING)))
                { CanUserCloseMessage = true });

                _logger.Error(
                    e,
                    "An exception occurred after saving the recipe '{RecipeId}'",
                    RecipeDetailsViewModel.Recipe.Id);
            }
        }

        #endregion

        #region CancelCommand

        private void CancelRecipeEditionExecute()
        {
            if (!RecipeDetailsViewModel.SaveIsRequired())
            {
                CancelRecipeEdition();
            }
            else
            {
                var popup =
                    new Popup(nameof(RecipePanelResources.RECIPE_LIBRARY_DISCARD_CHANGES),
                        nameof(RecipePanelResources.RECIPE_LIBRARY_DISCARD_CHANGES_DESCRIPTION))
                    {
                        Commands =
                        {
                            new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                            new PopupCommand(nameof(RecipePanelResources.RECIPE_LIBRARY_DISCARD),
                                new DelegateCommand(CancelRecipeEdition))
                        }
                    };
                Popups.Show(popup);
            }
        }

        private void CancelRecipeEdition()
        {
            Messages.HideAll();

            var editingRecipe = Recipes.SingleOrDefault(recipe => recipe.IsInEdition);
            if (editingRecipe != null) editingRecipe.IsInEdition = false;
            if (RecipeDetailsViewModel.IsNew)
            {
                RefreshRecipesList();
                Messages.Show(new UserMessage(MessageLevel.Error,
                        new LocalizableText(nameof(RecipePanelResources.RECIPE_LIBRARY_CREATION_CANCELED)))
                    { CanUserCloseMessage = true });
            }
            DisplayRecipeDetails();
            DetailsIsExpanded = false;
        }

        #endregion

        #endregion

        protected override void OnSelectedRecipeChanged()
        {
            if (!IsEditing)
            {
                DisplayRecipeDetails();
            }
        }

        private void DisplayRecipeDetails()
        {
            RecipeDetailsViewModel = SelectedRecipe != null
                ? new RecipeDetailsViewModel(SelectedRecipe, false, this)
                : null;
        }

        private bool SaveRecipe(ProcessRecipe recipeToSave, bool isNew = false)
        {
            Messages.HideAll();

            recipeToSave.Header.ModificationDate = DateTime.Now;
            recipeToSave.Header.Author = AccessManager.CurrentUser.Name;
            recipeToSave.IsInEdition = false;

            var relatedDataTableRecipe = Recipes.SingleOrDefault(recipe => recipe.IsInEdition);
            try
            {
                // Remove old scenario from system and disk
                if (relatedDataTableRecipe != null && RecipeManager.Contains(relatedDataTableRecipe.Id))
                {
                    RecipeManager.Delete(relatedDataTableRecipe);
                    Recipes.Remove(relatedDataTableRecipe);
                }

                // Add scenario on system and create file on disk
                if (!Recipes.Contains(recipeToSave))
                {
                    Recipes.Add(recipeToSave);
                }

                var saved = true;
                if (!isNew)
                {
                    saved = RecipeManager.Save(recipeToSave);
                    RefreshRecipesList();
                }

                SelectedRecipe = null;
                SelectedRecipe =
                    Recipes.SourceView.SingleOrDefault(recipe => recipe.Id == recipeToSave.Id);

                if (!saved) throw new InvalidOperationException("Error during recipe saving.");
            }
            catch (Exception e)
            {
                Messages.Show(
                    new UserMessage(
                        MessageLevel.Error,
                        new LocalizableText(nameof(RecipePanelResources.RECIPE_LIBRARY_ERROR_SAVING)))
                    {
                        CanUserCloseMessage = true
                    });

                _logger.Error(e, "An exception occurred while saving the recipe '{RecipeId}'", recipeToSave.Id);
                return false;
            }

            return true;
        }

        internal void DisplayRecipeEditor()
        {
            DisplayRecipeEditor(false);
        }


        internal void DisplayRecipeEditor(bool isNew)
        {
            SelectedRecipe.IsInEdition = true;
            if (isNew)
            {
                RecipeDetailsViewModel = new RecipeDetailsViewModel(SelectedRecipe, true, this, true);
                return;
            }

            RefreshRecipesList();
            var clonedScenario = RecipeManager.Clone(SelectedRecipe.Id) as ProcessRecipe;
            RecipeDetailsViewModel = new RecipeDetailsViewModel(clonedScenario, true, this);
        }

        private void SelectRecipe(string recipeId, bool isNew = false)
        {
            if (isNew)
            {
                SelectedRecipe = Recipes.Single(recipe => recipe.Id.Equals(recipeId));
                return;
            }
            SelectedRecipe = RecipeManager.Recipes.Values.Single(recipe => recipe.Id.Equals(recipeId));
        }

        private void UpdateCommandButtonVisibility()
        {
            // Ignore if the constructor has not finished his work.
            if (SaveRecipeCommand == null) return;

            if (RecipeDetailsViewModel != null && RecipeDetailsViewModel.IsEditing)
            {
                SaveRecipeCommand.IsVisible = true;
                CancelCommand.IsVisible = true;
                DetailsIsExpanded = true;
            }
            else
            {
                SaveRecipeCommand.IsVisible = false;
                CancelCommand.IsVisible = false;
            }

            OnPropertyChanged(nameof(IsEditing));
        }
    }
}
