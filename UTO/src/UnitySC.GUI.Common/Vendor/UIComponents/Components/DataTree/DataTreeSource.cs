using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Agileo.GUI.Commands;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.Core.KeyGesture;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.DragDrop;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Factory;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Interfaces;

using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree
{
    public class DataTreeSource<T> : TreeNodeBase<T>, IDataTreeSource where T : class
    {
        #region Explicit Implmentation of IDataTreeSource

        ICommand IDataTreeSource.SortCommand => SortCommand;

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

        bool IKeyGestureHandler.OnKeyDown(KeyEventArgs keyEventArgs) => OnKeyDown(keyEventArgs);

        #endregion

        #region Displayed Items Management

        /// <summary>
        /// Get the flat collection of currently viewed <see cref="TreeNode{T}"/>.
        /// </summary>
        public ObservableCollection<TreeNode<T>> DisplayedTreeElements { get; } = new ObservableCollection<TreeNode<T>>();

        /// <summary>
        /// [UNITY SPECIFICITY] Need to have a public method
        /// Recursively builds the full list of visible elements and displays the root and expanded elements.
        /// </summary>
        public void Refresh() => RefreshVisibleTreeNodes();

        /// <summary>
        /// Recursively builds the full list of visible elements and displays the root and expanded elements.
        /// </summary>
        internal override void RefreshVisibleTreeNodes()
        {
            // Recursively builds the full list of visible elements
            base.RefreshVisibleTreeNodes();

            DispatcherHelper.DoInUiThread(
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
                });
        }

        /// <summary>
        /// Removes the element and all of its children from the visible list
        /// </summary>
        internal void RemoveFromVisibleItems(TreeNode<T> item)
        {
            DispatcherHelper.DoInUiThread(
                () =>
                {
                    DisplayedTreeElements.Remove(item);

                    var subItems = item.GetFlattenElements();
                    DisplayedTreeElements.RemoveRange(subItems);
                });
        }

        internal void RefreshVisibleTreeElementsFrom(TreeNode<T> itemChanged)
        {
            DispatcherHelper.DoInUiThread(
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

        public DragDropEngine<T> DragDrop { get; } = new();

        public SortEngine<TreeNode<T>> Sort { get; } = new();

        public FilterEngine<TreeNode<T>> Filter { get; } = new();

        public KeyGestureActionList<TreeNode<T>> KeyGestureActions { get; }

        public SearchEngine<T> Search { get; } = new();

        public TreeNodeFactory<T> NodeFactory { get; }

        /// <summary>
        /// Gets/Sets if the parent node is automatically expanded when adding an element.
        /// </summary>
        public bool AutoExpandOnNodeAdded { get; set; } = true;

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

        private bool _isFocused;

        public bool IsFocused
        {
            get => _isFocused;
            set => SetAndRaiseIfChanged(ref _isFocused, value);
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
        /// <param name="instantiateTreeNode">(Optional) Function allowing to create an instance of <see cref="TreeNode{T}"/> from an instance of <see cref="T"/>.</param>
        public DataTreeSource(Func<T, IEnumerable<T>> getChildren, Func<T, TreeNode<T>> instantiateTreeNode = null) : this()
        {
            NodeFactory = new DelegateTreeNodeFactory<T>(getChildren, instantiateTreeNode);
        }

        private DataTreeSource()
        {
            ExpandAllCommand = new DelegateCommand(ExpandAll);
            CollapseAllCommand = new DelegateCommand(CollapseAll);
            SyncWithSelectedCommand = new DelegateCommand(SyncWithSelected, SyncWithSelectedCommandCanExecute);
            SortCommand = new DelegateCommand<string>(SortCommandExecute, SortCommandCanExecute);

            DragDrop.DragDropActionApplied += OnDragDropActionApplied;
            Search.ApplySearch += OnApplySearch;
            Search.SearchEnded += OnSearchEnded;
            Filter.ApplyFilter += OnApplyFilter;
            Sort.Applied += OnApplySort;

            KeyGestureActions = new KeyGestureActionList<TreeNode<T>>(() => SelectedElement)
            {
                new KeyGestureAction<TreeNode<T>>(_ => SelectPreviousItem(), new KeyGesture(Key.Up)),
                new KeyGestureAction<TreeNode<T>>(_ => SelectNextItem(), new KeyGesture(Key.Down)),
                new KeyGestureAction<TreeNode<T>>(Expand, new KeyGesture(Key.Right)),
                new KeyGestureAction<TreeNode<T>>(Collapse, new KeyGesture(Key.Left)),
                new KeyGestureAction<TreeNode<T>>(ToggleIsExpanded, new KeyGesture(Key.Space)),
                new KeyGestureAction<TreeNode<T>>(_ => SelectLastItem(), new KeyGesture(Key.End)),
                new KeyGestureAction<TreeNode<T>>(_ => SelectFirstItem(), new KeyGesture(Key.Home)),
                new KeyGestureAction<TreeNode<T>>(_ => FocusSearch(), new KeyGesture(Key.F, ModifierKeys.Control))
            };
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

        protected virtual void OnSearchEnded(object sender, EventArgs e)
        {
            IsFocused = true;
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
            if (node == null) return;

            Select(node);
        }

        /// <summary>
        /// Selects the TreeNode passed as parameter.
        /// </summary>
        public void Select(TreeNode<T> treeElement)
        {
            if (_selectedElement != treeElement && (treeElement == null || treeElement.IsSelectable))
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
            return CreateAndAddTreeNode(item, null,true);
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

            var newTreeElement = CreateAndAddTreeNode(newElement, parent, true);
            if (parent != null && AutoExpandOnNodeAdded)
            {
                parent.IsExpanded = true;
            }

            return newTreeElement;
        }

        /// <summary>Adds the elements of the specified collection to the end of the root collection of the <see cref="DataTreeSource{T}"/>.</summary>
        /// <param name="collection">The collection whose elements should be added.</param>
        public IEnumerable<TreeNode<T>> AddRange(IEnumerable<T> collection) => AddRange(collection, (TreeNode<T>)null);

        /// <summary>Adds the elements of the specified collection to the end of the related <see cref="TreeNode{T}" /> of the specified parent.</summary>
        /// <param name="collection">The collection whose elements should be added.</param>
        /// <param name="parent">The parent on which the elements should be added.</param>
        public IEnumerable<TreeNode<T>> AddRange(IEnumerable<T> collection, T parent)
        {
            var parentNode = GetTreeNode(parent);
            if (parentNode == null)
            {
                throw new InvalidOperationException("TreeNode not found for specified parent");
            }

            return AddRange(collection, parentNode);
        }

        /// <summary>Adds the elements of the specified collection to the end of the specified <see cref="TreeNode{T}" />.</summary>
        /// <param name="collection">The collection whose elements should be added.</param>
        /// <param name="parent">The parent on which the elements should be added.</param>
        public IEnumerable<TreeNode<T>> AddRange(IEnumerable<T> collection, TreeNode<T> parent)
        {
            var nodes = new List<TreeNode<T>>();
            foreach (var item in collection)
            {
                if (item == null) throw new InvalidOperationException("Item cannot be null.");
                nodes.Add(CreateAndAddTreeNode(item, parent, false));
            }

            if (parent != null && AutoExpandOnNodeAdded)
            {
                parent.IsExpanded = true;
            }

            RefreshVisibleTreeNodes();
            return nodes;
        }

        private TreeNode<T> CreateAndAddTreeNode(T element, TreeNode<T> parent, bool syncDisplay)
        {
            var viewModel = RecursiveBuild(element);

            if (parent != null)
            {
                parent.InternalAdd(viewModel, syncDisplay);
            }
            else
            {
                InternalAdd(viewModel, syncDisplay);
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
        public bool Remove(TreeNode<T> node) => Remove(node, true);

        public bool Remove(TreeNode<T> node, bool syncDisplay)
        {
            // Delete from parent
            var parent = node.Parent;
            if (parent != null)
            {
                parent.InternalRemove(node, syncDisplay);
            }
            else
            {
                InternalRemove(node, syncDisplay);
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

        /// <summary>
        /// Removes the elements of the specified collection from the <see cref="DataTreeSource{T}" />.
        /// </summary>
        /// <param name="collection">The collection whose elements should be removed of the <see cref="DataTreeSource{T}" />.</param>
        public void RemoveRange(IEnumerable<T> collection)
        {
            RemoveRange(collection.Select(GetTreeNode).Where(x => x != null));
        }

        /// <summary>
        /// Removes the elements of the specified collection from the <see cref="DataTreeSource{T}" />.
        /// </summary>
        /// <param name="collection">The collection whose elements should be removed of the <see cref="DataTreeSource{T}" />.</param>
        public void RemoveRange(IEnumerable<TreeNode<T>> collection)
        {
            foreach (var node in collection)
            {
                Remove(node, false);
            }

            RefreshVisibleTreeNodes();
        }

        /// <summary>Removes all the elements that match the conditions defined by the specified predicate.</summary>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="DataTreeSource{T}" />.</returns>
        public int RemoveAll(Predicate<T> match)
        {
            var flattenElement = GetFlattenElements().Where(x => match(x.Model)).ToList();

            foreach (var node in flattenElement)
            {
                Remove(node, false);
            }

            RefreshVisibleTreeNodes();

            return flattenElement.Count;
        }

        #endregion

        #region Commands

        #region Expand all

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

        #endregion

        #region Collapse all

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

        #endregion

        #region Sync with selected

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

        #region Sort Command

        /// <summary>
        /// Gets a command used to sort the elements by passing a key corresponding to the <see cref="SortDefinition.DisplayName"/> property as a parameter.
        /// This command is used by the <see cref="Controls.DataTree"/> control to automatically make the link with the headers and the <see cref="SortEngine{T}"/>.
        /// </summary>
        public DelegateCommand<string> SortCommand { get; }

        private bool SortCommandCanExecute(string propertyName) => Sort.SortDefinitions.Any(definition => definition.PropertyName == propertyName);

        private void SortCommandExecute(string propertyName) => Sort.SetCurrentSorting(propertyName);

        #endregion

        #endregion

        #region Key Gesture

        private bool OnKeyDown(KeyEventArgs keyEventArgs)
        {
            foreach (var keyGestureAction in KeyGestureActions)
            {
                var result = keyGestureAction.ExecuteIfMatched(keyEventArgs, SelectedElement);
                if (!result) continue;

                keyEventArgs.Handled = true;
                return true;
            }

            return false;
        }

        private void SelectLastItem()
        {
            var lastItem = DisplayedTreeElements.LastOrDefault(node => node.IsSelectable);
            if (lastItem != null)
            {
                SelectedElement = lastItem;
            }
        }

        private void SelectFirstItem()
        {
            var firstItem = DisplayedTreeElements.FirstOrDefault(node => node.IsSelectable);
            if (firstItem != null)
            {
                SelectedElement = firstItem;
            }
        }

        private void SelectPreviousItem()
        {
            var index = SelectedElement == null ? -1 : DisplayedTreeElements.IndexOf(SelectedElement);

            if (index >= 0 && index < DisplayedTreeElements.Count)
            {
                var previousItem = DisplayedTreeElements.Take(index).LastOrDefault(node => node.IsSelectable);
                if (previousItem != null)
                {
                    SelectedElement = previousItem;
                }
            }
            else
            {
                SelectFirstItem();
            }
        }

        private void SelectNextItem()
        {
            var index = SelectedElement == null ? -1 : DisplayedTreeElements.IndexOf(SelectedElement);

            if (index >= 0)
            {
                var nextItem = DisplayedTreeElements.Skip(index + 1).FirstOrDefault(node => node.IsSelectable);
                if (nextItem != null)
                {
                    SelectedElement = nextItem;
                }
            }
            else
            {
                SelectFirstItem();
            }
        }

        private static void Expand(TreeNode<T> treeNode)
        {
            if (treeNode is { IsExpandable: true, IsExpanded: false })
            {
                treeNode.IsExpanded = true;
            }
        }

        private static void Collapse(TreeNode<T> treeNode)
        {
            if (treeNode is { IsExpandable: true, IsExpanded: true })
            {
                treeNode.IsExpanded = false;
            }
        }

        private static void ToggleIsExpanded(TreeNode<T> treeNode)
        {
            if (treeNode is { IsExpandable: true })
            {
                treeNode.IsExpanded = !treeNode.IsExpanded;
            }
        }

        private void FocusSearch()
        {
            if (Search?.SearchDefinitions.Count > 0)
            {
                Search.IsFocused = true;
            }
        }

        #endregion
    }
}
