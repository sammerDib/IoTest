using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree.DragDrop
{
    public interface IDragDropEngine
    {
        bool RefreshCanApplyDragDropAction();

        void ApplyCurrentDragDropAction();

        IDragDropAction CurrentDragDropAction { get; }

        int? GetIndex(object element);
    }

    public class DragDropEngine<T> : ObservableObject, IDragDropEngine where T : class
    {
        #region IDragDropEngine

        IDragDropAction IDragDropEngine.CurrentDragDropAction => CurrentDragDropAction;

        bool IDragDropEngine.RefreshCanApplyDragDropAction() => RefreshCanApplyDragDropAction();
        void IDragDropEngine.ApplyCurrentDragDropAction() => ApplyCurrentDragDropAction();

        int? IDragDropEngine.GetIndex(object element)
        {
            if (element is TreeNode<T> treeElement)
            {
                return GetIndex(treeElement);
            }

            return null;
        }

        #endregion
        
        public event EventHandler DragDropActionApplied;

        #region Properties

        private DragDropActionParameter<T> _currentDragDropAction;

        public DragDropActionParameter<T> CurrentDragDropAction
        {
            get => _currentDragDropAction;
            private set
            {
                if (_currentDragDropAction == value) return;
                _currentDragDropAction = value;
                OnPropertyChanged();
            }
        }

        public bool IsApplying { get; private set; }

        #endregion

        #region Indexes

        public int? GetIndex(TreeNode<T> element)
        {
            return element.IsRoot ? element.TreeSource.Nodes.IndexOf(element) : element.Parent?.Nodes.IndexOf(element);
        }

        #endregion

        public void SetupNewDragDropAction(TreeNode<T> draggedTreeElement)
        {
            CurrentDragDropAction?.ResetProperties();
            CurrentDragDropAction = draggedTreeElement == null ? null : new DragDropActionParameter<T>(draggedTreeElement);
        }

        public bool RefreshCanApplyDragDropAction()
        {
            if (CurrentDragDropAction == null || DragDropImplementation == null) return false;

            var canApply = true;
            if (CurrentDragDropAction.ActionType != DragDropAction.None)
            {
                if (CurrentDragDropAction.ActionType.ChangeParent())
                {
                    // An element can not change parent if the target parent is already the current parent
                    if (CurrentDragDropAction.NewParent == CurrentDragDropAction.DraggedElement.Parent) canApply = false;

                    // An item can not be added to its own sub-list
                    if (CurrentDragDropAction.NewParent != null && 
                        CurrentDragDropAction.NewParent.GetAllParents().Contains(CurrentDragDropAction.DraggedElement)) canApply = false;
                }

                if (CurrentDragDropAction.ActionType == DragDropAction.ChangeOrder)
                {
                    // Moving an item to the same place is useless
                    var draggedElementIndex = GetIndex(CurrentDragDropAction.DraggedElement);
                    if (draggedElementIndex == CurrentDragDropAction.NewIndex || draggedElementIndex + 1 == CurrentDragDropAction.NewIndex) canApply = false;
                }

                // Adds integration internal logic
                canApply = canApply && DragDropImplementation.CanApplyDragDropAction(CurrentDragDropAction);
            }
            else
            {
                canApply = false;
            }

            CurrentDragDropAction.SetupTreeElementProperties(canApply);
            return canApply;
        }

        #region Apply Actions

        internal void ApplyCurrentDragDropAction()
        {
            if (RefreshCanApplyDragDropAction())
            {
                IsApplying = true;
                if (DragDropImplementation.ApplyDragDropAction(CurrentDragDropAction))
                {
                    ApplyDragDropAction(CurrentDragDropAction);
                }

                IsApplying = false;
            }

            SetupNewDragDropAction(null);
        }

        public void ApplyDragDropAction(DragDropActionParameter<T> dragDropAction)
        {
            if (dragDropAction.ActionType.ChangeParent()) ApplyChangeParent(dragDropAction);
            if (dragDropAction.ActionType.ChangeOrder()) ApplyChangeOrder(dragDropAction);

            DragDropActionApplied?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyChangeParent(DragDropActionParameter<T> dragDropAction)
        {
            var draggedElement = dragDropAction.DraggedElement;
            if (draggedElement.IsRoot)
            {
                draggedElement.TreeSource.InternalRemove(draggedElement, false);
            }
            else
            {
                draggedElement.Parent.InternalRemove(draggedElement, false);
            }

            if (dragDropAction.NewParent == null)
            {
                // Add to root
                draggedElement.TreeSource.InternalAdd(draggedElement, false);
            }
            else
            {
                // Add to new parent
                dragDropAction.NewParent.InternalAdd(draggedElement, false);
            }
        }

        private void ApplyChangeOrder(DragDropActionParameter<T> dragDropAction)
        {
            var newIndex = dragDropAction.NewIndex;
            var draggedElement = dragDropAction.DraggedElement;

            if (draggedElement.IsRoot)
            {
                draggedElement.TreeSource.InternalMove(draggedElement, newIndex, false);
            }
            else
            {
                var parent = draggedElement.Parent;
                if (parent == null) return;
                if (!parent.Nodes.Contains(draggedElement)) return;

                parent.InternalMove(draggedElement, newIndex, false);
            }
        }
        
        #endregion

        #region Setup

        public DelegateDragDropImplementation<T> DragDropImplementation { get; set; }
        
        #endregion
    }

    public class DelegateDragDropImplementation<T> where T : class
    {
        private readonly Func<DragDropActionParameter<T>, bool> _applyDragDropExecute;
        private readonly Func<DragDropActionParameter<T>, bool> _applyDragDropCanExecute;

        /// <summary>
        /// Defined if the Drag & Drop action can be performed.
        /// If the return value of the method is <see cref="bool.True"/>, you must override the <see cref="ApplyDragDropAction"/> method to implement the behavior related to models.
        /// </summary>
        public bool CanApplyDragDropAction(DragDropActionParameter<T> dragDropAction)
        {
            return _applyDragDropCanExecute == null || _applyDragDropCanExecute(dragDropAction);
        }

        /// <summary>
        /// Apply the action of Drag & Drop on the model.
        /// The TreeElements move action will be performed automatically if the return value of the method is <see cref="bool.True"/>.
        /// If you don't want to trigger the action automatically, return false.
        /// </summary>
        public bool ApplyDragDropAction(DragDropActionParameter<T> dragDropAction)
        {
            return _applyDragDropExecute(dragDropAction);
        }

        public void OnDragDropActionApplied(TreeNode<T> draggedElement, TreeNode<T> previousParent)
        {
        }

        public DelegateDragDropImplementation(Func<DragDropActionParameter<T>, bool> applyDragDropExecute, Func<DragDropActionParameter<T>, bool> applyDragDropCanExecute)
        {
            _applyDragDropExecute = applyDragDropExecute;
            _applyDragDropCanExecute = applyDragDropCanExecute;
        }
    }
}
