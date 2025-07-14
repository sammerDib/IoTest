using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public static class GridViewColumnHeaderBehaviors
    {
        #region MinWidth

        /// <summary>
        /// The minimum width attached dependency property
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.RegisterAttached(
            "MinWidth", typeof(double), typeof(GridViewColumnHeaderBehaviors),
            new PropertyMetadata((double)0, OnMinWidthChangedCallback));

        /// <summary>
        /// Sets the minimum width.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetMinWidth(DependencyObject obj, double value)
        {
            obj.SetValue(MinWidthProperty, value);
        }

        /// <summary>
        /// Gets the minimum width.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns></returns>
        public static double GetMinWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(MinWidthProperty);
        }

        private static void OnMinWidthChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var columnHeader = d as GridViewColumnHeader;
            if (columnHeader == null)
                return;

            UpdateWidth(columnHeader);
            columnHeader.SizeChanged += ColumnHeader_SizeChanged;
        }

        private static void ColumnHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateWidth(sender as GridViewColumnHeader);
        }

        private static void UpdateWidth(GridViewColumnHeader columnHeader)
        {
            if (columnHeader?.Column == null)
                return;

            var minWidth = (double)columnHeader.GetValue(MinWidthProperty);

            if (columnHeader.Column.Width <= minWidth && !GridViewColumnBehaviors.GetCollapsed(columnHeader.Column))
            {
                columnHeader.Column.Width = minWidth;
            }
        }

        #endregion MinWidth
    }
}
