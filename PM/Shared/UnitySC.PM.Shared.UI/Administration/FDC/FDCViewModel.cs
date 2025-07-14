using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.ClientProxy.FDC;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

using Dto = UnitySC.DataAccess.Dto;

namespace UnitySC.PM.Shared.UI.Administration.FDC
{
    public class FDCViewModel : ObservableObject, IMenuContentViewModel
    {
        private ServiceInvoker<IUserService> _userService;
        private ServiceInvoker<ILogService> _logService;
        private ILogger _logger;
        private IDialogOwnerService _dialogOwnerService;


        private List<FDCActorViewModel> _fdcActorViewModels;

        public List<FDCActorViewModel> FDCActorViewModels
        {
            get => _fdcActorViewModels; set { if (_fdcActorViewModels != value) { _fdcActorViewModels = value; OnPropertyChanged(); } }
        }


        public ObservableCollection<FDCItemViewModel> FDCItems { get; set; }

        public FDCViewModel(ILogger<FDCViewModel> logger, IDialogOwnerService dialogOwnerService)
        {
            _logger = logger;
            _dialogOwnerService = dialogOwnerService;

            FDCItems = new ObservableCollection<FDCItemViewModel>();
            var _globalStatusSupervisor = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();
            _globalStatusSupervisor.OnStateChanged += _globalStatusSupervisor_OnStateChanged;
            FDCActorViewModels = new List<FDCActorViewModel>();

            var pmFDCSupervisor = ClassLocator.Default.GetInstance<FDCSupervisor>();

            FDCActorViewModels.Add(new FDCActorViewModel(pmFDCSupervisor, pmFDCSupervisor.Actor.ToString(), logger, dialogOwnerService));
            FDCActorViewModels.Add(new FDCActorViewModel(new FDCSupervisor(ClassLocator.Default.GetInstance<ILogger<FDCSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>(),ActorType.DataAccess), "DataAccess", logger, dialogOwnerService));
            FDCActorViewModels.Add(new FDCActorViewModel(new FDCSupervisor(ClassLocator.Default.GetInstance<ILogger<FDCSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>(), ActorType.DataflowManager), "Dataflow", logger, dialogOwnerService));


            // For design
            //var newFDCItem=new FDCItemViewModel() { Name = "fdcName", Value="42", Unit = "µm", SendFrequency = FDCSendFrequency.Hour, ValueType = FDCValueType.TypeInt };

            //FDCItems.Add(newFDCItem);
        }

        private void _globalStatusSupervisor_OnStateChanged(PMGlobalStates state)
        {
            OnPropertyChanged(nameof(IsEnabled));
        }

        private void Init()
        {

            IsBusy = true;
            BusyMessage = "Waiting for servers...";
            Task.Run(() =>
            {
                _userService = new ServiceInvoker<IUserService>("UserService", ClassLocator.Default.GetInstance<SerilogLogger<IUserService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
                _logService = new ServiceInvoker<ILogService>("LogService", ClassLocator.Default.GetInstance<SerilogLogger<ILogService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
                foreach (var fdcActorViewModel in FDCActorViewModels)
                {
                    fdcActorViewModel.Init();
                }
                IsBusy = false;
            }
            );
        }

        public bool IsEnabled
        {
            get
            {
                var _globalStatusSupervisor = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();
                if (_globalStatusSupervisor != null && (_globalStatusSupervisor.CurrentState == PMGlobalStates.Free || _globalStatusSupervisor.CurrentState == PMGlobalStates.Busy))
                    return true;

                return false;
            }
        }

        public void Refresh()
        {
            Init();
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private string _busyMessage;

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; OnPropertyChanged(); } }
        }

        private FilterItemViewModel<Dto.User> _selectedUser;

        public FilterItemViewModel<Dto.User> SelectedUser
        {
            get => _selectedUser; set { if (_selectedUser != value) { _selectedUser = value; OnPropertyChanged(); } }
        }

        public bool CanClose()
        {
            foreach (var fdcActorViewModel in FDCActorViewModels)
            {
                if (!fdcActorViewModel.CanClose())
                    return false;
            }
            return true;
        }

  
    }
}
