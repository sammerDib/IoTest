using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;
using MvvmValidation;
using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Core;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public delegate void HasChildChangedEvent(object sender);

    public class DataflowViewModel : DFViewModelBase, IBusy, IMenuContentViewModel
    {
        private ILogger _logger;
        private IMessenger _messenger;
        private ServiceInvoker<IToolService> _toolService;
        private List<Product> _products;
        private List<TreeViewItemViewModel> _removedItems = new List<TreeViewItemViewModel>();
        private RootTreeViewModel _rootTreeViewModel;
        private SharedSupervisors _sharedSupervisors;
        private IUserSupervisor _userSupervisor;
        private IDFClientConfiguration _dfClientConfiguration;
        protected ValidationHelper Validator { get; private set; }

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

        private NotifierVM _notifierVM;

        public NotifierVM NotifierVM
        {
            get
            {
                if (_notifierVM == null)
                    _notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                return _notifierVM;
            }
        }

        private bool _connexionError;

        public bool ConnexionError
        {
            get => _connexionError; set { if (_connexionError != value) { _connexionError = value; OnPropertyChanged(); } }
        }
      
        public DataflowViewModel(ILogger logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            _toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), messenger, ClientConfiguration.GetDataAccessAddress());
            Validator = new ValidationHelper();
            _rootTreeViewModel = new RootTreeViewModel(this, ActorCategory.Manager);
            _sharedSupervisors = ClassLocator.Default.GetInstance<SharedSupervisors>();
            _dfClientConfiguration = ClassLocator.Default.GetInstance<IDFClientConfiguration>();
            _logger.Information("ToolKey = " + _dfClientConfiguration.ToolKey + "ExternalUserControlsDir = " + _dfClientConfiguration.ExternalUserControlsDir);
            _logger.Information("DataAccessAddress" + ClientConfiguration.GetDataAccessAddress().ToString());            
        }

        public override async void Init()
        {
            IsBusy = true;
            await Task.Run(() => InitConnection());
            IsBusy = false;
            if (string.IsNullOrEmpty(CommunicationError))
            {
                _rootTreeViewModel.OnSelectTreeViewChanged -= OnSelectTreeViewChanged;
                _rootTreeViewModelCollection.Clear();
                _rootTreeViewModel.OnSelectTreeViewChanged += OnSelectTreeViewChanged;
                RootTreeViewModelCollection.Add(_rootTreeViewModel);
                _rootTreeViewModel.Init(_products);
                OnPropertyChanged(nameof(RootTreeViewVM));
            }
        }

        private void OnSelectTreeViewChanged()
        {
            var user = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser;
            if (!user.Rights.Contains(UserRights.RecipeEdition))
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
                    EnableAdd = true; // add dataflow
                    EnableRemove = true; // remove step
                }
                else if (treeitem is DataflowItemViewModel)
                {
                    EnableAdd = false;
                    EnableRemove = true; // remove dataflow
                }

                EnableSave = true; //RootTreeViewModelCollection.Any(x => x.HasChildrenChanged); // how can I notify parents from any childs change
            }

            AddCommand.NotifyCanExecuteChanged();
            RemoveCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
        }

        private void InitConnection()
        {
            try
            {
                // Data Access test
                _products = _toolService.Invoke(r => r.GetProductAndSteps(false));

                foreach (var actorType in _dfClientConfiguration.AvailableModules)
                {
                    try
                    {       
                        // TODO For the moment there is no ADC server
                        if (actorType!=ActorType.ADC)
                            _sharedSupervisors.GetGlobalStatusSupervisor(actorType).Init();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Init global status supervisor error for {actorType}");
                        CommunicationError = $"Connection to {actorType} server error";
                    }
                }

                // User right test
                try
                {
                    _userSupervisor = ClassLocator.Default.GetInstance<IUserSupervisor>();
                    _userSupervisor.UserChanged -= _userSupervisor_UserChanged;
                    _userSupervisor.UserChanged += _userSupervisor_UserChanged;
                    var user = _userSupervisor.CurrentUser;
                    if (user == null || !user.Rights.Contains(UserRights.RecipeReadonly))
                        CommunicationError = "Permission required to display dataflow";
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Get current user error");
                    CommunicationError = "User management error";
                }
            }
            catch (Exception ex)
            {
                CommunicationError = "Connection to DataAcces server error";
                _logger.Error(ex, "Connection with IToolService error");
                _toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), _messenger, ClientConfiguration.GetDataAccessAddress());
            }
        }

        private void _userSupervisor_UserChanged(UnifiedUser user)
        {
            CommunicationError = null;
            Init();
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

        private bool _showImportCommand = true;

        public bool ShowImportCommand
        {
            get => _showImportCommand; set { if (_showImportCommand != value) { _showImportCommand = value; OnPropertyChanged(); } }
        }
        

        #region Commands

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
                          ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Product must be save before adding step", "Step Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                      else
                      {
                          // create step
                          treeViewItemViewModel.IsSelected = false;
                          RootTreeViewVM.SelectedTreeViewItem = ((ProductViewModel)treeViewItemViewModel).NewStep();
                          RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;
                      }
                  }
                  else if (treeViewItemViewModel is StepViewModel)
                  {
                      if (((StepViewModel)treeViewItemViewModel).Id == 0)
                          ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Step must be save before adding dataflow", "Step Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                      else
                      {
                          // create dataflow
                          treeViewItemViewModel.IsSelected = false;
                          RootTreeViewVM.SelectedTreeViewItem = ((StepViewModel)treeViewItemViewModel).NewDataflow();
                          RootTreeViewVM.SelectedTreeViewItem.IsSelected = true;
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
                           canRemove = false;
                           _logger.Error("[CanCurrentUserDeleteRecipe] Unable to delete the recipe", ex);
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

                      // Notifiy PMs that dataflows changed
                      //foreach (var actorType in _dfClientConfiguration.AvailableModules)
                      //{
                      //    _sharedSupervisors.GetGlobalStatusSupervisor(actorType).DataflowsChanged();
                      //}
                  }
                  catch (Exception ex)
                  {
                      ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowException(ex, "Error during save");
                      Init();
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
                  Init();
              },
              () => { return true; }));
            }
        }
        #endregion Commands

        public void Refresh()
        {
            Init();
        }
        public bool CanClose()
        {
            throw new NotImplementedException();
        }
        public bool IsEnabled => true;

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
                }
            }
        }
    }
}
