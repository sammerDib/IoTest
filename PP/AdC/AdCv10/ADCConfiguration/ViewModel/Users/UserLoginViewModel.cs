using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.ViewModel
{
    public class UserLoginViewModel : ViewModelWithMenuBase
    {
        private ServiceInvoker<IUserService> _userService;
        private ServiceInvoker<ILogService> _logService;

        private UserViewModel _user = null;

        public UserViewModel User { get => _user; set { _user = value; OnPropertyChanged(); } }


        private string _connectionError;
        public string ConnectionError
        {
            get => _connectionError; set { if (_connectionError != value) { _connectionError = value; OnPropertyChanged(); } }
        }


        private string _login = string.Empty;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                var mappper = Services.Services.Instance.MapperService.Mapper;
                //User = mappper.Map<UserViewModel>(_userService.Invoke(x => x.GetUser(_login, false));
                User = new UserViewModel()
                {
                    Login = "AdminDev",
                    FirstName = "R",
                    LastName = "T",
                    Role = RoleUser.Expert,
                };
                ConnectionError = null;
                OnPropertyChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); ConnectionError = null; }
        }

        private bool _connected = false;
        public bool Connected { get => _connected; set { _connected = value; OnPropertyChanged(); } }

        public UserLoginViewModel()
        {
#warning ** USP **  constant address need refacto to gat adresss
            var address = new ServiceAddress() { Host = "localhost", Port = 2221 }; //ClientConfiguration.GetDataAccessAddress()

            _userService = new ServiceInvoker<IUserService>("UserService",
                ClassLocator.Default.GetInstance<SerilogLogger<IUserService>>(), 
                ClassLocator.Default.GetInstance<IMessenger>(), address);

            _logService = new ServiceInvoker<ILogService>("LogService", 
                ClassLocator.Default.GetInstance<SerilogLogger<ILogService>>(),
                ClassLocator.Default.GetInstance<IMessenger>(), address);

            Task.Factory.StartNew(() =>
           {
               try
               {
                   IsBusy = true;
                   // Init database connexion
                   _userService.Invoke(x => x.GetAll(null));
               }
               catch (Exception ex)
               {
                   string msg = "Database connection error";
                   Services.Services.Instance.LogService.LogError(msg, ex);
                   System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show(msg, ex); }));
               }
               finally
               {
                   IsBusy = false;
               }
           });
        }

        private AutoRelayCommand _connectionViewCommand = null;
        public AutoRelayCommand ConnectionViewCommand
        {
            get
            {
                return _connectionViewCommand ?? (_connectionViewCommand = new AutoRelayCommand(
            () =>
            {
#warning ** USP ** TODO FDS connection user
                // TODO FDS connection user
                // Connected = _userService.Invoke(x=> x.CheckUserPassword(
                //     Services.Services.Instance.MapperService.Mapper.Map<Dto.User>(User),
                //     Security.ComputeHash(Password)
                //     ));
                Connected = true;

                if (!Connected)
                    ConnectionError = "Invalid account or password";
                else
                    _logService.Invoke(x => x.Connect(Services.Services.Instance.MapperService.Mapper.Map<Dto.User>(User)));

            },
            () => { return !String.IsNullOrEmpty(Login) && !String.IsNullOrEmpty(Password); }));
            }
        }
    }
}
