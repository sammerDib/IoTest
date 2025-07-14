using System.Windows;
using System.Windows.Controls;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree.Controls
{
    public class DataTreeItemsControl : ItemsControl
    {
        static DataTreeItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataTreeItemsControl), new FrameworkPropertyMetadata(typeof(DataTreeItemsControl)));
        }

        /// <summary>Creates or identifies the element used to display the specified item.</summary>
        /// <returns>The element used to display the specified item.</returns>
        protected override DependencyObject GetContainerForItemOverride() => new DataTreeItem();
    }
}
