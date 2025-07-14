using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.ModelingFramework;

using UnitySC.GUI.Common.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.EquipmentTree;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.GUI.Common.Views.Panels.Setup.EquipmentSettings
{
    public class EquipmentSettingsPanel : SetupNodePanel<UnityScConfiguration, EquipmentConfiguration>
    {
        #region Fields

        private readonly List<DeviceSettingsViewModel> _cacheDeviceViewModels = new();
        private readonly Dictionary<string, ExecutionMode> _loadedExecutionModes;
        private readonly Dictionary<string, ExecutionMode> _currentExecutionModes;
        private readonly Package _model;

        #endregion

        #region Constructors

        static EquipmentSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(EquipmentSettingsPanel), typeof(EquipmentSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(EquipmentSettingsResources)));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SetupPanelResources)));
        }

        public EquipmentSettingsPanel()
            : base("DesignTime constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public EquipmentSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            _model = (Package)App.Instance.EquipmentManager.Equipment.GetTopContainer();

            _loadedExecutionModes = new Dictionary<string, ExecutionMode>();
            _currentExecutionModes = new Dictionary<string, ExecutionMode>();
            foreach (var device in _model.AllDevices())
            {
                _loadedExecutionModes.Add(device.Name, device.ExecutionMode);
                _currentExecutionModes.Add(device.Name, device.ExecutionMode);
            }

            EquipmentTree = new EquipmentTree(d =>
                {
                    SelectedDevice = GetDeviceSettingsViewModel(d);
                })
                { SelectedDevice = RetrieveDevice() };

            Commands.Add(
                new BusinessPanelCommand(
                    nameof(EquipmentSettingsResources.S_SETUP_EQUIPMENT_SETTINGS_ALL_SIMULATED),
                    new DelegateCommand(AllSimulatedCommandExecute), PathIcon.Lan));

            Commands.Add(
                new BusinessPanelCommand(
                    nameof(EquipmentSettingsResources.S_SETUP_EQUIPMENT_SETTINGS_ALL_REAL),
                    new DelegateCommand(AllRealCommandExecute), PathIcon.LoadPorts));
        }

        #endregion Constructors

        #region Properties

        public EquipmentTree EquipmentTree { get; }

        private DeviceSettingsViewModel _selectedDevice;

        public DeviceSettingsViewModel SelectedDevice
        {
            get => _selectedDevice;
            set => SetAndRaiseIfChanged(ref _selectedDevice, value);
        }

        #endregion Properties

        #region Private Methods

        private DeviceSettingsViewModel GetDeviceSettingsViewModel(Device device)
        {
            if (device == null) return null;
            var deviceViewModel = _cacheDeviceViewModels.FirstOrDefault(viewModel => viewModel.Name.Equals(device.Name) && viewModel.InstanceId.Equals(device.InstanceId));
            if (deviceViewModel != null) return deviceViewModel;
            deviceViewModel = new DeviceSettingsViewModel(device);
            _cacheDeviceViewModels.Add(deviceViewModel);
            return deviceViewModel;
        }

        private Device RetrieveDevice()
        {
            if (SelectedDevice == null)
            {
                SelectedDevice = new DeviceSettingsViewModel(
                    _model.Equipments.First().Devices.First());
            }

            if (string.IsNullOrEmpty(SelectedDevice.Name))
            {
                return null;
            }

            if (SelectedDevice.InstanceId < 1)
            {
                return _model.Equipments.First().Devices.SelectRecursive(d => d.Devices).FirstOrDefault(d => d.Name.Equals(SelectedDevice.Name));
            }

            return _model.Equipments.First().Devices.SelectRecursive(d => d.Devices).FirstOrDefault(d => d.Name.Equals(SelectedDevice.Name) && d.InstanceId == SelectedDevice.InstanceId);
        }

        #endregion

        #region Commands

        private void AllSimulatedCommandExecute()
        {
            SetExecutionMode(EquipmentTree.Devices, ExecutionMode.Simulated);
            OnPropertyChanged();
        }

        private void AllRealCommandExecute()
        {
            SetExecutionMode(EquipmentTree.Devices, ExecutionMode.Real);
            OnPropertyChanged();
        }

        private void SetExecutionMode(
            IEnumerable<DeviceTreeItem> devices,
            ExecutionMode executionMode)
        {
            foreach (var device in devices)
            {
                device.Device.ExecutionMode = executionMode;
                SetExecutionMode(device.Items, executionMode);
            }
        }

        #endregion

        #region Override

        protected override bool ChangesNeedRestart => true;

        protected override EquipmentConfiguration GetNode(UnityScConfiguration rootConfig) => rootConfig?.EquipmentConfig;

        protected override void SaveConfig()
        {
            _model.SaveToFile(Path.Combine(CurrentConfigNode.EquipmentsFolderPath, CurrentConfigNode.EquipmentFileName));

            _currentExecutionModes.Clear();
            foreach (var device in _model.AllDevices())
            {
                _currentExecutionModes.Add(device.Name, device.ExecutionMode);
            }
        }

        protected override bool ConfigurationEqualsLoaded()
        {
            foreach (var modifiedDevice in _model.AllDevices())
            {
                var loadedExecutionMode = _loadedExecutionModes[modifiedDevice.Name];
                if (loadedExecutionMode != modifiedDevice.ExecutionMode)
                {
                    return false;
                }
            }

            return true;
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            foreach (var modifiedDevice in _model.AllDevices())
            {
                var currentExecutionMode = _currentExecutionModes[modifiedDevice.Name];
                if (currentExecutionMode != modifiedDevice.ExecutionMode)
                {
                    return false;
                }
            }

            return true;
        }

        protected override void UndoChanges()
        {
            foreach (var modifiedDevice in _model.AllDevices())
            {
                modifiedDevice.ExecutionMode = _currentExecutionModes[modifiedDevice.Name];
            }

            OnPropertyChanged();
        }

        public override void OnShow()
        {
            RetrieveDevice();

            base.OnShow();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            foreach (var deviceVm in _cacheDeviceViewModels)
            {
                deviceVm.Refresh();
            }
            base.OnPropertyChanged(propertyName);
        }

        #endregion Override
    }
}
