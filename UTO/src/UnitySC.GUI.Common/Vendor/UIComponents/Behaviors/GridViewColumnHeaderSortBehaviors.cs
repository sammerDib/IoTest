using System.ComponentModel;
using System.Windows;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public static class GridViewColumnHeaderSortBehaviors
    {
        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
            "IsActive", typeof(bool), typeof(GridViewColumnHeaderSortBehaviors), new PropertyMetadata(default(bool)));

        public static void SetIsActive(DependencyObject element, bool value)
        {
            element.SetValue(IsActiveProperty, value);
        }

        public static bool GetIsActive(DependencyObject element)
        {
            return (bool)element.GetValue(IsActiveProperty);
        }

        #endregion

        #region Direction

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.RegisterAttached(
            "Direction", typeof(ListSortDirection), typeof(GridViewColumnHeaderSortBehaviors), new PropertyMetadata(default(ListSortDirection)));

        public static void SetDirection(DependencyObject element, ListSortDirection value)
        {
            element.SetValue(DirectionProperty, value);
        }

        public static ListSortDirection GetDirection(DependencyObject element)
        {
            return (ListSortDirection)element.GetValue(DirectionProperty);
        }

        #endregion

        #region EnableSorting

        public static readonly DependencyProperty EnableSortingProperty = DependencyProperty.RegisterAttached(
            "EnableSorting", typeof(bool), typeof(GridViewColumnHeaderSortBehaviors), new PropertyMetadata(default(bool)));

        public static void SetEnableSorting(DependencyObject element, bool value)
        {
            element.SetValue(EnableSortingProperty, value);
        }

        public static bool GetEnableSorting(DependencyObject element)
        {
            return (bool)element.GetValue(EnableSortingProperty);
        }

        #endregion
    }
}
