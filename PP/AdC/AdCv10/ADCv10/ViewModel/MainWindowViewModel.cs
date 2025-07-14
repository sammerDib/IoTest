using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel;

namespace ADC.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class MainWindowViewModel : ObservableRecipient, IBusy
    {
        private ObservableRecipient _mainViewVM;

        private UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.RecipeViewModel _currentRecipe;

        private List<Product> _products;
        private string _userName = "";

        public string UserName
        { get => _userName; set { _userName = value; OnPropertyChanged(); } }

        private string _subTitle = "";

        public string SubTitle
        { get => _subTitle; set { _subTitle = value; OnPropertyChanged(); } }

        private IMessenger _messenger;

        // on le met la et pas dans App, car lorsque l'on est en Embedded, l'App est celui de l'hote
        private bool _isEmbedded = false;

        public bool IsEmbedded
        { get => _isEmbedded; set { _isEmbedded = value; OnPropertyChanged(); } }


        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        public MainWindowViewModel(IMessenger messenger)
        {
            _messenger = messenger;
            Init();
        }

        private void Init()
        {
            var _dbToolServiceProxy = ClassLocator.Default.GetInstance<DbToolServiceProxy>();

            _products = _dbToolServiceProxy.GetProductAndSteps();

            _rootTreeViewModelCollection.Clear();
            _rootTreeViewModel = new RootTreeViewModel(this,ActorCategory.PostProcessing);

            _rootTreeViewModel.OnSelectTreeViewChanged += OnSelectTreeViewChanged;
            RootTreeViewModelCollection.Add(_rootTreeViewModel);
            _rootTreeViewModel.Init(_products);
            OnPropertyChanged(nameof(RootTreeViewVM));
        }

        private void OnSelectTreeViewChanged()
        {
            var treeitem = RootTreeViewVM.SelectedTreeViewItem;

            if ((_currentRecipe != null) && !(MainViewViewModel as RecipeViewModel).CanCloseRecipe())
            {
                return;
            }

            if (_currentRecipe != null ) 
                _currentRecipe.HasChanged = false;

            if (_currentRecipe != null && (_currentRecipe.Key == Guid.Empty))
            {
                var previousRecipe = _currentRecipe;
                _currentRecipe = null;
                DeleteTreeViewItem(previousRecipe);
                
            }
            if (treeitem is UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.RecipeViewModel recipeViewModel)
            {
                if (recipeViewModel.Key != Guid.Empty)
                {
                    (MainViewViewModel as RecipeViewModel).LoadRecipeFromBase(recipeViewModel.Key);
                    recipeViewModel.RecipeInfo = (MainViewViewModel as RecipeViewModel).Recipe;
                }
                _currentRecipe = recipeViewModel;
            }
            else
            {
                (MainViewViewModel as RecipeViewModel).Recipe = null;
                _currentRecipe = null;
            }
        }

        private bool CanCloseCurrentRecipe()
        {
            var treeitem = RootTreeViewVM.SelectedTreeViewItem;
            if (treeitem is UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.RecipeViewModel recipeViewModel)
            {
                (MainViewViewModel as RecipeViewModel).CanCloseRecipe();
                // We remove from the tree any new unsaved recipe
                if ((_currentRecipe != null) && (recipeViewModel.Key == Guid.Empty))
                {
                    var previousRecipe = _currentRecipe;
                    _currentRecipe = null;
                    DeleteTreeViewItem(previousRecipe);
                }
            }
            return true;
        }

        private ObservableCollection<RootTreeViewModel> _rootTreeViewModelCollection = new ObservableCollection<RootTreeViewModel>();

        public ObservableCollection<RootTreeViewModel> RootTreeViewModelCollection
        {
            get => _rootTreeViewModelCollection; set { if (_rootTreeViewModelCollection != value) { _rootTreeViewModelCollection = value; OnPropertyChanged(); OnPropertyChanged(nameof(RootTreeViewVM)); } }
        }

        private RootTreeViewModel _rootTreeViewModel;

        public RootTreeViewModel RootTreeViewVM
        {
            get
            {
                if (_rootTreeViewModelCollection == null || _rootTreeViewModelCollection.Count == 0)
                    return null;
                return _rootTreeViewModelCollection[0];
            }
        }

        /// <summary>
        /// View model et view en fonction du mode de démarrage de l'application : Operator ou Recipe
        /// </summary>
        public ObservableRecipient MainViewViewModel
        {
            get
            {
                if (_mainViewVM == null)
                {
                    _mainViewVM = new RecipeViewModel(_messenger);
                    _mainViewVM.PropertyChanged += _mainViewVM_PropertyChanged;
                }

                return _mainViewVM;
            }
        }

        private void _mainViewVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (RootTreeViewVM.SelectedTreeViewItem is UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.RecipeViewModel recipeViewModel)
            {
                recipeViewModel.NotifiyNameUpdate();
            }
        }

        private AutoRelayCommand<TreeViewItemViewModel> _addcommand;

        public AutoRelayCommand<TreeViewItemViewModel> AddCommand
        {
            get
            {
                return _addcommand ?? (_addcommand = new AutoRelayCommand<TreeViewItemViewModel>(
              (treeViewItemViewModel) =>
              {
                  if (treeViewItemViewModel == null)
                      ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("No Node selected !", "Dataflow Tree view Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                  if (treeViewItemViewModel is RootTreeViewModel)
                  {
                      // create product
                      treeViewItemViewModel.IsSelected = false;
                      RootTreeViewVM.SelectedTreeViewItem = ((RootTreeViewModel)treeViewItemViewModel).NewProduct();
                      RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;
                  }
                  else if (treeViewItemViewModel is ProductViewModel)
                  {
                      if (((ProductViewModel)treeViewItemViewModel).Id == 0)
                          ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Product must be saved before adding step", "Step Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                      else
                      {
                          // create step
                          treeViewItemViewModel.IsSelected = false;
                          RootTreeViewVM.SelectedTreeViewItem = ((ProductViewModel)treeViewItemViewModel).NewStep();
                          RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;
                      }
                  }
                  else if (treeViewItemViewModel is StepViewModel stepViewModel)
                  {
                      if (((StepViewModel)treeViewItemViewModel).Id == 0)
                          ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Step must be saved before adding recipe", "Step Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                      else
                      {
                          if (!CanCloseCurrentRecipe())
                              return;
                          _currentRecipe = null;
                          // create recipe
                          treeViewItemViewModel.IsSelected = true;// false;

                          (MainViewViewModel as RecipeViewModel).NewRecipe();
                          var recipeInfo = (MainViewViewModel as RecipeViewModel).Recipe;
                          var recipeName = $"ADC {DateTime.Now.ToString("yyyyMMdd HHmmss")}";
                          recipeInfo.Name = recipeName;
                          recipeInfo.StepId = stepViewModel.Id;

                          var userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
                          recipeInfo.UserId = userId;

                          recipeInfo.ActorType = ActorType.ADC;

                          recipeInfo.Created = DateTime.Now;

                          var moduleConfiguration = ClassLocator.Default.GetInstance<ModuleConfiguration>();
                          var dbToolServiceProxy = ClassLocator.Default.GetInstance<DbToolServiceProxy>();

                          var chamber = dbToolServiceProxy.GetChamber(moduleConfiguration.ToolKey, moduleConfiguration.ChamberKey);
                          recipeInfo.CreatorChamberId = chamber.Id;

                          // TODO should use that instead
                          //_recipeSupervisor.CreateRecipe(recipeName, stepViewModel.Id, userId);

                          var recipeVM = new UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.RecipeViewModel(((StepViewModel)treeViewItemViewModel), recipeInfo);
                          recipeVM.IsSelected = true;
                          ((StepViewModel)treeViewItemViewModel).Children.Add(recipeVM);
                          if(RootTreeViewVM.SelectedTreeViewItem!= null)
                              RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;

                          (MainViewViewModel as RecipeViewModel).NotifyUpdateRecipe();
                      }
                  }
              },
              (treeViewItemViewModel) => { return true; }));
            }
        }

        private AutoRelayCommand<TreeViewItemViewModel> _removecommand;

        public AutoRelayCommand<TreeViewItemViewModel> RemoveCommand
        {
            get
            {
                return _removecommand ?? (_removecommand = new AutoRelayCommand<TreeViewItemViewModel>(
              (treeViewItemViewModel) =>
              {
                  if (treeViewItemViewModel == null)
                      ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("No Node selected !", "Dataflow Tree view Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                  else
                  {
                      bool canRemove = false;
                      try
                      {
                          canRemove = treeViewItemViewModel.CanUserRemove();
                          if (canRemove)
                          {
                              DeleteTreeViewItem(treeViewItemViewModel);
                          }
                          else
                          {
                              ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("You cannot delete this item because you are not the owner"
                              , "Remove warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.No);
                          }
                      }
                      catch (Exception ex)
                      {
                          var dbgmsg = ex.Message;
                          canRemove = false;
                      }
                  }
              },
              (treeViewItemViewModel) => { return true; }));
            }
        }

        private void DeleteTreeViewItem(TreeViewItemViewModel treeitem)
        {
            var msgresult = ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Do you really want to definitively remove this item ?"
                           + Environment.NewLine + Environment.NewLine
                           + "All configurations, items and history linked will be lost", "Remove item Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (msgresult == MessageBoxResult.Yes)
            {
                treeitem.Archive();
                RootTreeViewVM.RemoveNode(treeitem);
            }
        }

        private void DeleteTreeViewItem(UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.RecipeViewModel treeitem)
        {
            treeitem.Archive();
            RootTreeViewVM.RemoveNode(treeitem);
        }

        private AutoRelayCommand<TreeViewItemViewModel> _importCommand;

        public AutoRelayCommand<TreeViewItemViewModel> ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new AutoRelayCommand<TreeViewItemViewModel>(
              (treeViewItemViewModel) =>
              {
                  if (treeViewItemViewModel == null)
                      return;
                  if (treeViewItemViewModel is StepViewModel stepViewModel)
                  {
                      if (!CanCloseCurrentRecipe())
                          return;

                      _currentRecipe=null;
                      treeViewItemViewModel.IsSelected = true;
                      if ((MainViewViewModel as RecipeViewModel).LoadRecipe())
                      {
                          var recipeInfo = (MainViewViewModel as RecipeViewModel).Recipe;
                          var recipeName = $"{Path.GetFileNameWithoutExtension((MainViewViewModel as RecipeViewModel).CurrentFileName)} {DateTime.Now.ToString("yyyyMMdd HHmmss")}";
                          recipeInfo.Name = recipeName;

                          recipeInfo.StepId = stepViewModel.Id;

                          var userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
                          recipeInfo.UserId = userId;
                          recipeInfo.ActorType = ActorType.ADC;
                          recipeInfo.Created = DateTime.Now;

                          var moduleConfiguration = ClassLocator.Default.GetInstance<ModuleConfiguration>();
                          var dbToolServiceProxy = ClassLocator.Default.GetInstance<DbToolServiceProxy>();

                          var chamber = dbToolServiceProxy.GetChamber(moduleConfiguration.ToolKey, moduleConfiguration.ChamberKey);
                          recipeInfo.CreatorChamberId = chamber.Id;

                          var recipeVM = new UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.RecipeViewModel(((StepViewModel)treeViewItemViewModel), recipeInfo);
                          recipeVM.IsSelected = true;
                          ((StepViewModel)treeViewItemViewModel).Children.Add(recipeVM);

                          if (RootTreeViewVM.SelectedTreeViewItem != null)
                              RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;

                          (MainViewViewModel as RecipeViewModel).NotifyUpdateRecipe();
                      }
                  }
                  //var item = ((StepViewModel)treeViewItemViewModel).ImportRecipe();
                  //if (item != null)
                  //{
                  //    RootTreeViewVM.SelectedTreeViewItem = item;
                  //    RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;
                  //}
              },
              (treeViewItemViewModel) => { return treeViewItemViewModel != null && treeViewItemViewModel is StepViewModel; }));
            }
        }

        public AutoRelayCommand ShowAboutCommand { get; } = new AutoRelayCommand(() => { new Header.About().ShowDialog(); });

        public AutoRelayCommand<System.ComponentModel.CancelEventArgs> ClosingApplicationCommand { get; }
            = new AutoRelayCommand<System.ComponentModel.CancelEventArgs>((arg) =>
            {
                arg.Cancel = true;

                // on transfert le traitement et la decision au service d'arret (ShutdownService);
                Services.Services.Instance.ShutdownService.ShutdownApp();
            });

        private AutoRelayCommand _loadedApplicationCommand = null;

        public AutoRelayCommand LoadedApplicationCommand
        {
            get
            {
                return _loadedApplicationCommand != null ? _loadedApplicationCommand : _loadedApplicationCommand

            = new AutoRelayCommand(() =>
            {
                /*
                string recipe = App.CommandLineArgs["path"];
                if (recipe != null && MainViewViewModel is RecipeViewModel)
                    ((RecipeViewModel)MainViewViewModel).LoadRecipe(recipe);
            */
            });
            }
        }
    }
}
