using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UC;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.Shared.UI.Recipes.Management
{
    public class RecipesManagementViewModel : ObservableObject, IBusy, IMenuContentViewModel
    {
        public bool CanClose()
        {
            bool canClose;
            if (CurrentEditorUC != null)
            {
                canClose = CurrentEditorUC.CanClose();
                if (canClose)
                    CurrentEditorUC = null;
                return canClose;
            }
            if (RootTreeViewVM != null)
            {
                return RootTreeViewVM.CanClose();
            }
            return true;
        }

        public bool IsEnabled
        {
            get
            {
                var _globalStatusSupervisor = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();
                return (_globalStatusSupervisor != null && (_globalStatusSupervisor.CurrentState == PMGlobalStates.Free || _globalStatusSupervisor.CurrentState == PMGlobalStates.Busy));
            }
        }

        private ILogger _logger;
        private IMessenger _messenger;
        private ServiceInvoker<IToolService> _toolService;
        private List<Product> _products;
        private List<TreeViewItemViewModel> _removedItems = new List<TreeViewItemViewModel>();
        private RootTreeViewModel _rootTreeViewModel;
        private IDialogOwnerService _dialogOwnerService;

        private ApplicationMode _mode;

        public ApplicationMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    Refresh();
                    OnPropertyChanged();
                    EditRecipeCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private ObservableCollection<RootTreeViewModel> _rootTreeViewModelCollection = new ObservableCollection<RootTreeViewModel>();

        public ObservableCollection<RootTreeViewModel> RootTreeViewModelCollection
        {
            get => _rootTreeViewModelCollection; set { if (_rootTreeViewModelCollection != value) { _rootTreeViewModelCollection = value; OnPropertyChanged(); OnPropertyChanged(nameof(RootTreeViewVM)); } }
        }

        public RootTreeViewModel RootTreeViewVM
        {
            get
            {
                if (_rootTreeViewModelCollection == null || _rootTreeViewModelCollection.Count == 0)
                    return null;
                return _rootTreeViewModelCollection[0];
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        public void Refresh()
        {
            _toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), _messenger, ClientConfiguration.GetDataAccessAddress());
            _products = _toolService.Invoke(r => r.GetProductAndSteps(false));
            _rootTreeViewModel.Update(_products);
            _rootTreeViewModel.GetAllNodes().ForEach(x => x.HasChanged = false);
            if (_rootTreeViewModel.SelectedTreeViewItem is null)
            {
                _rootTreeViewModel.SelectedTreeViewItem = _rootTreeViewModel;
                _rootTreeViewModel.SelectedTreeViewItem.IsSelected = true;
            }
            OnPropertyChanged(nameof(RootTreeViewVM));
        }

        public RecipesManagementViewModel(ILogger logger, IMessenger messenger, IDialogOwnerService dialogOwnerService)
        {
            _logger = logger;
            _messenger = messenger;
            _dialogOwnerService = dialogOwnerService;
            _toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), messenger, ClientConfiguration.GetDataAccessAddress());
            _rootTreeViewModel = new RootTreeViewModel(this,UnitySC.Shared.Data.Enum.ActorCategory.ProcessModule);
            _rootTreeViewModel.OnSelectTreeViewChanged += OnSelectTreeViewChanged;
            RootTreeViewModelCollection.Add(_rootTreeViewModel);

            var _globalStatusSupervisor = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();
            _globalStatusSupervisor.OnStateChanged += _globalStatusSupervisor_OnStateChanged;

#if HMI_DEV
            Task.Run(async () =>
            {
                // This code is used to open directly a recipe in edition mode at the startup
                var globalStatusSupervisor = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();
                await globalStatusSupervisor.AsyncWaitServerIsReady();
                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var actorType = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>().Configuration.Actor;

                    if (actorType == ActorType.ANALYSE)
                    {
                        var recipeEditor = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeEditor(actorType);
                        if (recipeEditor != null)
                        {
                            recipeEditor.Init(true);
                            recipeEditor.LoadRecipe(new Guid("32b6c8b3-ba32-4e3b-aa49-5386cae03809"));
                            recipeEditor.ExitEditor -= RecipeEditor_ExitEditor;
                            recipeEditor.ExitEditor += RecipeEditor_ExitEditor;
                        }
                        CurrentEditorUC = recipeEditor;
                    }
                }
                ));
            });
#endif
        }

        private void _globalStatusSupervisor_OnStateChanged(PMGlobalStates state)
        {
            OnPropertyChanged(nameof(IsEnabled));
        }


        private void OnSelectTreeViewChanged()
        {
            var user = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser;
            if ((user == null) || (!user.Rights.Contains(UserRights.RecipeEdition)))
            {
                EnableAdd = false;
                EnableRemove = false;
                EnableSave = false;
            }
            else
            {
                var treeitem = RootTreeViewVM.SelectedTreeViewItem;

                if (treeitem == null)
                {
                    EnableAdd = false;
                    EnableRemove = false;
                }
                else if (treeitem is RootTreeViewModel)
                {
                    EnableAdd = true; //add product
                    EnableRemove = false;
                }
                else if (treeitem is ProductViewModel)
                {
                    EnableAdd = true; // add step
                    EnableRemove = true; // remove product
                }
                else if (treeitem is StepViewModel)
                {
                    EnableAdd = true; // add recipe
                    EnableRemove = true; // remove step
                }
                else if (treeitem is RecipeViewModel)
                {
                    EnableAdd = false;
                    EnableRemove = true; // remove recipe
                }

                EnableSave = true; //RootTreeViewModelCollection.Any(x => x.HasChildrenChanged); // how can I notify parents from any childs change
            }

            AddCommand.NotifyCanExecuteChanged();
            RemoveCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            ImportCommand.NotifyCanExecuteChanged();
        }

       private AsyncRelayCommand<TreeViewItemViewModel> _importCommand;

        public AsyncRelayCommand<TreeViewItemViewModel> ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new AsyncRelayCommand<TreeViewItemViewModel>(
              async (treeViewItemViewModel) =>
              {
                  if (treeViewItemViewModel == null)
                      return;

                  // Import recipe
                  treeViewItemViewModel.IsSelected = false;
                  var item = await ((StepViewModel)treeViewItemViewModel).ImportRecipeAsync();
                  if (item != null)
                  {
                      RootTreeViewVM.SelectedTreeViewItem = item;
                      RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;
                  }
              },
              (treeViewItemViewModel) => { return treeViewItemViewModel != null && treeViewItemViewModel is StepViewModel; }));
            }
        }

        private bool _enableAdd;

        public bool EnableAdd
        {
            get => _enableAdd; set { if (_enableAdd != value) { _enableAdd = value; OnPropertyChanged(); } }
        }

        private bool _enableRemove;

        public bool EnableRemove
        {
            get => _enableRemove; set { if (_enableRemove != value) { _enableRemove = value; OnPropertyChanged(); } }
        }

        private bool _enableSave;

        public bool EnableSave
        {
            get => _enableSave; set { if (_enableSave != value) { _enableSave = value; OnPropertyChanged(); } }
        }

        private bool _showImportCommand = false;

        public bool ShowImportCommand
        {
            get => _showImportCommand; set { if (_showImportCommand != value) { _showImportCommand = value; OnPropertyChanged(); } }
        }

        #region Commands

        private AutoRelayCommand<RecipeViewModel> _shareUnShareRecipeCommand;

        public AutoRelayCommand<RecipeViewModel> ShareUnShareRecipeCommand
        {
            get
            {
                return _shareUnShareRecipeCommand ?? (_shareUnShareRecipeCommand = new AutoRelayCommand<RecipeViewModel>(
              (recipe) =>
                {
                    try
                    {
                        var userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
                        ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>().Invoke(x => x.ChangeRecipeSharedState(recipe.Key, userId, !recipe.IsShared));
                        recipe.IsShared = !recipe.IsShared;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Share unshare error", ex);
                        _dialogOwnerService.ShowMessageBox($"Share unshare error : {ex.Message}");
                    }
                },
              (recipe) => { return true; }));
            }
        }

        private IRecipeEditorUc _currentEditorUC;

        public IRecipeEditorUc CurrentEditorUC
        {
            get => _currentEditorUC; set { if (_currentEditorUC != value) { _currentEditorUC = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand<RecipeViewModel> _editRecipeCommand;

        public AutoRelayCommand<RecipeViewModel> EditRecipeCommand
        {
            get
            {
                return _editRecipeCommand ?? (_editRecipeCommand = new AutoRelayCommand<RecipeViewModel>(
              (recipe) =>
              {
                  var recipeEditor = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeEditor(recipe.ActorType);

                  if (recipeEditor != null)
                  {
                      recipeEditor.Init(true);
                      try
                      {
                          recipeEditor.LoadRecipe(recipe.Key);
                      }
                      catch (Exception ex)
                      {
                          ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Load recipe error");
                          _dialogOwnerService.ShowMessageBox($"Load recipe error : {ex.Message}");
                          return;
                      }
                      recipeEditor.ExitEditor -= RecipeEditor_ExitEditor;
                      recipeEditor.ExitEditor += RecipeEditor_ExitEditor;
                  }
                  CurrentEditorUC = recipeEditor;
              },
              (recipe) =>
              {
                  var user = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser;
                  return Mode != ApplicationMode.Production && (user != null && user.Rights.Contains(UserRights.RecipeEdition));
              }));
            }
        }

        private AsyncRelayCommand<RecipeViewModel> _exportRecipeCommand;

        public AsyncRelayCommand<RecipeViewModel> ExportRecipeCommand
        {
            get
            {
                return _exportRecipeCommand ?? (_exportRecipeCommand = new AsyncRelayCommand<RecipeViewModel>(
              async (recipe) =>
              {
                  var recipeEditor = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeEditor(recipe.ActorType);

                  if (recipeEditor != null)
                  {
                      recipeEditor.Init(true);
                      try
                      {
                          IsBusy = true;

                          await recipeEditor.ExportRecipeAsync(recipe.Key);

                          IsBusy = false;
                      }
                      catch (Exception ex)
                      {
                          ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Export recipe error");
                          _dialogOwnerService.ShowMessageBox($"Export recipe error : {ex.Message}");
                          IsBusy = false;
                          return;
                      }
                  }
              },
              (recipe) =>
              {
                  var user = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser;
                  return user != null && user.Rights.Contains(UserRights.RecipeEdition);
              }));
            }
        }

        private void RecipeEditor_ExitEditor(object sender, EventArgs e)
        {
            CurrentEditorUC = null;

            var currentSelected = RootTreeViewVM.SelectedTreeViewItem;

            if (currentSelected == null)
                return;
            Refresh();
            currentSelected.IsSelected = true;
            RootTreeViewVM.SelectedTreeViewItem = currentSelected;

            // We refresh the recipe summary
            if (RootTreeViewVM.SelectedTreeViewItem is RecipeViewModel selectedRecipeVM)
            {
                if (selectedRecipeVM.Parent is StepViewModel selectedStepVM)
                {
                    selectedStepVM.UpdateRecipe(selectedRecipeVM.Key);
                }
                selectedRecipeVM.RefreshRecipeSummary();
                selectedRecipeVM.IsSelected = true;
            }
        }

        public void RefreshSelectedRecipe()
        {
            // We refresh the recipe 
            if (RootTreeViewVM.SelectedTreeViewItem is RecipeViewModel selectedRecipeVM)
            {
                if (selectedRecipeVM.Parent is StepViewModel selectedStepVM)
                {
                    selectedStepVM.UpdateRecipe(selectedRecipeVM.Key);
                }
                selectedRecipeVM.RefreshRecipeSummary();
                
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
                      _dialogOwnerService.ShowMessageBox("No Node selected !", "Dataflow Tree view Warning",
                          MessageBoxButton.OK, MessageBoxImage.Warning);

                  if (treeViewItemViewModel is RootTreeViewModel)
                  {
                      try
                      {
                          // create product
                          treeViewItemViewModel.IsSelected = false;
                          RootTreeViewVM.SelectedTreeViewItem = ((RootTreeViewModel)treeViewItemViewModel).NewProduct();
                          RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;
                      }
                      catch
                      {
                          treeViewItemViewModel.IsSelected = true;
                          _dialogOwnerService.ShowMessageBox("Failed to create new product", "Product Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                      }
                  }
                  else if (treeViewItemViewModel is ProductViewModel)
                  {
                      if (((ProductViewModel)treeViewItemViewModel).Id == 0)
                          _dialogOwnerService.ShowMessageBox("Product must be saved before adding step", "Step Warning",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                      else
                      {
                          try
                          {
                              // create step
                              treeViewItemViewModel.IsSelected = false;
                              RootTreeViewVM.SelectedTreeViewItem = ((ProductViewModel)treeViewItemViewModel).NewStep();
                              RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;
                          }
                          catch
                          {
                              treeViewItemViewModel.IsSelected = true;
                              _dialogOwnerService.ShowMessageBox("Failed to create new step", "Step Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                          }
                      }
                  }
                  else if (treeViewItemViewModel is StepViewModel)
                  {
                      if (((StepViewModel)treeViewItemViewModel).Id == 0)
                          _dialogOwnerService.ShowMessageBox("Step must be saved before adding recipe", "Recipe Warning",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                      else
                      {
                          try
                          {
                              // create recipe
                              treeViewItemViewModel.IsSelected = false;
                              RootTreeViewVM.SelectedTreeViewItem = ((StepViewModel)treeViewItemViewModel).NewRecipe();
                              RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;
                          }
                          catch
                          {
                              treeViewItemViewModel.IsSelected = true;
                              _dialogOwnerService.ShowMessageBox("Failed to create new recipe", "Recipe Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                          }
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
                      _dialogOwnerService.ShowMessageBox("No Node selected !", "Dataflow Tree view Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                              _dialogOwnerService.ShowMessageBox("You cannot delete this item because you are not the owner"
                              , "Remove warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                          }
                      }
                      catch (Exception ex)
                      {
                          canRemove = false;
                          if (ex.Message.Contains("used in a dataflow"))
                          {
                              _dialogOwnerService.ShowMessageBox("You cannot delete this item because it is used in a dataflow recipe"
                                  , "Remove Error", MessageBoxButton.OK, MessageBoxImage.Error);
                          }
                          else
                          { 
                              _dialogOwnerService.ShowMessageBox($"Failed to delete the recipe {ex.Message}"
                                  , "Remove Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
                           }
                          _logger.Error("[DeleteRecipe] Unable to delete the recipe", ex);
                      }
                  }
              },
              (treeViewItemViewModel) => { return true; }));
            }
        }

        private AutoRelayCommand _savecommand;

        public AutoRelayCommand SaveCommand
        {
            get
            {
                return _savecommand ?? (_savecommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      // Remove items
                      foreach (var item in _removedItems)
                      {
                          item.Archive();
                      }
                      _removedItems.Clear();

                      // save Products / Steps / Dataflow...
                      RootTreeViewVM.Save();
                  }
                  catch (Exception ex)
                  {
                      _dialogOwnerService.ShowMessageBox($"Error during save : {ex.Message}");
                      Refresh();
                  }
              },
              () => { return EnableSave; }));
            }
        }

        private AutoRelayCommand _refreshCommand;

        public AutoRelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new AutoRelayCommand(
              () =>
              {
                  Refresh();
              },
              () => { return true; }));
            }
        }

        #endregion Commands

        private void DeleteTreeViewItem(TreeViewItemViewModel treeitem)
        {
            var msgresult = _dialogOwnerService.ShowMessageBox("Do you really want to definitively remove this item ?"
                                                               + Environment.NewLine + Environment.NewLine
                                                               + "All configurations, items and history linked will be lost", "Remove item Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (msgresult == MessageBoxResult.Yes)
            {
                treeitem.Archive();
                RootTreeViewVM.RemoveNode(treeitem);
            }
        }
    }
}
