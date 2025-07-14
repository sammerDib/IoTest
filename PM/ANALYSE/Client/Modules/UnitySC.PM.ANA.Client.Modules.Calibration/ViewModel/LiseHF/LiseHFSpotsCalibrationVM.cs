using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Calibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.LiseHF
{
    public class LiseHFSpotsCalibrationVM : ViewModelBaseExt
    {
        private CalibrationSupervisor _calibrationSupervisor;

        private string _probeId;

        public LiseHFSpotsCalibrationVM(string probeID)
        {
            _probeId = probeID;
            _calibrationSupervisor = ClassLocator.Default.GetInstance<CalibrationSupervisor>();
            _calibrationSupervisor.LiseHFSpotCalibrationEvent += CalibrationSupervisor_LiseHFSpotCalibrationEvent;
            SpotsCalibrations.CollectionChanged += SpotsCalibrations_CollectionChanged;
        }

        private void SpotsCalibrations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    (newItem as SpotCalibrationVM).PropertyChanged += LiseHFSpotsCalibrationVM_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var newItem in e.OldItems)
                {
                    (newItem as SpotCalibrationVM).PropertyChanged -= LiseHFSpotsCalibrationVM_PropertyChanged;
                }
            }
        }

        private void LiseHFSpotsCalibrationVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AreAllObjectivesSelected));
        }

        private void UpdateSpotCalibrations()
        {
            SpotsCalibrations.Clear();

            foreach (var objective in ServiceLocator.CamerasSupervisor.Objectives.Where(o => o.ObjType != ObjectiveConfig.ObjectiveType.INT).OrderBy(o => o.ObjType))
            {
                var objectiveCalibration = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(objective.DeviceID);
                var pixelSizeX = objectiveCalibration.Image.PixelSizeX;
                var pixelSizeY = objectiveCalibration.Image.PixelSizeY;

                SpotsCalibrations.Add(new SpotCalibrationVM() { ObjectiveID = objective.DeviceID, ObjectiveName = objective.Name, PixelSizeX = pixelSizeX, PixelSizeY = pixelSizeY });
            }

            if (!(LiseHFCalibration is null))
            {
                foreach (var spotPosition in LiseHFCalibration.SpotPositions)
                {
                    var spotCalibration = SpotsCalibrations.FirstOrDefault(sc => sc.ObjectiveID == spotPosition.ObjectiveDeviceId);
                    if (spotCalibration != null)
                    {
                        spotCalibration.CalibrationDate = spotPosition.Date;
                        spotCalibration.CalibrationStatus = StepStates.Done;
                        var pixelSizeTolerance = new Length(0.0001, LengthUnit.Micrometer);
                        // Check if Pixel Size changed
                        if (!spotCalibration.PixelSizeX.Near(spotPosition.PixelSizeX, pixelSizeTolerance) || !spotCalibration.PixelSizeY.Near(spotPosition.PixelSizeY, pixelSizeTolerance))
                        {
                            spotCalibration.CalibrationStatus = StepStates.Error;
                            spotCalibration.ErrorMessage = "Pixel size changed, a new calibration is needed";
                        }
                        spotCalibration.XOffsetum = spotPosition.XOffset.Micrometers;
                        spotCalibration.YOffsetum = spotPosition.YOffset.Micrometers;
                        spotCalibration.CamExposureTime_ms = spotPosition.CamExposureTime_ms;
                    }
                }
            }
            CalibrationProgress = 0;
        }

        private void CalibrationSupervisor_LiseHFSpotCalibrationEvent(LiseHFSpotCalibrationResults liseHFSpotCalibrationResult)
        {
            foreach (var spotCalibPosition in liseHFSpotCalibrationResult.SpotCalibPositions)
            {
                var spotCalibration = SpotsCalibrations.FirstOrDefault(sc => sc.ObjectiveID == spotCalibPosition.ObjectiveDeviceId);
                if (spotCalibration != null)
                {
                    switch (spotCalibPosition.Status.State)
                    {
                        case FlowState.Waiting:
                            spotCalibration.ErrorMessage = null;
                            spotCalibration.CalibrationStatus = StepStates.InProgress;
                            break;

                        case FlowState.InProgress:
                            spotCalibration.ErrorMessage = null;
                            spotCalibration.CalibrationStatus = StepStates.InProgress;
                            break;

                        case FlowState.Error:
                            spotCalibration.CalibrationStatus = StepStates.Error;
                            spotCalibration.CalibrationDate = null;
                            spotCalibration.XOffsetum = null;
                            spotCalibration.YOffsetum = null;
                            spotCalibration.ErrorMessage = spotCalibPosition.Status.Message;
                            break;
                        case FlowState.Canceled:
                            spotCalibration.ErrorMessage = null;
                            if (spotCalibration.CalibrationDate != null)
                                spotCalibration.CalibrationStatus = StepStates.Done;
                            else
                                spotCalibration.CalibrationStatus = StepStates.NotDone;
                            break;
                        case FlowState.Partial:
                            break;

                        case FlowState.Success:
                            spotCalibration.ErrorMessage = null;
                            spotCalibration.CalibrationStatus = StepStates.Done;
                            spotCalibration.CalibrationDate = spotCalibPosition.Date;
                            spotCalibration.XOffsetum = spotCalibPosition.XOffset.Micrometers;
                            spotCalibration.YOffsetum = spotCalibPosition.YOffset.Micrometers;
                            spotCalibration.CamExposureTime_ms = spotCalibPosition.CamExposureTime_ms;
                            break;

                        default:
                            break;
                    }
                }
            }

            CalibrationProgress = (liseHFSpotCalibrationResult.SpotCalibPositions.Count(sc => sc.Status.IsFinished) * 100) / _nbObjectivesToCalibrate;

            if (liseHFSpotCalibrationResult.Status.State == FlowState.Error)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox(liseHFSpotCalibrationResult.Status.Message, "LIseHF calibration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (liseHFSpotCalibrationResult.Status.IsFinished)
            {
                IsCalibrationInProgress = false;
                IsModified = true;
            }
            UpdateAllCanExecutes();
        }

        public void UpdateLiseHFCalibration()
        {
            StringBuilder stringBuilderErrors = new StringBuilder();
            foreach (var spotCalibration in SpotsCalibrations)
            {
                if (spotCalibration.CalibrationStatus == StepStates.Done)
                {

                    StringBuilder currentSpotCalibrationErr = new StringBuilder();
                    currentSpotCalibrationErr.Append(spotCalibration.CalibrationDate.HasValue ? "" : $"{nameof(spotCalibration.CalibrationDate)} ");
                    currentSpotCalibrationErr.Append(spotCalibration.XOffsetum.HasValue ? "" : $"{nameof(spotCalibration.XOffsetum)} ");
                    currentSpotCalibrationErr.Append(spotCalibration.YOffsetum.HasValue ? "" : $"{nameof(spotCalibration.YOffsetum)} ");
                    if (currentSpotCalibrationErr.Length > 0)
                    {
                        stringBuilderErrors.AppendLine($"{spotCalibration.ObjectiveID} => Following value(s) is(are) null : {currentSpotCalibrationErr.ToString()}");
                    }
                    var spotPosition = LiseHFCalibration.SpotPositions.FirstOrDefault(sp => sp.ObjectiveDeviceId == spotCalibration.ObjectiveID);
                    if (spotPosition != null)
                    {
                        spotPosition.Date = Convert.ToDateTime(spotCalibration.CalibrationDate);
                        spotPosition.PixelSizeX = spotCalibration.PixelSizeX;
                        spotPosition.PixelSizeY = spotCalibration.PixelSizeY;
                        spotPosition.XOffset = new Length(Convert.ToDouble(spotCalibration.XOffsetum), LengthUnit.Micrometer);
                        spotPosition.YOffset = new Length(Convert.ToDouble(spotCalibration.YOffsetum), LengthUnit.Micrometer);
                        spotPosition.CamExposureTime_ms = spotCalibration.CamExposureTime_ms ?? 0.0;
                    }
                    else
                    {
                        spotPosition = new LiseHFObjectiveSpotCalibration()
                        {
                            ObjectiveDeviceId = spotCalibration.ObjectiveID,
                            Date = Convert.ToDateTime(spotCalibration.CalibrationDate),
                            PixelSizeX = spotCalibration.PixelSizeX,
                            PixelSizeY = spotCalibration.PixelSizeY,
                            XOffset = new Length(Convert.ToDouble(spotCalibration.XOffsetum), LengthUnit.Micrometer),
                            YOffset = new Length(Convert.ToDouble(spotCalibration.YOffsetum), LengthUnit.Micrometer),
                            CamExposureTime_ms = spotCalibration.CamExposureTime_ms ?? 0.0
                        };
                        LiseHFCalibration.SpotPositions.Add(spotPosition);
                    }
                }
            }
            if (stringBuilderErrors.Length > 0)
            {
                throw new Exception($"{stringBuilderErrors.ToString()}");
            }
        }

        public bool IsCalibrationComplete()
        {
            return SpotsCalibrations.All(sc => sc.CalibrationStatus == StepStates.Done);
        }

        private LiseHFCalibrationData _liseHFCalibration = null;

        public LiseHFCalibrationData LiseHFCalibration
        {
            get => _liseHFCalibration;
            set
            {
                if (_liseHFCalibration != value)
                {
                    _liseHFCalibration = value;

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        UpdateSpotCalibrations();
                    }));
                    OnPropertyChanged();
                }
            }
        }

        private bool? _areAllObjectivesSelected = false;

        public bool? AreAllObjectivesSelected
        {
            get
            {
                if (!SpotsCalibrations.Any(m => m.IsSelected))
                    _areAllObjectivesSelected = false;
                else
                {
                    if (SpotsCalibrations.Any(m => !m.IsSelected))
                        _areAllObjectivesSelected = null;
                    else
                        _areAllObjectivesSelected = true;
                }

                return _areAllObjectivesSelected;
            }

            set
            {
                if (_areAllObjectivesSelected == value)
                {
                    return;
                }

                _areAllObjectivesSelected = value;
                foreach (var spotCalibration in SpotsCalibrations)
                {
                    spotCalibration.IsSelected = (bool)_areAllObjectivesSelected;
                }

                OnPropertyChanged();
            }
        }

        private ObservableCollection<SpotCalibrationVM> _spotsCalibrations;

        public ObservableCollection<SpotCalibrationVM> SpotsCalibrations
        {
            get => _spotsCalibrations ?? (_spotsCalibrations = new ObservableCollection<SpotCalibrationVM>());
            set => SetProperty(ref _spotsCalibrations, value);
        }

        private bool _isCalibrationInProgress;

        public bool IsCalibrationInProgress
        {
            get => _isCalibrationInProgress;
            set => SetProperty(ref _isCalibrationInProgress, value);
        }

        private bool _isModified = false;

        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); } }
        }

        private int _nbObjectivesToCalibrate = 0;

        private int _calibrationProgress = 0;

        public int CalibrationProgress
        {
            get => _calibrationProgress; set { if (_calibrationProgress != value) { _calibrationProgress = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _startLiseHFSpotsCalibrationCommand;

        public AutoRelayCommand StartLiseHFSpotsCalibrationCommand
        {
            get
            {
                return _startLiseHFSpotsCalibrationCommand ?? (_startLiseHFSpotsCalibrationCommand = new AutoRelayCommand(
                    () =>
                    {
                        CalibrationProgress = 0;
                        var ObjectivesToCalibrate = SpotsCalibrations.Where(sc => sc.IsSelected).Select(sc => sc.ObjectiveID).ToList();
                        IsCalibrationInProgress = true;

                        _nbObjectivesToCalibrate = ObjectivesToCalibrate.Count;
                        var liseHFSpotsCalibrationInput = new LiseHFSpotCalibrationInput(_probeId, ObjectivesToCalibrate);

                        _calibrationSupervisor.StartLiseHFSpotCalibration(liseHFSpotsCalibrationInput);
                    },
                    () => { return SpotsCalibrations.Count(sc => sc.IsSelected) > 0; }
                ));
            }
        }

        private AutoRelayCommand _stopLiseHFSpotsCalibrationCommand;

        public AutoRelayCommand StopLiseHFSpotsCalibrationCommand
        {
            get
            {
                return _stopLiseHFSpotsCalibrationCommand ?? (_stopLiseHFSpotsCalibrationCommand = new AutoRelayCommand(
                    () =>
                    {
                        _calibrationSupervisor.StopLiseHFSpotCalibration();
                        IsCalibrationInProgress = false;
                    },
                    () => { return true; }
                ));
            }
        }

    }
}
