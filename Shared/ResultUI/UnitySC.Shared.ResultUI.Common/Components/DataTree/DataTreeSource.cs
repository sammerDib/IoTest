using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.ResultUI.Common.Components.DataTree.DragDrop;
using UnitySC.Shared.ResultUI.Common.Components.DataTree.Events;
using UnitySC.Shared.ResultUI.Common.Components.DataTree.Factory;
using UnitySC.Shared.ResultUI.Common.Components.DataTree.Interfaces;
using UnitySC.Shared.ResultUI.Common.Components.Generic.Filters;
using UnitySC.Shared.ResultUI.Common.Components.Generic.Search;
using UnitySC.Shared.ResultUI.Common.Components.Generic.Sort;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.Helper;

using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree
{
    public class DataTreeSource<T> : TreeNodeBase<T>, IDataTreeSource where T : class
    {
        public event EventHandler DisplayedElementsChanged;

        #region Explicit Implmentation of IDataTreeSource

        ICollection IDataTreeSource.SourceView => SourceView;

        ISearchEngine IDataTreeSource.Search => Search;

        IDragDropEngine IDataTreeSource.DragDropBehavior => DragDrop;

        ISortEngine IDataTreeSource.Sort => Sort;

        FilterEngine IDataTreeSource.Filter => Filter;

        public ICollection SourceView => DisplayedTreeElements;

        object IDataTreeSource.SelectedValue
        {
            get => SelectedValue;
            set => SelectedValue = value as T;
        }

        bool IDataTreeSource.OnKeyDown(KeyEventArgs keyEventArgs) => OnKeyDown(keyEventArgs);

        #endregion

        #region Displayed Items Management

        /// <summary>
        /// Get the flat collection of currently viewed <see cref="TreeNode{T}"/>.
        /// </summary>
        public ObservableCollection<TreeNode<T>> DisplayedTreeElements { get; } = new ObservableCollection<TreeNode<T>>();

        /// <summary>
        /// Recursively builds the full list of visible elements and displays the root and expanded elements.
        /// </summary>
        internal override void RefreshVisibleTreeNodes()
        {
            // Recursively builds the full list of visible elements
            base.RefreshVisibleTreeNodes();

            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    // Displays the root and expanded elements
                    DisplayedTreeElements.Clear();
                    var flattenCollection = new List<TreeNode<T>>();
                    foreach (var element in VisibleTreeElements)
                    {
                        flattenCollection.Add(element);
                        if (element.IsExpanded)
                        {
                            flattenCollection.AddRange(element.GetFlattenVisibleElements());
                        }
                    }

                    DisplayedTreeElements.AddRange(flattenCollection);
                    DisplayedElementsChanged?.Invoke(this, EventArgs.Empty);
                });
        }

        /// <summary>
        /// Removes the element and all of its children from the visible list
        /// </summary>
        internal void RemoveFromVisibleItems(TreeNode<T> item)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    DisplayedTreeElements.Remove(item);

                    var subItems = item.GetFlattenElements();
                    DisplayedTreeElements.RemoveRange(subItems);
                });
        }

        internal void RefreshVisibleTreeElementsFrom(TreeNode<T> itemChanged)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    if (itemChanged.IsExpanded)
                    {
                        // Do nothing if the changed element is not currently visible
                        if (!DisplayedTreeElements.Contains(itemChanged))
                        {
                            return;
                        }

                        // Adds visible child elements
                        var itemsToAdd = itemChanged.GetFlattenVisibleElements();
                        var currentIndex = DisplayedTreeElements.IndexOf(itemChanged) + 1;
                        foreach (var itemToAdd in itemsToAdd)
                        {
                            DisplayedTreeElements.Insert(currentIndex, itemToAdd);
                            currentIndex++;
                        }
                    }
                    else
                    {
                        // Removes all child elements
                        var itemsToRemove = itemChanged.GetFlattenElements();
                        foreach (var itemToRemove in itemsToRemove)
                        {
                            DisplayedTreeElements.Remove(itemToRemove);
                        }
                    }
                });
        }

        #endregion

        #region Properties

        public override DataTreeSource<T> TreeSource
        {
            get => this;
            internal set => throw new InvalidOperationException();
        }

        public DragDropEngine<T> DragDrop { get; } = new DragDropEngine<T>();

        public SortEngine<TreeNode<T>> Sort { get; } = new SortEngine<TreeNode<T>>();

        public FilterEngine<TreeNode<T>> Filter { get; } = new FilterEngine<TreeNode<T>>();

        public List<KeyGestureAction<T>> KeyGestureActions { get; } = new List<KeyGestureAction<T>>();

        public SearchEngine<T> Search { get; } = new SearchEngine<T>();

        public TreeNodeFactory<T> NodeFactory { get; }

        #endregion Properties

        #region Notified Properties

        private TreeNode<T> _selectedElement;

        /// <summary>
        /// Gets or sets the selected <see cref="TreeNode{T}"/>
        /// </summary>
        public TreeNode<T> SelectedElement
        {
            get => _selectedElement;
            set => Select(value);
        }

        /// <summary>
        /// Gets or sets the selected <see cref="T"/> instance.
        /// </summary>
        public T SelectedValue
        {
            get => _selectedElement?.Model;
            set => Select(value);
        }

        #endregion Notified Properties

        #region Constructors

        /// <summary>
        /// Instantiate a <see cref="DataTreeSource{T}"/> with a custom <see cref="TreeNodeFactory{T}"/> implementation.
        /// </summary>
        public DataTreeSource(TreeNodeFactory<T> nodeFactory) : this()
        {
            NodeFactory = nodeFactory;
        }

        /// <summary>
        /// Instantiate a <see cref="DataTreeSource{T}"/>.
        /// </summary>
        /// <param name="getChildren">Function to get child elements from an instance of <see cref="T"/>.</param>
        /// <param name="instanciateTreeNode">(Optional) Function allowing to create an instance of <see cref="TreeNode{T}"/> from an instance of <see cref="T"/>.</param>
        public DataTreeSource(Func<T, IEnumerable<T>> getChildren, Func<T, TreeNode<T>> instanciateTreeNode = null) : this()
        {
            NodeFactory = new DelegateTreeNodeFactory<T>(getChildren, instanciateTreeNode);
        }

        private DataTreeSource()
        {
            ExpandAllCommand = new AutoRelayCommand(ExpandAll);
            CollapseAllCommand = new AutoRelayCommand(CollapseAll);
            SyncWithSelectedCommand = new AutoRelayCommand(SyncWithSelected, SyncWithSelectedCommandCanExecute);
            
            DragDrop.DragDropActionApplied += OnDragDropActionApplied;
            Search.ApplySearch += OnApplySearch;
            Filter.ApplyFilter += OnApplyFilter;
            Sort.Applied += OnApplySort;
        }

        #endregion

        #region Event Handlers

        #region Drag and Drop

        protected virtual void OnDragDropActionApplied(object sender, EventArgs e)
        {
            RefreshVisibleTreeNodes();
        }

        #endregion

        protected virtual void OnApplySearch(object sender, EventArgs e)
        {
            RefreshVisibleTreeNodes();
            if (string.IsNullOrEmpty(Search.SearchText))
            {
                if (SyncWithSelectedCommandCanExecute())
                {
                    SyncWithSelected();
                }
                else
                {
                    CollapseAll();
                }
            }
            else
            {
                ExpandAll();
            }
        }

        protected virtual void OnApplyFilter(object sender, EventArgs e)
        {
            RefreshVisibleTreeNodes();
        }

        protected virtual void OnApplySort(object sender, EventArgs e)
        {
            RefreshVisibleTreeNodes();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Get the instance of TreeNode associated with the model passed as a parameter. Returns null if no matching element was found.
        /// </summary>
        public TreeNode<T> GetTreeNode(T model)
        {
            foreach (var element in Nodes)
            {
                if (ReferenceEquals(element.Model, model))
                {
                    return element;
                }

                var treeElement = element.GetTreeElementByModel(model);
                if (treeElement != null)
                {
                    return treeElement;
                }
            }

            return null;
        }
        
        #endregion

        #region Build

        /// <summary>
        /// Builds the entire <see cref="TreeNode{T}"/> tree from the given collection.
        /// </summary>
        public void Reset(IEnumerable<T> roots)
        {
            InternalClear(false);
            foreach (var rootElement in roots)
            {
                InternalAdd(RecursiveBuild(rootElement), false);
            }

            RefreshVisibleTreeNodes();
            Filter.UpdatePossibleValue();

            RefreshSelectedElement();
        }

        /// <summary>
        /// Instantiates a <see cref="TreeNode{T}"/> for the given model and then constructs its child elements.
        /// </summary>
        /// <returns>A <see cref="TreeNode{T}"/> instance</returns>
        private TreeNode<T> RecursiveBuild(T element)
        {
            var treeElement = NodeFactory.InstantiateTreeNode(element);
            treeElement.TreeSource = this;
            BuildChildren(treeElement);
            return treeElement;
        }

        /// <summary>
        /// Retrieves the displayable child elements of the <see cref="TreeNode{T}"/> model and initiates their recursive construction.
        /// </summary>
        private void BuildChildren(TreeNode<T> treeElement)
        {
            var childs = NodeFactory.GetChildren(treeElement.Model);
            if (childs != null)
            {
                foreach (var extendedObject in childs)
                {
                    treeElement.InternalAdd(RecursiveBuild(extendedObject), false);
                }
            }
        }

        /// <summary>
        /// In the case of a reset, retrieves the new node associated with the previous selected model to select it again.
        /// </summary>
        private void RefreshSelectedElement()
        {
            var selectedElement = _selectedElement;
            if (selectedElement == null) return;

            var selectedValue = selectedElement.Model;
            var newSelectedElement = GetTreeNode(selectedValue);
            if (ReferenceEquals(_selectedElement, newSelectedElement)) return;

            Select(newSelectedElement);
        }

        #endregion

        #region Select

        /// <summary>
        /// Selects the TreeNode associated with the element passed as parameter.
        /// </summary>
        public void Select(T model)
        {
            var node = GetTreeNode(model);
            Select(node);
        }

        /// <summary>
        /// Selects the TreeNode passed as parameter.
        /// </summary>
        public void Select(TreeNode<T> treeElement)
        {
            if (_selectedElement != treeElement)
            {
                if (treeElement == null || treeElement.IsSelectable)
                {
                    if (_selectedElement != null)
                    {
                        _selectedElement.IsSelected = false;
                    }

                    _selectedElement = treeElement;
                    if (_selectedElement != null)
                    {
                        _selectedElement.IsSelected = true;
                    }
                }
            }

            OnPropertyChanged(nameof(SelectedElement));
            OnPropertyChanged(nameof(SelectedValue));
        }

        /// <summary>
        /// Disable current selection.
        /// </summary>
        public void Unselect()
        {
            if (_selectedElement != null)
            {
                _selectedElement.IsSelected = false;
                _selectedElement = null;

                OnPropertyChanged(nameof(SelectedElement));
                OnPropertyChanged(nameof(SelectedValue));
            }
        }

        /// <summary>
        /// Selects and expands the TreeNode associated with the element passed as parameter.
        /// </summary>
        public bool SelectAndExpand(T model)
        {
            var node = GetTreeNode(model);
            if (node == null) return false;

            SelectAndExpand(node);
            return true;
        }

        /// <summary>
        /// Selects and expands the TreeNode passed as parameter.
        /// </summary>
        public void SelectAndExpand(TreeNode<T> treeElement)
        {
            Select(treeElement);
            var item = treeElement.Parent;
            while (item != null)
            {
                item.IsExpanded = true;
                item = item.Parent;
            }
        }

        #endregion

        #region Add Element

        /// <summary>
        /// Adds an element to the root collection of the <see cref="DataTreeSource{T}"/>.
        /// </summary>
        public TreeNode<T> Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return CreateAndAddTreeNode(item, null);
        }

        /// <summary>
        /// Adds an element to the specified <see cref="T"/> object.
        /// </summary>
        public TreeNode<T> Add(T newElement, T parent)
        {
            var parentNode = GetTreeNode(parent);
            return parentNode == null ? null : Add(newElement, parentNode);
        }

        /// <summary>
        /// Adds an element to the specified <see cref="TreeNode{T}"/>.
        /// </summary>
        public TreeNode<T> Add(T newElement, TreeNode<T> parent)
        {
            if (newElement == null) throw new ArgumentNullException(nameof(newElement));

            var newTreeElement = CreateAndAddTreeNode(newElement, parent);
            if (parent != null)
            {
                parent.IsExpanded = true;
            }

            return newTreeElement;
        }

        private TreeNode<T> CreateAndAddTreeNode(T element, TreeNode<T> parent)
        {
            var viewModel = RecursiveBuild(element);

            if (parent != null)
            {
                parent.InternalAdd(viewModel, true);
            }
            else
            {
                InternalAdd(viewModel, true);
            }
            
            return viewModel;
        }

        #endregion

        #region Remove

        /// <summary>
        /// Remove the associated <see cref="TreeNode{T}"/> from the <see cref="DataTreeSource{T}"/>.
        /// </summary>
        /// <param name="model">Model to remove.</param>
        public bool Remove(T model)
        {
            var node = GetTreeNode(model);
            if (node == null) return false;
            return Remove(node);
        }

        /// <summary>
        /// Remove the <see cref="TreeNode{T}"/> from the <see cref="DataTreeSource{T}"/>.
        /// </summary>
        /// <param name="node"><see cref="TreeNode{T}"/> to remove.</param>
        public bool Remove(TreeNode<T> node)
        {
            // Delete from parent
            var parent = node.Parent;
            if (parent != null)
            {
                parent.InternalRemove(node, true);
            }
            else
            {
                InternalRemove(node, true);
            }

            // Unselect element
            if (_selectedElement == node)
            {
                Unselect();
            }

            RemoveChildren(node);

            return true;
        }

        /// <summary>
        /// Recursively removes and disposes all children
        /// </summary>
        private static void RemoveChildren(TreeNode<T> node)
        {
            foreach (var childViewModel in node.Nodes.ToList())
            {
                RemoveChildren(childViewModel);
                node.InternalRemove(childViewModel, false);
            }

            node.Dispose();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets a command allowing to expand all the nodes.
        /// </summary>
        public ICommand ExpandAllCommand { get; }

        /// <summary>
        /// Expands all the nodes.
        /// </summary>
        public void ExpandAll()
        {
            var allElements = GetFlattenElements();
            foreach (var treeElement in allElements.Where(treeElement => treeElement.IsExpandable))
            {
                treeElement.SetIsExpanded(true);
            }

            RefreshVisibleTreeNodes();
        }

        /// <summary>
        /// Gets a command allowing to collapse all the nodes.
        /// </summary>
        public ICommand CollapseAllCommand { get; }

        /// <summary>
        /// Collapses all the nodes.
        /// </summary>
        public void CollapseAll()
        {
            var allElements = GetFlattenElements();
            foreach (var treeElement in allElements.Where(treeElement => treeElement.IsExpandable))
            {
                treeElement.SetIsExpanded(false);
            }

            RefreshVisibleTreeNodes();
        }

        /// <summary>
        /// Gets a command allowing to collapse all the nodes and to expand only the nodes allowing to display the selected element.
        /// </summary>
        public ICommand SyncWithSelectedCommand { get; }

        private bool SyncWithSelectedCommandCanExecute() => SelectedElement != null;

        /// <summary>
        /// Collapses all the nodes and expands only the nodes allowing to display the selected element.
        /// </summary>
        public void SyncWithSelected()
        {
            if (SelectedElement == null) return;
            var allElements = GetFlattenElements();
            foreach (var treeElement in allElements.Where(treeElement => treeElement.IsExpandable))
            {
                treeElement.SetIsExpanded(false);
            }

            SelectedElement.RecursiveExpand();

            RefreshVisibleTreeNodes();
        }

        #endregion

        #region Key Gesture

        private bool OnKeyDown(KeyEventArgs keyEventArgs)
        {
            foreach (var keyGestureAction in KeyGestureActions)
            {
                var result = keyGestureAction.ExecuteIfMatched(keyEventArgs, SelectedElement);
                if (result)
                {
                    keyEventArgs.Handled = true;
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
