namespace ADCConfiguration.ViewModel.Recipe
{
    public class FileStatusViewModel : ViewModelWithMenuBase
    {
        public int UserId { get; set; }

        public string MD5 { get; set; }

        private string _fileName;
        public string FileName
        {
            get => _fileName; set { if (_fileName != value) { _fileName = value; OnPropertyChanged(); } }
        }

        private int _oldVersion;
        public int OldVersion
        {
            get => _oldVersion; set { if (_oldVersion != value) { _oldVersion = value; OnPropertyChanged(); } }
        }

        private int _newVersion;
        public int NewVersion
        {
            get => _newVersion; set { if (_newVersion != value) { _newVersion = value; OnPropertyChanged(); } }
        }

        private FileState _state;
        public FileState State
        {
            get => _state; set { if (_state != value) { _state = value; OnPropertyChanged(); } }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage; set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }
    }
}
