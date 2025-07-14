using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace UnitySC.GUI.Common.Vendor.UIComponents.DataTemplateSelectors
{
    /// <summary>
    /// <see cref="DataTemplateSelector"/> allowing to return the first dataTemplate for which the DataType matches with the item.
    /// It is necessary to use the DataType = "{x: Type MyType}" formalism on DataTemplates so that the selector obtains the object of type Type and not a string.
    /// </summary>
    [ContentProperty(nameof(DataTemplates))]
    public class GenericDataTemplateSelector : DataTemplateSelector
    {
        // ReSharper disable once CollectionNeverUpdated.Global
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<DataTemplate> DataTemplates { get; } = new Collection<DataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item == null
                ? null
                : DataTemplates.FirstOrDefault(genericDataTemplate => IsOfType(item, genericDataTemplate.DataType));
        }

        private static bool IsOfType(object item, object targetType)
        {
            if (targetType == null)
                return true;
            return targetType is Type type && type.IsInstanceOfType(item);
        }
    }
}
