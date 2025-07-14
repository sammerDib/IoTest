using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Recipes.Components;

using Humanizer;

using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;
using UnitySC.GUI.Common.Vendor.UIComponents.Components;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Library
{
    public class ScenarioDetailsViewModel : Notifier, IDisposable
    {
        private enum MoveDirection
        {
            Up,
            Down
        }

        #region Fields

        private List<string> _validationErrors;
        private bool _isSaveRequired;

        #endregion Fields

        #region Constructor

        public ScenarioDetailsViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioDetailsViewModel" /> class.
        /// </summary>
        public ScenarioDetailsViewModel(SequenceRecipe scenario, bool isEditing, ScenarioLibraryPanel ownerPanel, bool isNew = false)
        {
            Scenario = scenario;
            IsEditing = isEditing;
            OwnerPanel = ownerPanel;
            IsNew = isNew;
            _isSaveRequired = isNew;

            EditInstructionCommand = new DelegateCommand<RecipeInstruction>(EditCommandExecute, EditCommandCanExecute);
            MoveUpCommand = new DelegateCommand<RecipeInstruction>(instruction => MoveInstructionExecute(instruction, MoveDirection.Up), instruction => MoveInstructionCanExecute(instruction, MoveDirection.Up));
            MoveDownCommand = new DelegateCommand<RecipeInstruction>(instruction => MoveInstructionExecute(instruction, MoveDirection.Down), instruction => MoveInstructionCanExecute(instruction, MoveDirection.Down));
            RemoveCommand = new DelegateCommand<RecipeInstruction>(RemoveCommandExecute, RemoveCommandCanExecute);
            AddInstructionCommand = new DelegateCommand<Type>(AddInstructionExecute, AddInstructionCanExecute);

            LocalizedModificationDate = LocalizableDateTime.WithStandardFormat(scenario.Header.ModificationDate);
        }

        #endregion Constructor

        #region Properties

        private int _selectedScenarioInstruction;
        private bool _disposedValue;

        public int SelectedScenarioInstruction
        {
            get { return _selectedScenarioInstruction; }
            set
            {
                if (_selectedScenarioInstruction == value) return;
                _selectedScenarioInstruction = value;
                OnPropertyChanged(nameof(SelectedScenarioInstruction));
            }
        }

        public SequenceRecipe Scenario { get; }

        public bool IsEditing { get; }

        public bool IsNew { get; }

        public ScenarioLibraryPanel OwnerPanel { get; }

        public string ScenarioId
        {
            get { return Scenario.Id; }
            set
            {
                if (Scenario.Id == value) return;
                Scenario.Id = value;
                OnPropertyChanged(nameof(ScenarioId));
                ValidateScenario();
            }
        }

        public string VersionId
        {
            get { return Scenario.Header.VersionId; }
            set
            {
                if (Scenario.Header.VersionId == value) return;
                Scenario.Header.VersionId = value;
                OnPropertyChanged(nameof(VersionId));
                ValidateScenario();
            }
        }

        public string ScenarioDescription
        {
            get { return Scenario.Header.Description; }
            set
            {
                if (Scenario.Header.Description == value) return;
                Scenario.Header.Description = value;
                OnPropertyChanged(nameof(ScenarioDescription));
                ValidateScenario();
            }
        }

        public RecipeGroup SelectedScenarioGroup
        {
            get { return OwnerPanel.GroupSelector.Groups.SingleOrDefault(group => group.Name.Equals(Scenario.Header.GroupName)); }
            set
            {
                if (Scenario.Header.GroupName == value?.Name) return;
                Scenario.Header.GroupName = value?.Name;
                OnPropertyChanged();
                ValidateScenario();
            }
        }

        public LocalizableDateTime LocalizedModificationDate { get; }

        #endregion Properties

        #region Commands

        #region Edit

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICommand EditInstructionCommand { get; }

        private bool EditCommandCanExecute(RecipeInstruction itemToEdit) => itemToEdit != null;

        private void EditCommandExecute(RecipeInstruction instruction) => ShowScenarioInstructionEditor(instruction, true);

        #endregion

        #region Move

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DelegateCommand<RecipeInstruction> MoveUpCommand { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DelegateCommand<RecipeInstruction> MoveDownCommand { get; }

        private bool MoveInstructionCanExecute(RecipeInstruction instruction, MoveDirection direction)
        {
            var currentInstructionIndex = Scenario.Instructions.ToList().FindIndex(i => ReferenceEquals(i, instruction));
            if (currentInstructionIndex == -1) return false;

            return direction == MoveDirection.Down ?
                currentInstructionIndex != Scenario.Instructions.Count - 1 :
                currentInstructionIndex > 0;
        }

        private void MoveInstructionExecute(RecipeInstruction instruction, MoveDirection direction)
        {
            var currentInstructionIndex = Scenario.Instructions.ToList().FindIndex(i => ReferenceEquals(i, instruction));
            if (currentInstructionIndex == -1) return;

            var offset = direction == MoveDirection.Down ? 1 : -1;
            Scenario.Instructions.Move(currentInstructionIndex, currentInstructionIndex + offset);

            ValidateScenario();
        }

        #endregion

        #region Remove

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICommand RemoveCommand { get; }

        private bool RemoveCommandCanExecute(RecipeInstruction instruction) => instruction != null;

        private void RemoveCommandExecute(RecipeInstruction instruction)
        {
            var instructionName = GetInstructionName(instruction);
            var popupTitle = new LocalizableText(nameof(ScenarioResources.SCENARIO_DELETE_INSTRUCTION), instructionName);

            var confirmationPopup = new Popup(popupTitle);
            confirmationPopup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));
            confirmationPopup.Commands.Add(new PopupCommand(nameof(ScenarioResources.SCENARIO_DELETE), new DelegateCommand(() =>
            {
                Scenario.Instructions.Remove(instruction);

                // Scenario validation needed
                ValidateScenario();
            })));

            OwnerPanel.Popups.Show(confirmationPopup);
        }

        #endregion

        #region Instructions

        public DelegateCommand<Type> AddInstructionCommand { get; }

        private void AddInstructionExecute(Type instructionType)
        {
            var instance = Activator.CreateInstance(instructionType);
            if (instance is RecipeInstruction instruction)
            {
                ShowScenarioInstructionEditor(instruction, false);
            }
        }

        private bool AddInstructionCanExecute(Type arg)
        {
            return arg != null && arg.IsSubclassOf(typeof(RecipeInstruction));
        }

        #endregion

        #endregion

        #region Public

        public bool CanBeSaved()
        {
            return (!_validationErrors?.Any() ?? true) && _isSaveRequired;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3400:Methods should not return constants",
            Justification = "Temporaire, en attendant une implÃ©mentation correcte qui compare le Scenario en Ã©dition avec celui existant.")]
        public bool SaveIsRequired()
        {
            var alreadyExists = App.Instance.ScenarioManager.Recipes.Any(scenarioEntry => scenarioEntry.Value.Equals(Scenario));
            return !alreadyExists;
        }

        #endregion

        #region Validation

        private void ValidateScenario()
        {
            OwnerPanel.Messages.HideAll();
            _validationErrors = Scenario.Validate().ToList();
            if (_validationErrors.Count > 0)
            {
                OwnerPanel.Messages.Show(new UserMessage(MessageLevel.Warning, string.Join(Environment.NewLine, _validationErrors)));
            }
            else
            {
                _isSaveRequired = SaveIsRequired();
            }
        }

        #endregion Validation

        #region Private

        private void ShowScenarioInstructionEditor(RecipeInstruction instruction, bool editExistingInstruction)
        {
            var instanceToEdit = editExistingInstruction ? instruction.Clone() as RecipeInstruction : instruction;
            if (instanceToEdit == null) return;

            bool CommandFilter(DeviceCommand command) => true;

            var instructionViewModel = RecipeInstructionViewModelFactory.Build(instanceToEdit, CommandFilter);

            if (instructionViewModel != null)
            {
                var instructionName = GetInstructionName(instruction);
                var popupTitle = editExistingInstruction
                    ? new LocalizableText(nameof(ScenarioResources.SCENARIO_EDITING_INSTRUCTION), instructionName)
                    : new LocalizableText(nameof(ScenarioResources.SCENARIO_ADDING_INSTRUCTION), instructionName);

                // show editor
                var editionPopup = new InstructionValidationPopup(popupTitle, instructionViewModel)
                {
                    Commands =
                    {
                        new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                        new PopupCommand(editExistingInstruction ? nameof(ScenarioResources.SCENARIO_APPLY) : nameof(ScenarioResources.SCENARIO_ADD),
                            new DelegateCommand(() =>
                            {
                                if (editExistingInstruction)
                                {
                                    var index = Scenario.Instructions.ToList().FindIndex(i => ReferenceEquals(i, instruction));
                                    Scenario.Instructions.Remove(instruction);
                                    Scenario.Instructions.Insert(index, instanceToEdit.Clone() as RecipeInstruction);
                                }
                                else if (Scenario.Instructions.Count > 0 && SelectedScenarioInstruction != -1)
                                {
                                    Scenario.Instructions.Insert(
                                        SelectedScenarioInstruction + 1,
                                        instanceToEdit.Clone() as RecipeInstruction);
                                    SelectedScenarioInstruction += 1;
                                }
                                else
                                {
                                    Scenario.Instructions.Add(instanceToEdit.Clone() as RecipeInstruction);
                                }

                                // Scenario validation needed
                                ValidateScenario();
                            }, () =>
                            {
                                var condition = true;
                                if(instanceToEdit is WaitStatusThresholdInstruction waitStatusThresholdInstruction)
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
                Scenario.Instructions.Add(instanceToEdit);

                // Scenario validation needed
                ValidateScenario();
            }
        }

        private string GetInstructionName(RecipeInstruction instruction)
        {
            return instruction is BaseRecipeInstruction baseInstruction ? LocalizationManager.GetString(baseInstruction.LocalizationKey) : instruction.GetType().Name.Humanize(LetterCasing.Sentence);
        }

        #endregion

        #region IDisposable
        

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    LocalizedModificationDate?.Dispose();
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
