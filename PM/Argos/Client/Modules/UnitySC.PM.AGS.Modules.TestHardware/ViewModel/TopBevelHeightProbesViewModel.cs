namespace UnitySC.PM.AGS.Modules.TestHardware.ViewModel
{
    public class TopBevelHeightProbesViewModel : SettingVM
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

        public TopBevelHeightProbesViewModel()
        {
            Header = "Top Bevel Height";
            IsEnabled = true;
            this.IsConnected = true;
        }
    }
}
