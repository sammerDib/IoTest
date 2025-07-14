using System.Collections.Generic;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Factory
{
    public abstract class TreeNodeFactory<T> where T : class
    {
        /// <summary>
        /// Instantiates a <see cref="TreeNode{T}"/> for the given model.
        /// </summary>
        /// <returns>A <see cref="TreeNode{T}"/> instance</returns>
        public abstract TreeNode<T> InstantiateTreeNode(T model);

        /// <summary>
        /// Retrieves from the model the collection of elements that can be displayed in the <see cref="DataTreeSource{T}"/>.
        /// </summary>
        public abstract IEnumerable<T> GetChildren(T model);
    }
}
