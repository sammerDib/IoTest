using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Axes.Models
{
    public class AxisStatus : ObservableObject
    {
        private bool _isEnabled;
        private bool _isMoving;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                    return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsMoving
        {
            get { return _isMoving; }
            set
            {
                if (_isMoving == value)
                    return;
                _isMoving = value;
                OnPropertyChanged();
            }
        }

        public AxisStatus()
        {
            IsEnabled = false;
            IsMoving = false;
        }
    }
}
