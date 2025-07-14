using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Dataflow.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public class StepViewModel : TreeViewItemViewModel
    {
        private Step _step;
        private ServiceInvoker<IDbRecipeService> _dbRecipeService;
        private ServiceInvoker<IToolService> _toolService;
        private ModuleConfiguration _moduleConfiguration;
        private IDFClientConfiguration _dfClientConfiguration;
        private ModuleConfiguration _ppConfiguration;

        public LayersEditorViewModel LayersEditorVM { get; private set; }

        public string Name
        {
            get => _step.Name; set { if (_step.Name != value) { _step.Name = new PathString(value).RemoveInvalidFilePathCharacters("_", false); OnPropertyChanged(); } }
        }

        public string Comment
        {
            get => _step.Comment; set { if (_step.Comment != value) { _step.Comment = value; OnPropertyChanged(); } }
        }

        public int Id => _step.Id;

        private string _validationErrorMessage = string.Empty;

        public string ValidationErrorMessage
        {
            get => _validationErrorMessage; set { if (_validationErrorMessage != value) { _validationErrorMessage = value; OnPropertyChanged(); } }
        }

        public StepViewModel(Step step, TreeViewItemViewModel parent) : base(parent, true)
        {
            LayersEditorVM = new LayersEditorViewModel(() => { HasChanged = true; });
            _step = step;
            LayersEditorVM.Init(_step);
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Comment));
            HasChanged = false;
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
            _toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());

            switch (parent.ActorCategory)
            {
                case UnitySC.Shared.Data.Enum.ActorCategory.Unknown:
                    break;

                case UnitySC.Shared.Data.Enum.ActorCategory.ProcessModule:
                    _moduleConfiguration = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>().Configuration;
                    break;

                case UnitySC.Shared.Data.Enum.ActorCategory.PostProcessing:
                    _moduleConfiguration = ClassLocator.Default.GetInstance<ModuleConfiguration>();
                    break;

                case UnitySC.Shared.Data.Enum.ActorCategory.Manager:
                    _dfClientConfiguration = ClassLocator.Default.GetInstance<IDFClientConfiguration>();
                    break;

                default:
                    break;
            }
        }

        internal void Update(Step step)
        {
            var oldStep = _step;
            _step = step;
            if (oldStep.Name != _step.Name)
                OnPropertyChanged(nameof(Name));
            if (oldStep.Comment != _step.Comment)
                OnPropertyChanged(nameof(Comment));

            HasChanged = false;
            LayersEditorVM.Init(_step);

            if (HasDummyChild)
            {
                return;
            }

            UpdateChildren();
        }

        public override string ToString()
        {
            return _step.Name;
        }

        protected override void LoadChildren()
        {
            switch (Parent.ActorCategory)
            {
                case UnitySC.Shared.Data.Enum.ActorCategory.Manager:
                    foreach (var dataflowInfo in _dbRecipeService.Invoke(x => x.GetDataflowInfos(_step.Id, _dfClientConfiguration.ToolKey, false)))
                    {
                        var dataflow = new DataflowItemViewModel(dataflowInfo, this);
                        base.Children.Add(dataflow);
                    }
                    break;

                case UnitySC.Shared.Data.Enum.ActorCategory.Unknown:
                case UnitySC.Shared.Data.Enum.ActorCategory.ProcessModule:
                case UnitySC.Shared.Data.Enum.ActorCategory.PostProcessing:
                    var recipesList = _dbRecipeService.Invoke(x => x.GetRecipeList(_moduleConfiguration.Actor, _step.Id, _moduleConfiguration.ChamberKey, _moduleConfiguration.ToolKey, false, false));
                    IsSelected = true;
                    foreach (var recipeInfo in recipesList.OrderBy(r => r.Name))
                    {
                        base.Children.Add(new RecipeViewModel(this, recipeInfo));
                    }
                    break;

                default:
                    break;
            }
        }

        protected void UpdateChildren()
        {
            var recipesList = _dbRecipeService.Invoke(x => x.GetRecipeList(_moduleConfiguration.Actor, _step.Id, _moduleConfiguration.ChamberKey, _moduleConfiguration.ToolKey, false, false));

            foreach (var recipeInfo in recipesList.OrderBy(r => r.Name))
            {
                var recipeVM = base.Children.FirstOrDefault(r => (r as RecipeViewModel).Key == recipeInfo.Key);
                if (recipeVM == null)
                {
                    //var index=base.Children.IndexOf( base.Children.Where(r => String.Compare((r as RecipeViewModel).Name, recipeInfo.Name) < 0).Last());
                    base.Children.Add(new RecipeViewModel(this, recipeInfo));
                    //base.Children.Insert(index,new RecipeViewModel(this, recipeInfo));
                }
                else
                    // If the name change we should order it
                    (recipeVM as RecipeViewModel).Update(recipeInfo);
            }

            foreach (RecipeViewModel recipeVM in base.Children.ToList())
            {
                if (!recipesList.Any(r => r.Key == recipeVM.Key))
                {
                    base.Children.Remove(recipeVM);
                }
            }

            SortChildren();
        }

        private void SortChildren()
        {
            //TODO: Déplacer la proprieté name dans la class TreeViewItemViewModel
            //TODO: Déplacer la propriété Name dans la classe TreeViewItemViewModel pour simplifier le code ici
            // base.Children.Sort(r => (r as TreeViewItemViewModel).Name);
            base.Children.OrderBy(data =>
            {
                if (data is RecipeViewModel recipe)
                {
                    return recipe.Name;
                }
                else if (data is DataflowItemViewModel dataflowItem)
                {
                    return dataflowItem.Name;
                }
                else
                {
                    return string.Empty;
                }
            }).ToList();
        }

        public async Task<RecipeViewModel> ImportRecipeAsync()
        {
            if (!IsExpanded)
                IsExpanded = true;

            var busyVm = (Parent?.Parent as RootTreeViewModel)?.BusyVm;


            var recipeEditor = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeEditor(_moduleConfiguration.Actor);
            recipeEditor.Init(true);
            if (busyVm !=null) busyVm.IsBusy = true;
                     
            var userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
            RecipeInfo recipeInfo = null;
          
            recipeInfo = await recipeEditor.ImportRecipeAsync(_step.Id, userId);
   
            if (recipeInfo != null)
            {
                var recipeVM = new RecipeViewModel(this, recipeInfo);
                base.Children.Add(recipeVM);
                SortChildren();
                if (busyVm != null) busyVm.IsBusy = false;
  
                return recipeVM;
            }
            if (busyVm != null) busyVm.IsBusy = false;
            return null;
        }

        public TreeViewItemViewModel NewRecipe()
        {
            if (!IsExpanded)
                IsExpanded = true;

            switch (Parent.ActorCategory)
            {
                case UnitySC.Shared.Data.Enum.ActorCategory.Unknown:
                case UnitySC.Shared.Data.Enum.ActorCategory.ProcessModule:
                case UnitySC.Shared.Data.Enum.ActorCategory.PostProcessing:
                    var recipeEditor = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeEditor(_moduleConfiguration.Actor);
                    recipeEditor.Init(true);
                    var recipeName = $"{_moduleConfiguration.Actor} {DateTime.Now.ToString("yyyyMMdd HHmmss")}";
                    var userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
                    var recipeInfo = recipeEditor.CreateNewRecipe(recipeName, _step.Id, userId);
                    var recipeVM = new RecipeViewModel(this, recipeInfo);
                    recipeVM.IsSelected = true;
                    base.Children.Add(recipeVM);
                    SortChildren();
                    return recipeVM;

                case UnitySC.Shared.Data.Enum.ActorCategory.Manager:
                    var newDF = NewDataflow();
                    newDF.IsSelected = true;
                    SortChildren();
                    return newDF;

                default:
                    break;
            }

            return null;
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

        private void UpdateValidationErrorMessage()
        {
            if (!AreNamesUnique())
            {
                ValidationErrorMessage = "Step name must be unique";
            }
            else
            {
                ValidationErrorMessage = string.Empty;
            }
        }

        private bool AreNamesUnique()
        {
            return Parent.Children.Select(m => (m as StepViewModel).Name.Trim()).Distinct().Count() == Parent.Children.Count;
        }

        public override void Save()
        {
            Name = Name.Trim();
            if (HasChanged)
            {
                _step.Layers = LayersEditorVM.Layers.Select(x => x.GetLayerFromLayerVM()).ToList();
                _step.Id = _toolService.Invoke(x => x.SetStep(_step, ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id));
                // Get step from db to update layers Id
                _step = _toolService.Invoke(x => x.GetStep(_step.Id));
                LayersEditorVM.Init(_step);
            }

            if (base.Children != null)
            {
                foreach (var childNode in base.Children)
                {
                    childNode.Save();
                }
            }
            HasChanged = false;
        }

        public override void Archive()
        {
            if (_step.Id != 0)
                _toolService.Invoke(x => x.ArchiveAStep(_step.Id, ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id));
        }

        public bool CanClose()
        {
            //I can close a step if it hasn't been changed.
            return !HasChanged;
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
                    () => { UpdateValidationErrorMessage(); return HasChanged & (ValidationErrorMessage == string.Empty) && !LayersEditorVM.IsEditing; }
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
                        if (_step.Id != 0)
                        {
                            var stepFromBase = _toolService.Invoke(x => x.GetStep(_step.Id));
                            if (stepFromBase != null)
                            {
                                Update(stepFromBase);
                            }
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

        private AutoRelayCommand _addRecipe;

        public AutoRelayCommand AddRecipe
        {
            get
            {
                return _addRecipe ?? (_addRecipe = new AutoRelayCommand(
                () =>
                {
                    try
                    {
                        NewRecipe();
                    }
                    catch
                    {
                        ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Failed to create new recipe",
                        "Step Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                },
              () => { return !HasChanged; }));
            }
        }

        public override bool CanUserRemove()
        {
            if (HasDummyChild)
            {
                Children.Clear();
                LoadChildren();
            }
            foreach (var recipeVM in Children)
            {
                if (!recipeVM.CanUserRemove())
                {
                    return false;
                }
            }
            return true;
        }

        internal void UpdateRecipe(Guid key)
        {
            var recipesList = _dbRecipeService.Invoke(x => x.GetRecipeList(_moduleConfiguration.Actor, _step.Id, _moduleConfiguration.ChamberKey, _moduleConfiguration.ToolKey, false, false));
            var recipeInfo = recipesList.FirstOrDefault(x => x.Key == key);
            if (recipeInfo != null && recipeInfo.Key == key)
            {
                var recipeVM = base.Children.FirstOrDefault(r => (r as RecipeViewModel).Key == recipeInfo.Key);
                if (recipeVM != null)
                {
                    // If the name change we should order it
                    (recipeVM as RecipeViewModel).Update(recipeInfo);
                }
                SortChildren();
            }
        }

        public TreeViewItemViewModel NewDataflow()
        {
            var newDataflowName = $"DF {DateTime.Now.ToString("yyyyMMdd HHmmss")}";

            var newDF = new DataflowItemViewModel(new DataAccess.Dto.ModelDto.LocalDto.DataflowInfo() { Name = newDataflowName, StepId = _step.Id }, this);
            if (base.HasDummyChild)
                IsExpanded = true;
            base.Children.Add(newDF);
            newDF.HasChanged = true;
            return newDF;
        }
    }
}
