using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Data.Enum;
using System.Collections.ObjectModel;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.OpticalPowermeter
{
    public class OpticalPowermeterVM : ObservableObject
    {
        private OpticalPowermeterSupervisor _supervisor;

        public string CustomTxt { get; set; }
        public string Status { get; set; }

        public PowerIlluminationFlow Flow { get; set; }

        public OpticalPowermeterVM(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public OpticalPowermeterVM(OpticalPowermeterSupervisor supervisor, string flow)
        {
            _supervisor = supervisor;

            Messenger.Register<StateChangedMessage>(this, (r, m) => { UpdateState(m.Flow, m.State); });
            Messenger.Register<PowerChangedMessage>(this, (r, m) => { UpdatePower(m.Flow, m.Power, m.PowerCal_mW, m.RFactor); });
            Messenger.Register<MaxPowerChangedMessage>(this, (r, m) => { UpdateMaximumPower(m.Flow, m.MaximumPower); });
            Messenger.Register<MinPowerChangedMessage>(this, (r, m) => { UpdateMinimumPower(m.Flow, m.MinimumPower); });
            Messenger.Register<WavelengthChangedMessage>(this, (r, m) => { UpdateWavelength(m.Flow, m.Wavelength); });
            Messenger.Register<BeamDiameterChangedMessage>(this, (r, m) => { UpdateBeamDiameter(m.Flow, m.BeamDiameter); });
            Messenger.Register<WavelengthRangeChangedMessage>(this, (r, m) => { UpdateWavelengthRange(m.Flow, m.WavelengthRange); });
            Messenger.Register<IdentifierChangedMessage>(this, (r, m) => { UpdateIdentifier(m.Flow, m.Identifier); });
            Messenger.Register<CustomChangedMessage>(this, (r, m) => { UpdateCustom(m.Flow, m.Custom); });
            Messenger.Register<AvailableWavelengthsChangedMessage>(this, (r, m) => { UpdateAvailableWavelengths(m.Wavelengths); });
            Messenger.Register<DarkAdjustStateChangedMessage > (this, (r, m) => { UpdateDarkAdjustState(m.Flow, m.DarkAdjustState); });
            Messenger.Register<DarkOffsetChangedMessage>(this, (r, m) => { UpdateDarkOffset(m.Flow, m.DarkOffset_mW); });
            Messenger.Register<ResponsivityChangedMessage>(this, (r, m) => { UpdateResponsivity(m.Flow, m.Responsivity); });
            Messenger.Register<SensorTypeChangedMessage>(this, (r, m) => { UpdateSensorType(m.Flow, m.SensorType); });

            Flow = (flow == PowerIlluminationFlow.HS.ToString()) ? PowerIlluminationFlow.HS : PowerIlluminationFlow.HT;

            Task.Run(() => _supervisor.TriggerUpdateEvent(Flow));
        }

        private IMessenger _messenger;

        public IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                return _messenger;
            }
        }

        private void UpdateState(PowerIlluminationFlow flow, string value)
        {
            if (flow == Flow)
            {
                State = value;
            }
        }

        private void UpdatePower(PowerIlluminationFlow flow, double value, double powerCal_mW, double rfactor)
        {
            if (flow == Flow)
            {
                Power = value;
                PowerCal_mW = powerCal_mW;
                RFactor = rfactor;
            }
        }

        private void UpdateMaximumPower(PowerIlluminationFlow flow, double value)
        {
            if (flow == Flow)
            {
                GetMaximumPower = value;
            }
        }

        private void UpdateMinimumPower(PowerIlluminationFlow flow, double value)
        {
            if (flow == Flow)
            {
                GetMinimumPower = value;
            }
        }

        private void UpdateWavelength(PowerIlluminationFlow flow, uint value)
        {
            if (flow == Flow)
            {
                GetWavelength = value;
            }
        }

        private void UpdateBeamDiameter(PowerIlluminationFlow flow, uint value)
        {
            if (flow == Flow)
            {
                GetBeamDiameter = value;
            }
        }

        private void UpdateWavelengthRange(PowerIlluminationFlow flow, double value)
        {
            if (flow == Flow)
            {
                GetWavelengthRange = value;
            }
        }

        private void UpdateIdentifier(PowerIlluminationFlow flow, string value)
        {
            if (flow == Flow)
            {
                GetId = value;
            }
        }

        private void UpdateCustom(PowerIlluminationFlow flow, string value)
        {
            if (flow == Flow)
            {
                GetCustom = value;
            }
        }

        private void UpdateAvailableWavelengths(List<string> wavelengths)
        {
            Wavelengths = wavelengths;
        }


        private void UpdateDarkAdjustState(PowerIlluminationFlow flow, string value)
        {
            if (flow == Flow)
            {
                DarkAdjustState = value;
            }
        }

        private void UpdateDarkOffset(PowerIlluminationFlow flow, double value)
        {
            if (flow == Flow)
            {
                GetDarkOffset = value;
            }
        }

        private void UpdateResponsivity(PowerIlluminationFlow flow, double value)
        {
            if (flow == Flow)
            {
                GetResponsivity = value;
            }
        }

        private void UpdateSensorType(PowerIlluminationFlow flow, string value)
        {
            if (flow == Flow)
            {
                GetSensorType = value;
            }
        }

        private void UpdateSensorAttenuation(PowerIlluminationFlow flow, int value)
        {
            if (flow == Flow)
            {
                GetSensorAttenuation = value;
            }
        }

        private string _state;

        public string State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _power;

        public double Power
        {
            get => _power;
            set
            {
                if (_power != value)
                {
                    _power = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _powerCal_mW;

        public double PowerCal_mW
        {
            get => _powerCal_mW;
            set
            {
                if (_powerCal_mW != value)
                {
                    _powerCal_mW = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _rfactor;

        public double RFactor
        {
            get => _rfactor;
            set
            {
                if (_rfactor != value)
                {
                    _rfactor = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _maximumPower;

        public double GetMaximumPower
        {
            get => _maximumPower;
            set
            {
                if (_maximumPower != value)
                {
                    _maximumPower = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _minimumPower;

        public double GetMinimumPower
        {
            get => _minimumPower;
            set
            {
                if (_minimumPower != value)
                {
                    _minimumPower = value;
                    OnPropertyChanged();
                }
            }
        }

        private uint _wavelength;

        public uint GetWavelength
        {
            get => _wavelength;
            set
            {
                if (_wavelength != value)
                {
                    _wavelength = value;
                    OnPropertyChanged();
                }
            }
        }

        private uint _beamDiameter;

        public uint GetBeamDiameter
        {
            get => _beamDiameter;
            set
            {
                if (_beamDiameter != value)
                {
                    _beamDiameter = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _wavelengthRange;

        public double GetWavelengthRange
        {
            get => _wavelengthRange;
            set
            {
                if (_wavelengthRange != value)
                {
                    _wavelengthRange = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _getId;

        public string GetId
        {
            get => _getId;
            set
            {
                if (_getId != value)
                {
                    _getId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _getCustom;

        public string GetCustom
        {
            get => _getCustom;
            set
            {
                if (_getCustom != value)
                {
                    _getCustom = value;

                    OnPropertyChanged();
                }
            }
        }

        private double _getResponsivity;
        public double GetResponsivity
        {
            get => _getResponsivity;
            set
            {
                if (_getResponsivity != value)
                {
                    _getResponsivity = value;

                    OnPropertyChanged();
                }
            }
        }



        private string _darkAdjustState;

        public string DarkAdjustState
        {
            get => _darkAdjustState;
            set
            {
                if (_darkAdjustState != value)
                {
                    _darkAdjustState = value;

                    OnPropertyChanged();
                }
            }
        }

        private double _getDarkOffset;

        public double GetDarkOffset
        {
            get => _getDarkOffset;
            set
            {
                if (_getDarkOffset != value)
                {
                    _getDarkOffset = value;

                    OnPropertyChanged();
                }
            }
        }

        private string _getSensorType;

        public string GetSensorType
        {
            get => _getSensorType;
            set
            {
                if (_getSensorType != value)
                {
                    _getSensorType = value;

                    OnPropertyChanged();
                }
            }
        }

        private string _getSensorName;

        public string GetSensorName
        {
            get => _getSensorName;
            set
            {
                if (_getSensorName != value)
                {
                    _getSensorName = value;

                    OnPropertyChanged();
                }
            }
        }

        private int _getSensorAttenuation;

        public int GetSensorAttenuation
        {
            get => _getSensorAttenuation;
            set
            {
                if (_getSensorAttenuation != value)
                {
                    _getSensorAttenuation = value;

                    OnPropertyChanged();
                }
            }
        }

        private AutoRelayCommand _customCommand;

        public AutoRelayCommand CustomCommand
        {
            get
            {
                return _customCommand ?? (_customCommand = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.CustomCommand(Flow, CustomTxt));
                    }));
            }
        }

        private List<string> _wavelengths;

        public List<string> Wavelengths
        {
            get => _wavelengths;
            set
            {
                if (_wavelengths != value)
                {
                    _wavelengths = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
