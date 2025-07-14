using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Controls
{
    /// <summary>
    /// Control used internally by the DataTree to display items in a virtualized <see cref="ItemsControl"/>.
    /// This control is not intended to be used alone.
    /// </summary>
    public class DataTreeItemsControl : ItemsControl
    {
        static DataTreeItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataTreeItemsControl), new FrameworkPropertyMetadata(typeof(DataTreeItemsControl)));
        }

        /// <summary>Creates or identifies the element used to display the specified item.</summary>
        /// <returns>The element used to display the specified item.</returns>
        protected override DependencyObject GetContainerForItemOverride() => new DataTreeItem();

        public static readonly DependencyProperty GridViewProperty = DependencyProperty.Register(
            nameof(GridView),
            typeof(GridView),
            typeof(DataTreeItemsControl),
            new PropertyMetadata(default(GridView)));

        public GridView GridView
        {
            get { return (GridView)GetValue(GridViewProperty); }
            set { SetValue(GridViewProperty, value); }
        }
    }
}
