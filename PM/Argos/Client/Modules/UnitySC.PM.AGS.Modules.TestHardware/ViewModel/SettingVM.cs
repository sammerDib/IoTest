using GalaSoft.MvvmLight;

namespace UnitySC.PM.AGS.Modules.TestHardware.ViewModel
{
    public class SettingVM : ViewModelBase
    {
        public SettingVM()
        {
        }

        /// <summary> Header dans l'IHM </summary>
        public string Header { get; protected set; }

        public bool IsEnabled { get; protected set; } = false;

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; RaisePropertyChanged(); } }
        }

        private string _busyMessage = "Calibrating";

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; RaisePropertyChanged(); } }
        }
    }
}
