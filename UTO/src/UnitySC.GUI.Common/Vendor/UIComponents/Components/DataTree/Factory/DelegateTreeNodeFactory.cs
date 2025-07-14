using System;
using System.Collections.Generic;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Factory
{
    public class DelegateTreeNodeFactory<T> : TreeNodeFactory<T> where T : class
    {
        private readonly Func<T, IEnumerable<T>> _getChildren;
        private readonly Func<T, TreeNode<T>> _instantiateTreeNode;

        /// <summary>
        /// Instantiates a <see cref="DelegateTreeNodeFactory{T}"/>.
        /// </summary>
        /// <param name="getChildren">Function to retrieves from the model the collection of elements that can be displayed in the <see cref="DataTreeSource{T}"/></param>
        /// <param name="instantiateTreeNode">Function to instantiates a <see cref="TreeNode{T}"/> for the given model.</param>
        public DelegateTreeNodeFactory(Func<T, IEnumerable<T>> getChildren, Func<T, TreeNode<T>> instantiateTreeNode = null)
        {
            _getChildren = getChildren;
            _instantiateTreeNode = instantiateTreeNode;
        }

        #region Overrides of TreeNodeFactory<T>

        public override TreeNode<T> InstantiateTreeNode(T model)
        {
            return _instantiateTreeNode != null ? _instantiateTreeNode(model) : new TreeNode<T>(model);
        }

        public override IEnumerable<T> GetChildren(T model) => _getChildren(model);

        #endregion
    }
}
