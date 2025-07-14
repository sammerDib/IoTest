namespace UnitySC.PM.AGS.Modules.TestHardware.ViewModel
{
    public class BottomWidthProbesViewModel : SettingVM
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

        public BottomWidthProbesViewModel()
        {
            Header = "Bottom Width";
            IsEnabled = true;
            this.IsConnected = true;
        }
    }
}
