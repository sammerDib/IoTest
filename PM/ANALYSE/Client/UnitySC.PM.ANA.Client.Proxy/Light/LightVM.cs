using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Proxy.Light
{
    public class LightVM : ViewModelBaseExt
    {
        #region Fields

        private ILightService _lightSupervisor;

        private string _deviceID;

        private string _name;

        private double _intensity;

        private AutoRelayCommand _intensityChangedCommand;

        #endregion Fields

        #region Constructors

        public LightVM(LightConfig lightConfiguration, LightsSupervisor lightsSupervisor)
        {
            _deviceID = lightConfiguration.DeviceID;
            _name = lightConfiguration.Name;
            _lightSupervisor = lightsSupervisor;
            _intensity = _lightSupervisor.GetLightIntensity(_deviceID)?.Result ?? 0;
            Description = lightConfiguration.Description;
            Position = lightConfiguration.Position;
            IsMainLight = lightConfiguration.IsMainLight;
        }

        #endregion Constructors

        #region Public methods

        public void Refresh()
        {
            _intensity = _lightSupervisor.GetLightIntensity(_deviceID)?.Result ?? 0;
            OnPropertyChanged(nameof(Intensity));
        }

        #endregion Public methods

        #region Properties

        public string Name { get => _name; }

        public string DeviceID { get => _deviceID; }

        public double Intensity
        {
            get => _intensity;
            set
            {
                if (_intensity == value)
                    return;
                _intensity = value;
                if (_intensity > 100)
                    _intensity = 100;

                _lightSupervisor.SetLightIntensity(_deviceID, _intensity);
                UpdateAllCanExecutes();
                OnPropertyChanged();
            }
        }

        public void SetIntensityFromHardware(double intensity)
        {
            _intensity = intensity;
            OnPropertyChanged(nameof(Intensity));
            UpdateAllCanExecutes();
        }

        private bool _isTurnedOn = true;

        public bool IsTurnedOn
        {
            get => _isTurnedOn;
            set
            {
                if (_isTurnedOn != value)
                {
                    _isTurnedOn = value;
                    if (value)
                        _lightSupervisor.SetLightIntensity(_deviceID, _intensity);
                    else
                        _lightSupervisor.SetLightIntensity(_deviceID, 0);
                    OnPropertyChanged();
                }
            }
        }

        private bool _isLocked = false;

        public bool IsLocked
        {
            get => _isLocked; set { if (_isLocked != value) { _isLocked = value; UpdateAllCanExecutes(); OnPropertyChanged(); } }
        }

        public bool IsMainLight { get; private set; }

        public ModulePositions Position { get; private set; }

        public string Description { get; private set; }

        #endregion Properties

        #region RelayCommand

        public AutoRelayCommand IntensityChangedCommand
        {
            get
            {
                return _intensityChangedCommand ?? (_intensityChangedCommand = new AutoRelayCommand(
                    () => { _lightSupervisor.SetLightIntensity(_deviceID, _intensity); }));
            }
        }

        private AutoRelayCommand _increaseIntensity;

        public AutoRelayCommand IncreaseIntensity
        {
            get
            {
                return _increaseIntensity ?? (_increaseIntensity = new AutoRelayCommand(
                    () =>
                    {
                        Intensity = _intensity + 1;
                    },
                    () => { return (Intensity < 100) && !IsLocked; }
                ));
            }
        }

        private AutoRelayCommand _decreaseIntensity;

        public AutoRelayCommand DecreaseIntensity
        {
            get
            {
                return _decreaseIntensity ?? (_decreaseIntensity = new AutoRelayCommand(
                    () =>
                    {
                        Intensity = _intensity - 1;
                    },
                    () => { return (Intensity > 0) && !IsLocked; }
                ));
            }
        }

        #endregion RelayCommand
    }
}
