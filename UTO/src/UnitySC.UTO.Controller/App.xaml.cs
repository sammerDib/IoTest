using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Recipes.Annotations;

using GEM;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.GUI.Common.Configuration;
using UnitySC.GUI.Common.Equipment;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.UTO.Controller.Components.DataFlow;
using UnitySC.UTO.Controller.Configuration;
using UnitySC.UTO.Controller.Counters;
using UnitySC.UTO.Controller.Logging;
using UnitySC.UTO.Controller.Remote;
using UnitySC.UTO.Controller.Views;

namespace UnitySC.UTO.Controller
{
    /// <summary>Interaction logic for App.xaml</summary>
    public partial class App : INotifyPropertyChanged
    {
        //Set to false if GEM is not supported
        internal bool IsGemSupported = true;

        //Set to false if GEM300 is not supported
        internal bool IsGem300Supported = true;

        internal bool IsGemEnabled => _isPv2Supported.Value && IsGemSupported;

        private readonly Lazy<bool> _isPv2Supported = new(() => G_E_M.IsLicensed);

        public ConfigManager<UserProfileAssociations> UserProfileManager { get; set; }

        internal EventLogObserver _eventLogObserver;

        internal IdleDetection.IdleDetection IdleDetection;

        internal string UserProfilesXmlPath = "Configuration\\XML\\UserProfiles.xml";
        internal string EventLogConfigPath = "";
        internal bool ThroughputDisplay;

        #region Properties

        public App()
        {
            var args = Environment.GetCommandLineArgs();
            var dictionary = new Dictionary<string, string>();

            for (var index = 1; index < args.Length; index += 2)
            {
                dictionary.Add(args[index].Remove(0, 2), args[index + 1]);
            }

            if (dictionary.TryGetValue(nameof(ConfigurationPath), out var configXmlPath))
            {
                ConfigurationPath = configXmlPath;
            }

            if (dictionary.TryGetValue(nameof(UserProfilesXmlPath), out var userPath))
            {
                UserProfilesXmlPath = userPath;
            }

            if (dictionary.TryGetValue(nameof(EventLogConfigPath), out var eventLogPath))
            {
                EventLogConfigPath = eventLogPath;
            }

            if (dictionary.TryGetValue(nameof(ThroughputDisplay), out var throughputDisplayAsString)
                && bool.TryParse(throughputDisplayAsString, out var throughputDisplay))
            {
                ThroughputDisplay = throughputDisplay;
            }
            
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //Exit if rorze software is running
            if (Process.GetProcesses().Any(p => p.ProcessName.Equals("EFEM")))
            {
                MessageBox.Show(L10N.RORZE_SOFT_ERROR);
                Environment.Exit(1);
            }
            base.OnStartup(e);
        }

        /// <summary>
        /// Gets the current instance of <see cref="App"/>.
        /// </summary>
        public static App UtoInstance => IsInDesignMode ? null : (App)Application.Current;

        public ControllerConfiguration ControllerConfig => Config as ControllerConfiguration;

        /// <summary>Gets the equipment manager.</summary>
        public ControllerEquipmentManager ControllerEquipmentManager
            => EquipmentManager as ControllerEquipmentManager;

        /// <summary>Gets the current instance of <see cref="App" />.</summary>
        public static App ControllerInstance
            => Notifier.IsInDesignModeStatic
                ? null
                : (App)Application.Current;

        /// <summary>Gets the MainWindow ViewModel</summary>
        public MainWindowViewModel MainWindowViewModel => UserInterface as MainWindowViewModel;

        /// <inheritdoc />
        public override string ApplicationName => "UTO Controller";

        public GemController GemController { get; private set; }

        public DataFlowClientConfigurationManager DataFlowClientConfigurationManager
        {
            get;
            private set;
        }

        public JobQueuer.JobQueueManager JobQueueManager { get; private set; }

        public UnityDeviceUiManagerService DeviceUiManagerService { get; private set; }

        public CounterManager CountersManager { get; set; }

        #endregion

        /// <inheritdoc />
        protected override void CreateAdditionalComponents()
        {
            #region Tracer

            if (!string.IsNullOrEmpty(EventLogConfigPath))
            {
                TraceManager.Instance().AddListener(new EventLogListener(EventLogConfigPath));
            }

            #endregion Tracer

            base.CreateAdditionalComponents();

            CreateUserProfileManager();

            // GEM Controller
            if (IsGemEnabled)
            {
                GemController = new GemController();
                GemController.UserErrorRaised += AnyComponent_UserErrorRaised;
                GemController.UserWarningRaised += AnyComponent_UserWarningRaised;
                GemController.UserInformationRaised += AnyComponent_UserInformationRaised;
            }

            DataFlowClientConfigurationManager = new DataFlowClientConfigurationManager();
            DeviceUiManagerService = new UnityDeviceUiManagerService();
            CountersManager = new CounterManager(Config.ApplicationPath.AutomationConfigPath);
        }

        private void CreateUserProfileManager()
        {
            UserProfileManager = new ConfigManager<UserProfileAssociations>(
                new XmlDataContractStoreStrategy(UserProfilesXmlPath),
                new DataContractCloneStrategy(),
                new DataContractCompareStrategy<UserProfileAssociations>());
        }

        /// <inheritdoc />
        protected override void SetupAdditionalComponents()
        {
            SetupUserProfileManager();

            // GEM
            SetupGemController();

            if (!string.IsNullOrEmpty(EventLogConfigPath))
            {
                _eventLogObserver = new EventLogObserver();
                _eventLogObserver.OnSetup(ControllerEquipmentManager.Equipment);
            }

            JobQueueManager = new JobQueuer.JobQueueManager();

            foreach (var device in ControllerEquipmentManager.Equipment.AllOfType<GenericDevice>())
            {
                device.UserInformationRaised += AnyComponent_UserInformationRaised;
                device.UserWarningRaised += AnyComponent_UserWarningRaised;
                device.UserErrorRaised += AnyComponent_UserErrorRaised;
            }

            ControllerEquipmentManager.Robot.CommandConfirmationRequested +=
                Robot_CommandConfirmationRequested;

            foreach (var processModule in ControllerEquipmentManager.ProcessModules.Values)
            {
                processModule.SmokeDetected += ProcessModule_SmokeDetected;
            }

            DataFlowClientConfigurationManager.Setup();
            DeviceUiManagerService.Setup();

            Task.Run(
                () =>
                {
                    // Automatically start communication
                    ControllerEquipmentManager.Efem.StartCommunication();

                    ControllerEquipmentManager.Equipment.AllDevices<AbstractDataFlowManager>()
                        .First()
                        .StartCommunication();

                    if (!ControllerEquipmentManager.Ffu.IsCommunicationStarted)
                    {
                        ControllerEquipmentManager.Ffu.StartCommunication();
                    }

                    if (!ControllerEquipmentManager.LightTower.IsCommunicationStarted)
                    {
                        ControllerEquipmentManager.LightTower.StartCommunication();
                    }

                    foreach (var processModule in ControllerEquipmentManager.ProcessModules.Values)
                    {
                        if (!processModule.IsCommunicationStarted)
                        {
                            processModule.StartCommunication();
                        }
                    }

                    if (Config.InitRequiredAtStartup)
                    {
                        ControllerEquipmentManager.Controller.Initialize(
                            !Instance.Config.UseWarmInit);
                    }
                });

            UtoInstance.AccessRights.UserLogon += AccessRights_UserLogon;
            UtoInstance.AccessRights.UserLogoff += AccessRights_UserLogoff;
        }

        private void SetupUserProfileManager()
        {
            // Try to load the configuration from disk
            if (UserProfileManager.Load())
            {
                Tracer.Trace(
                    ApplicationName,
                    TraceLevelType.Info,
                    $"UserProfileAssociations configuration loaded.",
                    new TraceParam(UserProfileManager.Loaded.ToString()));
            }
            else
            {
                UserProfileManager.Load(new UserProfileAssociations());
                UserProfileManager.Save();

                Tracer.Trace(
                    ApplicationName,
                    TraceLevelType.Error,
                    $"Error during loading UserProfileAssociations configuration. Default configuration set.");
            }
        }

        private void SetupGemController()
        {
            if (IsGemEnabled)
            {
                GemController.OnSetup(ControllerConfig, EquipmentManager.Equipment);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetAdditionalLocalizationResources()
        {
            // Enable localization feature
            var xmlResources = new XmlDictionaryProvider(Config.Path.LocalizationPath);
            LocalizationManager.AddLocalizationProvider(xmlResources, true);

            return new List<Type> { typeof(L10N) };
        }

        protected override void DisposeAdditionalComponents()
        {
            EquipmentFactory.CloseSimulationWindow();

            foreach (var device in ControllerEquipmentManager.Equipment.AllOfType<GenericDevice>())
            {
                device.UserInformationRaised -= AnyComponent_UserInformationRaised;
                device.UserWarningRaised -= AnyComponent_UserWarningRaised;
                device.UserErrorRaised -= AnyComponent_UserErrorRaised;
            }

            ControllerEquipmentManager.Robot.CommandConfirmationRequested -=
                Robot_CommandConfirmationRequested;

            foreach (var processModule in ControllerEquipmentManager.ProcessModules.Values)
            {
                processModule.SmokeDetected -= ProcessModule_SmokeDetected;
            }

            if (GemController != null)
            {
                GemController.UserErrorRaised -= AnyComponent_UserErrorRaised;
                GemController.UserWarningRaised -= AnyComponent_UserWarningRaised;
                GemController.UserInformationRaised -= AnyComponent_UserInformationRaised;
                GemController.Dispose();
            }

            EquipmentManager?.Dispose();

            if (!string.IsNullOrEmpty(EventLogConfigPath))
            {
                _eventLogObserver.Dispose();
            }

            UtoInstance.AccessRights.UserLogon -= AccessRights_UserLogon;
            UtoInstance.AccessRights.UserLogoff -= AccessRights_UserLogoff;

            if (IdleDetection != null)
            {
                IdleDetection.PropertyChanged -= IdleDetection_PropertyChanged;
            }

            base.DisposeAdditionalComponents();
        }

        /// <inheritdoc />
        protected override Window CreateMainWindow()
        {
            var mainWindow = new MainWindow();

            if (Config?.GlobalSettings?.IsDeveloperDebugModeEnabled ?? false)
            {
                mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                mainWindow.ResizeMode = ResizeMode.CanResize;
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            return mainWindow;
        }

        /// <inheritdoc />
        protected override UserInterface CreateUserInterface()
        {
            return new MainWindowViewModel(AccessRights, Localizer, UserInterfaceLogger);
        }

        #region Configuration

        /// <summary>Useful to customize default <see cref="IConfigManager" /> object</summary>
        /// <returns>Instance that implements<see cref="IConfigManager" /></returns>
        protected override IConfigManager CreateConfigurationManager()
        {
            var configManager = new ConfigManager<ControllerConfiguration>(
                new XmlDataContractStoreStrategy(ConfigurationPath),
                new DataContractCloneStrategy(),
                new DataContractCompareStrategy<ControllerConfiguration>());

            return configManager;
        }

        protected override IConfigManager LoadDefaultConfiguration()
        {
            var config = new ControllerConfiguration();
            (ConfigurationManager as ConfigManager<ControllerConfiguration>)?.Load(config);
            return ConfigurationManager;
        }

        #endregion Configuration

        #region Event Handlers

        private void Robot_CommandConfirmationRequested(object sender, CommandConfirmationRequestedEventArgs e)
        {
            var robot = sender as Robot;

            //Always accept command in case of UTO Controller
            robot?.CommandGranted(e.Uuid);
        }

        private void ProcessModule_SmokeDetected(object sender, EventArgs e)
        {
            if (sender is not DriveableProcessModule processModule)
            {
                return;
            }

            var popup = new Popup(
                nameof(L10N.POPUP_SMOKE_DETECTED),
                nameof(L10N.POPUP_SMOKE_DETECTED_OPERATOR_MESSAGE))
            {
                SeverityLevel = MessageLevel.Warning
            };

            popup.Commands.Add(
                new PopupCommand(
                    Agileo.GUI.Properties.Resources.S_YES,
                    new DelegateCommand(
                        () =>
                        {
                            Logger.Warning(
                                "Smoke detected. The emergency stop procedure has been interrupted.");
                            processModule.ResetSmokeDetectorAlarmAsync();
                        })));

            popup.Commands.Add(
                new PopupCommand(
                    Agileo.GUI.Properties.Resources.S_NO,
                    new DelegateCommand(
                        () => Logger.Warning("Smoke detected. Stopping the machine."))));

            Current.UserInterface.Popups.Show(popup);
        }

        protected override void App_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            Tracer?.Trace(
                ApplicationName,
                TraceLevelType.Fatal,
                $"Application Dispatcher Unhandled Exception. Message={e.Exception.Message}",
                new TraceParam(e.Exception.ToTraceParam()));
            CountersManager.IncrementCounter(CounterDefinition.FatalErrorCounter);
            base.App_DispatcherUnhandledException(sender, e);
        }

        private void AccessRights_UserLogon(
            Agileo.Common.Access.Users.User user,
            Agileo.Common.Access.Users.UserEventArgs e)
        {
            if (IdleDetection != null)
            {
                return;
            }

            IdleDetection =
                new IdleDetection.IdleDetection(ControllerConfig.InactivityTimeoutDuration);
            IdleDetection.PropertyChanged += IdleDetection_PropertyChanged;
        }

        private void AccessRights_UserLogoff(
            Agileo.Common.Access.Users.User user,
            Agileo.Common.Access.Users.UserEventArgs e)
        {
            IdleDetection.PropertyChanged -= IdleDetection_PropertyChanged;
            IdleDetection.Dispose();
            IdleDetection = null;
        }

        private void IdleDetection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (UtoInstance.AccessRights.IsAnybodyLogged)
            {
                var currentUser = UtoInstance.AccessRights.CurrentUser;
                UtoInstance.AccessRights.Logoff(currentUser.Name, currentUser.Password);
            }
        }

        private void AnyComponent_UserInformationRaised(object sender, UserInformationEventArgs e)
        {
            DisplayUserMessage(MessageLevel.Info, sender, e.Message);
        }

        private void AnyComponent_UserWarningRaised(object sender, UserInformationEventArgs e)
        {
            DisplayUserMessage(MessageLevel.Warning, sender, e.Message);
        }

        private void AnyComponent_UserErrorRaised(object sender, UserInformationEventArgs e)
        {
            DisplayUserMessage(MessageLevel.Error, sender, e.Message);
        }

        private void DisplayUserMessage(MessageLevel level, object sender, string message)
        {
            var userMessage = message;
            if (sender is GenericDevice device)
            {
                userMessage = $"{device.Name} - {message}";
            }

            MainUserMessageDisplayer.HideAll();

            MainUserMessageDisplayer.Show(
                new UserMessage(level, userMessage) { CanUserCloseMessage = true });
        }

        #endregion

        protected override UnityScConfiguration GetConfig()
        {
            return (ControllerConfiguration)ConfigurationManager?.Current;
        }

        protected override ILogger GetApplicationLogger()
        {
            return GetLogger(nameof(Controller));
        }

        protected override string GetApplicationName()
        {
            return "UTO Controller";
        }

        protected override EquipmentManager CreateEquipmentManager(
            EquipmentConfiguration equipmentConfig,
            string currentExecutionPath)
        {
            return EquipmentFactory.CreateEquipmentManager(equipmentConfig);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}
