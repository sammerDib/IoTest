using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.CommonUI.ViewModel.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.Shared.ADCConfiguration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.DieCutUpUI.Common.ViewModel;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.PM.DMT.CommonUI.ViewModel
{
    public class RecipeEditionVM : NavigationVM, IRecipeInfo
    {
        #region Private Fields

        private double _backSize;
        private string _comment;
        private string _content;
        private AutoRelayCommand _executeRecipeCommand;
        private double _frontSize;
        private bool _isNewRecipe;
        private List<MeasureVM> _measures;
        private string _name;
        private AutoRelayCommand _saveCommand;
        private MeasureVM _selectedMeasure;
        private RecipeExecutionVM _recipeExecutionVM;
        private DieCutUpRecipeEditionVM _dieCutUpRecipeEditionVM;
        private readonly IRecipeManager _recipeManager;
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private readonly RecipeSupervisor _recipeSupervisor;
        private readonly GlobalStatusSupervisor _globalStatusSupervisor;
        private readonly IDialogOwnerService _dialogService;
        private readonly Mapper _mapper;

        #endregion Private Fields

        #region Public Constructors

        public RecipeEditionVM(IRecipeManager recipeManager, CalibrationSupervisor calibrationSupervisor, RecipeSupervisor recipeSupervisor,
            GlobalStatusSupervisor globalStatusSupervisor, IDialogOwnerService dialogService, Mapper mapper)
        {
            _recipeManager = recipeManager;
            _calibrationSupervisor = calibrationSupervisor;
            _recipeSupervisor = recipeSupervisor;
            _globalStatusSupervisor = globalStatusSupervisor;
            _dialogService = dialogService;
            _mapper = mapper;
            _dieCutUpRecipeEditionVM = new DieCutUpRecipeEditionVM(dialogService);
        }

        #endregion Public Constructors

        #region Public Properties

        public DieCutUpRecipeEditionVM DieCutUpRecipeEditionVM 
        {
            get => _dieCutUpRecipeEditionVM; set { if (_dieCutUpRecipeEditionVM != value) { _dieCutUpRecipeEditionVM = value; OnPropertyChanged(); } }
        }

        public ActorType ActorType { get; set; }

        public double BackSize
        { get => _backSize; set { if (_backSize != value) { _backSize = value; OnPropertyChanged(); } } }

        public string Comment
        {
            get => _comment; set { if (_comment != value) { _comment = value; IsRecipeModified = true; OnPropertyChanged(); } }
        }

        public string Content
        {
            get => _content; set { if (_content != value) { _content = value; OnPropertyChanged(); } }
        }

        public DateTime Created { get; set; }

        public AutoRelayCommand ExecuteRecipeCommand
        {
            get
            {
                return _executeRecipeCommand ?? (_executeRecipeCommand = new AutoRelayCommand(
              () =>
              {
                  DMTRecipe recipe = _mapper.AutoMap.Map<DMTRecipe>(this);

                  bool noMeasuresSelected = recipe.Measures.TrueForAll(r => !r.IsEnabled);
                  if (noMeasuresSelected)
                  {
                      string message = "No measures are selected.";
                      var result = _dialogService.ShowMessageBox(message, "", MessageBoxButton.OK, MessageBoxImage.Stop);

                      if (result == MessageBoxResult.OK)
                      {
                          return;
                      }
                  }

                  CheckDeadPixelsCalibration(recipe);

                  if (CheckHighAngleDarkFieldCalibration(recipe))
                  {
                      string message = "High angle dark-field calibration is missing. You cannot run the recipe without this file.";
                      var result = _dialogService.ShowMessageBox(message, "", MessageBoxButton.OK, MessageBoxImage.Stop);

                      if (result == MessageBoxResult.OK)
                      {
                          return;
                      }
                  }

                  if (IsRecipeModified)
                  {
                      var result = _dialogService.ShowMessageBox(
                          "The recipe has not been saved. Do you want to save it now ?", "Save recipe",
                          MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                      switch (result)
                      {
                          case MessageBoxResult.Yes:
                              SaveCommand.Execute(this);
                              break;

                          case MessageBoxResult.No:
                              break;

                          case MessageBoxResult.Cancel:
                              return;
                      }
                  }

                  if (_recipeExecutionVM == null)
                      _recipeExecutionVM = new RecipeExecutionVM(recipe, _recipeSupervisor, _dialogService);
                  else
                      _recipeExecutionVM.Recipe = recipe;
                  Navigate(_recipeExecutionVM);
              }, () => true));
            }
        }

        private bool CheckHighAngleDarkFieldCalibration(DMTRecipe recipe)
        {
            bool needFrontCalibHighAngleDarkField = recipe.Measures.Exists(r => r.MeasureType == MeasureType.HighAngleDarkFieldMeasure
                                                                                              && r.Side == Side.Front
                                                                                              && r.IsEnabled);
            bool needBackCalibHighAngleDarkField = recipe.Measures.Exists(r => r.MeasureType == MeasureType.HighAngleDarkFieldMeasure
                                                                                              && r.Side == Side.Back
                                                                                              && r.IsEnabled);

            bool highAngleDarkfieldCalibFrontExist = needFrontCalibHighAngleDarkField && _calibrationSupervisor.IsHighAngleDarkFieldMaskAvailableForSide(Side.Front);
            bool highAngleDarkfieldCalibBackExist = needBackCalibHighAngleDarkField && _calibrationSupervisor.IsHighAngleDarkFieldMaskAvailableForSide(Side.Back);

            return (needFrontCalibHighAngleDarkField && !highAngleDarkfieldCalibFrontExist) ||
                (needBackCalibHighAngleDarkField && !highAngleDarkfieldCalibBackExist);
        }

        private void CheckDeadPixelsCalibration(DMTRecipe recipe)
        {
            var recipeHasFrontsideMeasure = recipe.Measures.Exists(measure => measure.Side == Side.Front);
            var recipeHasBacksideMeasure = recipe.Measures.Exists(measure => measure.Side == Side.Back);
            if (recipeHasFrontsideMeasure && !_calibrationSupervisor.DoesDeadPixelsCalibrationExist(Side.Front))
            {
                _globalStatusSupervisor.SendUIMessage(new UnitySC.Shared.Tools.Service.Message(MessageLevel.Warning, $"Dead pixel calibration is missing for side {Side.Front}"));
            }
            if (recipeHasBacksideMeasure && !_calibrationSupervisor.DoesDeadPixelsCalibrationExist(Side.Back))
            {
                _globalStatusSupervisor.SendUIMessage(new UnitySC.Shared.Tools.Service.Message(MessageLevel.Warning, $"Dead pixel calibration is missing for side {Side.Back}"));
            }
        }

        public double FrontSize
        { get => _frontSize; set { if (_frontSize != value) { _frontSize = value; OnPropertyChanged(); } } }

        public bool IsBSPerspectiveCalibrationUsed { get; set; }
        public bool IsFSPerspectiveCalibrationUsed { get; set; }

        public bool IsNewRecipe
        { get => _isNewRecipe; set { if (_isNewRecipe != value) { _isNewRecipe = value; OnPropertyChanged(); } } }

        public bool IsRecipeModified { get; set; } = false;
        public bool IsShared { get; set; }
        public bool IsTemplate { get; set; }

        public bool HasBackSideMeasures => !(Measures is null) && Measures.Any(measure => measure.Side == Side.Back);
        public Guid Key { get; set; }

        private bool _isFrontSideBackSideSelected;

        public bool IsFrontSideBackSideSelected
        {
            get { return _isFrontSideBackSideSelected; }
            set
            {
                _isFrontSideBackSideSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isDieCutUpSelected;
        public bool IsDieCutUpSelected
        {
            get { return _isDieCutUpSelected; }
            set 
            { 
                _isDieCutUpSelected = value;
                OnPropertyChanged();
            }
        }


        public List<MeasureVM> Measures
        {
            get => _measures;
            set
            {
                if (_measures != value)
                {
                    if (_measures != null)
                    {
                        // We unsubscribe the PropertyChanged
                        foreach (var measure in _measures)
                        {
                            measure.PropertyChanged -= Measure_PropertyChanged;
                        }
                    }

                    _measures = value;

                    foreach (var measure in _measures)
                    {
                        measure.PropertyChanged += Measure_PropertyChanged;
                    }

                    SelectedMeasure = _measures[0];                   

                    FrontSize = 41 * _measures.Count(m => m.Side == Side.Front);
                    BackSize = 41 * _measures.Count(m => m.Side == Side.Back);
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = new PathString(value).RemoveInvalidFilePathCharacters("_", false);
                    OnPropertyChanged();
                    IsRecipeModified = true;
                }
            }
        }

        private DatabaseDirectAccess GetAdcDatabase()
        {
            var adcConnectionString = ConfigurationManager.AppSettings["AdcConnectionString"];
            if (adcConnectionString == null)
                Logger.Error("Unable to retrieve the ADC Connection string");
            else
            {
                var dataBase = new DatabaseDirectAccess(adcConnectionString);
                return dataBase;
            }
            return null;
        }

        private List<ADCRecipe> _adcRecipes;

        public List<ADCRecipe> AdcRecipes
        {
            get
            {
                if (_adcRecipes == null)
                {
                    var adcDatabase = GetAdcDatabase();
                    if (adcDatabase != null)
                        _adcRecipes = adcDatabase.GetRecipes();
                }
                return _adcRecipes;
            }
        }

        private bool _areAcquisitionsSavedInDatabase;

        public bool AreAcquisitionsSavedInDatabase
        {
            get => _areAcquisitionsSavedInDatabase;
            set
            {
                if (value != _areAcquisitionsSavedInDatabase)
                {
                    _areAcquisitionsSavedInDatabase = value;
                    OnPropertyChanged();
                    IsRecipeModified = true;
                }
            }
        }

        public override string PageName => "Recipe Edition";

        public AutoRelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new AutoRelayCommand(
                    () =>
                    {
                        _recipeManager.SaveRecipe();
                    },
                    () =>
                    {
                        return IsRecipeModified;
                    }
                ));
            }
        }

        public void EndRecipeEdition()
        {
            // We go home
            Navigate(this);
        }

        public MeasureVM SelectedMeasure
        {
            get => _selectedMeasure;
            set
            {
                if (_selectedMeasure == value)
                {
                    return;
                }

                if (_selectedMeasure != null)
                {
                    if (!_selectedMeasure.CanChangeTab())
                        return;
                }
                _selectedMeasure = value;
                _selectedMeasure.PrepareDisplay();
                OnPropertyChanged();
            }
        }

        public int? StepId { get; set; }
        public int? UserId { get; set; }
        public int Version { get; set; }
        public int? CreatorChamberId { get; set; }

        public Step Step { get; set; }

        #endregion Public Properties

        #region Private Methods

        private void Measure_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var propertyChanged = e.PropertyName;
            var measure = (sender as MeasureVM);
            if (measure != null && propertyChanged == nameof(measure.IsEnabled))
            {
                if (measure.IsEnabled)
                    SelectedMeasure = measure;
            }

            IsRecipeModified = true;
        }

        #endregion Private Methods
    }
}
