using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.Shared.UI.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static void SetTargetValue<T>(this System.Windows.FrameworkElement element, DependencyProperty dp, T value)
        {
            var binding = BindingOperations.GetBinding(element, dp);
            if (binding == null) return;
            var name = binding.Path.Path;
            var splits = name.Split('.');
            var target = element.DataContext;
            if (target == null) return;
            for (var i = 0; i < splits.Length; i++)
            {
                PropertyInfo property;
                if (i == splits.Length - 1)
                {
                    property = target.GetType().GetProperty(splits[i]);
                    property.SetValue(target, value);
                }
                else
                {
                    property = target.GetType().GetProperty(splits[i]);
                    target = property.GetValue(target);
                }
            }
        }
    }
}
