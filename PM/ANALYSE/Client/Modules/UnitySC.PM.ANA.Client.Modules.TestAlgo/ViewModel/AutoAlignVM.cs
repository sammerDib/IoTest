using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestAlgo.ViewModel
{
    public class AutoAlignVM : AlgoBaseVM
    {
        private ChuckSupervisor _chuckSupervisor;
        public AutoAlignVM() : base("Auto Align")
        {
            _chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
        }

        private AutoAlignResult _result;

        public AutoAlignResult Result
        {
            get => _result; set { if (_result != value) { _result = value; OnPropertyChanged(); } }
        }

        internal void UpdateResult(AutoAlignResult alResult)
        {
            Result = alResult;
            BusyMessage = alResult.Status.Message;
            if (alResult.Status.IsFinished)
                IsBusy = false;
        }

        private AutoRelayCommand _cancelCommand;
        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelAutoAlign();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _startAutoAign;

        public AutoRelayCommand StartAutoAlign
        {
            get
            {
                return _startAutoAign ?? (_startAutoAign = new AutoRelayCommand(
              () =>
              {
                  var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
                  if (!cameraSupervisor.Camera.IsGrabbing)
                  {
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Camera must be started", "Camera", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                      return;
                  }
                  Result = null;
                  IsBusy = true;

                  var autoAlignInput = new AutoAlignInput()
                  {
                      Wafer = _chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic,
                      InitialContext = new ObjectiveContext(cameraSupervisor.MainObjective.DeviceID)
                  };

                  ClassLocator.Default.GetInstance<AlgosSupervisor>().StartAutoAlign(autoAlignInput);
              },
              () => { return true; }));
            }
        }

        public override void Dispose()
        {
        }
    }
}
