using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.ResultUI.Common.Components.DataTree.Interfaces;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree.DragDrop
{
    public interface IDragDropAction
    {
        int NewIndex { get; }

        DragDropAction ActionType { get; }

        void Update(ITreeNode newParent, int newIndex, DragDropAction action);

        ITreeNode DraggedNode { get; }
    }

    public class DragDropActionParameter<T> : ObservableObject, IDragDropAction where T : class
    {
        #region IDragDropAction

        int IDragDropAction.NewIndex => NewIndex;

        DragDropAction IDragDropAction.ActionType => ActionType;
        ITreeNode IDragDropAction.DraggedNode => DraggedElement;

        void IDragDropAction.Update(ITreeNode newParent, int newIndex, DragDropAction action)
        {
            Update(newParent as TreeNode<T>, newIndex, action); 
        }
        
        #endregion

        public TreeNode<T> DraggedElement { get; }

        public TreeNode<T> NewParent { get; private set; }
        public int NewIndex { get; private set; }
        public DragDropAction ActionType { get; private set; } = DragDropAction.None;

        private bool _canBeApplied;

        public bool CanBeApplied
        {
            get => _canBeApplied;
            protected set
            {
                if (_canBeApplied == value) return;
                _canBeApplied = value;
                OnPropertyChanged();
            }
        }
        
        public DragDropActionParameter(TreeNode<T> draggedElement)
        {
            DraggedElement = draggedElement;
        }

        internal void Update(TreeNode<T> newParent, int newIndex, DragDropAction actionType)
        {
            ResetProperties();

            NewParent = newParent;
            NewIndex = newIndex;
            ActionType = actionType;
        }

        internal void ResetProperties()
        {
            if (NewParent != null) NewParent.IsNextParentOfDraggedElement = false;

            if (DraggedElement.IsExpanded)
            {
                DraggedElement.IsExpanded = false;
            }
        }

        internal void SetupTreeElementProperties(bool canBeApplied)
        {
            CanBeApplied = canBeApplied;
            if (NewParent == null) return;
            NewParent.IsNextParentOfDraggedElement = ActionType.ChangeParent() && canBeApplied;
        }
    }

    public class ChangeParentDragDropAction<T> : DragDropActionParameter<T> where T : class
    {
        public ChangeParentDragDropAction(TreeNode<T> draggedElement, TreeNode<T> newParent) : base(draggedElement)
        {
            Update(newParent, 0, DragDropAction.ChangParent);
            CanBeApplied = true;
        }
    }
}
