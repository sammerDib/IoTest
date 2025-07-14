using System;
using System.IO;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.UserControls;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.CarrierConfigurations.ViewModels;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.ViewModels;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation
{
    /// <summary>
    ///     Class representing LoadPort global ViewModel
    /// </summary>
    public class LoadPortSimulationViewModel : SimulationData, IDisposable
    {
        #region Properties

        public bool IsCommandFailed
        {
            get => _isCommandFailed;
            set
            {
                if (SetAndRaiseIfChanged(ref _isCommandFailed, value))
                {
                    _loadPort.SimulationData.IsCommandExecutionFailed = _isCommandFailed;
                }
            }
        }

        #endregion Properties

        #region IDisposable

        public void Dispose()
        {
            if (_loadPort != null)
            {
                _loadPort.StatusValueChanged -= LoadPort_StatusValueChanged;
            }

            LoadPortControlViewModel?.Dispose();
        }

        #endregion IDisposable

        #region Event handlers

        private void LoadPort_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            // Update properties bound to HMI
            switch (e.Status.Name)
            {
                case nameof(LoadPort.IsClamped):
                    LoadPortControlViewModel.IsClamped = (bool)e.NewValue;
                    break;
                case nameof(LoadPort.IsDocked):
                    LoadPortControlViewModel.IsDocked = (bool)e.NewValue;
                    break;
                case nameof(LoadPort.IsDoorOpen):
                    LoadPortControlViewModel.IsDoorClosed = !(bool)e.NewValue;
                    break;
                case nameof(LoadPort.PhysicalState):
                    LoadPortControlViewModel.LoadPortState = (LoadPortState)e.NewValue;
                    break;
                case nameof(LoadPort.HandOffLightState):
                    LoadPortControlViewModel.HandOffModeLed = ToLedState((LightState)e.NewValue);
                    break;
                case nameof(LoadPort.LoadLightState):
                    LoadPortControlViewModel.LoadModeLed = ToLedState((LightState)e.NewValue);
                    break;
                case nameof(LoadPort.UnloadLightState):
                    LoadPortControlViewModel.UnloadModeLed = ToLedState((LightState)e.NewValue);
                    break;
                case nameof(LoadPort.ManualModeLightState):
                    LoadPortControlViewModel.ManualModeLed = ToLedState((LightState)e.NewValue);
                    break;
                case nameof(LoadPort.AutoModeLightState):
                    LoadPortControlViewModel.AutoModeLed = ToLedState((LightState)e.NewValue);
                    break;
                case nameof(LoadPort.ReservedLightState):
                    LoadPortControlViewModel.ReserveModeLed = ToLedState((LightState)e.NewValue);
                    break;
                case nameof(LoadPort.ErrorLightState):
                    LoadPortControlViewModel.AlarmModeLed = ToLedState((LightState)e.NewValue);
                    break;

                //E84 Signals
                case nameof(LoadPort.I_VALID):
                    OnPropertyChanged(nameof(Valid));
                    break;
                case nameof(LoadPort.O_L_REQ):
                    OnPropertyChanged(nameof(LReq));
                    break;
                case nameof(LoadPort.I_CS_0):
                    OnPropertyChanged(nameof(Cs0));
                    break;
                case nameof(LoadPort.O_U_REQ):
                    OnPropertyChanged(nameof(UReq));
                    break;
                case nameof(LoadPort.O_READY):
                    OnPropertyChanged(nameof(Ready));
                    break;
                case nameof(LoadPort.I_TR_REQ):
                    OnPropertyChanged(nameof(TrReq));
                    break;
                case nameof(LoadPort.I_BUSY):
                    OnPropertyChanged(nameof(Busy));
                    break;
                case nameof(LoadPort.I_COMPT):
                    OnPropertyChanged(nameof(Compt));
                    break;
                case nameof(LoadPort.O_HO_AVBL):
                    OnPropertyChanged(nameof(HoAvbl));
                    break;
                case nameof(LoadPort.I_CONT):
                    OnPropertyChanged(nameof(Cont));
                    break;
                case nameof(LoadPort.O_ES):
                    OnPropertyChanged(nameof(Es));
                    break;
                case nameof(LoadPort.I_CS_1):
                    OnPropertyChanged(nameof(Cs1));
                    break;
            }
        }

        #endregion Event handlers

        private LedState ToLedState(LightState state)
        {
            switch (state)
            {
                case LightState.Undetermined:
                    return LedState.Disabled;
                case LightState.Off:
                    return LedState.Off;
                case LightState.On:
                    return LedState.On;
                case LightState.Flashing:
                case LightState.FlashingSlow:
                case LightState.FlashingFast:
                    return LedState.Blinking;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        #region Fields

        private bool _isCommandFailed;

        // ViewModels
        private readonly LoadPort _loadPort;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadPortControlViewModel" /> class.
        /// </summary>
        /// <remarks>Reserved for Design mode</remarks>
        public LoadPortSimulationViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException("This constructor must be used only in design mode");
            }

            _isCommandFailed = false;
        }

        public LoadPortSimulationViewModel(LoadPort loadPort)
            : base(loadPort)
        {
            if (loadPort == null)
            {
                throw new ArgumentNullException(nameof(loadPort));
            }

            _loadPort = loadPort;
            _loadPort.StatusValueChanged += LoadPort_StatusValueChanged;

            _isCommandFailed = _loadPort.SimulationData.IsCommandExecutionFailed;

            var carrierConfigurations = CarrierConfigurations.Models.CarrierConfigurations.Deserialize(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SimulatorConfig.cfg"));

            LoadPortControlViewModel = new LoadPortControlViewModel(_loadPort, carrierConfigurations);
            SimulatorCarrierConfigurationsViewModel =
                new SimulatorCarrierConfigurationsViewModel(carrierConfigurations);
        }

        #endregion Constructors

        #region ViewModels Properties

        public bool Valid
        {
            get => _loadPort.I_VALID;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.I_VALID), value);
                OnPropertyChanged();
            }
        }

        public bool Cs0
        {
            get => _loadPort.I_CS_0;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.I_CS_0), value);
                OnPropertyChanged();
            }
        }

        public bool Cs1
        {
            get => _loadPort.I_CS_1;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.I_CS_1), value);
                OnPropertyChanged();
            }
        }

        public bool TrReq
        {
            get => _loadPort.I_TR_REQ;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.I_TR_REQ), value);
                OnPropertyChanged();
            }
        }

        public bool Busy
        {
            get => _loadPort.I_BUSY;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.I_BUSY), value);
                OnPropertyChanged();
            }
        }

        public bool Compt
        {
            get => _loadPort.I_COMPT;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.I_COMPT), value);
                OnPropertyChanged();
            }
        }

        public bool Cont
        {
            get => _loadPort.I_CONT;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.I_CONT), value);
                OnPropertyChanged();
            }
        }

        public bool UReq
        {
            get => _loadPort.O_U_REQ;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.O_U_REQ), value);
                OnPropertyChanged();
            }
        }

        public bool LReq
        {
            get => _loadPort.O_L_REQ;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.O_L_REQ), value);
                OnPropertyChanged();
            }
        }

        public bool Ready
        {
            get => _loadPort.O_READY;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.O_READY), value);
                OnPropertyChanged();
            }
        }

        public bool HoAvbl
        {
            get => _loadPort.O_HO_AVBL;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.O_HO_AVBL), value);
                OnPropertyChanged();
            }
        }

        public bool Es
        {
            get => _loadPort.O_ES;
            set
            {
                _loadPort.SetStatusValue(nameof(LoadPort.O_ES), value);
                OnPropertyChanged();
            }
        }

        public LoadPortControlViewModel LoadPortControlViewModel { get; }

        public SimulatorCarrierConfigurationsViewModel SimulatorCarrierConfigurationsViewModel { get; }

        #endregion ViewModels Properties
    }
}
