using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace UnitySC.GUI.Common.Vendor.UIComponents.StyleSelectors
{
    [ContentProperty(nameof(Styles))]
    public class GenericStyleSelector : StyleSelector
    {
        // ReSharper disable once CollectionNeverUpdated.Global
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<GenericStyle> Styles { get; } = new Collection<GenericStyle>();

        public override Style SelectStyle(object item, DependencyObject container)
        {
            return Styles.FirstOrDefault(s => IsOfType(item, s.Type))?.Style;
        }

        private static bool IsOfType(object item, object targetType)
        {
            if (targetType == null)
                return true;
            return targetType is Type type && type.IsInstanceOfType(item);
        }
    }
}
