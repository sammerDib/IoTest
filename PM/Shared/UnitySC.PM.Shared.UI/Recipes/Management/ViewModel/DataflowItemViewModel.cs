using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.Graph;
using UnitySC.Shared.Dataflow.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;
using UnitySC.DataAccess.Dto;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public delegate void DataflowGraphChangedEvent();


    public class DuplicatedDataflowNameException : Exception
    {
 
    }

    public class DataflowItemViewModel : TreeViewItemViewModel
    {

        private DataflowInfo _dataflowInfo;
        private IDFClientConfiguration _dfClientConfiguration;

        public string Name
        {
            get => _dataflowInfo.Name; 
            set 
            { 
                if (_dataflowInfo.Name != value)
                {
                   var newName= new PathString(value).RemoveInvalidFilePathCharacters("_", false);
                   if (!IsDataflowNameAlreadyUsedByOtherRecipe(newName, Dataflow.RootNode.Component.Key))
                    {
                        _dataflowInfo.Name = newName;
                        OnPropertyChanged();
                        DataflowChanged();
                    }
                    else
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"A dataflow recipe with the name {newName} already exists.", "Rename dataflow recipe", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
        }

        private bool IsDataflowNameAlreadyUsedByOtherRecipe(string dataflowName, Guid recipeKey)
        {
            
            return _dbRecipeService.Invoke(x => x.IsDataflowNameAlreadyUsedByOtherRecipe(dataflowName, recipeKey, false));

        }

        public string Comment
        {
            get => _dataflowInfo.Comment; set { if (_dataflowInfo.Comment != value) { _dataflowInfo.Comment = value; OnPropertyChanged(); DataflowChanged(); } }
        }

        public int Id => _dataflowInfo.Id;


        private ServiceInvoker<IDbRecipeService> _dbRecipeService;

        private DataflowGraphViewModel _dataflow;
        public DataflowGraphViewModel Dataflow
        {
            get
            {
                if (_dataflow == null)
                {
                    _dataflow = new DataflowGraphViewModel(_dataflowInfo.StepId);
                    _dataflow.Name = Name;
                    _dataflow.Changed += DataflowChanged;
                    try
                    {
                        _dataflow.LoadDataflow(_dbRecipeService.Invoke(x => x.GetDataflow(Id)));
                    }
                    catch (Exception ex)
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"Error during loading dataflow: {ex.Message}", "Loading dataflow", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                return _dataflow;
            }
        }

        public void DataflowChanged()
        {
            HasChanged = true;
        }

        public DataflowItemViewModel(DataflowInfo dataflowInfo, TreeViewItemViewModel parent) : base(parent, false)
        {
            _dataflowInfo = dataflowInfo;
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
            _dfClientConfiguration = ClassLocator.Default.GetInstance<IDFClientConfiguration>();
            HasChanged = false;
            this.PropertyChanged += DataflowItemVM_PropertyChanged;
        }
        private void DataflowItemVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HasChanged))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DoSaveDFRecipe.NotifyCanExecuteChanged();
                });

            }

        }

        public override string ToString()
        {
            return Name;
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
            if (HasChanged)
            {
                // Check that the dataflow name is unique, it could happen if it is a new recipe and the name has not been changed

                if (IsDataflowNameAlreadyUsedByOtherRecipe(Name, Dataflow.RootNode.Component.Key))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"Could not save the recipe because a dataflow recipe with the name \"{Name}\" already exists", "Save dataflow recipe", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    throw (new DuplicatedDataflowNameException());
                }

                Dataflow.RootNode.Component.Name = Name;
                Dataflow.RootNode.Component.Comment = Comment;
                var userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
                var id = _dbRecipeService.Invoke(x => x.SetDataflow(Dataflow.RootNode.Component, userId, _dataflowInfo.StepId, _dfClientConfiguration.ToolKey, true));
                if (Dataflow.RootNode.Component.Key == Guid.Empty)
                    Dataflow.RootNode.Component.Key = _dbRecipeService.Invoke(x => x.GetDataflow(id)).Key;

                HasChanged = false;
            }
           
        }

        public override void Archive()
        {           
            if (Dataflow.RootNode.Key != Guid.Empty)
                _dbRecipeService.Invoke(x => x.ArchiveAllVersionOfDataflow(Dataflow.RootNode.Key, ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id));
        }
        private AutoRelayCommand _doSaveDFRecipe;

        public AutoRelayCommand DoSaveDFRecipe
        {
            get
            {
                return _doSaveDFRecipe ?? (_doSaveDFRecipe = new AutoRelayCommand(
                    async () =>
                    {

                        try
                        {
                            await Task.Run(() => Save());
                        }
                        catch (DuplicatedDataflowNameException)
                        {
                        }
                        catch (Exception ex)
                        {
                            ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Error during saving the dataflow");
                            ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowException(ex, "Error during saving the dataflow");
                        }
                    },
                    () => { return HasChanged; }
                ));
            }
        }

        public override bool CanUserRemove()
        {
            return true;
        }

    }
}
