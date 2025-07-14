using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Interfaces
{
    /// <summary>
    /// Interface defining the contract between the generic <see cref="TreeNode{T}"/> implementation and the <see cref="Controls.DataTreeItem"/> control.
    /// </summary>
    public interface ITreeNode
    {
        bool IsExpanded { get; }

        ITreeNode Parent { get; }

        int VisibleChildCount { get; }

        int Level { get; }

        bool IsClicked { get; set; }

        bool IsNextParentOfDraggedElement { get; }

        bool IsExpandable { get; }

        bool IsMouseOver { get; set; }

        bool IsSelected { get; }

        ICommand OnDragCommand { get; }

        ICommand SelectCommand { get; }

        IDataTreeSource DataTreeSource { get; }
    }
}
