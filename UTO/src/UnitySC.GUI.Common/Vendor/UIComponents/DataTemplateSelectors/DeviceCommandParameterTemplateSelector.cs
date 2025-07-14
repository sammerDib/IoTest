using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.UIComponents.DataTemplateSelectors
{
    [ContentProperty(nameof(DataTemplates))]
    public class DeviceCommandParameterTemplateSelector : DataTemplateSelector
    {
        // ReSharper disable once CollectionNeverUpdated.Global
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<ParameterDataTemplate> DataTemplates { get; } = new Collection<ParameterDataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var parameter = item as Parameter;
            var csharpType = parameter?.Type as CSharpType;

            return csharpType != null
                ? DataTemplates.FirstOrDefault(genericDataTemplate =>
                    IsOfType(csharpType.PlatformType, genericDataTemplate.ParameterType))
                : null;
        }

        private static bool IsOfType(Type platformType, Type baseType)
        {
            return baseType != null && baseType.IsAssignableFrom(platformType);
        }
    }

    public class ParameterDataTemplate : DataTemplate
    {
        public Type ParameterType { get; set; }
    }
}
