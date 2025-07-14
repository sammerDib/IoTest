using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.MarkDownViewer;
using UnitySC.GUI.Common.Vendor.UIComponents.Extensions;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Status
{
    public class DeviceStatusViewModel : Notifier
    {
        public DeviceStatusViewModel(Agileo.EquipmentModeling.Device device, DeviceStatus status)
        {
            Device = device;
            Status = status;
        }

        public Agileo.EquipmentModeling.Device Device { get; }

        public DeviceStatus Status { get; }

        public object Value => Device.GetStatusValue(Status.Name);

        public string HumanizedName => Status.GetHumanizedName();

        public MarkDownViewerViewModel MarkDownToolTip => !string.IsNullOrEmpty(Status.DocumentationAsMarkdown)
            ? new MarkDownViewerViewModel(Status.DocumentationAsMarkdown)
            : null;

        public void UpdateProperties()
        {
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(MarkDownToolTip));
        }
    }
}
