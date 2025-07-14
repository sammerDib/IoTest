using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestAlgo.ViewModel
{
    public class AutolightVM : AlgoBaseVM
    {
        public AutolightVM() : base("Autolight")
        {
            ClassLocator.Default.GetInstance<AlgosSupervisor>().AutoLightChangedEvent += AutolightVM_AutoLightChangedEvent;
        }

        private AutolightResult _result;

        public AutolightResult Result
        {
            get => _result; set { if (_result != value) { _result = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _startAutoLight;

        public AutoRelayCommand StartAutoLight
        {
            get
            {
                return _startAutoLight ?? (_startAutoLight = new AutoRelayCommand(
              () =>
              {
                  if (!ClassLocator.Default.GetInstance<CamerasSupervisor>().Camera.IsGrabbing)
                  {
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Camera must be started", "Camera", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                      return;
                  }
                  Result = null;
                  IsBusy = true;
                  var alInput = new AutolightInput(ClassLocator.Default.GetInstance<CamerasSupervisor>().Camera.Configuration.DeviceID, "VIS_WHITE_LED", 30, new ScanRangeWithStep(1, 100, 0.1));
                  ClassLocator.Default.GetInstance<AlgosSupervisor>().StartAutoLight(alInput);

              },
              () => { return true; }));
            }
        }

        private void AutolightVM_AutoLightChangedEvent(AutolightResult autoLightResult)
        {
            UpdateResult(autoLightResult);

        }

        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelAutoLight();
              },
              () => { return true; }));
            }
        }

        public void UpdateResult(AutolightResult autoLightResult)
        {
            Result = autoLightResult;
            if (Result.Status.IsFinished)
                IsBusy = false;
        }

        public override void Dispose()
        {
        }
    }
}
