using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree
{
    public abstract class TreeNodeBase<T> : ObservableObject, IDisposable where T : class
    {
        #region Properties

        private readonly List<TreeNode<T>> _nodes = new List<TreeNode<T>>();

        /// <summary>
        /// Collection of children elements
        /// </summary>
        public ReadOnlyCollection<TreeNode<T>> Nodes => _nodes.AsReadOnly();

        private readonly List<TreeNode<T>> _visibleNodes = new List<TreeNode<T>>();

        /// <summary>
        /// Collection of child elements to display when the element is expanded after sorts and filters are applied.
        /// </summary>
        public ReadOnlyCollection<TreeNode<T>> VisibleTreeElements => _visibleNodes.AsReadOnly();

        public bool IsExpandable => _visibleNodes.Count > 0;

        public abstract DataTreeSource<T> TreeSource { get; internal set; }

        #endregion

        #region Display

        /// <summary>
        /// Allows you to handle the internal collections without affecting the display of the <see cref="DataTreeSource{T}"/>,
        /// all the recursive content will be redisplayed at the end of the execution of the action.
        /// </summary>
        /// <param name="syncDisplay">Allows you to activate or deactivate the reinterpretation of visible elements.</param>
        /// <param name="action">Action to execute.</param>
        protected void ModifyCollection(bool syncDisplay, Action action = null)
        {
            if (syncDisplay)
            {
                // Remove all child elements from the DataTreeSource visualization.
                foreach (var treeElementViewModel in _nodes)
                {
                    TreeSource.RemoveFromVisibleItems(treeElementViewModel);
                }
            }

            // Execute action
            action?.Invoke();

            // Notify the change of the collection to interpret the list of visible child elements
            RaiseCollectionChanged(syncDisplay);
        }

        /// <summary>
        /// Rebuild the list of visible children and notify the change to the <see cref="DataTreeSource{T}"/>.
        /// </summary>
        /// <param name="syncDisplay"></param>
        private void RaiseCollectionChanged(bool syncDisplay)
        {
            if (syncDisplay)
            {
                RefreshVisibleTreeNodes();

                // If the changed element is a TreeElement, refresh the displayed elements from it.
                if (this is TreeNode<T> treeElement)
                {
                    TreeSource.RefreshVisibleTreeElementsFrom(treeElement);
                }
            }

            OnCollectionChanged();
        }

        #endregion
        
        protected virtual void OnCollectionChanged()
        {

        }

        #region Public Methods

        /// <summary>
        /// Recursively retrieves a list containing all of the child elements.
        /// </summary>
        public List<TreeNode<T>> GetFlattenElements()
        {
            var flattenCollection = new List<TreeNode<T>>();
            foreach (var element in _nodes)
            {
                flattenCollection.Add(element);
                flattenCollection.AddRange(element.GetFlattenElements());
            }
            return flattenCollection;
        }
        
        #endregion
        
        #region Internal API

        internal void InternalMove(TreeNode<T> viewModel, int newIndex, bool syncDisplay)
        {
            ModifyCollection(syncDisplay, () =>
            {
                if (newIndex == -1 || viewModel == null) return;

                var realIndex = _nodes.IndexOf(viewModel) < newIndex ? newIndex - 1 : newIndex;
                var previousIndex = _nodes.IndexOf(viewModel);

                _nodes.RemoveAt(previousIndex);
                _nodes.Insert(realIndex, viewModel);
            });
        }

        internal void InternalAdd(TreeNode<T> treeNode, bool syncDisplay)
        {
            ModifyCollection(syncDisplay, () =>
            {
                if (this is TreeNode<T> self)
                {
                    treeNode.Parent = self;
                }

                _nodes.Add(treeNode);
            });
        }

        internal void InternalRemove(TreeNode<T> treeNode, bool syncDisplay)
        {
            ModifyCollection(syncDisplay, () =>
            {
                treeNode.Parent = null;
                _nodes.Remove(treeNode);
            });

            OnPropertyChanged(nameof(IsExpandable));
        }

        internal void InternalClear(bool syncDisplay)
        {
            ModifyCollection(syncDisplay, () =>
            {
                _nodes.Clear();
            });
        }

        #endregion

        #region Sorting and Filtering

        /// <summary>
        /// Recursively builds the list of visible child elements by applying the active sorts and filters.
        /// </summary>
        internal virtual void RefreshVisibleTreeNodes()
        {
            _visibleNodes.Clear();

            if (Nodes.Count == 0)
            {
                return;
            }

            foreach (var child in Nodes)
            {
                child.RefreshVisibleTreeNodes();
            }

            var visibleElements = Nodes.Where(node => node.IsVisible);

            if (TreeSource != null)
            {
                // Sorting
                visibleElements = TreeSource.Sort.GetAll(visibleElements);

                // Filtering & Search
                visibleElements = visibleElements.Where(node => node.VisibleTreeElements.Count > 0 || (TreeSource.Filter.Match(node) && TreeSource.Search.Match(node.Model)));
            }

            _visibleNodes.AddRange(visibleElements);

            OnPropertyChanged(nameof(IsExpandable));
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
