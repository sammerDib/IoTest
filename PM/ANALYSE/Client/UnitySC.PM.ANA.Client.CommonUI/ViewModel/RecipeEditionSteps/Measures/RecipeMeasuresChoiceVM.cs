using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Client.CommonUI.Helpers;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class RecipeMeasuresChoiceVM : RecipeWizardStepBaseVM
    {
        private INavigable _lastCreatedMeasure = null;

        private ANARecipeVM _editedRecipe;

        public INavigable LastCreatedMeasure
        {
            get => _lastCreatedMeasure; set { if (_lastCreatedMeasure != value) { _lastCreatedMeasure = value; OnPropertyChanged(); } }
        }

        public RecipeMeasuresChoiceVM(ANARecipeVM editedRecipe)
        {
            Name = "Measurement";
            IsEnabled = true;
            IsMeasure = false;
            IsValidated = false;

            _editedRecipe = editedRecipe;

            var availableMeasures = Proxy.ServiceLocator.MeasureSupervisor.GetAvailableMeasures()?.Result;
            if (availableMeasures != null)
            {
                ThicknessMeasureChoices = new ObservableCollection<MeasureChoiceVM>();
                DistanceRoughnessMeasureChoices = new ObservableCollection<MeasureChoiceVM>();
                WaferShapeMeasureChoices = new ObservableCollection<MeasureChoiceVM>();

                foreach (var measureType in availableMeasures)
                {
                    switch (measureType)
                    {
                        case MeasureType.Thickness:
                            AddToRecipeMeasureChoices(ThicknessMeasureChoices, measureType);
                            break;

                        case MeasureType.TSV:
                        case MeasureType.NanoTopo:
                        case MeasureType.Topography:
                        case MeasureType.EdgeTrim:
                        case MeasureType.Trench:
                            AddToRecipeMeasureChoices(DistanceRoughnessMeasureChoices, measureType);
                            break;

                        case MeasureType.Bow:
                        case MeasureType.Warp:
                            AddToRecipeMeasureChoices(WaferShapeMeasureChoices, measureType);
                            break;

                        case MeasureType.Step:
                        case MeasureType.XYCalibration:
                            // Nothing to do
                            break;

                        default:
                            throw new NotImplementedException("Unknown measure type");
                    }
                }
            }
            else
            {
                Logger.Error("Impossible to get available measures");
            }

            ChosenMeasures = new ObservableCollection<ChosenMeasureVM>();
            LoadRecipeData();
        }

        private void AddToRecipeMeasureChoices(ObservableCollection<MeasureChoiceVM> measureChoices, MeasureType measureType)
        {
            if (!measureChoices.Any(c => c.Type == measureType))
            {
                var measureChoice = new MeasureChoiceVM(measureType,
                    ResourceHelper.GetMeasureName(measureType),
                    ResourceHelper.GetMeasureDescription(measureType),
                    ResourceHelper.GetMeasureImage(measureType));
                measureChoices.Add(measureChoice);
            }
        }

        #region Method Members

        private void LoadRecipeData(bool createMeasurePages = true)
        {
            if ((_editedRecipe.Measures is null) || _editedRecipe.Measures.Count == 0)
                return;

            ChosenMeasures.Clear();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (var measure in _editedRecipe.Measures)
                    {
                        var newChosenMeasure = new ChosenMeasureVM(measure.MeasureType, measure.Name, measure.IsActive);

                        AddChosenMeasure(newChosenMeasure);

                        if (createMeasurePages)
                        {
                            AdddMeasurePage(newChosenMeasure);
                        }
                    }
                }));

            IsModified = false;
            IsValidated = true;
        }

        private void SaveRecipeDataAndCreatePages()
        {
            // we remove the measure pages that are not needed anymore

            foreach (var measurePage in ServiceLocator.NavigationManager.AllPages.Where(p => (p as RecipeWizardStepBaseVM).IsMeasure).ToList())
            {
                if (!(ChosenMeasures.Any(m => m.PreviousName == (measurePage as RecipeMeasureVM).Name && m.Type == (measurePage as RecipeMeasureVM).MeasureType)))
                {
                    _editedRecipe.Measures.RemoveAll(m => m.Name == (measurePage as RecipeMeasureVM).Name);
                    (measurePage as RecipeMeasureVM).Dispose();
                    ServiceLocator.NavigationManager.AllPages.Remove(measurePage);
                }
            }

            CreateMeasurePages();

            _editedRecipe.IsModified = true;
            IsModified = false;
        }

        private void CreateMeasurePages()
        {
            foreach (var measure in ChosenMeasures)
            {
                // we check if the measure already exists in the recipe
                if (!(_editedRecipe.Measures.Any(m => m.Name == measure.PreviousName)))
                {
                    // We add the measure to the recipe
                    MeasureSettingsBase measureSettings = CreateMeasureSettings(measure);

                    if (!(measureSettings is null))
                    {
                        measureSettings.IsActive = measure.IsActive;
                        measureSettings.Name = measure.Name;
                        measureSettings.IsConfigured = false;
                        _editedRecipe.Measures.Add(measureSettings);
                    }
                }
                else
                {
                    var existingMeasure = _editedRecipe.Measures.First(m => m.Name == measure.PreviousName);
                    existingMeasure.Name = measure.Name;
                    existingMeasure.IsActive = measure.IsActive;
                }

                // we check if the measure page already exists
                var existingMeasurePage = ServiceLocator.NavigationManager.AllPages.FirstOrDefault(p => (p as RecipeWizardStepBaseVM).IsMeasure && ((p as RecipeMeasureVM).Name == measure.PreviousName) && ((p as RecipeMeasureVM).MeasureType == measure.Type));
                if (existingMeasurePage is null)
                {
                    AdddMeasurePage(measure);
                }
                else
                {
                    (existingMeasurePage as RecipeMeasureVM).IsEnabled = measure.IsActive;
                    (existingMeasurePage as RecipeMeasureVM).IsActive = measure.IsActive;
                    (existingMeasurePage as RecipeMeasureVM).Name = measure.Name;
                }
            }
        }

        private void AdddMeasurePage(ChosenMeasureVM measure)
        {
            var RecipeMeasureChoice = ServiceLocator.NavigationManager.AllPages.LastOrDefault(p => (p is RecipeMeasuresChoiceVM));
            int NewMeasureIndex = ServiceLocator.NavigationManager.AllPages.IndexOf(RecipeMeasureChoice) + 1;
            var LastMeasure = ServiceLocator.NavigationManager.AllPages.LastOrDefault(p => (p as RecipeWizardStepBaseVM).IsMeasure);
            if (LastMeasure != null)
                NewMeasureIndex = ServiceLocator.NavigationManager.AllPages.IndexOf(LastMeasure) + 1;
            LastCreatedMeasure = new RecipeMeasureVM(_editedRecipe, measure.Type, measure.Name, measure.IsActive);
            ServiceLocator.NavigationManager.AllPages.Insert(NewMeasureIndex, LastCreatedMeasure);
        }

        private void UpdateValidationErrorMessage()
        {
            if (ChosenMeasures.Count(m => m.IsActive) == 0)
                ValidationErrorMessage = "No measure selected";
            else if (!AreNamesUnique())
            {
                ValidationErrorMessage = "Measure names must be unique and different to old names";
            }
            else if (!IsModified)
                ValidationErrorMessage = "Measures choice is not modified";
            else
                ValidationErrorMessage = string.Empty;
        }

        private bool AreNamesUnique()
        {
            foreach (var chosenMeasure in ChosenMeasures)
            {
                // Check if the name is used by the other measures
                foreach (var otherchosenMeasure in ChosenMeasures)
                {
                    if (otherchosenMeasure != chosenMeasure)
                    {
                        if (String.Equals(chosenMeasure.Name.TrimStart().TrimEnd(), otherchosenMeasure.Name.TrimStart().TrimEnd(), StringComparison.OrdinalIgnoreCase) || String.Equals(chosenMeasure.Name.TrimStart().TrimEnd(),otherchosenMeasure.PreviousName.TrimStart().TrimEnd(), StringComparison.OrdinalIgnoreCase))
                        { 
                            return false; 
                        }
                    }
                }
            }
            return true;
        }

        private string GetUniqueNameForChosenMeasure(MeasureType measureType)
        {
            var measureName = ResourceHelper.GetMeasureName(measureType);
            string measureNameToCheck = measureName;
            if (!ChosenMeasures.Any(m => String.Equals(m.Name, measureNameToCheck, StringComparison.OrdinalIgnoreCase)))
                return measureNameToCheck;
            for (int i = 1; i < 100; i++)
            {
                measureNameToCheck = measureName + " " + i.ToString();
                if (!ChosenMeasures.Any(m => String.Equals(m.Name, measureNameToCheck, StringComparison.OrdinalIgnoreCase)))
                    return measureNameToCheck;
            }

            return measureName;
        }

        private void RemoveChosenMeasure(ChosenMeasureVM measure)
        {
            if (measure is null)
                return;
            measure.PropertyChanged -= ChosenMeasure_PropertyChanged;
            ChosenMeasures.Remove(measure);
            IsModified = true;
        }

        #endregion Method Members

        #region Properties

        private ObservableCollection<MeasureChoiceVM> _thicknessMeasureChoices;

        public ObservableCollection<MeasureChoiceVM> ThicknessMeasureChoices
        {
            get => _thicknessMeasureChoices; set { if (_thicknessMeasureChoices != value) { _thicknessMeasureChoices = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<MeasureChoiceVM> _distanceRoughnessMeasureChoices;

        public ObservableCollection<MeasureChoiceVM> DistanceRoughnessMeasureChoices
        {
            get => _distanceRoughnessMeasureChoices; set { if (_distanceRoughnessMeasureChoices != value) { _distanceRoughnessMeasureChoices = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<MeasureChoiceVM> _waferShapeMeasureChoices;

        public ObservableCollection<MeasureChoiceVM> WaferShapeMeasureChoices
        {
            get => _waferShapeMeasureChoices; set { if (_waferShapeMeasureChoices != value) { _waferShapeMeasureChoices = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<ChosenMeasureVM> _chosenMeasures;

        public ObservableCollection<ChosenMeasureVM> ChosenMeasures
        {
            get => _chosenMeasures; set { if (_chosenMeasures != value) { _chosenMeasures = value; OnPropertyChanged(); } }
        }

        private bool? _areAllMeasuresSelected = false;

        public bool? AreAllMeasuresSelected
        {
            get
            {
                if (!ChosenMeasures.Any(m => m.IsSelected))
                    _areAllMeasuresSelected = false;
                else
                {
                    if (ChosenMeasures.Any(m => !m.IsSelected))
                        _areAllMeasuresSelected = null;
                    else
                        _areAllMeasuresSelected = true;
                }

                return _areAllMeasuresSelected;
            }

            set
            {
                if (_areAllMeasuresSelected == value)
                {
                    return;
                }

                _areAllMeasuresSelected = value;
                foreach (var measure in ChosenMeasures)
                {
                    measure.IsSelected = (bool)_areAllMeasuresSelected;
                }

                OnPropertyChanged();
            }
        }

        private bool _isModified = false;

        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); OnPropertyChanged(nameof(RequiresValidation)); } }
        }

        public bool RequiresValidation => !IsValidated || IsModified || !IsReadyToValidate;

        private bool IsReadyToValidate => ChosenMeasures.Count(m => m.IsActive) > 0 && AreNamesUnique();

        private string _validationErrorMessage = string.Empty;

        public string ValidationErrorMessage
        {
            get => _validationErrorMessage; set { if (_validationErrorMessage != value) { _validationErrorMessage = value; OnPropertyChanged(); } }
        }

        #endregion Properties

        #region RelayCommands

        private AutoRelayCommand<MeasureType> _chooseMeasure;

        public AutoRelayCommand<MeasureType> ChooseMeasure
        {
            get
            {
                return _chooseMeasure ?? (_chooseMeasure = new AutoRelayCommand<MeasureType>(
                    (measureType) =>
                    {
                        var chosenMeasureName = GetUniqueNameForChosenMeasure(measureType);

                        var newChosenMeasure = new ChosenMeasureVM(measureType, chosenMeasureName);
                        AddChosenMeasure(newChosenMeasure);
                        OnPropertyChanged(nameof(AreAllMeasuresSelected));
                        IsModified = true;
                    },
                    (measureType) => { return true; }
                ));
            }
        }

        private void AddChosenMeasure(ChosenMeasureVM newChosenMeasure)
        {
            ChosenMeasures.Add(newChosenMeasure);
            newChosenMeasure.PropertyChanged += ChosenMeasure_PropertyChanged;
        }

        private void ChosenMeasure_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AreAllMeasuresSelected));
            IsModified = true;
        }

        private AutoRelayCommand _deleteSelectedMeasures;

        public AutoRelayCommand DeleteSelectedMeasures
        {
            get
            {
                return _deleteSelectedMeasures ?? (_deleteSelectedMeasures = new AutoRelayCommand(
                    () =>
                    {
                        foreach (var selectedMeasure in ChosenMeasures.Where(s => s.IsSelected).ToList())
                        {
                            RemoveChosenMeasure(selectedMeasure);
                            OnPropertyChanged(nameof(AreAllMeasuresSelected));
                        }
                    },
                    () => { return AreAllMeasuresSelected != false; }
                ));
            }
        }

        private AutoRelayCommand<ChosenMeasureVM> _deleteChosenMeasure;

        public AutoRelayCommand<ChosenMeasureVM> DeleteChosenMeasure
        {
            get
            {
                return _deleteChosenMeasure ?? (_deleteChosenMeasure = new AutoRelayCommand<ChosenMeasureVM>(
                    (m) =>
                    {
                        RemoveChosenMeasure(m);
                    },
                    (m) => { return true; }
                ));
            }
        }

        private AutoRelayCommand _validateMeasuresChoice;

        public AutoRelayCommand ValidateMeasuresChoice
        {
            get
            {
                return _validateMeasuresChoice ?? (_validateMeasuresChoice = new AutoRelayCommand(
                    () =>
                    {
                        if (IsModified)
                        {
                            IsValidated = true;
                            SaveRecipeDataAndCreatePages();

                            IsModified = false;
                        }
                        var FirstActivedMeasurePage = ServiceLocator.NavigationManager.AllPages.FirstOrDefault(p => (p as RecipeWizardStepBaseVM).IsMeasure && (p as RecipeWizardStepBaseVM).IsEnabled);
                        ServiceLocator.NavigationManager.NavigateToPage(FirstActivedMeasurePage);
                    },
                    () => { UpdateValidationErrorMessage(); return IsReadyToValidate; }
                ));
            }
        }

        private AutoRelayCommand _cancelMeasuresChoice;

        public AutoRelayCommand CancelMeasuresChoice
        {
            get
            {
                return _cancelMeasuresChoice ?? (_cancelMeasuresChoice = new AutoRelayCommand(
                    () =>
                    {
                        LoadRecipeData(false);
                    },
                    () => { return !(_editedRecipe.Measures is null) && IsModified; }
                ));
            }
        }

        public bool CanCancelMeasuresChoice
        {
            get => (!(_editedRecipe.Measures is null) && (_editedRecipe.Measures.Count > 0));
        }

        #endregion RelayCommands

        private MeasureSettingsBase CreateMeasureSettings(ChosenMeasureVM measure)
        {
            switch (measure.Type)
            {
                case MeasureType.Thickness:
                    return new ThicknessSettings();

                case MeasureType.TSV:
                    return new TSVSettings();

                case MeasureType.NanoTopo:
                    return new NanoTopoSettings();

                case MeasureType.Bow:
                    return new BowSettings();

                case MeasureType.Topography:
                    return new TopographySettings();

                case MeasureType.EdgeTrim:
                    return new EdgeTrimSettings();

                case MeasureType.Trench:
                    return new TrenchSettings();

                case MeasureType.Warp:
                    return new WarpSettings();

                default:
                    break;
            }

            return null;
        }

        #region INavigable

        public override Task PrepareToDisplay()
        {
            foreach (var chosenMeasure in ChosenMeasures)
            {
                chosenMeasure.PreviousName = chosenMeasure.Name;
            }
            return Task.CompletedTask;
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (IsReadyToValidate && IsModified && !forceClose)
            {
                var result = ServiceLocator.DialogService.ShowMessageBox("The measures choice has not been validated. Do you really want to leave ?", "Measures choice", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }
            IsModified = false;
            return true;
        }

        #endregion INavigable
    }
}
