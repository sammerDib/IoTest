using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Hardware.ClientProxy.Plc;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

using Message = UnitySC.Shared.Tools.Service.Message;

namespace UnitySC.PM.Shared.UI.Main
{
    public class PMViewModel : ObservableObject
    {
        //Each attempt will take at most 90 secs, kept 1 here for a "reasonable" delay
        private const int MaxReconnectionAttemptsToDatabase = 1;
        private bool _isStandAlone = true;
        private const int MaxReconnectionAttempToServer = 1;
        public MainMenuViewModel MainMenuViewModel { get; private set; }
        public bool IsServerConnected { get; set; }
        public ActorType ActorType { get; private set; }
        private ILogger _logger;
        private ApplicationMode _startMode;
        private IUserSupervisor _userSupervisor;
        private PlcSupervisor _plcSupervisor;
        private bool _isHardwareReservedByMe = false;

        public PMViewModel(ActorType actorType, ApplicationMode startMode, MainMenuViewModel mainmenuViewModel = null, bool showLogin = true)
        {
            _startMode = startMode;
            _logger = ClassLocator.Default.GetInstance<ILogger<PMViewModel>>();
            _userSupervisor = ClassLocator.Default.GetInstance<IUserSupervisor>();
            ActorType = actorType;
            MainMenuViewModel = mainmenuViewModel;
            CommunicationError = null;

            GlobalStatusSupervisor = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();
            GlobalStatusSupervisor.OnStateChanged += GlobalStatusSupervisor_OnStateChanged;
            GlobalStatusSupervisor.OnStateToolModeChanged += GlobalStatusSupervisor_OnStateToolModeChanged;
            OnPropertyChanged(nameof(GlobalStatusSupervisor));
            _userSupervisor.UserChanged += UserSupervisor_UserChanged;

            ClassLocator.Default.GetInstance<IMessenger>().Register<ClearAllMessages>(this, (r, m) => ClearAllMessages(m));
            ClassLocator.Default.GetInstance<IMessenger>().Register<ClearMessage>(this, (r, m) => ClearMessage(m));
            _isStandAlone = showLogin;
            Init(showLogin);
        }

        private void ClearMessage(ClearMessage m)
        {
            GlobalStatusSupervisor.ClearMessage(m.MessageToClear);
        }

        private void ClearAllMessages(ClearAllMessages m)
        {
            GlobalStatusSupervisor.ClearAllMessages();
        }

        private void UserSupervisor_UserChanged(UnifiedUser user)
        {
            HomeCommand.Execute(null);
            if (user == null)
            {
                Login = null;
                UserProfile = UserProfiles.Basic;
                CanChangeMode = false;
            }
            else
            {
                Login = user.Name;
                UserProfile = user.Profile;
                CanChangeMode = user.Rights.Contains(UserRights.ChangePMMode);
            }
            if (MainMenuViewModel != null)
                MainMenuViewModel.Refresh(Mode, user);
        }

        private bool _canChangeMode;

        public bool CanChangeMode
        {
            get => _canChangeMode; set { if (_canChangeMode != value) { _canChangeMode = value; OnPropertyChanged(); MaintenanceCommand.NotifyCanExecuteChanged(); } }
        }

        private UIGlobalStates _currentUIState;

        public UIGlobalStates CurrentUIState
        {
            get => _currentUIState;
            set
            {
                if (_currentUIState != value)
                {
                    _currentUIState = value;
                    OnPropertyChanged();
                }
                CurrentStateInfo = Resources.ResourceManager.GetString("UIGlobalStates." + _currentUIState.ToString(), CultureInfo.CurrentCulture);
            }
        }

        private string _login;

        public string Login
        {
            get => _login; set { if (_login != value) { _login = value; OnPropertyChanged(); LogOffCommand.NotifyCanExecuteChanged(); } }
        }

        private UserProfiles _userProfile;

        public UserProfiles UserProfile
        {
            get => _userProfile; set { if (_userProfile != value) { _userProfile = value; OnPropertyChanged(); } }
        }

        public PMConfiguration Configuration { get; private set; }

        private void GlobalStatusSupervisor_OnStateChanged(PMGlobalStates state)
        {
            if (Mode == ApplicationMode.Maintenance && state != PMGlobalStates.Busy && state != PMGlobalStates.ErrorHandling)
            {
                Mode = ApplicationMode.Production;
            }
            if (Mode == ApplicationMode.Maintenance && state == PMGlobalStates.ErrorHandling)
            {
                Mode = ApplicationMode.Maintenance;
            }

            //Switch to production mode, just to return to the main window
            if (state == PMGlobalStates.Error)
            {
                if (Mode != ApplicationMode.Production)
                {
                    Mode = ApplicationMode.Production;
                }
                else
                {
                    _ = SetModeAsync(Mode);
                }
            }

            switch (state)
            {
                case PMGlobalStates.NotInitialized:
                    CurrentUIState = UIGlobalStates.NotInitialized;
                    break;

                case PMGlobalStates.Initializing:
                    CurrentUIState = UIGlobalStates.Initializing;
                    break;

                case PMGlobalStates.Free:
                    CurrentUIState = UIGlobalStates.Available;
                    break;

                case PMGlobalStates.Error:
                    CurrentUIState = UIGlobalStates.Error;
                    break;

                case PMGlobalStates.Busy:
                    if (Mode == ApplicationMode.Production)
                    {
                        if (_isHardwareReservedByMe)
                            CurrentUIState = UIGlobalStates.Available;
                        else
                            CurrentUIState = UIGlobalStates.InUse;
                    }
                    else
                    {
                        CurrentUIState = UIGlobalStates.Maintenance;
                    }
                    break;
            }
        }

        private async void GlobalStatusSupervisor_OnNewMessage(Message message)
        {
            if (IsSmokeDetected(message))
            {
                await ExecuteResetSmokeAlarm();
            }
        }

        private bool IsSmokeDetected(Message message)
        {
            return message.Level == MessageLevel.Error && message.UserContent.Contains("Smoke detected") && GlobalStatusSupervisor.CurrentState != PMGlobalStates.Error;
        }

        private async Task ExecuteResetSmokeAlarm()
        {
            var result = await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Smoke detected. \nThe sensor has detected smoke. The system will shut down the machine. Do you want to interrupt the emergency stop procedure ?", "Smoke", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.None);
            });
            if (result == MessageBoxResult.Yes)
            {
                _logger.Warning($"Smoke detected. The emergency stop procedure has been interrupted.");
                await Task.Run(() => PlcSupervisor.SmokeDetectorResetAlarm());
            }
            else
            {
                _logger.Warning($"Smoke detected. Stopping the machine.");
            }
        }

        public PlcSupervisor PlcSupervisor
        {
            get
            {
                if (_plcSupervisor == null)
                {
                    _plcSupervisor = ClassLocator.Default.GetInstance<PlcSupervisor>();
                }

                return _plcSupervisor;
            }
        }

        private string _currentStateInfo;

        public string CurrentStateInfo
        {
            get => _currentStateInfo; set { if (_currentStateInfo != value) { _currentStateInfo = value; OnPropertyChanged(); } }
        }

        private ToolMode _currentToolMode = ToolMode.Unknown;

        public ToolMode CurrentToolMode
        {
            get => _currentToolMode; set { if (_currentToolMode != value) { _currentToolMode = value; OnPropertyChanged(); } }
        }

        public GlobalStatusSupervisor GlobalStatusSupervisor { get; set; }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _busyContent = "Please wait...";

        public string BusyContent
        {
            get => _busyContent; set { if (_busyContent != value) { _busyContent = value; OnPropertyChanged(); } }
        }

        private string _communicationError = null;

        public string CommunicationError
        {
            get => _communicationError; set { if (_communicationError != value) { _communicationError = value; OnPropertyChanged(); } }
        }

        private ApplicationMode _mode;

        public ApplicationMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _ = SetModeAsync(value);
                }
            }
        }

        private async Task SetModeAsync(ApplicationMode value)
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                try
                {
                    ApplicationMode previousValue = _mode;
                    bool canChange = false;
                    _mode = value;

                    // Check Menu
                    if ((CurrentMenu != null) && (MainMenuViewModel != null) && !MainMenuViewModel.Groups.SelectMany(x => x.MenuItems).Where(x => x.CompatibleWith.Contains(_mode)).Contains(CurrentMenu))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (CurrentMenu.CanClose())
                            {
                                canChange = true;
                            }
                        });
                    }
                    else
                    {
                        canChange = true;
                    }

                    // Try to reserve/release hardware
                    if (canChange)
                    {
                        if (_mode == ApplicationMode.Maintenance)
                        {
                            _isHardwareReservedByMe = true;
                            canChange = GlobalStatusSupervisor.ReserveHardware()?.Result ?? false;
                            if (!canChange)
                            {
                                _logger.Warning("Can't reserve hardware");
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Can't reserve hardware", "Reserve Hardware", MessageBoxButton.OK, MessageBoxImage.Warning);
                                });
                            }
                        }
                        else
                        {
                            _isHardwareReservedByMe = false;
                            GlobalStatusSupervisor.ReleaseHardware();
                        }
                    }

                    if (canChange)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (MainMenuViewModel != null)
                                MainMenuViewModel.Refresh(_mode, ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser);

                            HomeCommand.Execute(null);
                        }
                        ));
                    }
                    else
                    {
                        _mode = previousValue;
                    }

                    OnPropertyChanged(nameof(Mode));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Set Mode failed");
                }
            });

            IsBusy = false;
        }

        private async void Init(bool showLogin)
        {
            CommunicationError = null;
            IsBusy = true;

            await Task.Run(async () =>
            {
                await ConnectToServerAsync();
                await ConnectToDatabase();
            });

            NotifyClientStartedFDC();

            IsBusy = false;

            if ((CommunicationError == null) && (showLogin))
            {
                OpenConnectionWindow();
                Mode = _startMode;
            }

            await Task.Run(() => StartServerHealthCheckAsync());
        }

        private Guid _clientId = Guid.NewGuid();

        private string _fdcClientName => $"{ActorType.ToString()}_Client_{_clientId}";
        private void NotifyClientStartedFDC()
        {
            if (!_isStandAlone)
                return;

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var clientFDCsSupervisor = ClassLocator.Default.GetInstance<ClientFDCsSupervisor>();
                        clientFDCsSupervisor.ClientStarted(_fdcClientName);
                        if (clientFDCsSupervisor.IsChannelOpened())
                        {
                            StartClientHeartBeat();
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        // Happens when not yet implemented in the client
                        return;
                    }
                    Task.Delay(10000).Wait();
                }
            });
        }

        private void StartClientHeartBeat()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var clientFDCsSupervisor = ClassLocator.Default.GetInstance<ClientFDCsSupervisor>();
                        clientFDCsSupervisor.ClientIsRunning(_fdcClientName);
                    }
                    catch (Exception)
                    {
                    }
                    Task.Delay(FDCsConsts.HeartBeatDelay_ms).Wait();
                }
            });
        }

        public void ConnectUser()
        {
            if (CommunicationError == null)
            {
                OpenConnectionWindow();
                Mode = _startMode;
            }
        }

        private async Task ConnectToDatabase()
        {
            var nbAttempts = 0;
            while (nbAttempts < MaxReconnectionAttemptsToDatabase)
            {
                try
                {
                    ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>().Invoke(x => x.IsConnectionAvailable());
                    return;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Connection with IDbRecipeService error");
                    if (nbAttempts == MaxReconnectionAttemptsToDatabase - 1)
                        CommunicationError = "Connection to DataAcces server error";
                }

                nbAttempts++;
                await Task.Delay(2000);
            }
        }

        private async Task ConnectToServerAsync()
        {
            int nbAttempts = 0;
            while (nbAttempts < MaxReconnectionAttempToServer)
            {
                try
                {
                    if (!GlobalStatusSupervisor.IsChannelOpened()) 
                        GlobalStatusSupervisor.Init();
                    IsServerConnected = true;
                    return;
                }
                catch (Exception ex)
                {
                    IsServerConnected = false;
                    _logger.Error(ex, "Init global status supervisor error");
                    if (nbAttempts == MaxReconnectionAttempToServer - 1)
                        CommunicationError = "Connection to Process Module server error";
                }

                nbAttempts++;
                await Task.Delay(2000);
            }
        }

        private async Task StartServerHealthCheckAsync()
        {
            while (IsServerConnected)
            {
                if (!GlobalStatusSupervisor.IsChannelOpened())
                {
                    CommunicationError = "Server is no more reachable";
                    IsServerConnected = false;
                    ClassLocator.Default.GetInstance<IMessenger>().Send(new ConnexionStateForActor() { Actor = ActorType, Status = ConnexionState.Faulted });
                    return;
                }
                await Task.Delay(2000);
            }
        }

        private void OpenConnectionWindow()
        {
#if DEBUG
            _userSupervisor.Connect("SUPERVISOR", string.Empty);
            return;
#else
            var result = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<UnitySC.PM.Shared.UI.Connection.ConnectionWindow>(
                new UnitySC.PM.Shared.UI.Connection.ConnectionViewModel(_userSupervisor, _logger));
            if (!result.HasValue || !result.Value)
                Application.Current.Shutdown();
#endif
        }

        public bool CanClose()
        {
            if (MainMenuViewModel is null)
                return true;
            return MainMenuViewModel.CanClose();
        }

        private AutoRelayCommand<IMenuItem> _navigateCommand;

        public AutoRelayCommand<IMenuItem> NavigateCommand
        {
            get
            {
                return _navigateCommand ?? (_navigateCommand = new AutoRelayCommand<IMenuItem>(
              (menu) =>
              {
                  if (menu != null)
                  {
                      try
                      {
                          menu.ViewModel.Refresh();
                          CurrentMenu = menu;
                          CurrentControl = CurrentMenu.UserControl;
                      }
                      catch (Exception ex)
                      {
                          ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "init " + menu.Name);
                          ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Init " + menu.Name);
                      }
                  }
              }));
            }
        }

        private IMenuItem _currentMenu;

        public IMenuItem CurrentMenu
        {
            get => _currentMenu; set { if (_currentMenu != value) { _currentMenu = value; OnPropertyChanged(); HomeCommand.NotifyCanExecuteChanged(); } }
        }

        private UserControl _currentControl;

        public UserControl CurrentControl
        {
            get => _currentControl; set { if (_currentControl != value) { _currentControl = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _homeCommand;

        public AutoRelayCommand HomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new AutoRelayCommand(
              () =>
              {
                  if (CurrentMenu != null)
                  {
                      if (CurrentMenu.ViewModel.CanClose())
                      {
                          CurrentMenu = null;
                          CurrentControl = null;
                      }
                  }
              },
              () => { return CurrentMenu != null; }));
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

        private AutoRelayCommand _initCommand;

        public AutoRelayCommand InitCommand
        {
            get
            {
                return _initCommand ?? (_initCommand = new AutoRelayCommand(
              () =>
              {
                  Init(true);
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _logOffCommand;

        public AutoRelayCommand LogOffCommand
        {
            get
            {
                return _logOffCommand ?? (_logOffCommand = new AutoRelayCommand(
              () =>
              {
                  _userSupervisor.Disconnect();
                  OpenConnectionWindow();
              },
              () => { return !string.IsNullOrEmpty(Login); }));
            }
        }

        private AutoRelayCommand _maintenanceCommand;

        public AutoRelayCommand MaintenanceCommand
        {
            get
            {
                return _maintenanceCommand ?? (_maintenanceCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      Mode = ApplicationMode.Maintenance;
                      var clientFDCsSupervisor = ClassLocator.Default.GetInstance<ClientFDCsSupervisor>();
                      clientFDCsSupervisor.ApplicationModeLocalChanged(true);
                  }
                  catch (Exception)
                  {
                      // Happens when not yet implemented in the client
                  }
              },
              () => { return CanChangeMode; }));
            }
        }

        private AutoRelayCommand _releaseCommand;

        public AutoRelayCommand ReleaseCommand
        {
            get
            {
                return _releaseCommand ?? (_releaseCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      Mode = ApplicationMode.Production;
                      var clientFDCsSupervisor = ClassLocator.Default.GetInstance<ClientFDCsSupervisor>();
                      clientFDCsSupervisor.ApplicationModeLocalChanged(false);
                  }
                  catch (Exception)
                  {
                      // Happens when not yet implemented in the client
                  }
              },
             () => { return true; }));
            }
        }

        private AutoRelayCommand _shutdownCommand;

        public AutoRelayCommand ShutdownCommand
        {
            get
            {
                return _shutdownCommand ?? (_shutdownCommand = new AutoRelayCommand(
                    () =>
                    {
                        Application.Current.Shutdown();
                    }));
            }
        }

        private void GlobalStatusSupervisor_OnStateToolModeChanged(ToolMode mode)
        {
            CurrentToolMode = mode;
        }
    }
}
