using System;
using System.Collections.Generic;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using Utils.ViewModel;

using Dto = UnitySC.DataAccess.Dto;

namespace ADC.ViewModel.Ada.ChamberTreeView
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class RootTreeViewModel : TreeViewItemViewModel
    {
        private Action _onSelectedItemChanged;

        public RootTreeViewModel() : base(null, false)
        {
        }

        public void Init(IEnumerable<Dto.Tool> tools, Action onSelectedItemChanged)
        {
            base.Children.Clear();
            foreach (Dto.Tool tool in tools)
                base.Children.Add(new ToolViewModel(tool));
            IsExpanded = true;
            _onSelectedItemChanged = onSelectedItemChanged;
        }

        private TreeViewItemViewModel _selectedTreeViewItem;
        public TreeViewItemViewModel SelectedTreeViewItem
        {
            get { return _selectedTreeViewItem; }
            set
            {
                _selectedTreeViewItem = value;
                OnPropertyChanged(nameof(SelectedTreeViewItem));
                if (_onSelectedItemChanged != null)
                    _onSelectedItemChanged.Invoke();
            }
        }

        private void SelectedTreeViewItemChanged()
        {
            TreeViewItemViewModel node = FindSelectedNode(this);
            if (node is ChamberViewModel)
                SelectedTreeViewItem = node;
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

