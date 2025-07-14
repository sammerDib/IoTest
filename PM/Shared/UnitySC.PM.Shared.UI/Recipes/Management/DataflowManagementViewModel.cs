using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.UI.Recipes.Management
{
    public class DataflowManagementViewModel : ObservableRecipient
    {
        private DataflowViewModel _dataflowViewModel;       


        public DataflowManagementViewModel()
        {             
             CurrentControl = DataflowViewModel;            
        }

        private object _currentControl;
        public object CurrentControl
        {
            get => _currentControl;
            set
            {
                if (_currentControl != value)
                {
                    _currentControl = value;
                    if (_currentControl == null)                        
                        _currentControl = DataflowViewModel;
                    OnPropertyChanged(); 
                } 
            }
        }
        
        public DataflowViewModel DataflowViewModel
        {
            get
            {
                if (_dataflowViewModel == null)
                {
                    _dataflowViewModel = ClassLocator.Default.GetInstance<DataflowViewModel>();
                    _dataflowViewModel.Init();
                }
                return _dataflowViewModel;
            }
        }                        
    }
}
