using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using UnitySC.PM.ANA.Client.Controls;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class RecipeDiesSelectionVM : RecipeWizardStepBaseVM, IDisposable
    {
        private bool _suspendEstimatedTimeUpdate = false;
        private bool IsReadyToValidate => ChosenDies.Count > 0;

        private bool SuspendEstimatedTimeUpdate
        {
            get { return _suspendEstimatedTimeUpdate; }
            set
            {
                _suspendEstimatedTimeUpdate = value;
                if (!_suspendEstimatedTimeUpdate)
                    UpdateEstimatedAnalysesTime();
            }
        }

        public ANARecipeVM EditedRecipe { get; set; }

        public RecipeDiesSelectionVM(ANARecipeVM editedRecipe)
        {
            Name = "Dies Selection";
            IsEnabled = true;
            IsMeasure = false;
            IsValidated = false;
            EditedRecipe = editedRecipe;
            LoadRecipeData();
        }

        #region Method Members

        private void LoadRecipeData()
        {
            if ((EditedRecipe.Dies is null) || (EditedRecipe.Dies.Count == 0))
            {
                ChosenDies.Clear();
                return;
            }

            SuspendEstimatedTimeUpdate = true;
            ChosenDies.ToList().ForEach(d => ChosenDies.Remove(d));
            EditedRecipe.Dies.ForEach(d => ChosenDies.Add(d));
            SuspendEstimatedTimeUpdate = false;
            IsValidated = true;
            IsModified = false;
        }

        private void SaveRecipeData()
        {
            EditedRecipe.IsModified = true;
            EditedRecipe.Dies = ChosenDies.ToList<DieIndex>();
            IsModified = false;
        }

        private void UpdateValidationErrorMessage()
        {
            if (ChosenDies.Count == 0)
                ValidationErrorMessage = "No dies selected";
            else if (!IsModified)
                ValidationErrorMessage = "Dies selection is not modified";
            else
                ValidationErrorMessage = string.Empty;
        }

        public void Dispose()
        {
            if (!(ChosenDies is null))
            {
                ChosenDies.Clear();
                ChosenDies.CollectionChanged -= ChosenDies_CollectionChanged;
            }

            if (!(ChosenDiesWithSelection is null))
            {
                ChosenDiesWithSelection.ToList().ForEach(d => ChosenDiesWithSelection.Remove(d));
                ChosenDiesWithSelection.CollectionChanged -= ChosenDiesWithSelection_CollectionChanged;
            }
        }

        #endregion Method Members

        #region Properties

        private bool _isModified = false;

        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); OnPropertyChanged(nameof(RequiresValidation)); } }
        }


        public bool RequiresValidation => !IsValidated || IsModified || !IsReadyToValidate;

        private ObservableCollection<DieIndex> _chosenDiesSelected = new ObservableCollection<DieIndex>();

        public ObservableCollection<DieIndex> ChosenDiesSelected
        {
            get => _chosenDiesSelected;
            set
            {
                if (_chosenDiesSelected != value)
                {
                    _chosenDiesSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<DieIndex> _chosenDies;

        // This list is used by the wafer map control
        public ObservableCollection<DieIndex> ChosenDies
        {
            get
            {
                if (_chosenDies is null)
                {
                    _chosenDies = new ObservableCollection<DieIndex>();
                    _chosenDies.CollectionChanged += ChosenDies_CollectionChanged;
                }
                return _chosenDies;
            }
            set
            {
                if (_chosenDies != value)
                {
                    _chosenDies = value;

                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan _estimatedAnalysesTime;

        public TimeSpan EstimatedAnalysesTime
        {
            get => _estimatedAnalysesTime; set { if (_estimatedAnalysesTime != value) { _estimatedAnalysesTime = value; OnPropertyChanged(); } }
        }

        private void ChosenDies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                // We remove all the items in the ChosenDiesWithSelection but without a clear to avoid the memory leaks
                ChosenDiesWithSelection.ToList().ForEach(d => ChosenDiesWithSelection.Remove(d));
            }

            if (!(e.NewItems is null))
            {
                foreach (var newItem in e.NewItems)
                {
                    var newDieIndexWithSelection = new DieIndexWithSelectionVM(newItem as DieIndex, false, ChosenDiesWithSelection);
                    ChosenDiesWithSelection.Add(newDieIndexWithSelection);
                }
            }
            if (!(e.OldItems is null))
            {
                foreach (var oldItem in e.OldItems)
                {
                    var dieIndexToRemove = oldItem as DieIndex;
                    var dieIndexWithSelection = ChosenDiesWithSelection.FirstOrDefault(di => (di.Index.Row == dieIndexToRemove.Row) && (di.Index.Column == dieIndexToRemove.Column));
                    if (!(dieIndexWithSelection is null))
                        ChosenDiesWithSelection.Remove(dieIndexWithSelection);
                }
            }
            IsModified = true;
            UpdateEstimatedAnalysesTime();
        }

        private void UpdateEstimatedAnalysesTime()
        {
            if (SuspendEstimatedTimeUpdate)
                return;

            var mapper = ClassLocator.Default.GetInstance<Mapper>();
            var recipe = mapper.AutoMap.Map<ANARecipe>(EditedRecipe);
            recipe.Dies = ChosenDies.ToList<DieIndex>();
            EstimatedAnalysesTime = ServiceLocator.ANARecipeSupervisor.GetEstimatedTime(recipe);
        }

        private ObservableCollection<DieIndexWithSelectionVM> _chosenDiesWithSelection;

        // This is used by the listbox
        public ObservableCollection<DieIndexWithSelectionVM> ChosenDiesWithSelection
        {
            get
            {
                if (_chosenDiesWithSelection is null)
                {
                    _chosenDiesWithSelection = new ObservableCollection<DieIndexWithSelectionVM>();
                    _chosenDiesWithSelection.CollectionChanged += ChosenDiesWithSelection_CollectionChanged;
                }
                return _chosenDiesWithSelection;
            }
            set
            {
                if (_chosenDiesWithSelection != value)
                {
                    _chosenDiesWithSelection = value;
                    OnPropertyChanged();
                }
            }
        }

        private void ChosenDiesWithSelection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                throw new Exception("Clear must not be used to avoid memory leaks");
            }

            if (!(e.NewItems is null))
            {
                foreach (var newItem in e.NewItems)
                {
                    var newDieIndex = newItem as DieIndexWithSelectionVM;
                    newDieIndex.PropertyChanged += NewDieIndex_PropertyChanged;
                }
            }

            if (!(e.OldItems is null))
            {
                foreach (var oldItem in e.OldItems)
                {
                    var dieIndexToRemove = oldItem as DieIndexWithSelectionVM;
                    dieIndexToRemove.PropertyChanged -= NewDieIndex_PropertyChanged;
                }
            }

            IsModified = true;
            OnPropertyChanged(nameof(AreAllChosenDiesSelected));
        }

        private void NewDieIndex_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AreAllChosenDiesSelected));
        }

        private bool? _areAllChosenDiesSelected = false;

        public bool? AreAllChosenDiesSelected
        {
            get
            {
                if (!ChosenDiesWithSelection.Any(m => m.IsSelected))
                    _areAllChosenDiesSelected = false;
                else
                {
                    if (ChosenDiesWithSelection.Any(m => !m.IsSelected))
                        _areAllChosenDiesSelected = null;
                    else
                        _areAllChosenDiesSelected = true;
                }

                return _areAllChosenDiesSelected;
            }

            set
            {
                if (_areAllChosenDiesSelected == value)
                {
                    return;
                }

                _areAllChosenDiesSelected = value;
                foreach (var dieIndex in ChosenDiesWithSelection)
                {
                    dieIndex.IsSelected = (bool)_areAllChosenDiesSelected;
                }

                OnPropertyChanged(nameof(AreAllChosenDiesSelected));
            }
        }

        private Rect _waferViewPort = new Rect(0, 0, 10, 10);

        public Rect WaferViewPort
        {
            get => _waferViewPort; set { if (_waferViewPort != value) { _waferViewPort = value; OnPropertyChanged(); } }
        }

        private double _viewBoxFactor = 1;

        public double ViewBoxFactor
        {
            get => _viewBoxFactor; set { if (_viewBoxFactor != value) { _viewBoxFactor = value; OnPropertyChanged(); } }
        }

        private Rect _movedWaferViewPort = new Rect(0, 0, 10, 10);

        public Rect MovedWaferViewPort
        {
            get => _movedWaferViewPort;
            set
            {
                if (_movedWaferViewPort != value)
                {
                    _movedWaferViewPort = value;

                    WaferMapViewPortPosition = new Point(-_movedWaferViewPort.Left * WaferMapDisplayControl.DisplayResolution * WaferMapViewScale, -_movedWaferViewPort.Top * WaferMapDisplayControl.DisplayResolution * WaferMapViewScale);
                    OnPropertyChanged();
                }
            }
        }

        private Rect _mainWaferViewPort = new Rect(0, 0, 10, 10);

        public Rect MainWaferViewPort
        {
            get => _mainWaferViewPort;
            set
            {
                if (_waferViewPort != value)
                {
                    _mainWaferViewPort = value;
                    WaferViewPort = new Rect(_mainWaferViewPort.X / WaferMapDisplayControl.DisplayResolution, _mainWaferViewPort.Y / WaferMapDisplayControl.DisplayResolution, _mainWaferViewPort.Width / WaferMapDisplayControl.DisplayResolution, _mainWaferViewPort.Height / WaferMapDisplayControl.DisplayResolution);
                    //Console.WriteLine($"MainWaferViewPort : {_mainWaferViewPort.X} : {_mainWaferViewPort.Y}");
                    OnPropertyChanged();
                }
            }
        }

        private Point _waferMapViewPortPosition = new Point(0, 0);

        public Point WaferMapViewPortPosition
        {
            get => _waferMapViewPortPosition;
            set
            {
                if (_waferMapViewPortPosition != value)
                {
                    _waferMapViewPortPosition = value;
                    //Console.WriteLine($"WaferMapViewPortPosition : {_waferMapViewPortPosition.X} : {_waferMapViewPortPosition.Y}");
                    OnPropertyChanged();
                }
            }
        }

        private double _waferMapViewScale = 0;

        public double WaferMapViewScale
        {
            get => _waferMapViewScale; set { if (_waferMapViewScale != value) { _waferMapViewScale = value; OnPropertyChanged(); } }
        }

        private double _zoomFactor = 1;

        public double ZoomFactor
        {
            get => _zoomFactor; set { if (_zoomFactor != value) { _zoomFactor = value; WaferMapViewScale = 1 / value; OnPropertyChanged(); } }
        }

        public WaferMapResult WaferMap => EditedRecipe?.WaferMap?.WaferMapData;

        public bool CanCancelDiesSelection
        {
            get => (!(EditedRecipe.Dies is null) && (EditedRecipe.Dies.Count > 0));
        }

        private string _validationErrorMessage = string.Empty;

        public string ValidationErrorMessage
        {
            get => _validationErrorMessage; set { if (_validationErrorMessage != value) { _validationErrorMessage = value; OnPropertyChanged(); } }
        }

        #endregion Properties

        #region RelayCommands

        private AutoRelayCommand _deleteSelectedDies;

        public AutoRelayCommand DeleteSelectedDies
        {
            get
            {
                return _deleteSelectedDies ?? (_deleteSelectedDies = new AutoRelayCommand(
                    () =>
                    {
                        SuspendEstimatedTimeUpdate = true;
                        foreach (var selectedDie in ChosenDiesWithSelection.Where(s => s.IsSelected).ToList())
                        {
                            ChosenDies.Remove(selectedDie.Index);
                            OnPropertyChanged(nameof(AreAllChosenDiesSelected));
                        }
                        SuspendEstimatedTimeUpdate = false;
                    },
                    () => { return AreAllChosenDiesSelected != false; }
                ));
            }
        }

        private AutoRelayCommand<DieIndexWithSelectionVM> _deleteChosenDie;

        public AutoRelayCommand<DieIndexWithSelectionVM> DeleteChosenDie
        {
            get
            {
                return _deleteChosenDie ?? (_deleteChosenDie = new AutoRelayCommand<DieIndexWithSelectionVM>(
                    (dieIndexToDelete) =>
                    {
                        ChosenDies.Remove(dieIndexToDelete.Index);
                        OnPropertyChanged(nameof(AreAllChosenDiesSelected));
                    },
                    (dieIndexToDelete) => { return true; }
                ));
            }
        }

        private AutoRelayCommand<double> _presetChooseDies;

        public AutoRelayCommand<double> PresetChooseDies
        {
            get
            {
                return _presetChooseDies ?? (_presetChooseDies = new AutoRelayCommand<double>(
                    (percentageDies) =>
                    {
                        SuspendEstimatedTimeUpdate = true;
                        ChosenDies.Clear();
                        DieIndexSelector.FillDiesNoDuplicates(ChosenDies, DieIndexSelector.SelectPercentageOfDiesOnGrid(WaferMap, percentageDies));
                        SuspendEstimatedTimeUpdate = false;

                        OnPropertyChanged(nameof(AreAllChosenDiesSelected));
                    },
                    (percentageDies) => { return true; }
                ));
            }
        }

        private AutoRelayCommand _validateDiesSelection;

        public AutoRelayCommand ValidateDiesSelection
        {
            get
            {
                return _validateDiesSelection ?? (_validateDiesSelection = new AutoRelayCommand(
                    () =>
                    {
                        if (IsModified)
                        {
                            IsValidated = true;
                            SaveRecipeData();
                        }
                        ServiceLocator.NavigationManager.NavigateToNextPage();
                    },
                    () => { UpdateValidationErrorMessage(); return IsReadyToValidate; }
                ));
            }
        }

        private AutoRelayCommand _cancelDiesSelection;

        public AutoRelayCommand CancelDiesSelection
        {
            get
            {
                return _cancelDiesSelection ?? (_cancelDiesSelection = new AutoRelayCommand(
                    () =>
                    {
                        LoadRecipeData();
                    },
                    () => { return !(EditedRecipe.Dies is null) && IsModified; }
                ));
            }
        }

        #endregion RelayCommands

        #region INavigable

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (IsReadyToValidate && IsModified && !forceClose)
            {
                var result = ServiceLocator.DialogService.ShowMessageBox("The dies selection has not been validated. Do you really want to leave ?", "Dies Selection", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }

            // We restore the last validated state
            LoadRecipeData();
            IsModified = false;
            return true;
        }



        #endregion INavigable
    }

    public class CalculateMinScaleConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var width = (double)values[0];
            var height = (double)values[1];

            var minScale = Math.Min(width / WaferMapDisplayControl.DisplayResolution, height / WaferMapDisplayControl.DisplayResolution);

            return minScale;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
