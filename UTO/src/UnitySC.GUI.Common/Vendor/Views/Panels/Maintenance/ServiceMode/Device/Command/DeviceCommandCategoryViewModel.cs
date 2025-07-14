using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command
{
    public sealed class DeviceCommandCategoryViewModel : NamedViewModel, IDisposable
    {
        public DeviceCommandCategoryViewModel(Agileo.EquipmentModeling.Device device, string name, IEnumerable<DeviceCommand> commands, ICommand sendCommand, ICommand abortCommand)
        {
            Name = name;

            DataTreeSource = new DataTreeSource<DeviceCommandViewModel>(_ => null);
            DataTreeSource.Reset(commands.Select(x => new DeviceCommandViewModel(device, x, sendCommand, abortCommand)));
        }

        public override string Name { get; }

        public DataTreeSource<DeviceCommandViewModel> DataTreeSource { get; }

        private DeviceCommandViewModel _selectedDeviceCommandViewModel;

        public DeviceCommandViewModel SelectedDeviceCommandViewModel
        {
            get => _selectedDeviceCommandViewModel;
            set => SetAndRaiseIfChanged(ref _selectedDeviceCommandViewModel, value);
        }

        public void UpdateProperties(DeviceCommand deviceCommand)
        {
            var commandViewModel = DataTreeSource.GetFlattenElements().Select(x => x.Model)
                .FirstOrDefault(x => ReferenceEquals(x.Model, deviceCommand));
            commandViewModel?.UpdateProperties();
        }

        public void Dispose()
        {
            _selectedDeviceCommandViewModel?.Dispose();

            foreach (var treeNode in DataTreeSource.GetFlattenElements())
            {
                treeNode.Model.Dispose();
            }

            DataTreeSource?.Dispose();
        }
    }
}
