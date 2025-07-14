using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    public class LiseHFRefCalibrationVM : ViewModelBaseExt
    {
        #region Fields

        #region Private

        private CalibrationSupervisor _calibrationSupervisor;
        private string _probeId;
        private LiseHFCalibrationData _liseHFCalibration = null;
        private bool? _areAllObjectivesSelected = false;
        private ObservableCollection<RefCalibrationVM> _refCalibrations;
        private bool _isCalibrationInProgress;
        private int _nbObjectivesToCalibrate = 0;
        private int _calibrationProgress = 0;
        private bool _isModified = false;
        private AutoRelayCommand _startLiseHFRefCalibrationCommand;
        private AutoRelayCommand _stopLiseHFRefCalibrationCommand;

        #endregion

        #region Public


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
                        UpdateRefCalibrations();
                    }));
                    OnPropertyChanged();
                }
            }
        }

        public bool? AreAllObjectivesSelected
        {
            get
            {
                if (RefCalibrations.All(refCalibration => refCalibration.IsSelected))
                {
                    return _areAllObjectivesSelected = true;
                }

                if (RefCalibrations.All(refCalibration => !refCalibration.IsSelected))
                {
                    return _areAllObjectivesSelected = false;
                }

                return _areAllObjectivesSelected = null;
            }

            set
            {
                if (SetProperty(ref _areAllObjectivesSelected, value))
                {
                    foreach (var refCalibration in RefCalibrations)
                    {
                        refCalibration.IsSelected = (bool)_areAllObjectivesSelected;
                    }
                }
            }
        }


        public ObservableCollection<RefCalibrationVM> RefCalibrations
        {
            get => _refCalibrations ?? (_refCalibrations = new ObservableCollection<RefCalibrationVM>());
            set => SetProperty(ref _refCalibrations, value);
        }


        public bool IsCalibrationInProgress
        {
            get => _isCalibrationInProgress;
            set
            {
                SetProperty(ref _isCalibrationInProgress, value);
            }
        }


        public bool IsModified
        {
            get => _isModified;
            set => SetProperty(ref _isModified, value);
        }


        public int CalibrationProgress
        {
            get => _calibrationProgress;
            set => SetProperty(ref _calibrationProgress, value);
        }


        public AutoRelayCommand StartLiseHFRefCalibrationCommand
        {
            get
            {
                return _startLiseHFRefCalibrationCommand ?? (_startLiseHFRefCalibrationCommand = new AutoRelayCommand(
                    () =>
                    {
                        CalibrationProgress = 0;
                        var ObjectivesToCalibrate = RefCalibrations.Where(sc => sc.IsSelected)
                            .Select(sc => sc.ObjectiveID).ToList();
                        IsCalibrationInProgress = true;
                        _nbObjectivesToCalibrate = ObjectivesToCalibrate.Count;

                        var liseHFRefCalibrationInput =
                            new LiseHFIntegrationTimeCalibrationInput(_probeId, ObjectivesToCalibrate);

                        _calibrationSupervisor.StartLiseHFIntegrationTimeCalibration(liseHFRefCalibrationInput);
                    },
                    () => { return RefCalibrations.Count(rc => rc.IsSelected) > 0; }
                ));
            }
        }


        public AutoRelayCommand StopLiseHFRefCalibrationCommand
        {
            get
            {
                return _stopLiseHFRefCalibrationCommand ?? (_stopLiseHFRefCalibrationCommand = new AutoRelayCommand(
                    () =>
                    {
                        _calibrationSupervisor.StopLiseHFIntegrationTimeCalibration();
                        IsCalibrationInProgress = false;
                    },
                    () => true));
            }
        }


        #endregion

        #endregion

        #region Constructors

        public LiseHFRefCalibrationVM(string probeID)
        {
            _probeId = probeID;
            _calibrationSupervisor = ClassLocator.Default.GetInstance<CalibrationSupervisor>();
            _calibrationSupervisor.LiseHFRefCalibrationEvent += CalibrationSupervisor_LiseHFRefCalibrationEvent;
            RefCalibrations.CollectionChanged += RefCalibrations_CollectionChanged;
        }

        #endregion

        #region Methods

        #region Private

        private void RefCalibrations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    (newItem as RefCalibrationVM).PropertyChanged += LiseHFRefCalibrationVM_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    (oldItem as RefCalibrationVM).PropertyChanged -= LiseHFRefCalibrationVM_PropertyChanged;
                }
            }
        }

        private void LiseHFRefCalibrationVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AreAllObjectivesSelected));
        }

        private void UpdateRefCalibrations()
        {
            RefCalibrations.Clear();

            foreach (var objective in ServiceLocator.CamerasSupervisor.Objectives
                         .Where(o => o.ObjType != ObjectiveConfig.ObjectiveType.INT).OrderBy(o => o.ObjType))
            {
                RefCalibrations.Add(new RefCalibrationVM()
                {
                    ObjectiveID = objective.DeviceID,
                    ObjectiveName = objective.Name
                });
            }

            if (!(LiseHFCalibration is null))
            {
                foreach (var integrationTimeCalibration in LiseHFCalibration.IntegrationTimes)
                {
                    var refCalibration = RefCalibrations.FirstOrDefault(rc => rc.ObjectiveID == integrationTimeCalibration.ObjectiveDeviceId);
                    if (refCalibration != null)
                    {
                        refCalibration.CalibrationDate = integrationTimeCalibration.Date;
                        refCalibration.CalibrationStatus = StepStates.Done;
                        refCalibration.StandardIntegrationTime = integrationTimeCalibration.StandardFilterIntegrationTime_ms;
                        refCalibration.LowIllumIntegrationTime = integrationTimeCalibration.LowIllumFilterIntegrationTime_ms;
                        refCalibration.StandardMaxCount = integrationTimeCalibration.StandardFilterBaseCount;
                        refCalibration.LowIllumMaxCount = integrationTimeCalibration.LowIllumFilterBaseCount;
                        refCalibration.StandardSignal = integrationTimeCalibration.StandardSignal;
                        refCalibration.LowIllumSignal = integrationTimeCalibration.LowIllumSignal;
                        refCalibration.WaveSignal = integrationTimeCalibration.WaveSignal;
                    }
                }
            }

            CalibrationProgress = 0;
        }
        private void CalibrationSupervisor_LiseHFRefCalibrationEvent(
            LiseHFIntegrationTimeCalibrationResults liseHFRefCalibrationResult)
        {
            foreach (var integrationTimeCalibration in liseHFRefCalibrationResult.CalibIntegrationTimes)
            {
                var refCalibration = RefCalibrations.FirstOrDefault(rc =>
                    rc.ObjectiveID == integrationTimeCalibration.ObjectiveDeviceId);
                if (refCalibration != null)
                {
                    switch (integrationTimeCalibration.Status.State)
                    {
                        case FlowState.Waiting:
                            refCalibration.ErrorMessage = null;
                            refCalibration.CalibrationStatus = StepStates.InProgress;
                            break;

                        case FlowState.InProgress:
                            refCalibration.ErrorMessage = null;
                            refCalibration.CalibrationStatus = StepStates.InProgress;
                            break;

                        case FlowState.Error:
                            refCalibration.CalibrationStatus = StepStates.Error;
                            refCalibration.CalibrationDate = null;
                            refCalibration.StandardIntegrationTime = null;
                            refCalibration.LowIllumIntegrationTime = null;
                            refCalibration.StandardMaxCount = null;
                            refCalibration.LowIllumMaxCount = null;
                            refCalibration.StandardSignal = null;
                            refCalibration.LowIllumSignal = null;
                            refCalibration.WaveSignal = null;
                            refCalibration.ErrorMessage = integrationTimeCalibration.Status.Message;
                            break;
                        case FlowState.Canceled:
                            refCalibration.ErrorMessage = null;
                            if (refCalibration.CalibrationDate != null)
                                refCalibration.CalibrationStatus = StepStates.Done;
                            else
                                refCalibration.CalibrationStatus = StepStates.NotDone;
                            break;

                        case FlowState.Partial:
                            break;

                        case FlowState.Success:
                            refCalibration.ErrorMessage = null;
                            refCalibration.CalibrationStatus = StepStates.Done;
                            refCalibration.CalibrationDate = integrationTimeCalibration.Date;
                            refCalibration.StandardIntegrationTime = integrationTimeCalibration.StandardFilterIntegrationTime_ms;
                            refCalibration.StandardMaxCount = integrationTimeCalibration.StandardFilterBaseCount;
                            refCalibration.LowIllumIntegrationTime = integrationTimeCalibration.LowIllumFilterIntegrationTime_ms;
                            refCalibration.LowIllumMaxCount = integrationTimeCalibration.LowIllumFilterBaseCount;
                            refCalibration.StandardSignal = integrationTimeCalibration.StandardSignal;
                            refCalibration.LowIllumSignal = integrationTimeCalibration.LowIllumSignal;
                            refCalibration.WaveSignal = integrationTimeCalibration.WaveSignal;
                            break;

                        default:
                            break;
                    }
                }
            }

            var count = liseHFRefCalibrationResult.CalibIntegrationTimes
                            .Count(c => c.Status.State == FlowState.Success || c.Status.State == FlowState.Error);
            CalibrationProgress = count * 100 / _nbObjectivesToCalibrate;

            if (liseHFRefCalibrationResult.Status.State == FlowState.Error)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox(
                    liseHFRefCalibrationResult.Status.Message, "LIseHF calibration Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            if (liseHFRefCalibrationResult.Status.IsFinished)
            {
                IsCalibrationInProgress = false;
                IsModified = true;
            }

            UpdateAllCanExecutes();
        }

        #endregion

        #region Public

        public void UpdateLiseHFCalibration()
        {
            StringBuilder stringBuilderErrors = new StringBuilder();
            foreach (var refCalibration in RefCalibrations)
            {
                if (refCalibration.CalibrationStatus == StepStates.Done)
                {
                    StringBuilder currentRefCalibrationErr = new StringBuilder();
                    currentRefCalibrationErr.Append(refCalibration.CalibrationDate.HasValue ? "" : $"{nameof(refCalibration.CalibrationDate)} ");
                    currentRefCalibrationErr.Append(refCalibration.StandardIntegrationTime.HasValue ? "" : $"{nameof(refCalibration.StandardIntegrationTime)} ");
                    currentRefCalibrationErr.Append(refCalibration.LowIllumIntegrationTime.HasValue ? "" : $"{nameof(refCalibration.LowIllumIntegrationTime)} ");

                    if (currentRefCalibrationErr.Length > 0)
                    {
                        stringBuilderErrors.AppendLine($"{refCalibration.ObjectiveID} => Following value(s) is(are) null : {currentRefCalibrationErr.ToString()}");
                    }


                    var integrationTimeCalibration = LiseHFCalibration.IntegrationTimes.FirstOrDefault(it => it.ObjectiveDeviceId == refCalibration.ObjectiveID);
                    if (integrationTimeCalibration != null)
                    {
                        integrationTimeCalibration.Date = Convert.ToDateTime(refCalibration.CalibrationDate);
                        integrationTimeCalibration.StandardFilterIntegrationTime_ms = Convert.ToDouble(refCalibration.StandardIntegrationTime);
                        integrationTimeCalibration.LowIllumFilterIntegrationTime_ms = Convert.ToDouble(refCalibration.LowIllumIntegrationTime);
                        integrationTimeCalibration.StandardFilterBaseCount = Convert.ToDouble(refCalibration.StandardMaxCount);
                        integrationTimeCalibration.LowIllumFilterBaseCount = Convert.ToDouble(refCalibration.LowIllumMaxCount);
                        integrationTimeCalibration.StandardSignal = refCalibration.StandardSignal;
                        integrationTimeCalibration.LowIllumSignal = refCalibration.LowIllumSignal;
                        integrationTimeCalibration.WaveSignal = refCalibration.WaveSignal;
                    }
                    else
                    {
                        integrationTimeCalibration = new LiseHFObjectiveIntegrationTimeCalibration()
                        {
                            ObjectiveDeviceId = refCalibration.ObjectiveID,
                            Date = Convert.ToDateTime(refCalibration.CalibrationDate),
                            StandardFilterIntegrationTime_ms = Convert.ToDouble(refCalibration.StandardIntegrationTime),
                            StandardFilterBaseCount = Convert.ToDouble(refCalibration.StandardMaxCount),
                            LowIllumFilterIntegrationTime_ms = Convert.ToDouble(refCalibration.LowIllumIntegrationTime),
                            LowIllumFilterBaseCount = Convert.ToDouble(refCalibration.LowIllumMaxCount),
                            StandardSignal = refCalibration.StandardSignal,
                            LowIllumSignal = refCalibration.LowIllumSignal,
                            WaveSignal = refCalibration.WaveSignal,
                        };
                        LiseHFCalibration.IntegrationTimes.Add(integrationTimeCalibration);
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
            return RefCalibrations.All(rc => rc.CalibrationStatus == StepStates.Done);
        }

        #endregion

        #endregion
    }
}
