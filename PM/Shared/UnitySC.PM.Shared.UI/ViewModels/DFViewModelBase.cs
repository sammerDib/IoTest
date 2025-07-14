using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.Shared.UI.Core
{
    public abstract class DFViewModelBase:ObservableRecipient
    {
        public DFViewModelBase()
        {
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }
        private string _communicationError;
        public string CommunicationError
        {
            get => _communicationError; set { if (_communicationError != value) { _communicationError = value; OnPropertyChanged(); } }
        }

        public abstract void Init();

        private AutoRelayCommand _initCommand;
        public AutoRelayCommand InitCommand
        {
            get
            {
                return _initCommand ?? (_initCommand = new AutoRelayCommand(
              () =>
              {
                  CommunicationError = null;
                  Init();
              },
              () => { return true; }));
            }
        }
    }
}
