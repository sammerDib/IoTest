using System;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestAlgo.ViewModel
{
    public class BwaVM : AlgoBaseVM, INavigable
    {
        private AxesSupervisor _axesSupervisor;

        private ReferentialSupervisor _referentialSupervisor;
        private ChuckSupervisor _chuckSupervisor;

        public BwaVM() : base("BWA")
        {
            _axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            _referentialSupervisor = ClassLocator.Default.GetInstance<ReferentialSupervisor>();
            ClassLocator.Default.GetInstance<AlgosSupervisor>().BWAChangedEvent += BwaVM_BWAChangedEvent;
            _chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
        }

        private void BwaVM_BWAChangedEvent(BareWaferAlignmentChangeInfo bwaChangeInfo)
        {
            if (bwaChangeInfo is BareWaferAlignmentResult newResult)
                UpdateResult(newResult);
        }

        private BareWaferAlignmentResult _result;

        public BareWaferAlignmentResult Result
        {
            get => _result; set { if (_result != value) { _result = value; OnPropertyChanged(); } }
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
                  Result = null;
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

        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelBWA();
              },
              () => { return true; }));
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
                  double y = -_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter.Millimeters / 2.0;
                  _axesSupervisor.GotoPosition(new XYPosition(new WaferReferential(), 0, y), AxisSpeed.Normal);
              },
              () => { return Result?.Status.State == FlowState.Success; }));
            }
        }

        private AutoRelayCommand _applyAlignment;

        public AutoRelayCommand ApplyAlignment
        {
            get
            {
                return _applyAlignment ?? (_applyAlignment = new AutoRelayCommand(
              () =>
              {
                  var waferReferentialSettings = new WaferReferentialSettings()
                  {
                      ShiftX = Result.ShiftStageX,
                      ShiftY = Result.ShiftStageY,
                      WaferAngle = Result.Angle,
                      ZTopFocus = ((XYZTopZBottomPosition)_axesSupervisor.GetCurrentPosition()?.Result).ZTop.Millimeters() ?? 0.Millimeters()
                  };
                  ServiceLocator.ReferentialSupervisor.SetSettings(waferReferentialSettings);
              },
              () => { return Result?.Status.State == FlowState.Success; }));
            }
        }

        private AutoRelayCommand _deleteAlignment;

        public AutoRelayCommand DeleteAlignment
        {
            get
            {
                return _deleteAlignment ?? (_deleteAlignment = new AutoRelayCommand(
              () =>
              {
                  _referentialSupervisor.DeleteSettings(ReferentialTag.Wafer);
                  GotToCenter();
              },
              () => { return true; }));
            }
        }

        private void GotToCenter()
        {
            _axesSupervisor.GoToChuckCenter(_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter, AxisSpeed.Normal);
        }

        public void UpdateResult(BareWaferAlignmentResult bwaResult)
        {
            Result = bwaResult;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MoveToCenter.NotifyCanExecuteChanged();
                MoveToNotch.NotifyCanExecuteChanged();
            }));

            if (Result.Status.IsFinished)
            {
                IsBusy = false;
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ApplyAlignment.NotifyCanExecuteChanged();
                }));
            }
        }

        public Task PrepareToDisplay()
        {
            return Task.CompletedTask;
        }

        public bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            ServiceLocator.ReferentialSupervisor.DeleteSettings((ReferentialTag.Wafer));
            return true;
        }

        public override void Dispose()
        {
        }
    }
}
