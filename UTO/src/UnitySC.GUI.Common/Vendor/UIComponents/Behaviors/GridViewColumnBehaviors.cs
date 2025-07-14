using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public static class GridViewColumnBehaviors
    {
        public static readonly DependencyProperty CollapsedProperty = DependencyProperty.RegisterAttached(
            "Collapsed", typeof(bool), typeof(GridViewColumnBehaviors), new PropertyMetadata(false, CollapsedCallback));

        public static void SetCollapsed(DependencyObject element, bool value)
        {
            element.SetValue(CollapsedProperty, value);
        }

        public static bool GetCollapsed(DependencyObject element)
        {
            return (bool)element.GetValue(CollapsedProperty);
        }

        private static void CollapsedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as GridViewColumn;
            if (column == null) return;

            if ((bool)e.NewValue)
            {
                SetLastWidth(column, column.Width);
                column.Width = 0;
            }
            else
            {
                column.Width = GetLastWidth(column);
            }
        }

        public static readonly DependencyProperty LastWidthProperty = DependencyProperty.RegisterAttached(
            "LastWidth", typeof(double), typeof(GridViewColumnBehaviors), new PropertyMetadata(double.NaN));

        public static void SetLastWidth(DependencyObject element, double value)
        {
            element.SetValue(LastWidthProperty, value);
        }

        public static double GetLastWidth(DependencyObject element)
        {
            return (double)element.GetValue(LastWidthProperty);
        }
    }

    public static class TreeViewItemBehaviors
    {
        public static readonly DependencyProperty IsReadonlyProperty = DependencyProperty.RegisterAttached(
            "IsReadonly", typeof(bool), typeof(TreeViewItemBehaviors), new PropertyMetadata(default(bool)));

        public static void SetIsReadonly(DependencyObject element, bool value)
        {
            element.SetValue(IsReadonlyProperty, value);
        }

        public static bool GetIsReadonly(DependencyObject element)
        {
            return (bool)element.GetValue(IsReadonlyProperty);
        }
    }
}
