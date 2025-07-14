using System;
using System.Collections.Generic;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.CarrierConfigurations.ViewModels
{
    /// <summary>
    /// Store the Configuration of carriers to be simulated
    /// </summary>
    public class SimulatorCarrierConfigurationsViewModel : Notifier
    {
        #region Variables

        private SimulatorCarrierConfigurationViewModel _selectedCarrierDataVm;

        private Models.CarrierConfigurations _model;
        private IList<string> _carrierTypes;
        private string _newCarrierConfigName;
        // Carrier type (200mm, 300mm, ...) for add
        private string _newCarrierType;
        private int _newSlotNumber;
        // Selected carrier to datagrid
        private string _selectedConfiguration;
        #endregion Variables

        #region Constructors

        /// <summary>
        /// Design-Time constructor
        /// </summary>
        public SimulatorCarrierConfigurationsViewModel()
        {
            if (!IsInDesignMode)
                throw new InvalidOperationException("This constructor must be used only in design mode");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatorCarrierConfigurationsViewModel"/> class.
        /// </summary>
        /// <param name="model">Defines SimulatorEquipmentData, dimensions of carrier and event</param>
        public SimulatorCarrierConfigurationsViewModel(Models.CarrierConfigurations model)
        {
            _model = model;
            _model.CarrierConfigChanged += model_CarrierConfigChanged;
            CarrierTypes = model.ListDimension;
        }

        #endregion Constructors

        #region ICommand

        #region AddCarrier
        /// <summary>
        /// ICommand to add a carrier configuration
        /// </summary>
        public ICommand AddCarrierCommand
        {
            get
            {
                return new DelegateCommand<string>(_ => AddCarrier(), _ => AddCarrierCanExecute());
            }
        }
        protected bool AddCarrierCanExecute()
        {
            return (!string.IsNullOrEmpty(NewCarrierConfigName) && (NewCarrierType != null) && (NewSlotNumber != 0) && !_model.CarrierConfigs.ContainsKey(NewCarrierConfigName));
        }

        /// <summary>
        /// Bind AddCarrier button of CarrierControlView.xaml
        /// </summary>
        protected void AddCarrier()
        {
            SampleDimension dimension = SampleDimension.NoDimension;
            if (SampleDimension.S100mm.ToString() == NewCarrierType)
                dimension = SampleDimension.S100mm;
            if (SampleDimension.S150mm.ToString() == NewCarrierType)
                dimension = SampleDimension.S150mm;
            if (SampleDimension.S200mm.ToString() == NewCarrierType)
                dimension = SampleDimension.S200mm;
            if (SampleDimension.S300mm.ToString() == NewCarrierType)
                dimension = SampleDimension.S300mm;

            _model.Add(NewCarrierConfigName, NewCarrierConfigName, dimension, Convert.ToByte(NewSlotNumber));
            OnPropertyChanged(nameof(CarrierConfigurationNames));

            // Reset new values for next configuration to create
            NewCarrierConfigName = "";
            NewSlotNumber = 0;
        }
        #endregion AddCarrier

        #region RemoveCarrier
        /// <summary>
        /// ICommand to remove a carrier configuration
        /// </summary>
        public ICommand RemoveCarrierCommand
        {
            get
            {
                return new DelegateCommand<string>(_ => RemoveCarrier(), _ => RemoveCarrierCanExecute());
            }

        }
        protected bool RemoveCarrierCanExecute()
        {
            return !string.IsNullOrEmpty(SelectedConfiguration);
        }

        /// <summary>
        /// Bind removeCarrier button of CarrierControlView.xaml
        /// </summary>
        protected void RemoveCarrier()
        {
            if (_model.CarrierConfigs.ContainsKey(SelectedConfiguration))
                _model.Remove(SelectedConfiguration);

            SelectedConfiguration = "";
        }
        #endregion RemoveCarrier

        #region SaveCarrier
        /// <summary>
        /// ICommand to save current carrier's configuration
        /// </summary>
        public ICommand SaveCarrierConfig
        {
            get
            {
                return new DelegateCommand<string>(_ => SaveConfig());
            }
        }

        /// <summary>
        /// Bind SaveConfiguration button of CarrierControlView.xaml
        /// </summary>
        public void SaveConfig()
        {
            _model.Save();
        }
        #endregion SaveCarrier

        #endregion ICommand

        #region Properties

        /// <summary>
        /// Gets or sets the selected carrier configuration
        /// </summary>
        public string SelectedConfiguration
        {
            get { return _selectedConfiguration; }
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedConfiguration, value))
                {
                    SelectedCarrierDataVm = _model.CarrierConfigs.ContainsKey(_selectedConfiguration)
                        ? new SimulatorCarrierConfigurationViewModel(_model.CarrierConfigs[_selectedConfiguration])
                        : null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the NewCarrierConfigName
        /// </summary>
        public string NewCarrierConfigName
        {
            get { return _newCarrierConfigName; }
            set { SetAndRaiseIfChanged(ref _newCarrierConfigName, value); }
        }

        /// <summary>
        /// Gets or sets the carrier type of configuration to create (200mm, 300mm, ...)
        /// </summary>
        public string NewCarrierType
        {
            get { return _newCarrierType; }
            set { SetAndRaiseIfChanged(ref _newCarrierType, value); }
        }

        /// <summary>
        /// Gets or sets the number of slot of configuration to create
        /// </summary>
        public int NewSlotNumber
        {
            get { return _newSlotNumber; }
            set { SetAndRaiseIfChanged(ref _newSlotNumber, value); }
        }

        /// <summary>
        ///Gets or sets the selected carrierDataViewModel
        /// </summary>
        public SimulatorCarrierConfigurationViewModel SelectedCarrierDataVm
        {
            get { return _selectedCarrierDataVm; }
            set
            {
                SetAndRaiseIfChanged(ref _selectedCarrierDataVm, value);
            }
        }

        /// <summary>
        /// Gets the name of all carrier configuration
        /// </summary>
        public List<string> CarrierConfigurationNames
        {
            get { return _model.CarrierConfigListNames; }

        }

        /// <summary>
        /// Gets or sets the possible carrier types (200mm and 300mm carrier)
        /// </summary>
        public IList<string> CarrierTypes
        {
            get { return _carrierTypes; }
            set { SetAndRaiseIfChanged(ref _carrierTypes, value); }
        }

        #endregion Properties

        #region INotifyPropertyChanged

        void model_CarrierConfigChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(CarrierConfigurationNames));
        }

        #endregion INotifyPropertyChanged

    }
}
