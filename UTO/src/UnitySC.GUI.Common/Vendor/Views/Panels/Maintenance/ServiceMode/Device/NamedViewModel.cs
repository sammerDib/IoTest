using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device
{
    public abstract class NamedViewModel : Notifier
    {
        public abstract string Name { get; }
    }
}
