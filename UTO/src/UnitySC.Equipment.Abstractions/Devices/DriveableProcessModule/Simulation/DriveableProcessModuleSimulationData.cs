

using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;

namespace UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.Simulation
{
    public class DriveableProcessModuleSimulationData : SimulationData
    {
        #region Constructors

        public DriveableProcessModuleSimulationData(DriveableProcessModule pm): base(pm)
        {
            
        }

        #endregion

        #region Properties

        private bool _is100mmSupported;

        public bool Is100mmSupported
        {
            get => _is100mmSupported;
            set
            {
                _is100mmSupported = value;
                OnPropertyChanged();
            }
        }

        private bool _is150mmSupported;

        public bool Is150mmSupported
        {
            get => _is150mmSupported;
            set
            {
                _is150mmSupported = value;
                OnPropertyChanged();
            }
        }

        private bool _is200mmSupported;

        public bool Is200mmSupported
        {
            get => _is200mmSupported;
            set
            {
                _is200mmSupported = value;
                OnPropertyChanged();
            }
        }

        private bool _is300mmSupported;

        public bool Is300mmSupported
        {
            get => _is300mmSupported;
            set
            {
                _is300mmSupported = value;
                OnPropertyChanged();
            }
        }

        private bool _is450mmSupported;

        public bool Is450mmSupported
        {
            get => _is450mmSupported;
            set
            {
                _is450mmSupported = value;
                OnPropertyChanged();
            }
        }

        private bool _isDoorOpen;

        public bool IsDoorOpen
        {
            get => _isDoorOpen;
            set
            {
                _isDoorOpen = value;
                OnPropertyChanged();
            }
        }

        private bool _isReadyToLoadUnload;

        public bool IsReadyToLoadUnload
        {
            get => _isReadyToLoadUnload;
            set
            {
                _isReadyToLoadUnload = value;
                OnPropertyChanged();
            }
        }

        private bool _transferValidationState;

        public bool TransferValidationState
        {
            get => _transferValidationState;
            set
            {
                _transferValidationState = value;
                OnPropertyChanged();
            }
        }
        
        #endregion
    }
}
