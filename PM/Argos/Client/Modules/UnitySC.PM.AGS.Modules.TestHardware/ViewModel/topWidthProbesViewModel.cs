//using UnitySC.PM.AGS.Client.Proxy.Axes;

namespace UnitySC.PM.AGS.Modules.TestHardware.ViewModel
{
    public class TopWidthProbesViewModel : SettingVM
    {
        private bool _isConnected;

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                RaisePropertyChanged();
            }
        }

        public TopWidthProbesViewModel()
        {
            Header = "Top";
            IsEnabled = true;
            this.IsConnected = true;
        }
    }
}
