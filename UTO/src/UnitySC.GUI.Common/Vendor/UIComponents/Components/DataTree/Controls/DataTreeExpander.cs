using System.Windows;
using System.Windows.Controls;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Interfaces;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Controls
{
    /// <summary>
    /// Control allowing the automated display of the expander as well as the indentation of a <see cref="DataTreeItem"/>.
    /// </summary>
    public class DataTreeExpander : Control
    {
        static DataTreeExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataTreeExpander), new FrameworkPropertyMetadata(typeof(DataTreeExpander)));
        }

        public static readonly DependencyProperty TreeNodeProperty = DependencyProperty.Register(
            nameof(TreeNode),
            typeof(ITreeNode),
            typeof(DataTreeExpander),
            new PropertyMetadata(default(ITreeNode)));

        public ITreeNode TreeNode
        {
            get { return (ITreeNode)GetValue(TreeNodeProperty); }
            set { SetValue(TreeNodeProperty, value); }
        }
    }
}
