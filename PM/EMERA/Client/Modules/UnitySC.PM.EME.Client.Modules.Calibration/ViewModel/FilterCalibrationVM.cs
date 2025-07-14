using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.EME.Client.Proxy.Dispatcher;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public class FilterCalibrationVM : CalibrationWizardStepBaseVM
    {
        private readonly IAlgoSupervisor _algoSupervisor;
        public FilterWheelBench FilterWheelBench { get; }
        private readonly IDialogOwnerService _dialogOwnerService;
        private readonly IDispatcher _dispatcher;

        public FilterCalibrationVM(FilterWheelBench filterWheelBench, ICalibrationService calibrationService, IAlgoSupervisor algoSupervisor, IDialogOwnerService dialogOwnerService, IDispatcher dispatcher)
            : base("Filter", calibrationService)
        {
            FilterWheelBench = filterWheelBench;
            _algoSupervisor = algoSupervisor;
            _dialogOwnerService = dialogOwnerService;
            _dispatcher = dispatcher;
            _algoSupervisor.FilterCalibrationChangedEvent += UpdateResult;           
        }       
        private void UpdateResult(FilterCalibrationResult result)
        {
            Result = result;
            if (result.Filters == null)
            {
                if (result.Status.IsFinished)
                {
                    IsBusy = false;
                }
                return;
            }
            _dispatcher.Invoke(() =>
            {
                foreach (var newFilter in result.Filters)
                {
                    var existingFilter = CalibrationResult.FirstOrDefault(f => f.Position == newFilter.Position);

                    if (existingFilter != null)
                    {
                        var index = CalibrationResult.IndexOf(existingFilter);
                        CalibrationResult[index] = new FilterVM(index, newFilter);
                    }
                    else
                    {
                        CalibrationResult.Add(new FilterVM(newFilter));
                    }
                }
                HasChanged = true;
            });

            IsBusy = !result.Status.IsFinished;
        }
        private void AFCameraVM_AFCameraChangedEvent(AutoFocusCameraResult afCameraResult)
        {
            UpdateAFCameraResult(afCameraResult);
        }
        private void UpdateAFCameraResult(AutoFocusCameraResult afCameraResult)
        {
            _dispatcher.Invoke(() =>
            {
                CameraAFResult = afCameraResult;
                if (afCameraResult.Status.IsFinished)
                {
                    CurrentCalibrationFilter.DistanceOnFocus = afCameraResult.SensorDistance;
                    var index = CalibrationResult.IndexOf(CurrentCalibrationFilter);
                    if (index >= 0)
                    {
                        CalibrationResult[index] = CurrentCalibrationFilter;
                    }
                    _algoSupervisor.AutoFocusCameraChangedEvent -= AFCameraVM_AFCameraChangedEvent;
                    IsBusy = false;
                }
            });
        }

        public override void Init()
        {
            HasChanged = false;
            var filterData = LoadCalibrationData<FilterData>() ?? new FilterData();
            if (!filterData.Filters.IsNullOrEmpty())
            {
                var filters = filterData.Filters.Select((x, i) => new FilterVM(i, x)).ToList();
                Filters = new ObservableCollection<FilterVM>(filters);
                CurrentFilter = Filters.FirstOrDefault();
                CalibrationResult = new ObservableCollection<FilterVM>(filters);
                Result = new FilterCalibrationResult()
                {
                    Filters = Filters.Select(x => x.Item).ToList(),
                    Status = new FlowStatus()
                    {
                        Message = "Loaded from previous calibration",
                        State = FlowState.Success
                    }
                };
            }
            else
            {
                ReloadDefaultFilters();
            }
        }

        public override bool CanCancelChanges() => true;


        public override void CancelChanges()
        {
            Init();
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (HasChanged && !forceClose)
            {
                var dialogRes = _dialogOwnerService.ShowMessageBox("The Filter Calibration has changed. Do you really want to quit without saving ?", "Filter Calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (dialogRes == MessageBoxResult.Yes)
                {
                    Init();
                    return true;
                }
                return false;
            }
            return true;
        }
        private bool CanExecuteAutofocus(FilterVM filterVM)
        {
            return filterVM != null;
        }
        private void PerformAutofocus(FilterVM filterVM)
        {
            _algoSupervisor.AutoFocusCameraChangedEvent += AFCameraVM_AFCameraChangedEvent;
            BusyMessage = $"Autofocus for {filterVM.Name} in progress...";
            IsBusy = true;
            CameraAFResult = null;
            FilterWheelBench.Move(filterVM.Position);
            var afCameraInput = new AutoFocusCameraInput(ScanRangeType.Small);
            _algoSupervisor.StartAutoFocusCamera(afCameraInput);
        }        
        private bool CanExecuteDeleteCalibrationItem(FilterVM filterVM)
        {
            return filterVM != null;
        }
        private void PerformDeleteCalibrationItem(FilterVM filterVM)
        {
            var messageBoxResult = _dialogOwnerService.ShowMessageBox(
                            $"Are you sure you want to delete {filterVM.Name} filter ?",
                            "Remove items Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question,
                            MessageBoxResult.No);

            if (messageBoxResult != MessageBoxResult.Yes)
                return;
            CalibrationResult.Remove(filterVM);
        }
        
        public override bool CanSave()
        {
            return HasChanged && (Result?.Status.State == FlowState.Success);           
        }

        public override void Save()
        {
            List<Filter> filterList = CalibrationResult.Select(filterVM => filterVM.Item).ToList();
            var calibration = new FilterData { Filters = filterList };
            _calibrationService.SaveCalibration(calibration);
            HasChanged = false;
            IsReadyToValidate = true;
            ValidateAndEnableNextPage();
        }

        public override void Dispose(bool manualDisposing)
        {
            if (manualDisposing)
            {
                _algoSupervisor.FilterCalibrationChangedEvent -= UpdateResult;
                UnsubscribeFromCollection(_calibrationResult);
            }           
        }

        private ObservableCollection<FilterVM> _filters;

        public ObservableCollection<FilterVM> Filters
        {
            get => _filters;
            set => SetProperty(ref _filters, value);
        }

        private FilterVM _currentFilter;

        public FilterVM CurrentFilter
        {
            get { return _currentFilter; }
            set
            {
                if (_currentFilter != value && value != null)
                {
                    _currentFilter = value;
                    _ = FilterWheelBench.MoveAsync(CurrentFilter.Position);
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand<FilterVM> _validateFilterCommand;

        public RelayCommand<FilterVM> ValidateFilterCommand =>
            _validateFilterCommand ?? (_validateFilterCommand = new RelayCommand<FilterVM>(ValidateFilter));


        private void ValidateFilter(FilterVM filter)
        {
            if (filter.Name.IsNullOrEmpty())
                return;

            if (Filters.Count(x => x.Name == filter.Name) > 1)
            {
                _dialogOwnerService.ShowMessageBox("A filter with the same name already exists", "Filter", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return;
            }

            filter.Position = FilterWheelBench.RotationPosition;
            filter.InstallationState = FilterInstallationState.Validated;
            var existingFilter = CalibrationResult.FirstOrDefault(f => f.Name == filter.Name);
            if (existingFilter != null)
            {
                var index = CalibrationResult.IndexOf(existingFilter);
                CalibrationResult[index] = filter;
            }
            else
            {
                CalibrationResult.Add(filter);
            }
            OnPropertyChanged(nameof(CalibrationResult));
            HasChanged = true;
            StartCalibration.NotifyCanExecuteChanged();
        }

        private IRelayCommand<double> _move;

        public IRelayCommand<double> Move
        {
            get
            {
                return _move ?? (_move = new RelayCommand<double>((position) => FilterWheelBench.Move(position)));
            }
        }

        private IRelayCommand<FilterVM> _moveLeft;
        public IRelayCommand<FilterVM> MoveLeft
        {
            get
            {
                if (_moveLeft == null)
                    _moveLeft = new RelayCommand<FilterVM>(PerformMoveLeft);

                return _moveLeft;
            }
        }

        private void PerformMoveLeft(FilterVM filter)
        {
            filter.Position = FilterWheelBench.GetCurrentPosition() - (FilterWheelBench.AxisConfiguration.PositionMax.Value / 180);
            FilterWheelBench.Move(filter.Position);
            MarkFilterAsModified(filter);
        }

        private static void MarkFilterAsModified(FilterVM filter)
        {
            filter.InstallationState = FilterInstallationState.Missing;
        }

        private IRelayCommand<FilterVM> _moveRight;

        public IRelayCommand<FilterVM> MoveRight
        {
            get
            {
                if (_moveRight == null)
                    _moveRight = new RelayCommand<FilterVM>(PerformMoveRight);

                return _moveRight;
            }
        }

        private void PerformMoveRight(FilterVM filter)
        {
            filter.Position = FilterWheelBench.GetCurrentPosition() + (FilterWheelBench.AxisConfiguration.PositionMax.Value / 180);
            FilterWheelBench.Move(filter.Position);
            MarkFilterAsModified(filter);
        }

        private AutoRelayCommand _reloadDefaultCalibration;

        public AutoRelayCommand ReloadDefaultCalibration =>
            _reloadDefaultCalibration ?? (_reloadDefaultCalibration = new AutoRelayCommand(ReloadDefaultFilters));

        private void ReloadDefaultFilters()
        {
            var dialogRes = _dialogOwnerService.ShowMessageBox("Do you really want to reload the default calibration and erase the current one?", 
                                                               "Calibration", 
                                                               MessageBoxButton.YesNo, 
                                                               MessageBoxImage.Question, 
                                                               MessageBoxResult.No);
            if (dialogRes == MessageBoxResult.No)
            {
                return;
            }
            var filters = FilterWheelBench.GetFilterSlots().Select((x, i) => new FilterVM(i, x)).ToList();
            Filters = new ObservableCollection<FilterVM>(filters);
            CurrentFilter = Filters?.FirstOrDefault();
            CalibrationResult.Clear();
            IsReadyToValidate = false;
        }

        private bool _isReadyToValidate;
        public bool IsReadyToValidate
        {
            get => _isReadyToValidate;
            set
            {
                SetProperty(ref _isReadyToValidate, value);
                UpdateAllCanExecutes();
            }
        }

        private AutoRelayCommand _skipCommand;
        public AutoRelayCommand SkipCommand
        {
            get
            {
                return _skipCommand ?? (_skipCommand = new AutoRelayCommand(
                    () =>
                    {
                        IsValidated = true;
                        NavigationManager.NavigateToNextPage();
                    }));
            }
        }

        private AutoRelayCommand _validateChoice;
        public AutoRelayCommand ValidateChoice
        {
            get
            {
                return _validateChoice ?? (_validateChoice = new AutoRelayCommand(
                    () =>
                    {
                        IsValidated = true;
                        NavigationManager.NavigateToNextPage();
                    },
                    () => IsReadyToValidate));
            }
        }

        private RelayCommand _startCalibration;

        public IRelayCommand StartCalibration
        {
            get
            {
                if (_startCalibration == null)
                    _startCalibration = new RelayCommand(PerformStartCalibration, CanStartCalibration);

                return _startCalibration;
            }
        }

        private void PerformStartCalibration()
        {
            BusyMessage = "Filters calibration in progress...";
            Result = null;
            IsBusy = true;
            var filters = _filters.ToList().ConvertAll(filter => filter.Item);
            var input = new FilterCalibrationInput { Filters = filters };
            _algoSupervisor.StartFilterCalibration(input);
        }

        private bool CanStartCalibration()
        {
            return _filters.All(filter => filter.InstallationState == FilterInstallationState.Validated);
        }       
        private ObservableCollection<FilterVM> _calibrationResult = new ObservableCollection<FilterVM>();

        public ObservableCollection<FilterVM> CalibrationResult
        {
            get => _calibrationResult;
            set
            {
                if (SetProperty(ref _calibrationResult, value))
                {
                    SubscribeToCollection(value);
                    UpdateAllCanExecutes();
                }
            }
        }
        private FilterVM _currentCalibrationFilter;
        public FilterVM CurrentCalibrationFilter
        {
            get => _currentCalibrationFilter;
            set => SetProperty(ref _currentCalibrationFilter, value);
        }
        private AutoFocusCameraResult _cameraAfResult;
        public AutoFocusCameraResult CameraAFResult
        {
            get => _cameraAfResult;
            set => SetProperty(ref _cameraAfResult, value);
        }
        private FilterCalibrationResult _result;
        public FilterCalibrationResult Result
        {
            get => _result;
            set
            {
                SetProperty(ref _result, value);
                UpdateAllCanExecutes();
            }
        }
        private RelayCommand _cancelCalibration;

        public IRelayCommand CancelCalibrationCalibration
        {
            get
            {
                if (_cancelCalibration == null)
                {
                    _cancelCalibration = new RelayCommand(PerformCancelCalibration);
                }

                return _cancelCalibration;
            }
        }

        private void PerformCancelCalibration()
        {
            IsBusy = false;
            _algoSupervisor.CancelPixelSizeComputation();
            _algoSupervisor.CancelAutoFocusCamera();
        }

        private AutoRelayCommand _saveFilter;

        public AutoRelayCommand SaveFilter
        {
            get
            {
                if (_saveFilter == null)
                    _saveFilter = new AutoRelayCommand(Save, CanSave);

                return _saveFilter;
            }
        }
        private AutoRelayCommand<FilterVM> _deleteCalibrationItem;
        public AutoRelayCommand<FilterVM> DeleteCalibrationItem
        {
            get
            {
                if (_deleteCalibrationItem == null)
                    _deleteCalibrationItem = new AutoRelayCommand<FilterVM>(PerformDeleteCalibrationItem, CanExecuteDeleteCalibrationItem);

                return _deleteCalibrationItem;
            }
        }        
        private AutoRelayCommand<FilterVM> _runAutoFocus;
        

        public AutoRelayCommand<FilterVM> RunAutoFocus
        {
            get
            {
                if (_runAutoFocus == null)
                    _runAutoFocus = new AutoRelayCommand<FilterVM>(PerformAutofocus, CanExecuteAutofocus);

                return _runAutoFocus;
            }
        }
        private void SubscribeToCollection(ObservableCollection<FilterVM> collection)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    item.PropertyChanged += OnFilterVMPropertyChanged;
                }
                collection.CollectionChanged += OnCalibrationResultCollectionChanged;
            }
        }
        private void UnsubscribeFromCollection(ObservableCollection<FilterVM> collection)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    item.PropertyChanged -= OnFilterVMPropertyChanged;
                }
                collection.CollectionChanged -= OnCalibrationResultCollectionChanged;
            }
        }
        private void OnCalibrationResultCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (FilterVM newItem in e.NewItems)
                {
                    newItem.PropertyChanged += OnFilterVMPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (FilterVM oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= OnFilterVMPropertyChanged;
                }
            }
        }
        private void OnFilterVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is FilterVM filter)
            {                
                HasChanged = true;                                
            }
        }
    }
}
