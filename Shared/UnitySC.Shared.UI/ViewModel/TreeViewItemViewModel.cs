using System.Collections.ObjectModel;
using System.Linq;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.UI.ViewModel
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : ValidationViewModelBase
    {
        #region Data

        private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        private readonly ObservableCollection<TreeViewItemViewModel> _children;
        private readonly TreeViewItemViewModel _parent;

        private bool _isExpanded;
        private bool _isSelected;

        private bool _disableCollapsing = false;
        private bool _disableCollapsingOnChildrenPendingChange = false;

        #endregion Data

        #region Constructors

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren)
                _children.Add(DummyChild);
        }

        // This is used to create the DummyChild instance.
        public TreeViewItemViewModel()
        {
        }

        #endregion Constructors

        #region Presentation Members

        #region Children

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        #endregion Children

        #region HasLoadedChildren

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }

        #endregion HasLoadedChildren

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    if (value == false)
                    {
                        if (DisableCollapsing)
                            return;

                        if (DisableCollapsingOnChildrenPendingChange && HasChildrenChanged)
                            return;
                    }

                    _isExpanded = value;
                    bool oldHasChanged = HasChanged;
                    OnPropertyChanged();
                    HasChanged = oldHasChanged;
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }
            }
        }

        public bool DisableCollapsing
        {
            get { return _disableCollapsing; }
            set
            {
                if (value != _disableCollapsing)
                {
                    _disableCollapsing = value;
                    bool oldHasChanged = HasChanged;
                    OnPropertyChanged();
                    HasChanged = oldHasChanged;
                    if (_disableCollapsing && !_isExpanded)
                        IsExpanded = true;
                }
            }
        }

        public bool DisableCollapsingOnChildrenPendingChange
        {
            get { return _disableCollapsingOnChildrenPendingChange; }
            set
            {
                if (value != _disableCollapsingOnChildrenPendingChange)
                {
                    _disableCollapsingOnChildrenPendingChange = value;
                    bool oldHasChanged = HasChanged;
                    OnPropertyChanged();
                    HasChanged = oldHasChanged;
                    if (_disableCollapsingOnChildrenPendingChange && HasChanged && !IsExpanded)
                        IsExpanded = true;
                }
            }
        }

        public bool HasChildrenChanged
        {
            get
            {
                if (HasDummyChild || Children.Count < 1)
                    return HasChanged;

                return Children.Any(x => x.HasChildrenChanged);
            }
        }

        #endregion IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    bool oldHasChanged = HasChanged;
                    this.OnPropertyChanged();
                    HasChanged = oldHasChanged;
                }
            }
        }

        #endregion IsSelected

        #region LoadChildren

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
        }

        public ActorCategory ActorCategory { get; set; }

        #endregion LoadChildren        

        #region Parent

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        #endregion Parent

        #endregion Presentation Members

        public virtual void Save()
        {
        }

        public virtual bool RemoveNode(TreeViewItemViewModel node)
        {
            return false;
        }
        public virtual bool CanUserRemove()
        {
            return false;
        }
        /// <summary>
        ///  Delete in data base
        /// </summary>
        public virtual void Archive() { }
    }
}
