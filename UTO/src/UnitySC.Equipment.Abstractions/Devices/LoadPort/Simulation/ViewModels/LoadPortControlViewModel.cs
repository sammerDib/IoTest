using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.SemiDefinitions;
using Agileo.UserControls;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.CarrierConfigurations.Models;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Controls;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Enums;

using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.ViewModels
{
    public class LoadPortControlViewModel : Notifier, IDisposable
    {
        #region Fields

        private readonly LoadPort _loadPort;
        private ObservableCollection<ItemUi> _carrierItems;
        private CarrierConfigurations.Models.CarrierConfigurations _carriersConfig;
        private string _selectedCarrierConfig;

        // Sensors
        private bool _isAbsentChecked = true;
        private bool _isAbsentEnabled = true;
        private bool _isCarrierPlacementOk;
        private bool _isCarrierPresent;
        private bool _isCorrectChecked;
        private bool _isCorrectEnabled = true;
        private bool _isIncorrectChecked;
        private bool _isIncorrectEnabled = true;
        private bool _isMappingVisible;

        // Indicators
        private LedState _handOffModeLed;
        private LedState _loadModeLed;
        private LedState _unloadModeLed;
        private LedState _manualModeLed;
        private LedState _autoModeLed;
        private LedState _reserveModeLed;
        private LedState _alarmModeLed;
        private LoadPortState _loadPortState;

        // Physical Sensors
        private bool _isClamped;
        private bool _isDocked;
        private bool _isDoorClosed;

        // TagReader
        private bool _isTagReaderAvailableChecked;
        private bool _isReadWriteFailChecked;
        private string _carrierTagReadResult;

        #endregion Fields

        #region Constructors

        public LoadPortControlViewModel()
        {
            if (!IsInDesignMode)
                throw new InvalidOperationException("This constructor must be used only in design mode");

            IsCarrierPlacementOk = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPortControlViewModel" /> class.
        /// </summary>
        /// <param name="loadPort">The load port.</param>
        /// <param name="carriersConfig">Defines SimulatorEquipmentData, current LoadPort name and ID</param>
        public LoadPortControlViewModel(LoadPort loadPort, CarrierConfigurations.Models.CarrierConfigurations carriersConfig)
        {
            _loadPort = loadPort;
            _carriersConfig = carriersConfig;
            _carriersConfig.CarrierConfigChanged += CarriersConfig_CarrierConfigChanged;
            _isTagReaderAvailableChecked = _loadPort.SimulationData.IsTagReadEnabled;
            _isReadWriteFailChecked = _loadPort.SimulationData.IsReadWriteFailed;
            _carrierTagReadResult = _loadPort.SimulationData.CarrierIdRead;
        }

        #endregion Constructors

        #region Carrier configuration

        /// <summary>
        /// Gets a value indicating the list of available CarrierConfiguration names
        /// </summary>
        public List<string> CarrierConfigNames => _carriersConfig.CarrierConfigListNames;

        /// <summary>
        /// Gets or sets the name of selected CarrierConfiguration
        /// </summary>
        public string SelectedCarrierConfig
        {
            get { return _selectedCarrierConfig; }
            set
            {
                _selectedCarrierConfig = value;
                if (value == null)
                    return;

                _loadPort.SimulationData.CarrierConfiguration = _carriersConfig.Get(_selectedCarrierConfig);

                // Update value stored in HMI for Carrier tag read
                CarrierTagReadResult = _carriersConfig.CarrierConfigs[value].Id;

                OnPropertyChanged(nameof(SelectedCarrierConfig));
            }
        }

        private void CarriersConfig_CarrierConfigChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(CarrierConfigNames));
        }

        #endregion Carrier configuration

        #region Sensors
        /// <summary>
        /// Gets or sets a value indicating if RadioButton absent is checked
        /// Update the specified mappingViews
        /// </summary>
        public bool IsAbsentChecked
        {
            get { return _isAbsentChecked; }
            set
            {
                if (!SetAndRaiseIfChanged(ref _isAbsentChecked, value))
                {
                    return;
                }

                if (_isAbsentChecked)
                {
                    IsCarrierPresent = false;
                    IsMappingVisible = false;
                    LoadPortState = LoadPortState.Unknown;

                    _loadPort.SetStatusValue(nameof(LoadPort.CarrierPresence), CassettePresence.Absent);
                }
            }
        }

        /// <summary>
        /// Gets or sets if check box Absent is enable
        /// </summary>
        public bool IsAbsentEnabled
        {
            get { return _isAbsentEnabled; }
            set { SetAndRaiseIfChanged(ref _isAbsentEnabled, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if RadioButton correct is checked
        /// Update the specified mappingViews
        /// </summary>
        public bool IsCorrectChecked
        {
            get { return _isCorrectChecked; }
            set
            {
                OnPropertyChanged(nameof(LoadPortState));
                if (SelectedCarrierConfig == null)
                {
                    return;
                }

                if (!SetAndRaiseIfChanged(ref _isCorrectChecked, value))
                {
                    return;
                }

                if (_isCorrectChecked)
                {
                    IsCarrierPresent = true;
                    IsCarrierPlacementOk = true;
                    IsMappingVisible = true;
                    CarrierItems = new ObservableCollection<ItemUi>(CarrierToItemUi());

                    _loadPort.SetStatusValue(nameof(LoadPort.CarrierPresence), CassettePresence.Correctly);
                }
            }
        }

        /// <summary>
        /// Gets or sets if check box Correct is enable
        /// </summary>
        public bool IsCorrectEnabled
        {
            get { return _isCorrectEnabled; }
            set { SetAndRaiseIfChanged(ref _isCorrectEnabled, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if RadioButton incorrect is checked
        /// Update the specified mappingViews
        /// </summary>
        public bool IsIncorrectChecked
        {
            get { return _isIncorrectChecked; }
            set
            {
                if (SelectedCarrierConfig == null)
                {
                    return;
                }

                if (!SetAndRaiseIfChanged(ref _isIncorrectChecked, value))
                {
                    return;
                }

                if (_isIncorrectChecked)
                {
                    IsCarrierPresent = true;
                    IsCarrierPlacementOk = false;
                    IsMappingVisible = true;
                    CarrierItems = new ObservableCollection<ItemUi>(CarrierToItemUi());

                    _loadPort.SetStatusValue(nameof(LoadPort.CarrierPresence), CassettePresence.PresentNoPlacement);
                }
            }
        }
        /// <summary>
        /// Gets or sets if check box Incorrect is enable
        /// </summary>
        public bool IsIncorrectEnabled
        {
            get { return _isIncorrectEnabled; }
            set { SetAndRaiseIfChanged(ref _isIncorrectEnabled, value); }
        }
        #endregion Sensors

        #region Button Indicators

        /// <summary>
        /// Gets or sets the state of HandOff led.
        /// </summary>
        public LedState HandOffModeLed
        {
            get { return _handOffModeLed; }
            set { _handOffModeLed = value; OnPropertyChanged(nameof(HandOffModeLed)); }
        }

        #endregion Button Indicators

        #region Indicators

        /// <summary>
        /// Gets or sets the state of Load led.
        /// </summary>
        public LedState LoadModeLed
        {
            get { return _loadModeLed; }
            set { _loadModeLed = value; OnPropertyChanged(nameof(LoadModeLed)); }
        }

        /// <summary>
        /// Gets or sets the state of Unload led.
        /// </summary>
        public LedState UnloadModeLed
        {
            get { return _unloadModeLed; }
            set { _unloadModeLed = value; OnPropertyChanged(nameof(UnloadModeLed)); }
        }

        /// <summary>
        /// Gets or sets the state of Manual led.
        /// </summary>
        public LedState ManualModeLed
        {
            get { return _manualModeLed; }
            set { _manualModeLed = value; OnPropertyChanged(nameof(ManualModeLed)); }
        }

        /// <summary>
        /// Gets or sets the state of Auto led.
        /// </summary>
        public LedState AutoModeLed
        {
            get { return _autoModeLed; }
            set { _autoModeLed = value; OnPropertyChanged(nameof(AutoModeLed)); }
        }

        /// <summary>
        /// Gets or sets the state of Reserve led.
        /// </summary>
        public LedState ReserveModeLed
        {
            get { return _reserveModeLed; }
            set { _reserveModeLed = value; OnPropertyChanged(nameof(ReserveModeLed)); }
        }

        /// <summary>
        /// Gets or sets the state of Alarm led.
        /// </summary>
        public LedState AlarmModeLed
        {
            get { return _alarmModeLed; }
            set { _alarmModeLed = value; OnPropertyChanged(nameof(AlarmModeLed)); }
        }

        #endregion Indicators

        #region Physical Sensors

        public bool IsClamped
        {
            get { return _isClamped; }
            set { _isClamped = value; OnPropertyChanged(nameof(IsClamped)); }
        }

        public bool IsDocked
        {
            get { return _isDocked; }
            set { _isDocked = value; OnPropertyChanged(nameof(IsDocked)); }
        }

        public bool IsDoorClosed
        {
            get { return _isDoorClosed; }
            set { _isDoorClosed = value; OnPropertyChanged(nameof(IsDoorClosed)); }
        }

        #endregion

        #region Properties

        public ObservableCollection<ItemUi> CarrierItems
        {
            get { return _carrierItems; }
            set { SetAndRaiseIfChanged(ref _carrierItems, value); }
        }

        public bool IsCarrierPlacementOk
        {
            get { return _isCarrierPlacementOk; }
            set { SetAndRaiseIfChanged(ref _isCarrierPlacementOk, value); }
        }

        public bool IsCarrierPresent
        {
            get { return _isCarrierPresent; }
            set { SetAndRaiseIfChanged(ref _isCarrierPresent, value); }
        }

        public bool IsMappingVisible
        {
            get { return _isMappingVisible; }
            set { SetAndRaiseIfChanged(ref _isMappingVisible, value); }
        }

        public CassetteType CassetteType
        {
            get { return _loadPort.Configuration.CassetteType; }
        }

        /// <summary>
        /// Gets or sets a value indicating the state of current carrier
        /// </summary>
        public LoadPortState LoadPortState
        {
            get
            {
                return _loadPortState;
            }
            set
            {
                if (SetAndRaiseIfChanged(ref _loadPortState, value))
                {
                    IsAbsentEnabled = SelectedCarrierConfig != null
                                      && (_loadPortState == LoadPortState.Unknown
                                          || _loadPortState == LoadPortState.Unclamped);
                    IsIncorrectEnabled = SelectedCarrierConfig != null
                                         && (_loadPortState == LoadPortState.Unknown
                                             || _loadPortState == LoadPortState.Unclamped);
                    IsCorrectEnabled = SelectedCarrierConfig != null
                                       && (_loadPortState == LoadPortState.Unknown
                                           || _loadPortState == LoadPortState.Unclamped);
                }
            }
        }

        #endregion Properties

        #region Read/Write

        /// <summary>
        /// Gets or sets a value indicating if Tag Reader is available and Carrier tag can be read
        /// </summary>
        public bool IsTagReaderAvailableChecked
        {
            get { return _isTagReaderAvailableChecked; }
            set
            {
                if (SetAndRaiseIfChanged(ref _isTagReaderAvailableChecked, value))
                {
                    _loadPort.SimulationData.IsTagReadEnabled = _isTagReaderAvailableChecked;
                }
            }
        }

        /// <summary>
        /// Gets or sets if check box chkReadWriteFail is checked,
        /// indicate if Read/Write Tag will fail
        /// </summary>
        public bool IsReadWriteFailChecked
        {
            get { return _isReadWriteFailChecked; }
            set
            {
                if (SetAndRaiseIfChanged(ref _isReadWriteFailChecked, value))
                {
                    _loadPort.SimulationData.IsReadWriteFailed = _isReadWriteFailChecked;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the carrier tag read result.
        /// </summary>
        public string CarrierTagReadResult
        {
            get { return _carrierTagReadResult; }
            set
            {
                if (SetAndRaiseIfChanged(ref _carrierTagReadResult, value))
                {
                    _loadPort.SimulationData.CarrierIdRead = _carrierTagReadResult;
                }
            }
        }

        #region ReadWriteFailed Command
        /// <summary>
        /// ICommand to gets or sets value of checkBoxReadWrite,
        /// indicate if Read/Write Tag will fail
        /// </summary>
        public ICommand ReadWriteFailedCommand
        {
            get
            {
                return new DelegateCommand<string>(_ => ReadWriteFailedCommandExecute());
            }
        }
        /// <summary>
        /// Bind checkBoxReadWrite of LoadPortControlView.xaml
        /// </summary>
        public void ReadWriteFailedCommandExecute()
        {
        }
        #endregion ReadWriteFailed Command

        #endregion Read/Write

        #region Private methods

        /// <summary>
        /// Returns an array of item to fill MappingView
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ItemUi> CarrierToItemUi()
        {
            CarrierConfiguration c = _loadPort.SimulationData.CarrierConfiguration;

            for (byte i = 0; i < c.MappingTable.Count; ++i)
            {
                switch (c.MappingTable[i])
                {
                    case SlotState.HasWafer:
                        yield return new ItemUi
                        {
                            Color = DisplayColor.Blue,
                            Id = i < c.MappingScribe.Count ? c.MappingScribe[i] : null,
                            Slot = i + 1
                        };

                        break;

                    case SlotState.CrossWafer:
                    case SlotState.DoubleWafer:
                    case SlotState.FrontBow:
                    case SlotState.Thick:
                    case SlotState.Thin:
                        yield return new ItemUi
                        {
                            Color = DisplayColor.Red,
                            Id = i < c.MappingScribe.Count ? c.MappingScribe[i] : null,
                            Slot = i + 1
                        };

                        break;
                        
                    default:
                        yield return new ItemUi
                        {
                            Color = DisplayColor.Undefined,
                            Id = null,
                            Slot = i + 1
                        };

                        break;
                }
            }
        }
        #endregion Private methods

        #region IDisposable

        public void Dispose()
        {
            if (_carriersConfig != null)
                _carriersConfig.CarrierConfigChanged -= CarriersConfig_CarrierConfigChanged;
            
            _loadPort?.Dispose();
        }

        #endregion IDisposable
    }
}
