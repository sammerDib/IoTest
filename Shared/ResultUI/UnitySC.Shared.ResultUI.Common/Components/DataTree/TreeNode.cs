using System.Collections.Generic;
using System.Windows.Input;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.ResultUI.Common.Components.DataTree.Interfaces;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree
{
    public class TreeNode<T> : TreeNodeBase<T>, ITreeNode where T : class
    {
        #region ITreeElement

        bool ITreeNode.IsExpanded => IsExpanded;

        ITreeNode ITreeNode.Parent => Parent;

        int ITreeNode.VisibleChildCount => VisibleTreeElements.Count;

        int ITreeNode.Level => Level;

        bool ITreeNode.IsClicked
        {
            get => IsClicked;
            set => IsClicked = value;
        }

        bool ITreeNode.IsNextParentOfDraggedElement => IsNextParentOfDraggedElement;

        bool ITreeNode.IsExpandable => IsExpandable;

        bool ITreeNode.IsMouseOver
        {
            get => IsMouseOver;
            set => IsMouseOver = value;
        }

        bool ITreeNode.IsSelected => IsSelected;

        ICommand ITreeNode.OnDragCommand => OnDragCommand;

        ICommand ITreeNode.SelectCommand => SelectCommand;

        IDataTreeSource ITreeNode.DataTreeSource => TreeSource;

        #endregion

        #region Notified Properties

        private bool _isExpanded;

        /// <summary>
        /// Determines whether the element is expanded or collapsed.
        /// Use the setter only if the <see cref="DataTreeSource{T}"/> needs to be updated.
        /// In case you need to modify multiple elements, prefer the <see cref="SetIsExpanded"/> method and call the <see cref="DataTreeSource{T}.RefreshVisibleTreeNodes"/>  method of the <see cref="DataTreeSource{T}"/>.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                OnPropertyChanged();
                TreeSource?.RefreshVisibleTreeElementsFrom(this);
            }
        }
        
        /// <summary>
        /// Updates the <see cref="IsExpanded"/> property without notifying the <see cref="DataTreeSource{T}"/> update.
        /// </summary>
        public void SetIsExpanded(bool value)
        {
            if (_isExpanded == value) return;
            _isExpanded = value;
            OnPropertyChanged(nameof(IsExpanded));
        }

        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnPropertyChanged();
                
                if (!value)
                {
                    // Reinterprets the visible elements of the parents.
                    Parent?.RefreshVisibleTreeNodes();
                    TreeSource.RemoveFromVisibleItems(this);
                }
                else
                {
                    // It is possible to add elements dynamically but for the sake of simplicity we reset the full contents of the parent.
                    if (Parent != null)
                    {
                        // If the element has a parent, it is sufficient to reinterpret its content
                        Parent.ModifyCollection(true);
                    }
                    else
                    {
                        TreeSource?.RefreshVisibleTreeNodes();
                    }
                }
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelectable = true;

        public bool IsSelectable
        {
            get => _isSelectable;
            set
            {
                if (_isSelectable == value) return;
                _isSelectable = value;
                OnPropertyChanged();
            }
        }

        private bool _isMouseOver;

        public bool IsMouseOver
        {
            get => _isMouseOver;
            set
            {
                if (_isMouseOver == value) return;
                _isMouseOver = value;
                OnPropertyChanged();
            }
        }

        private bool _isNextParentOfDraggedElement;

        public bool IsNextParentOfDraggedElement
        {
            get => _isNextParentOfDraggedElement;
            set
            {
                if (_isNextParentOfDraggedElement == value) return;
                _isNextParentOfDraggedElement = value;
                OnPropertyChanged();
            }
        }

        private bool _isClicked;

        public bool IsClicked
        {
            get => _isClicked;
            set
            {
                if (_isClicked == value) return;
                _isClicked = value;
                OnPropertyChanged();
            }
        }

        #endregion Notified Properties

        #region Properties

        public TreeNode<T> Parent { get; internal set; }

        public override DataTreeSource<T> TreeSource { get; internal set; }

        public T Model { get; }

        public int Level
        {
            get
            {
                if (Parent == null) return 0;
                return Parent.Level + 1;
            }
        }

        public bool HasChild => Nodes.Count > 0;

        #endregion Properties

        #region Expression-bodied Properties

        /// <summary>
        /// Returns the lowest level parent or itself if the element is root.
        /// </summary>
        public TreeNode<T> TopParent => Parent == null ? this : Parent.TopParent;

        public bool IsRoot => Parent == null || TreeSource.Nodes.Contains(this);

        #endregion Expression-bodied Properties

        public TreeNode(T model)
        {
            Model = model;

            SelectCommand = new AutoRelayCommand(SelectCommandExecute, SelectCommandCanExecute);
            OnDragCommand = new AutoRelayCommand(OnDragCommandExecute);
        }

        #region Public Methods
        
        public List<TreeNode<T>> GetAllParents()
        {
            var parents = new List<TreeNode<T>>();
            var currentParent = Parent;
            while (currentParent != null)
            {
                parents.Add(currentParent);
                currentParent = currentParent.Parent;
            }

            return parents;
        }

        public void Select()
        {
            if (SelectCommandCanExecute())
            {
                SelectCommandExecute();
            }
        }

        #endregion

        #region Internal Methods

        internal TreeNode<T> GetTreeElementByModel(T model)
        {
            foreach (var element in Nodes)
            {
                if (ReferenceEquals(element.Model, model)) return element;
                var e = element.GetTreeElementByModel(model);
                if (e != null) return e;
            }

            return null;
        }
        
        protected internal List<TreeNode<T>> GetFlattenVisibleElements()
        {
            var flattenCollection = new List<TreeNode<T>>();

            foreach (var element in VisibleTreeElements)
            {
                flattenCollection.Add(element);
                if (element.IsExpanded)
                {
                    flattenCollection.AddRange(element.GetFlattenVisibleElements());
                }
            }
            return flattenCollection;
        }

        /// <summary>
        /// Expand the element as well as all its parents without updating the <see cref="DataTreeSource{T}"/>
        /// </summary>
        internal void RecursiveExpand()
        {
            if (Parent == null) return;
            Parent.SetIsExpanded(true);
            Parent.RecursiveExpand();
        }

        #endregion

        #region Commands

        public AutoRelayCommand SelectCommand { get; }
        
        protected virtual void SelectCommandExecute()
        {
            TreeSource.SelectedElement = this;
        }

        protected virtual bool SelectCommandCanExecute()
        {
            return IsSelectable && TreeSource?.SelectedElement != this;
        }
        
        public AutoRelayCommand OnDragCommand { get; }

        private void OnDragCommandExecute()
        {
            TreeSource.DragDrop.SetupNewDragDropAction(this);
        }

        #endregion Commands

        #region Overrides

        protected override void OnCollectionChanged()
        {
            base.OnCollectionChanged();
            OnPropertyChanged(nameof(HasChild));
        }

        #endregion
    }
}
