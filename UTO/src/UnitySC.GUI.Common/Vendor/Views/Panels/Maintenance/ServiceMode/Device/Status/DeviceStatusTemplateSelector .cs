using System.Windows;
using System.Windows.Controls;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Status
{
    public class DeviceStatusTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }
        public DataTemplate QuantityTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not DeviceStatusViewModel vm) return null;

            if (vm.Status.Type is CSharpType type)
            {
                if (type.AssemblyQualifiedName.StartsWith("Units"))
                {
                    return QuantityTemplate;
                }
                if (type.PlatformType == typeof(bool))
                {
                    return BoolTemplate;
                }
            }

            return StringTemplate;
        }
    }
}
