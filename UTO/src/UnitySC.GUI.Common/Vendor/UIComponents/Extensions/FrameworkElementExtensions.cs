using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Extensions
{
    public static class FrameworkElementExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "It is safe here, we only get the value")]
        private static readonly PropertyInfo InheritanceContextProperty = typeof(DependencyObject).GetProperty("InheritanceContext", BindingFlags.NonPublic | BindingFlags.Instance);

        public static IEnumerable<DependencyObject> GetParents(this DependencyObject child)
        {
            while (child != null)
            {
                var parent = LogicalTreeHelper.GetParent(child);
                if (parent == null)
                {
                    if (child is FrameworkElement)
                    {
                        parent = VisualTreeHelper.GetParent(child);
                    }
                    if (parent == null && child is ContentElement contentElement)
                    {
                        parent = ContentOperations.GetParent(contentElement);
                    }
                    if (parent == null)
                    {
                        parent = InheritanceContextProperty.GetValue(child, null) as DependencyObject;
                    }
                }
                child = parent;
                yield return parent;
            }
        }

        public static T GetAncestor<T>(this DependencyObject child) where T : DependencyObject
        {
            return child.GetParents().OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T dependencyObject)
                {
                    yield return dependencyObject;
                }

                foreach (var childOfChild in GetChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }
}
