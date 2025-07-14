using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun
{
    public  class SettingVM : ObservableObject
    {
        /// <summary> Header dans l'IHM </summary>
        public string Header { get; protected set; }

        public bool IsEnabled { get; protected set; } = false;

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _busyMessage = "Run Recipe";

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; OnPropertyChanged(); } }
        }
    }
}
