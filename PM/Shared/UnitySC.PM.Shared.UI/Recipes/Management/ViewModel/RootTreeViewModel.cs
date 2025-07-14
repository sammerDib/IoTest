using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.DataAccess.Dto;
using UnitySC.PM.Shared.UI.Recipes.Management.View;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;
using UnitySC.PM.Shared.UI.Main;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public class RootTreeViewModel : TreeViewItemViewModel
    {
        private ActorCategory _actorCategory;

        public IBusy BusyVm { get; }

        public RootTreeViewModel(IBusy busyVm, ActorCategory actorCategory = ActorCategory.Unknown) : base(null, false)
        {
            IsExpanded = true;
            DisableCollapsing = true;
            _actorCategory = actorCategory;
            BusyVm = busyVm;
        }

        public void Init(IEnumerable<Product> products)
        {
            base.Children.Clear();
            foreach (var product in products)
            {
                base.Children.Add(new ProductViewModel(product, this, _actorCategory) { DisableCollapsingOnChildrenPendingChange = true });
            }
        }

        internal void Update(IEnumerable<Product> products)
        {
            // New and modified products
            foreach (var product in products)
            {
                var productTreeItem = base.Children.FirstOrDefault(p => (p as ProductViewModel).Id == product.Id);
                if (productTreeItem != null)
                {
                    var productTreeItemChanged = base.Children.FirstOrDefault(p => ((p as ProductViewModel).Id == product.Id) && (p.HasChanged || p.Children.Where(o => o.HasChanged).Count() > 0));

                    // the product already exists
                    if (productTreeItemChanged != null)
                    {
                        (productTreeItemChanged as ProductViewModel).Update(product);
                    }
                }
                else
                {
                    base.Children.Add(new ProductViewModel(product, this, _actorCategory) { DisableCollapsingOnChildrenPendingChange = true });
                }
            }

            // Removed products
            foreach (ProductViewModel productTreeItem in base.Children.ToList())
            {
                var product = products.FirstOrDefault(p => p.Id == productTreeItem.Id);
                if (product == null)
                {
                    base.Children.Remove(productTreeItem);
                }
            }
        }

        public TreeViewItemViewModel NewProduct()
        {
            var newprod = new ProductViewModel(new Product(), this, _actorCategory) { DisableCollapsingOnChildrenPendingChange = true };
            if (base.HasDummyChild)
                IsExpanded = true;
            base.Children.Add(newprod);           
            string prefixName = "New Product ";
            int productIndex = 1;
            while (ExistsProductName(string.Concat(prefixName, productIndex)))
            {
                productIndex++;
            }
            newprod.Name = prefixName + productIndex;
            return newprod;
        }
        public bool ExistsProductName(string productName)
        {           
            return base.Children.Any(x => (x as ProductViewModel).Name != null && (x as ProductViewModel).Name.Trim().Equals(productName));            
        }

        public delegate void OnSelectedTreeViewChangedEvent();

        public event OnSelectedTreeViewChangedEvent OnSelectTreeViewChanged;

        private TreeViewItemViewModel _selectedTreeViewItem;

        public TreeViewItemViewModel SelectedTreeViewItem
        {
            get { return _selectedTreeViewItem; }
            set
            {
                bool hasChangedBefore = HasChanged;
                _selectedTreeViewItem = value;
                OnPropertyChanged();
                HasChanged = hasChangedBefore;
                OnSelectTreeViewChanged?.Invoke();
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

        public override bool RemoveNode(TreeViewItemViewModel node)
        {
            if (node == null)
                return false;
            else if (base.Children != null)
            {
                foreach (var childNode in base.Children)
                {
                    if (childNode == node)
                    {
                        base.Children.Remove(node);
                        return true;
                    }
                    if (childNode.RemoveNode(node))
                        return true;
                }
            }
            return false;
        }

        public override void Save()
        {
            // on parcours l'arbre et on sauve les ajout et modification en database
            if (base.Children != null)
            {
                foreach (var childNode in base.Children)
                {
                    childNode.Save();
                }
            }
            HasChanged = false;
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

        public bool CanClose()
        {
            bool canClose = true;
            foreach (ProductViewModel productVM in base.Children)
            {
                canClose &= productVM.CanClose();
                if (!canClose)
                    break;
            }
            if (!canClose)
            {
                var message = "Some items have not been validated";
                var dialogViewModel = new CanQuitJobDialogViewModel(message);
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<CanQuitJobDialogView>(dialogViewModel);               
                switch (dialogViewModel.MessageResult)
                {
                    case MessageBoxResult.Yes:
                        Save();
                        return true;
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Cancel:
                        return false;         
                    default:
                        return false;                     
                }               
            }
            return true;
        }
        public int TotalNumberOfSteps
        {
            get
            {
                var steps = Children.SelectMany(parent => parent.Children);
                return steps.Count();
            }
        }
        #region Commands

        private AutoRelayCommand _selectedTreeViewItemChangedCommand = null;

        public AutoRelayCommand SelectedTreeViewItemChangedCommand
        {
            get { return _selectedTreeViewItemChangedCommand ?? (_selectedTreeViewItemChangedCommand = new AutoRelayCommand(() => { SelectedTreeViewItemChanged(); })); }
        }
        private AutoRelayCommand _addProduct;

        public AutoRelayCommand AddProduct
        {
            get
            {
                return _addProduct ?? (_addProduct = new AutoRelayCommand(
                () =>
                {
                    SelectedTreeViewItem = NewProduct();
                    SelectedTreeViewItem.IsSelected = true;
                },
              () => { return !HasChanged; }));
            }
        }
        #endregion Commands
    }
}
