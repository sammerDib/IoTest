using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class CamCalibAcquisitionVM : ObservableRecipient
    {
        public CamCalibAcquisitionVM(string name, bool isMandatory = true)
        {
            _name = name;
            _isMandatory = isMandatory;
        }

        private bool _isAcquired = false;

        public bool IsAcquired
        {
            get
            {
                return _isAcquired;
            }

            set
            {
                if (_isAcquired != value)
                {
                    bool oldValue = _isAcquired;
                    _isAcquired = value;
                    OnPropertyChanged();
                    Broadcast(oldValue, _isAcquired, nameof(IsAcquired));
                }
            }
        }

        private string _name;

        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private bool _isMandatory;

        public bool IsMandatory
        {
            get => _isMandatory; set { if (_isMandatory != value) { _isMandatory = value; OnPropertyChanged(); } }
        }
    }
}
