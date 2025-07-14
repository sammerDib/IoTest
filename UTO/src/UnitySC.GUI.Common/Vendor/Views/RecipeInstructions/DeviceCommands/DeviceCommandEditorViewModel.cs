using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.EquipmentTree;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands
{
    public class DeviceCommandEditorViewModel : InstructionEditorViewModel<DeviceCommandInstruction>
    {
        private readonly DeviceCommandInstruction _model;
        private readonly Func<DeviceCommand, bool> _commandFilter;

        private readonly List<DeviceViewModel> _cacheDeviceViewModels = new List<DeviceViewModel>();

        #region Constructors

        public DeviceCommandEditorViewModel(DeviceCommandInstruction model, Func<DeviceCommand, bool> commandFilter = null) : base(model)
        {
            _model = model;
            _commandFilter = commandFilter;

            EquipmentTree = new EquipmentTree(d =>
            {
                SelectedDevice = GetDeviceViewModel(d);
            })
            { SelectedDevice = RetrieveDevice() };
        }

        #endregion Constructors

        #region Properties

        private DeviceViewModel GetDeviceViewModel(Device device)
        {
            if (device == null) return null;
            var deviceViewModel = _cacheDeviceViewModels.FirstOrDefault(viewModel => viewModel.Name.Equals(device.Name) && viewModel.InstanceId.Equals(device.InstanceId));
            if (deviceViewModel != null) return deviceViewModel;
            deviceViewModel = new DeviceViewModel(_model, device, _commandFilter);
            _cacheDeviceViewModels.Add(deviceViewModel);
            return deviceViewModel;
        }

        public EquipmentTree EquipmentTree { get; }

        private DeviceViewModel _selectedDevice;

        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedDevice, value))
                {
                    Model.DeviceName = _selectedDevice?.Name ?? string.Empty;
                    Model.DeviceInstanceId = _selectedDevice?.InstanceId ?? 0;
                    _selectedDevice?.SelectedCommand?.UpdateModel();
                }
            }
        }

        public bool IsTimeoutEnabled
        {
            get
            {
                return Model.IsTimeoutEnabled;
            }
            set
            {
                Model.IsTimeoutEnabled = value;
                OnPropertyChanged();
            }
        }

        public long Timeout
        {
            get
            {
                return Model.Timeout;
            }
            set
            {
                Model.Timeout = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Private

        private Device RetrieveDevice()
        {
            if (string.IsNullOrEmpty(Model.DeviceName))
            {
                return null;
            }

            if (Model.DeviceInstanceId < 1)
            {
                return App.Instance.EquipmentManager.Equipment.Devices.SelectRecursive(d => d.Devices).FirstOrDefault(d => d.Name.Equals(Model.DeviceName));
            }

            return App.Instance.EquipmentManager.Equipment.Devices.SelectRecursive(d => d.Devices).FirstOrDefault(d => d.Name.Equals(Model.DeviceName) && d.InstanceId == Model.DeviceInstanceId);
        }

        #endregion Private
    }
}
