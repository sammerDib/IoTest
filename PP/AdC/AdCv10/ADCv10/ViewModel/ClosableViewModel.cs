using CommunityToolkit.Mvvm.ComponentModel;

namespace ADC.ViewModel
{
    /// <summary>
    /// View Model qui permet la ferneture des fenetres ouvertere par ShowDialogWindow
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ClosableViewModel : ObservableRecipient
    {
        private bool _closeSignal;
        public bool CloseSignal
        {
            get => _closeSignal; set { if (_closeSignal != value) { _closeSignal = value; OnPropertyChanged(); } }
        }
    }
}
