using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public class ProductViewModel : TreeViewItemViewModel
    {
        private Product _product;

        public List<WaferCategory> WaferCategories { get; private set; }              

        private WaferCategory _selectedCategory;

        public WaferCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _product.Name; set { if (_product.Name != value) { _product.Name = new PathString(value).RemoveInvalidFilePathCharacters("_",false); OnPropertyChanged(); } }
        }

        public int Id => _product.Id;

        private ServiceInvoker<IToolService> _toolService;

        public string Comment
        {
            get => _product.Comment; set { if (_product.Comment != value) { _product.Comment = value; OnPropertyChanged(); } }
        }

        private string _validationErrorMessage = string.Empty;
        public string ValidationErrorMessage
        {
            get => _validationErrorMessage; set { if (_validationErrorMessage != value) { _validationErrorMessage = value; OnPropertyChanged(); } }
        }


        public ProductViewModel(Product product, TreeViewItemViewModel parent, ActorCategory actorCategory = ActorCategory.Unknown) : base(parent, true)
        {
            ActorCategory = actorCategory;
            _product = product;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Comment));
            IsExpanded = true;
            _toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
            WaferCategories = _toolService.Invoke(x => x.GetWaferCategories());            
            SelectedCategory = WaferCategories.SingleOrDefault(x => x.Id == product.WaferCategoryId);
            if (SelectedCategory == null)
                SelectedCategory = WaferCategories.LastOrDefault();
            HasChanged = false;
        }

        public void Update(Product product)
        {
            var oldProduct = _product;
            _product = product;
            if (oldProduct.Name != _product.Name)
                OnPropertyChanged(nameof(Name));
            if (oldProduct.Comment != _product.Comment)
                OnPropertyChanged(nameof(Comment));
            SelectedCategory = WaferCategories.SingleOrDefault(x => x.Id == product.WaferCategoryId);
            HasChanged = false;

            if (this.HasDummyChild)
            {
                return;
            }

            // New and modified steps
            foreach (var step in product.Steps)
            {
                var stepTreeItem = base.Children.FirstOrDefault(s => ((s as StepViewModel).Id == step.Id));
                if (stepTreeItem != null)
                {
                    var stepTreeItemChanged = base.Children.FirstOrDefault(p => ((p as StepViewModel).Id == step.Id) && p.HasChanged);
                    if (stepTreeItemChanged != null)
                    {
                        // the step already exists
                        (stepTreeItem as StepViewModel).Update(step);
                    }
                }
                else
                {
                    base.Children.Add(new StepViewModel(step, this) { DisableCollapsingOnChildrenPendingChange = true });
                }
            }

            // Removed steps
            foreach (StepViewModel stepTreeItem in base.Children.ToList())
            {
                var step = product.Steps.FirstOrDefault(s => s.Id == stepTreeItem.Id);
                if (step == null)
                {
                    base.Children.Remove(stepTreeItem);
                }
            }
        }

        protected override void LoadChildren()
        {
            if (_product.Steps != null)
            {
                foreach (var step in _product.Steps)
                    base.Children.Add(new StepViewModel(step, this) { DisableCollapsingOnChildrenPendingChange = true });
            }
        }

        public override string ToString()
        {
            return _product.Name;
        }

        public TreeViewItemViewModel NewStep()
        {
            var newstep = new StepViewModel(new Step() { ProductId = _product.Id }, this) { DisableCollapsingOnChildrenPendingChange = true };
            if (base.HasDummyChild)
                IsExpanded = true;
            base.Children.Add(newstep);
            string prefixName = "New Step ";
            int stepIndex = 1;
            while (ExistsStepName(string.Concat(prefixName, stepIndex)))
            {
                stepIndex++;
            }
            newstep.Name = prefixName + stepIndex;
            newstep.IsExpanded = true;
            return newstep;
        }
        public bool ExistsStepName(string stepName)
        {
            return base.Children.Any(x => (x as StepViewModel).Name != null && (x as StepViewModel).Name.Trim().Equals(stepName));           
        }
        private void UpdateValidationErrorMessage()
        {
            if (!AreNamesUnique())
            {
                ValidationErrorMessage = "Product name must be unique";
            }           
            else
            {
                ValidationErrorMessage = string.Empty;
            }                
        }
        private bool AreNamesUnique()
        {
            return Parent.Children.Select(m => (m as ProductViewModel).Name.Trim()).Distinct().Count() == Parent.Children.Count;
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
            Name = Name.Trim();
            if (HasChanged)
            {
                _product.WaferCategory = SelectedCategory;
                _product.WaferCategoryId = SelectedCategory.Id;
                _product.Id = _toolService.Invoke(x => x.SetProduct(_product, ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id));
            }

            if (base.Children != null)
            {
                var stepChanged = base.Children.Where(x => x.HasChanged || x.Children.Any(o => o.HasChanged));                
                foreach (var childNode in stepChanged)
                {
                    childNode.Save();
                }
            }

            HasChanged = false;
        }

        public override void Archive()
        {
            if (_product.Id != 0)
                _toolService.Invoke(x => x.ArchiveAProduct(_product.Id, ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id));
        }

        public bool CanClose()
        {
            //It is not necessary to repeat the list of steps if the product has changed.
            if (HasChanged)
            {
                return false;
            }                
            bool canClose = true;
            foreach (StepViewModel stepVM in base.Children)
            {
                canClose &= stepVM.CanClose();
                //No need to go through the whole list if I find at least one changed step.
                if (!canClose)
                {
                    break;
                }                    
            }           
            return canClose;
        }

        private AutoRelayCommand _validate;
        public AutoRelayCommand Validate
        {
            get
            {
                return _validate ?? (_validate = new AutoRelayCommand(
                    () =>
                    {
                        Save();                        
                    },
                    () => { UpdateValidationErrorMessage(); return HasChanged & (ValidationErrorMessage == string.Empty); }
                ));
            }
        }
        private AutoRelayCommand _cancel;
        public AutoRelayCommand Cancel
        {
            get
            {
                return _cancel ?? (_cancel = new AutoRelayCommand(
                    () =>
                    {
                        List<Product> products = _toolService.Invoke(r => r.GetProductAndSteps(false));
                        var product = products.FirstOrDefault(x => x.Id == _product.Id);
                        if (product != null)
                        {
                            Update(product);
                        }
                        else
                        {
                            Parent.Children.Remove((TreeViewItemViewModel)this);
                           
                        }
                    },
                    () => { return HasChanged; }
                ));
            }
        }
        private AutoRelayCommand _addStep;

        public AutoRelayCommand AddStep
        {
            get
            {
                return _addStep ?? (_addStep = new AutoRelayCommand(
                () =>
                {                   
                    NewStep();                    
                },
              () => { return !HasChanged; }));
            }
        }
        public override bool CanUserRemove()
        {
            foreach (var stepVM in base.Children)
            {
                if (!stepVM.CanUserRemove())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
