using System;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.ManualWaferLoading.ViewModel
{
    public class BwaVM : ObservableObject
    {
        private AxesSupervisor _axesSupervisor;
        private ChuckSupervisor _chuckSupervisor;
        private ReferentialSupervisor _referentialSupervisor;

        public BwaVM():base()
        {
            _axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            ClassLocator.Default.GetInstance<AlgosSupervisor>().BWAChangedEvent += BwaVM_BWAChangedEvent;
            _chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
            _referentialSupervisor = ClassLocator.Default.GetInstance<ReferentialSupervisor>();
        }

        private void BwaVM_BWAChangedEvent(BareWaferAlignmentChangeInfo bwaChangeInfo)
        {
            if (bwaChangeInfo is BareWaferAlignmentResult newResult)
                UpdateResult(newResult);
        }

        public void UpdateResult(BareWaferAlignmentResult bwaResult)
        {
            Result = bwaResult;
            if(Result.Status.State == FlowState.Success) 
            {
                Score = (int)(bwaResult.Confidence * 100);
            }
            else
            {
                Score = 0;
            }            

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MoveToCenter.NotifyCanExecuteChanged();
                MoveToNotch.NotifyCanExecuteChanged();
            }));
            if (Result.Status.IsFinished)
            {
                IsMeasureTestInProgress = false;
                IsBusy = false;               
            }
        }
        private void GotToCenter()
        {
            _axesSupervisor.GoToChuckCenter(_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter, AxisSpeed.Normal);
        }

        private bool _isMeasureTestInProgress = false;

        public bool IsMeasureTestInProgress
        {
            get => _isMeasureTestInProgress; set { if (_isMeasureTestInProgress != value) { _isMeasureTestInProgress = value; OnPropertyChanged(); } }
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _busyMessage = "BWA in progress.. ";

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; OnPropertyChanged(); } }
        }
        private BareWaferAlignmentResult _result;
        public BareWaferAlignmentResult Result
        {
            get => _result; set { if (_result != value) { _result = value; OnPropertyChanged(); } }
        }
        private int _score = 0;
        public int Score
        {
            get => _score; set { if (_score != value) { _score = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _startBWA;
        public AutoRelayCommand StartBWA
        {
            get
            {
                return _startBWA ?? (_startBWA = new AutoRelayCommand(
              () =>
              {                  
                  if (!ClassLocator.Default.GetInstance<CamerasSupervisor>().Camera.IsGrabbing)
                  {
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Camera must be started", "Camera", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                      return;
                  }
                  IsMeasureTestInProgress = true;
                  Result = null;
                  Score = 0;
                  IsBusy = true;
                  var activeCameraId = ClassLocator.Default.GetInstance<CamerasSupervisor>().Camera.Configuration.DeviceID;
                  var bwaInput = new BareWaferAlignmentInput(_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic, activeCameraId);      
                  ClassLocator.Default.GetInstance<AlgosSupervisor>().StartBWA(bwaInput);                 
              },
              () => { return true; }));
            }
        }       
        private AutoRelayCommand _moveToCenter;
        public AutoRelayCommand MoveToCenter
        {
            get
            {
                return _moveToCenter ?? (_moveToCenter = new AutoRelayCommand(
              () =>
              {
                  GotToCenter();
              },
              () => { return Result?.Status.State == FlowState.Success; }));
            }
        }
        private AutoRelayCommand _moveToNotch;
        public AutoRelayCommand MoveToNotch
        {
            get
            {
                return _moveToNotch ?? (_moveToNotch = new AutoRelayCommand(
              () =>
              {
                  double yToBottom = -_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter.Millimeters / 2.0;
                  var ChuckCenter = (_axesSupervisor.GetChuckCenterPosition(_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter)?.Result 
                                    ?? new XYPosition(new StageReferential(), 0.0, 0.0)) 
                                    as XYPosition;

                  _axesSupervisor.GotoPosition(new XYPosition(new StageReferential(), ChuckCenter.X , ChuckCenter.Y + yToBottom), AxisSpeed.Normal);
              },
              () => { return Result?.Status.State == FlowState.Success; }));
            }
        }

        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelBWA();
                  IsMeasureTestInProgress = false;
                  IsBusy = false;                  
              },
              () => { return true; }));
            }
        }

    }
}
