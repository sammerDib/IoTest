using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.EME.Client.Proxy.Light
{
    public class LightVM : ViewModelBaseExt, IEquatable<LightVM>
    {
        private readonly IEMELightService _lightSupervisor;
        
        public LightVM(EMELightConfig lightConfiguration, IEMELightService lightsSupervisor)
        {
            DeviceID = lightConfiguration.DeviceID;
            Name = lightConfiguration.Name;
            Description = lightConfiguration.Description;

            if (lightsSupervisor == null)
                return;
            
            _lightSupervisor = lightsSupervisor;
            try
            {
                _lightSupervisor.InitLightSources();
                _lightSupervisor.RefreshLightSource(DeviceID);
            }
            catch (Exception)
            {
            }
        }

        #region Public methods

        public void Update(LightSourceChangedMessage lightMessage)
        {
            Power = lightMessage.Power;
            IsTurnedOn = lightMessage.SwitchOn;
            Temperature = lightMessage.Temperature;
            Intensity = lightMessage.Intensity;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LightVM);
        }

        public bool Equals(LightVM other)
        {
            if (other == null)
                return false;

            return Name == other.Name && DeviceID == other.DeviceID;
        }

        public override int GetHashCode()
        {
            return (Name, DeviceID).GetHashCode();
        }

        #endregion Public methods

        #region Properties

        public string Name { get; }

        public string DeviceID { get; }

        private double _power;

        public double Power
        {
            get => _power;
            set => SetProperty(ref _power, value);
        }

        private bool _isTurnedOn;

        public bool IsTurnedOn
        {
            get => _isTurnedOn;
            set => SetProperty(ref _isTurnedOn, value);
        }

        private bool _isLocked;

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    UpdateAllCanExecutes();
                    OnPropertyChanged();
                }
            }
        }


        public string Description { get; private set; }

        private double _temperature;

        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        private double _intensity;

        public double Intensity
        {
            get => _intensity;
            set => SetProperty(ref _intensity, value);
        }

        #endregion Properties


        #region Commands

        private IRelayCommand _switch;

        public IRelayCommand Switch
        {
            get
            {
                if (_switch == null)
                    _switch = new AsyncRelayCommand(PerformSwitchOnOff);

                return _switch;
            }
        }

        private async Task PerformSwitchOnOff()
        {
            if (IsTurnedOn)
            {
                _lightSupervisor.SwitchOn(DeviceID, true);
                await Task.Delay(200);
                _lightSupervisor.SetLightPower(DeviceID, Power);
            }
            else
                _lightSupervisor.SwitchOn(DeviceID, false);
        }

        private IRelayCommand _changePower;

        public IRelayCommand ChangePower
        {
            get
            {
                if (_changePower == null)
                    _changePower = new RelayCommand(PerformChangePower);

                return _changePower;
            }
        }

        private void PerformChangePower()
        {
            _lightSupervisor.SetLightPower(DeviceID, Power);
        }

        #endregion
    }
}
