using System;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.EquipmentTree
{
    public class EquipmentTree : Notifier
    {
        private readonly Action<Device> _onSelectedDeviceChanged;

        public EquipmentTree(Action<Device> onSelectedDeviceChanged, Func<Device, bool> isDisplayedDevice = null)
        {
            _onSelectedDeviceChanged = onSelectedDeviceChanged;
            Devices = new ObservableCollection<DeviceTreeItem>(App.Instance.EquipmentManager.Equipment.Devices.Select(device => new DeviceTreeItem(device)));

            var filter = isDisplayedDevice ?? (device => true);
            foreach (var deviceTreeItem in Devices)
            {
                deviceTreeItem.ApplyFilter(filter);
            }
        }

        public ObservableCollection<DeviceTreeItem> Devices { get; }

        private Device _selectedDevice;

        public Device SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedDevice, value))
                {
                    _onSelectedDeviceChanged?.Invoke(_selectedDevice);
                }
            }
        }

        private DeviceTreeItem _selectedTreeItem;

        public DeviceTreeItem SelectedTreeItem
        {
            get => _selectedTreeItem;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedTreeItem, value))
                {
                    SelectedDevice = value?.Device;
                }
            }
        }
    }
}
