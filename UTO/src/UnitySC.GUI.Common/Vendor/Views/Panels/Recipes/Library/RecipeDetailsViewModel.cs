using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Recipes.Components;

using Humanizer;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;
using UnitySC.GUI.Common.Vendor.UIComponents.Components;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Library;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Recipes.Library
{
    public class RecipeDetailsViewModel : Notifier, IDisposable
    {
        private enum MoveDirection
        {
            Up,
            Down
        }

        #region Constructor

        public RecipeDetailsViewModel(
            ProcessRecipe recipe,
            bool isEditing,
            RecipeLibraryPanel ownerPanel,
            bool isNew = false)
        {
            Recipe = recipe;
            IsEditing = isEditing;
            OwnerPanel = ownerPanel;
            IsNew = isNew;
            _isSaveRequired = isNew;

            EditInstructionCommand = new DelegateCommand<object>(EditCommandExecute, EditCommandCanExecute);
            MoveUpCommand = new DelegateCommand<object>(
                instruction => MoveInstructionExecute(instruction, MoveDirection.Up),
                instruction => MoveInstructionCanExecute(instruction, MoveDirection.Up));
            MoveDownCommand = new DelegateCommand<object>(
                instruction => MoveInstructionExecute(instruction, MoveDirection.Down),
                instruction => MoveInstructionCanExecute(instruction, MoveDirection.Down));
            RemoveCommand = new DelegateCommand<object>(RemoveCommandExecute, RemoveCommandCanExecute);

            AddInstructionCommand = new DelegateCommand<Type>(AddInstructionExecute, AddInstructionCanExecute);
            AddStepCommand = new DelegateCommand<Type>(AddStepCommandExecute, AddStepCommandCanExecute);
            DeleteStepCommand = new DelegateCommand(DeleteStepCommandExecute, DeleteStepCommandCanExecute);
            AddRelatedRecipeCommand = new DelegateCommand(AddRelatedRecipeCommandExecute);

            RecipeLocalizedModificationDate = LocalizableDateTime.WithStandardFormat(recipe.Header.ModificationDate);

            _listViewInstructions = InstructionByStep.ToList();
            _listViewRelatedRecipe = Recipe.SubRecipes.ToList();
            Recipe.SubRecipes.CollectionChanged += SubRecipes_CollectionChanged;
        }

        #endregion

        #region Fields

        private IEnumerable<string> _validationErrors;
        private bool _isSaveRequired;

        private List<RecipeInstruction> _listViewInstructions;

        #endregion

        #region Properties

        public RecipeLibraryPanel OwnerPanel { get; }

        public ProcessRecipe Recipe { get; }

        public bool IsEditing { get; }

        public bool IsNew { get; }

        #region Recipe properties

        public string RecipeId
        {
            get => Recipe.Id;
            set
            {
                if (Recipe.Id == value)
                {
                    return;
                }

                Recipe.Id = value;
                OnPropertyChanged();
                ValidateRecipe();
            }
        }

        public RecipeGroup RecipeGroup
        {
            get
                => OwnerPanel.GroupSelector.Groups.FirstOrDefault(
                    grp => string.Equals(grp.Name, Recipe.Header.GroupName));
            set
            {
                if (Recipe.Header.GroupName == value?.Name)
                {
                    return;
                }

                Recipe.Header.GroupName = value?.Name;
                OnPropertyChanged();
                ValidateRecipe();
            }
        }

        public string RecipeDescription
        {
            get => Recipe.Header.Description;
            set
            {
                if (Recipe.Header.Description == value)
                {
                    return;
                }

                Recipe.Header.Description = value;
                OnPropertyChanged();
                ValidateRecipe();
            }
        }

        public string RecipeVersionId
        {
            get => Recipe.Header.VersionId;
            set
            {
                if (Recipe.Header.VersionId == value)
                {
                    return;
                }

                Recipe.Header.VersionId = value;
                OnPropertyChanged();
                ValidateRecipe();
            }
        }

        public string RecipeEquipmentId
        {
            get => Recipe.Header.EquipmentId;
            set
            {
                if (Recipe.Header.EquipmentId == value)
                {
                    return;
                }

                Recipe.Header.EquipmentId = value;
                OnPropertyChanged();
                ValidateRecipe();
            }
        }

        public string RecipeSecurityId
        {
            get => Recipe.Header.SecurityId;
            set
            {
                if (Recipe.Header.SecurityId == value)
                {
                    return;
                }

                Recipe.Header.SecurityId = value;
                OnPropertyChanged();
                ValidateRecipe();
            }
        }

        public LocalizableDateTime RecipeLocalizedModificationDate { get; }

        private RecipeStep _selectedStep;
        
        public RecipeStep SelectedStep
        {
            get => _selectedStep;
            set
            {
                if (_selectedStep == value)
                {
                    return;
                }

                _selectedStep = value;
                UpdateListViewInstruction();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<RecipeInstruction> InstructionByStep
            => SelectedStep == null ? Recipe.Instructions : SelectedStep?.Instructions;

        private void UpdateListViewInstruction()
        {
            ListViewInstructionByStep = InstructionByStep.ToList();
        }

        //This ListViewInstructionByStep is a workaround for the Listview index to be refreshed when moving / adding an element to the list.
        public List<RecipeInstruction> ListViewInstructionByStep
        {
            get { return _listViewInstructions; }
            private set
            {
                _listViewInstructions = value;
                OnPropertyChanged();
            }

        }

        private int _selectedRecipeInstruction;

        public int SelectedRecipeInstruction
        {
            get => _selectedRecipeInstruction;
            set
            {
                if (_selectedRecipeInstruction == value)
                {
                    return;
                }

                _selectedRecipeInstruction = value;
                OnPropertyChanged();
            }
        }

        private RecipeReference _selectedSubRecipe;
        
        public RecipeReference SelectedSubRecipe
        {
            get => _selectedSubRecipe;
            set
            {
                if (_selectedSubRecipe == value)
                {
                    return;
                }

                _selectedSubRecipe = value;
                OnPropertyChanged();
            }
        }
        
        //This ListViewRelatedRecipe is a workaround for the Listview index to be refreshed when moving / adding an element to the list.
        private List<RecipeReference> _listViewRelatedRecipe;

        public List<RecipeReference> ListViewRelatedRecipe
        {
            get { return _listViewRelatedRecipe; }
            private set
            {
                _listViewRelatedRecipe = value; 
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region Add commands

        #region Add step

        public DelegateCommand<Type> AddStepCommand { get; }

        private void AddStepCommandExecute(Type obj)
        {
            var instance = Activator.CreateInstance(obj);
            if (instance is RecipeStep instruction)
            {
                instruction.Author = Recipe.Header.Author;
                Recipe.Steps.Add(instruction);
                Recipe.Steps.Sort((x, y) => OrderStepByType(x).CompareTo(OrderStepByType(y)));
                _isSaveRequired = true;
            }
        }

        private bool AddStepCommandCanExecute(Type obj)
        {
            if (obj == null || !obj.IsSubclassOf(typeof(RecipeStep)))
            {
                return false;
            }

            var instance = Activator.CreateInstance(obj);
            return instance is RecipeStep instruction
                   && Recipe.Steps.FirstOrDefault(s => s.Id == instruction.Id) == null;
        }

        #endregion

        #region Delete step

        public DelegateCommand DeleteStepCommand { get; }

        private void DeleteStepCommandExecute()
        {
            if (Recipe.Steps.Contains(SelectedStep))
            {
                Recipe.Steps.Remove(SelectedStep);
                Recipe.Steps.Sort((x, y) => OrderStepByType(x).CompareTo(OrderStepByType(y)));
                _isSaveRequired = true;
            }
        }

        private bool DeleteStepCommandCanExecute() => Recipe.Steps.Contains(SelectedStep);

        #endregion

        #region Add Instruction

        public DelegateCommand<Type> AddInstructionCommand { get; }

        private void AddInstructionExecute(Type instructionType)
        {
            var instance = Activator.CreateInstance(instructionType);
            if (instance is RecipeInstruction instruction)
            {
                ShowRecipeInstructionEditor(instruction, false);
            }
        }

        private bool AddInstructionCanExecute(Type arg)
            => arg?.IsSubclassOf(typeof(RecipeInstruction)) == true && SelectedStep != null;

        #endregion

        #region Add Related recipe

        public DelegateCommand AddRelatedRecipeCommand { get; }

        private void AddRelatedRecipeCommandExecute()
        {
            // RecipeIds used inside current Recipe
            var usedRecipeIds = new List<string> { Recipe.Id };
            usedRecipeIds.AddRange(Recipe.SubRecipes.Select(sr => sr.RecipeId));

            // Recipes where SubRecipes contains current Recipe Id
            var excludedRecipeIds = OwnerPanel.RecipeManager.Recipes.Values
                .Where(r => r.SubRecipes.Any(sr => sr.RecipeId == Recipe.Id))
                .Select(r => r.Id)
                .ToList();

            // Get all recipes where group is accessible,
            // excepted recipes already used in current recipe,
            // excepted recipes which reference, as sub-recipe, the current recipe
            var accessibleRecipeIds = OwnerPanel.RecipeManager.Recipes.Values
                .Where(r => OwnerPanel.GroupSelector.Groups.Any(g => g.Name == r.Header.GroupName))
                .Select(r => r.Id)
                .Except(usedRecipeIds)
                .Except(excludedRecipeIds)
                .ToList();

            var recipesComboBox = new ComboBox { ItemsSource = accessibleRecipeIds };
            var popup = new Popup(
                nameof(RecipePanelResources.RECIPE_EDITOR_RECIPE_SELECTION),
                nameof(RecipePanelResources.RECIPE_EDITOR_ADD_SUBRECIPE_MESSAGE)) { Content = recipesComboBox };
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_OK),
                    new DelegateCommand(
                        () =>
                        {
                            Recipe.SubRecipes.Add(new RecipeReference(recipesComboBox.SelectedValue.ToString()));

                            // Recipe validation needed
                            ValidateRecipe();
                        },
                        () => recipesComboBox.SelectedItem != null)));
            OwnerPanel.Popups.Show(popup);
        }

        #endregion

        #endregion

        #region Instruction commands

        #region Move

        public DelegateCommand<object> MoveUpCommand { get; }

        public DelegateCommand<object> MoveDownCommand { get; }

        private bool MoveInstructionCanExecute(object obj, MoveDirection direction)
        {
            int index;
            if (obj is RecipeInstruction instruction)
            {
                if (SelectedStep == null || Recipe?.Steps == null)
                {
                    return false;
                }

                index = Recipe.Steps
                    .FirstOrDefault(x => x.Id == SelectedStep.Id)
                    .Instructions.ToList()
                    .FindIndex(i => ReferenceEquals(i, instruction));
                if (index == -1)
                {
                    return false;
                }

                return direction == MoveDirection.Down ? index != InstructionByStep.Count - 1 : index > 0;
            }

            if (obj is RecipeReference reference)
            {
                index = Recipe.SubRecipes.ToList().FindIndex(x => x.RecipeId == reference.RecipeId);
                if (index == -1)
                {
                    return false;
                }

                return direction == MoveDirection.Down ? index != Recipe.SubRecipes.Count - 1 : index > 0;
            }

            return false;
        }

        private void MoveInstructionExecute(object obj, MoveDirection direction)
        {
            if (obj is RecipeInstruction)
            {
                var instruction = obj as RecipeInstruction;
                var currentInstructionIndex = Recipe.Steps
                    .FirstOrDefault(x => x.Id == SelectedStep.Id)
                    .Instructions.ToList()
                    .FindIndex(i => ReferenceEquals(i, instruction));
                var offset = direction == MoveDirection.Down ? 1 : -1;
                InstructionByStep.Move(currentInstructionIndex, currentInstructionIndex + offset);

                ValidateRecipe();
            }

            if (obj is RecipeReference)
            {
                var reference = obj as RecipeReference;
                var currentInstructionIndex =
                    Recipe.SubRecipes.ToList().FindIndex(x => x.RecipeId == reference.RecipeId);
                var offset = direction == MoveDirection.Down ? 1 : -1;
                Recipe.SubRecipes.Move(currentInstructionIndex, currentInstructionIndex + offset);

                ValidateRecipe();
            }
        }

        #endregion

        #region Remove

        public DelegateCommand<object> RemoveCommand { get; }

        private void RemoveCommandExecute(object obj)
        {
            if (obj is RecipeInstruction)
            {
                var instruction = obj as RecipeInstruction;
                var instructionName = GetInstructionName(instruction);

                var confirmationPopup = new Popup(
                    nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE_INSTRUCTION_TITLE),
                    new LocalizableText(
                        nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE_INSTRUCTION_DESCRIPTION),
                        instructionName));
                confirmationPopup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));
                confirmationPopup.Commands.Add(
                    new PopupCommand(
                        nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE),
                        new DelegateCommand(
                            () =>
                            {
                                InstructionByStep.Remove(instruction);

                                ValidateRecipe();
                            })));

                OwnerPanel.Popups.Show(confirmationPopup);
            }

            if (obj is RecipeReference)
            {
                var reference = obj as RecipeReference;
                var instructionName = reference.RecipeId;

                var confirmationPopup = new Popup(
                    nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE_SUBRECIPE_TITLE),
                    new LocalizableText(
                        nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE_SUBRECIPE_DESCRIPTION),
                        instructionName));
                confirmationPopup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));
                confirmationPopup.Commands.Add(
                    new PopupCommand(
                        nameof(RecipePanelResources.RECIPE_LIBRARY_DELETE),
                        new DelegateCommand(
                            () =>
                            {
                                Recipe.SubRecipes.RemoveAt(
                                    Recipe.SubRecipes.ToList().FindIndex(x => x.RecipeId == reference.RecipeId));

                                ValidateRecipe();
                            })));

                OwnerPanel.Popups.Show(confirmationPopup);
            }
        }

        private bool RemoveCommandCanExecute(object instruction) => instruction != null;

        #endregion

        #region Edit

        public ICommand EditInstructionCommand { get; }

        private bool EditCommandCanExecute(object itemToEdit) => itemToEdit != null;

        private void EditCommandExecute(object obj)
        {
            if (obj is RecipeInstruction)
            {
                var instruction = obj as RecipeInstruction;
                ShowRecipeInstructionEditor(instruction, true);
            }
        }

        #endregion

        #endregion

        #endregion

        private int OrderStepByType(RecipeStep step)
        {
            switch (step)
            {
                case PreProcess:
                    return 0;
                case Process:
                    return 1;
                case PostProcess:
                    return 2;
                default:
                    return 3;
            }
        }

        private void ShowRecipeInstructionEditor(RecipeInstruction instruction, bool editExistingInstruction)
        {
            var instanceToEdit = editExistingInstruction ? instruction.Clone() as RecipeInstruction : instruction;
            if (instanceToEdit == null)
            {
                return;
            }
            
            var instructionViewModel =
                RecipeInstructionViewModelFactory.Build(instanceToEdit, _ => true);

            if (instructionViewModel != null)
            {
                var instructionName = GetInstructionName(instruction);
                var popupTitle = editExistingInstruction
                    ? new LocalizableText(
                        nameof(RecipePanelResources.RECIPE_LIBRARY_EDITING_INSTRUCTION_DESCRIPTION),
                        instructionName)
                    : new LocalizableText(
                        nameof(RecipePanelResources.RECIPE_LIBRARY_ADDING_INSTRUCTION_DESCRIPTION),
                        instructionName);

                // show editor
                var editionPopup = new InstructionValidationPopup(popupTitle, instructionViewModel)
                {
                    Commands =
                    {
                        new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                        new PopupCommand(
                            editExistingInstruction
                                ? nameof(RecipePanelResources.RECIPE_LIBRARY_APPLY)
                                : nameof(RecipePanelResources.RECIPE_LIBRARY_ADD),
                            new DelegateCommand(
                                () =>
                                {
                                    if (editExistingInstruction)
                                    {
                                        var index = SelectedStep.Instructions.ToList()
                                            .FindIndex(i => ReferenceEquals(i, instruction));
                                        SelectedStep.Instructions.Remove(instruction);
                                        SelectedStep.Instructions.Insert(
                                            index,
                                            instanceToEdit.Clone() as RecipeInstruction);
                                    }
                                    else if (SelectedStep.Instructions.Count > 0 && SelectedRecipeInstruction != -1)
                                    {
                                        SelectedStep.Instructions.Insert(
                                            SelectedRecipeInstruction + 1,
                                            instanceToEdit.Clone() as RecipeInstruction);
                                        SelectedRecipeInstruction += 1;
                                    }
                                    else
                                    {
                                        SelectedStep.Instructions.Add(instanceToEdit.Clone() as RecipeInstruction);
                                    }

                                    ValidateRecipe();
                                },
                                () =>
                                {
                                    var condition = true;
                                    if (instanceToEdit is WaitStatusThresholdInstruction waitStatusThresholdInstruction)
                                    {
                                        condition = waitStatusThresholdInstruction.Thresholds.Count > 0;
                                    }

                                    return condition && !instanceToEdit.Validate().Any();
                                }))
                    }
                };

                OwnerPanel.Popups.Show(editionPopup);
            }
            else if (!editExistingInstruction)
            {
                Recipe.Instructions.Add(instanceToEdit);

                ValidateRecipe();
            }
        }

        private string GetInstructionName(RecipeInstruction instruction)
            => instruction is BaseRecipeInstruction baseInstruction
                ? LocalizationManager.GetString(baseInstruction.LocalizationKey)
                : instruction.GetType().Name.Humanize(LetterCasing.Sentence);

        private void ValidateRecipe()
        {
            OwnerPanel.Messages.HideAll();
            _validationErrors = Recipe.Validate().ToList();
            var errorsList = _validationErrors.ToList();
            if (errorsList.Count > 0)
            {
                errorsList.Insert(
                    0,
                    LocalizationManager.GetString(nameof(RecipePanelResources.RECIPE_EDITOR_VALIDATION_ERROR_LIST)));
                OwnerPanel.Messages.Show(
                    new UserMessage(MessageLevel.Warning, string.Join(Environment.NewLine, errorsList)));
            }
            else
            {
                _isSaveRequired = SaveIsRequired();
            }

            UpdateListViewInstruction();
            OnPropertyChanged();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3400:Methods should not return constants",
            Justification = "Temporaire, en attendant une implémentation correcte qui compare la Recipe en édition avec celui existant.")]
        public bool SaveIsRequired()
        {
            var alreadyExists = App.Instance.RecipeManager.Recipes.Any(recipeEntry => recipeEntry.Value.Equals(Recipe));
            return !alreadyExists;
        }

        public bool CanBeSaved() => (!_validationErrors?.Any() ?? true) && _isSaveRequired;

        /// <summary>
        /// Handle the CollectionChanged event from Recipe.SubRecipes collection
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The evnt args</param>
        private void SubRecipes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ListViewRelatedRecipe = Recipe.SubRecipes.ToList();
        }

        #region IDisposable

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Recipe.SubRecipes.CollectionChanged -= SubRecipes_CollectionChanged;
                    RecipeLocalizedModificationDate?.Dispose();
                }
                
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
