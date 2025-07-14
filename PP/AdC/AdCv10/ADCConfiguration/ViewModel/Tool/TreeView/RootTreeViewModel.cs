using System.Collections.Generic;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using Dto = UnitySC.DataAccess.Dto;

using Utils.ViewModel;

namespace ADCConfiguration.ViewModel.Tool.TreeView
{
    public class RootTreeViewModel : TreeViewItemViewModel
    {
        internal bool _isHistoryMode;
        public RootTreeViewModel(bool isHistoryMode) : base(null, false)
        {
            _isHistoryMode = isHistoryMode;
        }

        public void Init(IEnumerable<Dto.Tool> tools)
        {
            base.Children.Clear();
            foreach (var tool in tools)
                base.Children.Add(new ToolViewModel(tool, _isHistoryMode));
        }

        private TreeViewItemViewModel _selectedTreeViewItem;
        public TreeViewItemViewModel SelectedTreeViewItem
        {
            get { return _selectedTreeViewItem; }
            set
            {
                bool hasChangedBefore = HasChanged;
                _selectedTreeViewItem = value;
                OnPropertyChanged(nameof(SelectedTreeViewItem));
                HasChanged = hasChangedBefore;
            }
        }

        private void SelectedTreeViewItemChanged()
        {
            SelectedTreeViewItem = FindSelectedNode(this);
        }

        private TreeViewItemViewModel FindSelectedNode(TreeViewItemViewModel node)
        {
            if (node == null || node.IsSelected)
                return node;
            else if (node.Children != null)
            {
                foreach (var childNode in node.Children)
                {
                    var findedNode = FindSelectedNode(childNode);
                    if (findedNode != null)
                        return findedNode;
                }
            }
            return null;

        }

        private void FindChangedNodes(TreeViewItemViewModel currentNode, ref List<TreeViewItemViewModel> changedNodes)
        {
            if (currentNode == null || currentNode.HasChanged)
            {
                changedNodes.Add(currentNode);
            }

            if (currentNode.Children != null)
            {
                foreach (var childNode in currentNode.Children)
                {
                    FindChangedNodes(childNode, ref changedNodes);
                }
            }
        }

        private void GetNodes(TreeViewItemViewModel currentNode, ref List<TreeViewItemViewModel> nodes)
        {
            nodes.Add(currentNode);

            if (currentNode.Children != null)
            {
                foreach (var childNode in currentNode.Children)
                {
                    GetNodes(childNode, ref nodes);
                }
            }
        }

        public List<TreeViewItemViewModel> GetAllNodes()
        {
            List<TreeViewItemViewModel> nodes = new List<TreeViewItemViewModel>();
            GetNodes(this, ref nodes);
            return nodes;
        }


        #region Commands

        private AutoRelayCommand _selectedTreeViewItemChangedCommand = null;
        public AutoRelayCommand SelectedTreeViewItemChangedCommand
        {
            get { return _selectedTreeViewItemChangedCommand ?? (_selectedTreeViewItemChangedCommand = new AutoRelayCommand(() => { SelectedTreeViewItemChanged(); })); }
        }
    }

    #endregion
}

