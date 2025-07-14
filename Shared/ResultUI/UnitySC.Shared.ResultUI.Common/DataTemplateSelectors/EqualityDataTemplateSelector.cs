using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace UnitySC.Shared.ResultUI.Common.DataTemplateSelectors
{
    [ContentProperty(nameof(DataTemplates))]
    public class EqualityDataTemplateSelector : DataTemplateSelector
    {
        // ReSharper disable once CollectionNeverUpdated.Global
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<EqualityDataTemplate> DataTemplates { get; } = new Collection<EqualityDataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item == null ? null : DataTemplates.FirstOrDefault(genericDataTemplate => genericDataTemplate.Match(item));
        }
    }

    public class EqualityDataTemplate : DataTemplate
    {
        public object ValueToCompare { get; set; }

        public bool Match(object item)
        {
            if (ValueToCompare == null) return true;
            if (item.GetType() == ValueToCompare.GetType())
            {
                return item.Equals(ValueToCompare);
            }
            if (ValueToCompare is string && item.GetType().IsPrimitive)
            {
                object convertedValue = Convert.ChangeType(ValueToCompare, item.GetType());
                return item.Equals(convertedValue);
            }

            return false;
        }
    }
}
