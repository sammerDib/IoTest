namespace UnitySC.PM.AGS.Modules.TestHardware.ViewModel
{
    public class ApexHeightProbesViewModel : SettingVM
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

        public ApexHeightProbesViewModel()
        {
            Header = "Apex Height";
            IsEnabled = true;
            this.IsConnected = true;
        }
    }
}
