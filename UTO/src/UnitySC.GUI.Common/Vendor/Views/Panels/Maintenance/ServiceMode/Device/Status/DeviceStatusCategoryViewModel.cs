using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Status
{
    public class DeviceStatusCategoryViewModel : NamedViewModel
    {
        public DeviceStatusCategoryViewModel(Agileo.EquipmentModeling.Device device, string name, List<DeviceStatus> statuses)
        {
            Name = name;

            DataTreeSource = new DataTreeSource<DeviceStatusViewModel>(_ => null);
            DataTreeSource.Reset(statuses.Select(x => new DeviceStatusViewModel(device, x)));

            foreach (var status in statuses)
            {
                UpdateProperties(status);
            }
        }

        public override string Name { get; }

        public DataTreeSource<DeviceStatusViewModel> DataTreeSource { get; }

        public void UpdateProperties(DeviceStatus status)
        {
            var deviceStatus = DataTreeSource.GetFlattenElements().Select(x => x.Model)
                .FirstOrDefault(x => ReferenceEquals(x.Status, status));
            deviceStatus?.UpdateProperties();
        }
    }
}
