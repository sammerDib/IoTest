using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Core.Referentials;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun
{
    public class RecipeRunDashboardVM : TabViewModelBase, IDisposable
    {
        private RecipeRunVM _recipeRunVM;
       
        private ReferentialSupervisor _referentialSupervisor;
        private WaferReferentialSettings _waferRefBeforeRecipeRun;
        private bool _isInitDone = false;

        public RecipeRunDashboardVM(ANARecipeVM editedRecipe, RecipeRunVM recipeRunVM, bool isLiveViewMode = false) : this(isLiveViewMode)
        {
            _recipeRunVM = recipeRunVM;
            EditedRecipe = editedRecipe;

            if (EditedRecipe.Execution is null)
            {
                EditedRecipe.Execution = new ExecutionSettings();
            }
            else
            {
                if (!(editedRecipe.Execution.Alignment is null))
                {
                    RecipeRunSettings.IsAutofocusUsed = EditedRecipe.Execution.Alignment.RunAutoFocus;
                    RecipeRunSettings.IsEdgeAlignmentUsed = EditedRecipe.Execution.Alignment.RunBwa;
                    RecipeRunSettings.IsMarkAlignmentUsed = EditedRecipe.Execution.Alignment.RunMarkAlignment;
                }
            }

            _isInitDone = true;
        }

        private ChuckSupervisor _chuckSupervisor;

        public ChuckSupervisor ChuckSupervisor
        {
            get
            { 
                if (_chuckSupervisor is null)
                    _chuckSupervisor= ClassLocator.Default.GetInstance<ChuckSupervisor>();
                return _chuckSupervisor;
            }
            set 
            { 
                if (_chuckSupervisor != value) 
                { 
                    _chuckSupervisor = value; 
                    OnPropertyChanged(); 
                } 
            }
        }

        public RecipeRunDashboardVM(bool isLiveViewMode = false)
        {
            Title = "Dashboard";
            IsEnabled = true;
            IsLiveViewMode = isLiveViewMode;
           
            _referentialSupervisor = ClassLocator.Default.GetInstance<ReferentialSupervisor>();
            RecipeRunSettings.PropertyChanged += RecipeRunSettings_PropertyChanged;
            MeasurePointsResults.CollectionChanged += MeasurePointsResults_CollectionChanged;
            ServiceLocator.ANARecipeSupervisor.RecipeProgressChangedEvent += ANARecipeSupervisor_RecipeProgressChangedEventAsync;
            ServiceLocator.ANARecipeSupervisor.MeasureResultChangedEvent += ANARecipeSupervisor_MeasureResultChangedEvent;
        }

        private void MeasurePointsResults_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(e.NewItems is null) && (e.NewItems.Count > 0) && (!IsDisplayingLastResult))
                NumberOfNotSeenResults += e.NewItems.Count;
        }

        #region Properties

        private bool _isOneRecipeAlreadyStarted = false;

        public bool IsOneRecipeAlreadyStarted
        {
            get => _isOneRecipeAlreadyStarted;
            set => SetProperty(ref _isOneRecipeAlreadyStarted, value);
        }

        private ObservableCollection<MeasurePointResultVM> _measurePointsResults = new ObservableCollection<MeasurePointResultVM>();

        public ObservableCollection<MeasurePointResultVM> MeasurePointsResults
        {
            get => _measurePointsResults;
            set => SetProperty(ref _measurePointsResults, value);
        }

        public RecipeRunSettingsVM RecipeRunSettings { get; set; } = new RecipeRunSettingsVM();

        private ANARecipeVM _editedRecipe;

        public ANARecipeVM EditedRecipe
        {
            get => _editedRecipe;
            set => SetProperty(ref _editedRecipe, value);
        }

        private string _dataflowRecipeName;

        public string DataflowRecipeName
        {
            get => _dataflowRecipeName;
            set => SetProperty(ref _dataflowRecipeName, value);
        }

        private string _jobId;

        public string JobId
        {
            get => _jobId;
            set => SetProperty(ref _jobId, value);
        }

        private bool _isLiveViewMode;

        public bool IsLiveViewMode
        {
            get => _isLiveViewMode;
            set => SetProperty(ref _isLiveViewMode, value);
        }

        private List<Point> _measurePoints;

        public List<Point> MeasurePoints
        {
            get => _measurePoints;
            set => SetProperty(ref _measurePoints, value);
        }

        private Point? _currentPoint;

        public Point? CurrentPoint
        {
            get => _currentPoint;
            set => SetProperty(ref _currentPoint, value);
        }

        private int? _sequenceNumber = null;

        public int? SequenceNumber
        {
            get => _sequenceNumber;
            set => SetProperty(ref _sequenceNumber, value);
        }

        private MeasurePointInfo _currentPointInfo = null;

        public MeasurePointInfo CurrentPointInfo
        {
            get
            {
                return _currentPointInfo;
            }
            set
            {
                if (_currentPointInfo == value)
                {
                    return;
                }
                _currentPointInfo = value;
                var measurePoint = EditedRecipe.Points.Find(x => x.Id == _currentPointInfo.PointDataIndex);
                var referential = ReferentialHelper.CreateDieOrWaferReferential(value.Die);
                var position = new XYZTopZBottomPosition(referential, measurePoint.Position.X, measurePoint.Position.Y, double.NaN, double.NaN);
                var positionOnWafer = ServiceLocator.ReferentialSupervisor.ConvertTo(position, ReferentialTag.Wafer)?.Result.ToXYPosition();
                System.Diagnostics.Debug.WriteLine("DEBUG : CurrentPointInfo positionOnWafer");
                if (positionOnWafer != null)
                {
                    System.Diagnostics.Debug.WriteLine("DEBUG : CurrentPointInfo positionOnWafer Not null");
                    CurrentPoint = new Point(positionOnWafer.X, positionOnWafer.Y);
                }

                OnPropertyChanged();
            }
        }

        private Point? _currentSelectedPoint;
        public Point? CurrentSelectedPoint
        {
            get => _currentSelectedPoint;
            set => SetProperty(ref _currentSelectedPoint, value);
        }

        private MeasurePointResultVM _selectedPoint;

        public MeasurePointResultVM SelectedPoint
        {
            get => _selectedPoint;
            set
            {
                if (_selectedPoint == value)
                {
                    return;
                }
                _selectedPoint = value;
                if (!(_selectedPoint is null))
                {
                    var measurePoint = EditedRecipe.Points.Find(x => x.Id == _selectedPoint.PointIndex);
                    if (measurePoint != null)
                    {
                        var referential = ReferentialHelper.CreateDieOrWaferReferential(value.DieIndex);
                        var position = new XYZTopZBottomPosition(referential, measurePoint.Position.X, measurePoint.Position.Y, double.NaN, double.NaN);
                        var positionOnWafer = ServiceLocator.ReferentialSupervisor.ConvertTo(position, ReferentialTag.Wafer)?.Result.ToXYPosition();
                        if (positionOnWafer != null)
                        {
                            CurrentSelectedPoint = new Point(positionOnWafer.X, positionOnWafer.Y);
                        }
                    }

                    IsDisplayingLastResult = false;
                }
                else
                { 
                    CurrentSelectedPoint = null; 
                }

                OnPropertyChanged();
            }
        }

        private bool _isRecipeRunning = false;

        public bool IsRecipeRunning
        {
            get => _isRecipeRunning;
            set
            {
                if (_isRecipeRunning != value)
                {
                    _isRecipeRunning = value;

                    if (_isRecipeRunning)
                    {
                        IsDisplayingLastResult = true;
                        IsOneRecipeAlreadyStarted = true;
                    }
                    else
                    {
                        // AxesVM.WaferMap is reset on recipe load in case of "RunRecipe Mode"
                        // But if RecipeRunDashboardVM has been instanciated from "LiveView Mode", AxesVM.WaferMap is never reset
                        // This can lead to referential exception on position convert
                        // => ONLY in case of "LiveView Mode", we reset AxesVM.WaferMap as IsRecipeRunning is set to false
                        // (IsRecipeRunning is set to false on recipe success, error or cancel)
                        if (IsLiveViewMode)
                        {
                            Application.Current?.Dispatcher.Invoke(() =>
                            {
                                ServiceLocator.AxesSupervisor.AxesVM.WaferMap = null;
                            });
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }

        private bool _isRecipePaused = false;

        public bool IsRecipePaused
        {
            get => _isRecipePaused;
            set => SetProperty(ref _isRecipePaused, value);
        }

        private bool _isRecipeReallyPaused = false;

        public bool IsRecipeReallyPaused
        {
            get => _isRecipeReallyPaused;
            set => SetProperty(ref _isRecipeReallyPaused, value);
        }

        private bool _hideDieIndex = false;

        public bool HideDieIndex
        {
            get => _hideDieIndex;
            set => SetProperty(ref _hideDieIndex, value);
        }

        private bool _isRecipeReadyToStart = true;

        public bool IsRecipeReadyToStart
        {
            get => _isRecipeReadyToStart;
            set => SetProperty(ref _isRecipeReadyToStart, value);
        }

        private bool _displayProgress = false;

        public bool DisplayProgress
        {
            get => _displayProgress;
            set => SetProperty(ref _displayProgress, value);
        }

        private double _recipeExecutionProgress = 0;

        public double RecipeExecutionProgress
        {
            get => _recipeExecutionProgress;
            set => SetProperty(ref _recipeExecutionProgress, value);
        }

        private TimeSpan _estimatedExecutionTime = new TimeSpan();

        public TimeSpan EstimatedExecutionTime
        {
            get => _estimatedExecutionTime;
            set => SetProperty(ref _estimatedExecutionTime, value);
        }

        private TimeSpan _remainingExecutionTime = new TimeSpan();

        public TimeSpan RemainingExecutionTime
        {
            get => _remainingExecutionTime;
            set => SetProperty(ref _remainingExecutionTime, value);
        }

        private int _notMeasuredPointsCount = 0;

        public int NotMeasuredPointsCount
        {
            get => _notMeasuredPointsCount;
            set => SetProperty(ref _notMeasuredPointsCount, value);
        }

        private int _successMeasuredPointsCount = 0;

        public int SuccessMeasuredPointsCount
        {
            get => _successMeasuredPointsCount;
            set => SetProperty(ref _successMeasuredPointsCount, value);
        }

        private int _errorMeaseuredPointsCount = 0;

        public int ErrorMeaseuredPointsCount
        {
            get => _errorMeaseuredPointsCount;
            set => SetProperty(ref _errorMeaseuredPointsCount, value);
        }

        private int _measureResultCount = 0;

        public int MeasureResultCount
        {
            get => _measureResultCount;
            set => SetProperty(ref _measureResultCount, value);
        }

        private double _notMeasuredPointPercent = 0;

        public double NotMeasuredPointPercent
        {
            get => _notMeasuredPointPercent;
            set => SetProperty(ref _notMeasuredPointPercent, value);
        }

        private double _succeedMeasuredPointsPercent = 0;

        public double SucceedMeasuredPointsPercent
        {
            get => _succeedMeasuredPointsPercent;
            set => SetProperty(ref _succeedMeasuredPointsPercent, value);
        }

        private double _errorMeasuredPointsPercent = 0;

        public double ErrorMeasuredPointsPercent
        {
            get => _errorMeasuredPointsPercent;
            set => SetProperty(ref _errorMeasuredPointsPercent, value);
        }

        private bool _isDisplayingLastResult = true;

        public bool IsDisplayingLastResult
        {
            get => _isDisplayingLastResult;
            set
            {
                if (_isDisplayingLastResult != value)
                {
                    _isDisplayingLastResult = value;
                    if (_isDisplayingLastResult)
                        NumberOfNotSeenResults = 0;
                    OnPropertyChanged();
                }
            }
        }

        private int _numberOfNotSeenResults = 0;

        public int NumberOfNotSeenResults
        {
            get => _numberOfNotSeenResults;
            set => SetProperty(ref _numberOfNotSeenResults, value);
        }

        private int _lastVisibleResultIndex = 0;

        public int LastVisibleResultIndex
        {
            get
            {
                return _lastVisibleResultIndex;
            }

            set
            {
                if (_lastVisibleResultIndex == value)
                {
                    return;
                }

                _lastVisibleResultIndex = value;
                if (_lastVisibleResultIndex != MeasurePointsResults.Count)
                    IsDisplayingLastResult = false;
                if (!IsDisplayingLastResult)
                    NumberOfNotSeenResults = Math.Min(NumberOfNotSeenResults, MeasurePointsResults.Count - value);
                OnPropertyChanged(nameof(LastVisibleResultIndex));
            }
        }

        private bool _skipGlobalAutoFocus = false;

        public bool SkipGlobalAutoFocus
        {
            get => _skipGlobalAutoFocus;
            set => SetProperty(ref _skipGlobalAutoFocus, value);
        }

        private bool _skipEdgesAlignment = false;

        public bool SkipEdgesAlignment
        {
            get => _skipEdgesAlignment;
            set => SetProperty(ref _skipEdgesAlignment, value);
        }

        private bool _skipMarksAlignment = false;

        public bool SkipMarksAlignment
        {
            get => _skipMarksAlignment;
            set => SetProperty(ref _skipMarksAlignment, value);
        }

        private bool _useRepeta = false;

        public bool UseRepeta
        {
            get => _useRepeta;
            set
            {
                SetProperty(ref _useRepeta, value);
                UpdateEstimatedExecutionTime(UseRepeta ? NbRuns : 1);
            }
        }

        private int _nbRuns = 2;

        public int NbRuns
        {
            get => _nbRuns;
            set
            {
                SetProperty(ref _nbRuns, value);
                UpdateEstimatedExecutionTime(UseRepeta ? NbRuns : 1);
            }
        }

        #endregion Properties

        #region Events

        private void RecipeRunSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!_isInitDone)
                return;
            UpdateEstimatedExecutionTime(UseRepeta ? NbRuns : 1);
            EditedRecipe.Execution = new ExecutionSettings()
            {
                Alignment = new AlignmentParameters()
                {
                    RunAutoFocus = RecipeRunSettings.IsAutofocusUsed,
                    RunBwa = RecipeRunSettings.IsEdgeAlignmentUsed,
                    RunMarkAlignment = RecipeRunSettings.IsMarkAlignmentUsed
                },
                Strategy = RecipeRunSettings.CurrentMeasurementStrategy
            };
            EditedRecipe.IsModified = true;
        }

        private void ANARecipeSupervisor_MeasureResultChangedEvent(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex)
        {
            _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      var inProgressMeasurePointResult = MeasurePointsResults.FirstOrDefault(mpr => mpr.IsInProgress);
                      if (!(inProgressMeasurePointResult is null))
                      {
                          inProgressMeasurePointResult.IsInProgress = false;
                          inProgressMeasurePointResult.Result = res;

                          //MeasurePointsResults.Add(new MeasurePointResultVM(inProgressMeasurePointResult.MeasureName, inProgressMeasurePointResult.PointIndex, res, resultFolderPath, dieIndex, DataDectionary, inProgressMeasure.NbOfRepeat, inProgressMeasure.NbOfRepeat, false));
                          //if (inProgressMeasurePointResult.RepeatIndex < inProgressMeasure.NbOfRepeat)
                          //{
                          //    inProgressMeasurePointResult.NbRepeat = inProgressMeasure.NbOfRepeat;
                          //    inProgressMeasurePointResult.RepeatIndex++;
                          //}
                      }
                      if (!res.IsSubMeasurePoint)
                      {
                          switch (res.State)
                          {
                              case MeasureState.Success:
                                  SuccessMeasuredPointsCount++;
                                  break;

                              case MeasureState.Error:
                                  ErrorMeaseuredPointsCount++;
                                  break;

                              case MeasureState.NotMeasured:
                                  NotMeasuredPointsCount++;
                                  break;

                              default:
                                  break;
                          }
                          MeasureResultCount = MeasurePointsResults.Count(m => !m.IsSubMeasurePoint);
                          if (MeasureResultCount > 0)
                          {
                              SucceedMeasuredPointsPercent = (int)(0.5f + ((100f * SuccessMeasuredPointsCount) / MeasureResultCount));
                              NotMeasuredPointPercent = (int)(0.5f + ((100f * NotMeasuredPointsCount) / MeasureResultCount));
                              ErrorMeasuredPointsPercent = (int)(0.5f + ((100f * ErrorMeaseuredPointsCount) / MeasureResultCount));
                          }
                      }
                  }));
        }

        private async void ANARecipeSupervisor_RecipeProgressChangedEventAsync(RecipeProgress recipeProgress)
        {
            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged Start");
            var recipeRunning= await WaitRecipeRunningAsync();
            if (!recipeRunning)
            {
                System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged Not RecipeRunning");
                return;
            }
            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged RecipeRunning");
            //TODO FDS :  handle warning : call is not awaited 
            Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    switch (recipeProgress.RecipeProgressState)
                    {
                        case RecipeProgressState.AutoFocusInProgress:
                            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged AutoFocusInProgress");
                            RecipeRunSettings.AutoFocusState.StepState = StepStates.InProgress;
                            break;

                        case RecipeProgressState.EdgeAlignmentInProgress:
                            RecipeRunSettings.AutoFocusState.StepState = StepStates.Done;
                            RecipeRunSettings.EdgeAlignmentState.StepState = StepStates.InProgress;
                            break;

                        case RecipeProgressState.MarkAlignmentInProgress:
                            RecipeRunSettings.MarkAlignmentState.StepState = StepStates.InProgress;
                            RecipeRunSettings.AutoFocusState.StepState = StepStates.Done;
                            RecipeRunSettings.EdgeAlignmentState.StepState = StepStates.Done;
                            break;
                        case RecipeProgressState.SubMeasuring:
                            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged SubMeasuring");
                            IsRecipeReallyPaused = false;
                            RecipeRunSettings.AutoFocusState.StepState = StepStates.Done;
                            RecipeRunSettings.EdgeAlignmentState.StepState = StepStates.Done;
                            RecipeRunSettings.MarkAlignmentState.StepState = StepStates.Done;
                            RecipeExecutionProgress = (double)(EstimatedExecutionTime - recipeProgress.RemainingTime).TotalSeconds * 100 / EstimatedExecutionTime.TotalSeconds;
                            RemainingExecutionTime = recipeProgress.RemainingTime;

                            // Add In progress sub measure

                            CurrentPoint = new Point(recipeProgress.PointMeasureStarted.Position.X, recipeProgress.PointMeasureStarted.Position.Y);
                            if (recipeProgress.PointMeasureStarted.Die is null)
                            {
                                HideDieIndex = true;
                            }
                            else
                            {
                                HideDieIndex = false;
                                recipeProgress.PointMeasureStarted.Die = ServiceLocator.AxesSupervisor.AxesVM.ConvertDieIndexToDieUserIndex(CurrentPointInfo.Die);
                            }
                            if (!(CurrentPoint is null))
                            {
                                var measurePointResultVM = new MeasurePointResultVM(
                                    recipeProgress.PointMeasureStarted.MeasureName,
                                    recipeProgress.PointMeasureStarted.PointDataIndex,
                                    recipeProgress.PointMeasureStarted.Die,
                                    CurrentPoint.Value,
                                    recipeProgress.PointMeasureStarted.RepeatIndex,
                                    recipeProgress.PointMeasureStarted.NbOfRepeat,
                                    true,
                                    true);
                                MeasurePointsResults.Add(measurePointResultVM);
                            }
                            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged Sub Measuring End");
                            break;

                        case RecipeProgressState.ComputeMeasureFromSubMeasures:
                            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged Compute Measure From Sub Measures");
                            IsRecipeReallyPaused = false;
                            RecipeRunSettings.AutoFocusState.StepState = StepStates.Done;
                            RecipeRunSettings.EdgeAlignmentState.StepState = StepStates.Done;
                            RecipeRunSettings.MarkAlignmentState.StepState = StepStates.Done;
                            RecipeExecutionProgress = (double)(EstimatedExecutionTime - recipeProgress.RemainingTime).TotalSeconds * 100 / EstimatedExecutionTime.TotalSeconds;
                            RemainingExecutionTime = recipeProgress.RemainingTime;

                            // Add In progress measure

                            CurrentPoint = new Point(recipeProgress.PointMeasureStarted.Position.X, recipeProgress.PointMeasureStarted.Position.Y);
                            if (recipeProgress.PointMeasureStarted.Die is null)
                            {
                                HideDieIndex = true;
                            }
                            else
                            {
                                HideDieIndex = false;
                                recipeProgress.PointMeasureStarted.Die = ServiceLocator.AxesSupervisor.AxesVM.ConvertDieIndexToDieUserIndex(CurrentPointInfo.Die);
                            }
                            if (!(CurrentPoint is null))
                            {
                                var measurePointResultVM = new MeasurePointResultVM(
                                    recipeProgress.PointMeasureStarted.MeasureName,
                                    recipeProgress.PointMeasureStarted.PointDataIndex,
                                    recipeProgress.PointMeasureStarted.Die,
                                    CurrentPoint.Value,
                                    recipeProgress.PointMeasureStarted.RepeatIndex,
                                    recipeProgress.PointMeasureStarted.NbOfRepeat,
                                    true,
                                    false);
                                var nbMeasurePointsResults = MeasurePointsResults.Count;
                                MeasurePointsResults.Add(measurePointResultVM);
                            }
                            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged Compute Measure From Sub Measures End");
                            break;

                        case RecipeProgressState.Measuring:
                            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged Measuring");
                            IsRecipeReallyPaused = false;
                            RecipeRunSettings.AutoFocusState.StepState = StepStates.Done;
                            RecipeRunSettings.EdgeAlignmentState.StepState = StepStates.Done;
                            RecipeRunSettings.MarkAlignmentState.StepState = StepStates.Done;
                            RecipeExecutionProgress = (double)(EstimatedExecutionTime - recipeProgress.RemainingTime).TotalSeconds * 100 / EstimatedExecutionTime.TotalSeconds;
                            RemainingExecutionTime = recipeProgress.RemainingTime;

                            // Add In progress measure

                            CurrentPointInfo = recipeProgress.PointMeasureStarted;
                            if (CurrentPointInfo.Die is null)
                                HideDieIndex = true;
                            else
                            {
                                HideDieIndex = false;
                                CurrentPointInfo.Die = ServiceLocator.AxesSupervisor.AxesVM.ConvertDieIndexToDieUserIndex(CurrentPointInfo.Die);
                            }
                            if (!(CurrentPoint is null))
                                MeasurePointsResults.Add(new MeasurePointResultVM(CurrentPointInfo.MeasureName, CurrentPointInfo.PointDataIndex, CurrentPointInfo.Die, CurrentPoint.Value, CurrentPointInfo.RepeatIndex, CurrentPointInfo.NbOfRepeat, true));
                            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged Measuring End");
                            break;

                        case RecipeProgressState.InPause:
                            IsRecipeReallyPaused = true;
                            PauseRecipe.NotifyCanExecuteChanged();
                            break;

                        case RecipeProgressState.Canceled:
                            ResetStatus();
                            break;

                        case RecipeProgressState.Error:
                            ResetStatus();
                            if (!IsLiveViewMode)
                                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Recipe execution failed", "Recipe execution", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;

                        case RecipeProgressState.Success:
                            System.Diagnostics.Debug.WriteLine("DEBUG : RecipeProgressChanged Success");
                            IsRecipeRunning = false;
                            IsRecipeReadyToStart = true;
                            RecipeExecutionProgress = 100;
                            _referentialSupervisor.SetSettings(_waferRefBeforeRecipeRun);
                            if (_recipeRunVM != null)
                                _recipeRunVM.EnableAllMeasures();
                            EditedRecipe.IsRunExecuted = true;
                            break;

                        default:
                            break;
                    }
                }));
            });
        }

        private void ResetStatus()
        {
            IsRecipeRunning = false;
            IsRecipeReadyToStart = true;
            RecipeRunSettings.AutoFocusState.StepState = StepStates.NotDone;
            RecipeRunSettings.EdgeAlignmentState.StepState = StepStates.NotDone;
            RecipeRunSettings.MarkAlignmentState.StepState = StepStates.NotDone;
            _referentialSupervisor.SetSettings(_waferRefBeforeRecipeRun);
        }

        private async Task<bool> WaitRecipeRunningAsync()
        {
            if (IsLiveViewMode)
            {
                // When we are in Live view mode we must wait the recipe started event is received and processed
                for (int i = 0; i < 500; i++)
                {
                    if (IsRecipeRunning)
                        return true;
                    await Task.Delay(100);
                }

                return false;
            }
            return true;
        }

        #endregion Events

        #region Commands

        private AutoRelayCommand _startRecipe;

        public AutoRelayCommand StartRecipe
        {
            get
            {
                return _startRecipe ?? (_startRecipe = new AutoRelayCommand(
                    () =>
                    {
                        if (!ChuckSupervisor.ChuckVM.Status.IsWaferClamped)
                        {
                            //The wafer must be clamped before recipe.
                            ChuckSupervisor.ClampWafer(ChuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic);
                            Thread.Sleep(1000);
                            if (ChuckSupervisor.ChuckVM.Status.IsWaferClamped)
                            {
                                BeginTheRecipe();
                            }
                            else
                            {
                                if (!IsLiveViewMode)
                                    ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("We cannot clamp the wafer", "Wafer", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
                                return;
                            }
                        }
                        else
                        {
                            BeginTheRecipe();
                        }
                    },
                    () => { return IsRecipeReadyToStart; }
                ));
            }
        }

        private void BeginTheRecipe()
        {
            try
            {
                ResetProgressState();
                IsRecipeRunning = true;
                _recipeRunVM.DisableAllMeasures();
                RemainingExecutionTime = EstimatedExecutionTime;
                var recipe = GetANARecipe();

                recipe.Execution = new ExecutionSettings()
                {
                    Alignment = new AlignmentParameters() { RunAutoFocus = RecipeRunSettings.IsAutofocusUsed && !SkipGlobalAutoFocus, RunBwa = RecipeRunSettings.IsEdgeAlignmentUsed && !SkipEdgesAlignment, RunMarkAlignment = RecipeRunSettings.IsMarkAlignmentUsed && !SkipMarksAlignment },
                    Strategy = RecipeRunSettings.CurrentMeasurementStrategy
                };

                // save wafer referential before executing the recipe
                _waferRefBeforeRecipeRun = _referentialSupervisor.GetSettings(ReferentialTag.Wafer)?.Result as WaferReferentialSettings;

                ServiceLocator.ANARecipeSupervisor.StartRecipe(recipe, UseRepeta ? NbRuns : 1);
            }
            catch (Exception ex)
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    if (!IsLiveViewMode)
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "BeginTheRecipe :Error during recipe");
                });
            }
        }

        public void ResetProgressState()
        {
            MeasurePointsResults.Clear();
            SuccessMeasuredPointsCount = 0;
            NotMeasuredPointsCount = 0;
            ErrorMeaseuredPointsCount = 0;
            SucceedMeasuredPointsPercent = 0;
            NotMeasuredPointPercent = 0;
            ErrorMeasuredPointsPercent = 0;
            ResetProgressStates();
            IsRecipeReadyToStart = false;
            IsRecipePaused = false;
            DisplayProgress = true;
            RecipeExecutionProgress = 0;
        }

        private AutoRelayCommand _pauseRecipe;

        public AutoRelayCommand PauseRecipe
        {
            get
            {
                return _pauseRecipe ?? (_pauseRecipe = new AutoRelayCommand(
                    () =>
                    {
                        IsRecipeReallyPaused = false;
                        IsRecipePaused = true;
                        ServiceLocator.ANARecipeSupervisor.PauseRecipe();
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _resumeRecipe;

        public AutoRelayCommand ResumeRecipe
        {
            get
            {
                return _resumeRecipe ?? (_resumeRecipe = new AutoRelayCommand(
                    () =>
                    {
                        IsRecipePaused = false;
                        ServiceLocator.ANARecipeSupervisor.ResumeRecipe();
                    },
                    () => { return IsRecipeReallyPaused; }
                ));
            }
        }

        private AutoRelayCommand _stopRecipe;

        public AutoRelayCommand StopRecipe
        {
            get
            {
                return _stopRecipe ?? (_stopRecipe = new AutoRelayCommand(
                    () =>
                    {
                        IsRecipePaused = false;
                        IsRecipeRunning = false;
                        ServiceLocator.ANARecipeSupervisor.StopRecipe();
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _goToLastMeasuredResult;

        public AutoRelayCommand GoToLastMeasuredResult
        {
            get
            {
                return _goToLastMeasuredResult ?? (_goToLastMeasuredResult = new AutoRelayCommand(
                    () =>
                    {
                        SelectedPoint = null;
                        IsDisplayingLastResult = true;
                    },
                    () => { return NumberOfNotSeenResults > 0; }
                ));
            }
        }

        private AutoRelayCommand _exportMeasuredPointsResults;

        public AutoRelayCommand ExportMeasuredPointsResults
        {
            get
            {
                return _exportMeasuredPointsResults ?? (_exportMeasuredPointsResults = new AutoRelayCommand(
                    () =>
                    {
                        DoExportMeasurePointsResults();
                    }
                    ,
                    () => { return MeasurePointsResults.Count() > 0; }
                ));
            }
        }

        private void DoExportMeasurePointsResults()
        {
            string csvFileName;
            var settings = new SaveFileDialogSettings
            {
                Title = "Results export",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "csv file (*.csv) | *.csv;",
                CheckFileExists = false,
                DefaultExt="*.csv"               
            };
            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();

            var rep = dialogService.ShowSaveFileDialog(settings);
            if (rep.HasValue && rep.Value)
            {
                csvFileName = settings.FileName;
            }
            else
                return;

            var sbCSV = new CSVStringBuilder();
            sbCSV.AppendLine("Site", "Repeat","NbRepeat", "Die Index Col", "Die Index Row", "X(mm)", "Y(mm)", "Measure", "Quality", "Value Name","Value","Value Unit","Value Name", "Value", "Value Unit", "Value Name", "Value", "Value Unit", "Value Name", "Value", "Value Unit", "Value Name", "Value", "Value Unit", "Value Name", "Value", "Value Unit");
            // ToList because MeasurePointsResults might be modified during the export
            foreach (var measurePointResult in MeasurePointsResults.ToList())
            {
                sbCSV.Append(measurePointResult.PointIndex.ToString());
                sbCSV.Append($"{measurePointResult.RepeatIndex}");
                sbCSV.Append($"{measurePointResult.NbRepeat}");
                if (measurePointResult.DieIndex != null)
                {
                    sbCSV.Append($"{measurePointResult.DieIndex.Column}");
                    sbCSV.Append($"{measurePointResult.DieIndex.Row}");
                }
                else
                {
                    sbCSV.Append("-");
                    sbCSV.Append("-");
                }
                sbCSV.Append(measurePointResult.Position.X.ToString("F3"));
                sbCSV.Append(measurePointResult.Position.Y.ToString("F3"));
                sbCSV.Append(measurePointResult.MeasureName);
                if (measurePointResult.Result != null)
                    sbCSV.Append(measurePointResult.Result.QualityScore.ToString("F2"));
                else
                    sbCSV.Append("0");
                if (measurePointResult.ResValues != null)
                {
                    foreach (var resValue in measurePointResult.ResValues)
                    {
                        sbCSV.Append(resValue.Name, resValue.Value.ToString("F3"), resValue.Unit);
                    }
                }
                sbCSV.AppendLine();
            }

            try
            {
                File.WriteAllText(csvFileName, sbCSV.ToString());
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Results have been exported with success.", "Results Export", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Failed to export the results", "Results Export", MessageBoxButton.OK, MessageBoxImage.Error);
                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Error($"Failed to export the results {ex.Message}" );
                return;
            }
            
        }

        #endregion Commands

        private ANARecipe GetANARecipe()
        {
            var mapper = ClassLocator.Default.GetInstance<Mapper>();
            var recipe = mapper.AutoMap.Map<ANARecipe>(EditedRecipe);
            return recipe;
        }

        public void ResetProgressStates()
        {
            RecipeRunSettings.AutoFocusState.StepState = StepStates.NotDone;
            RecipeRunSettings.EdgeAlignmentState.StepState = StepStates.NotDone;
            RecipeRunSettings.MarkAlignmentState.StepState = StepStates.NotDone;
        }

        private void UpdateMeasurePoints()
        {
            var newMeasurePoints = new List<Point>();

            foreach (var measure in EditedRecipe.Measures)
            {
                List<int> measurePointsIDs;
                if (measure.SubMeasurePoints != null && measure.SubMeasurePoints.Count > 0)
                {
                    measurePointsIDs = measure.SubMeasurePoints;
                }
                else
                {
                    measurePointsIDs = measure.MeasurePoints;
                }

                foreach (var measurePointIndex in measurePointsIDs)
                {
                    var measurePoint = EditedRecipe.Points.Find(p => p.Id == measurePointIndex);
                    if (measurePoint != null)
                    {
                        // Measure with sub measures don't rely on die
                        if (EditedRecipe.WaferMap is null || measure.IsMeasureWithSubMeasurePoints)
                        {
                            var positionOnWafer = new XYZTopZBottomPosition(new WaferReferential(), measurePoint.Position.X, measurePoint.Position.Y, measurePoint.Position.ZTop, measurePoint.Position.ZBottom);
                            newMeasurePoints.Add(new Point(positionOnWafer.X, positionOnWafer.Y));
                        }
                        else
                        {
                            foreach (var dieIndex in EditedRecipe.Dies)
                            {
                                var dieReferential = new DieReferential(dieIndex.Column, dieIndex.Row);
                                var position = new XYZTopZBottomPosition(dieReferential, measurePoint.Position.X, measurePoint.Position.Y, measurePoint.Position.ZTop, measurePoint.Position.ZBottom);
                                var positionOnWafer = ServiceLocator.ReferentialSupervisor.ConvertTo(position, ReferentialTag.Wafer)?.Result.ToXYZTopZBottomPosition();
                                if (positionOnWafer != null)
                                {
                                    newMeasurePoints.Add(new Point(positionOnWafer.X, positionOnWafer.Y));
                                }
                            }
                        }
                    }
                }
            }

            MeasurePoints = newMeasurePoints;
        }

        public override void Update()
        {
            var estimatedExecutionTime= ServiceLocator.ANARecipeSupervisor.GetEstimatedTime(GetANARecipe());
            Update(estimatedExecutionTime);
        }

        public void Update(TimeSpan estimatedExecutionTime)
        {
            UpdateMeasurePoints();
            EstimatedExecutionTime = estimatedExecutionTime;
            if (EditedRecipe.IsAlignmentMarksSkipped)
            {
                RecipeRunSettings.IsMarkAlignmentUsed = false;
            }
            OnPropertyChanged(nameof(EditedRecipe.IsAlignmentMarksSkipped));
        }

        private void UpdateEstimatedExecutionTime(int nbRuns)
        {
            var anaRecipe = GetANARecipe();
            EstimatedExecutionTime = ServiceLocator.ANARecipeSupervisor.GetEstimatedTime(anaRecipe, nbRuns);
        }

        public void Dispose()
        {
            if (_waferRefBeforeRecipeRun != null)
            {
                _referentialSupervisor.SetSettings(_waferRefBeforeRecipeRun);
            }
            RecipeRunSettings.PropertyChanged -= RecipeRunSettings_PropertyChanged;
            MeasurePointsResults.CollectionChanged -= MeasurePointsResults_CollectionChanged;
            ServiceLocator.ANARecipeSupervisor.RecipeProgressChangedEvent -= ANARecipeSupervisor_RecipeProgressChangedEventAsync;
            ServiceLocator.ANARecipeSupervisor.MeasureResultChangedEvent -= ANARecipeSupervisor_MeasureResultChangedEvent;
        }
    }
}
