using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.EquipmentTree
{
    public class DeviceTreeItem : Notifier
    {
        public Device Device { get; }

        public List<DeviceTreeItem> Items { get; } = new List<DeviceTreeItem>();

        public DeviceTreeItem(Device device)
        {
            Device = device;
            foreach (var subDevice in device.Devices)
            {
                Items.Add(new DeviceTreeItem(subDevice));
            }
        }

        public void ApplyFilter(Func<Device, bool> isDisplayedFilter)
        {
            foreach (var deviceTreeItem in Items)
            {
                deviceTreeItem.ApplyFilter(isDisplayedFilter);
            }

            IsVisible = Items.Any(item => item.IsVisible) || isDisplayedFilter(Device);
        }

        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            set => SetAndRaiseIfChanged(ref _isVisible, value);
        }
    }
}
