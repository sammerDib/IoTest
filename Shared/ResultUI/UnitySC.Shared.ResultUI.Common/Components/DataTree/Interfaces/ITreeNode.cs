using System.Windows.Input;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree.Interfaces
{
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
