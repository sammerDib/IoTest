using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.Recipes.Annotations;

using UnitySC.EFEM.Controller.Configuration;
using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EFEM.Controller.Views;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Efem;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;
using UnitySC.GUI.Common.Configuration;
using UnitySC.GUI.Common.Equipment;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Configuration;

namespace UnitySC.EFEM.Controller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Get the driver used to communicate with the host controller.
        /// </summary>
        public HostDriver HostDriver { get; private set; }

        public EfemEquipmentManager EfemEquipmentManager => EquipmentManager as EfemEquipmentManager;

        public EfemControllerConfiguration EfemConfig => Config as EfemControllerConfiguration;

        public static App EfemAppInstance => IsInDesignMode ? null : (App)Application.Current;

        private ControlState _controlState;

        public ControlState ControlState
        {
            get => _controlState;
            private set
            {
                if (_controlState == value) { return; }

                _controlState = value;
                OnPropertyChanged();
            }
        }

        public bool IsMaintenanceMode => EfemEquipmentManager.Efem.OperationMode == OperationMode.Maintenance;

        public UnityDeviceUiManagerService DeviceUiManagerService { get; private set; }

        #endregion Properties

        public App()
        {
            string[] args = Environment.GetCommandLineArgs();
            var dictionary = new Dictionary<string, string>();

            for (int index = 1; index < args.Length; index += 2)
            {
                dictionary.Add(args[index].Remove(0, 2), args[index + 1]);
            }

            if (dictionary.TryGetValue(nameof(ConfigurationPath), out var configXmlPath))
            {
                ConfigurationPath = configXmlPath;
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

        /// <inheritdoc />
        protected override void CreateAdditionalComponents()
        {
            base.CreateAdditionalComponents();

            // Communication with supervisor
            LogOnSplashScreen("Communication driver creation...", true);
            HostDriver = new HostDriver();
            DeviceUiManagerService = new UnityDeviceUiManagerService();
        }

        /// <inheritdoc />
        protected override void SetupAdditionalComponents()
        {
            // Register to configuration manager in order to propagate configuration changes
            ConfigurationManager.CurrentChanged += ConfigurationManager_CurrentChanged;

            // Call once to initialize components depending on configuration
            ConfigurationManager_CurrentChanged(ConfigurationManager, null);

            EfemEquipmentManager.Robot.CommandConfirmationRequested += Robot_CommandConfirmationRequested;
            EfemEquipmentManager.Efem.StatusValueChanged += Efem_StatusValueChanged;

            // Automatically start communication (Host don't have a command for that so it's better to do it auto at startup)
            EfemEquipmentManager.Efem.StartCommunication();

            if (!EfemEquipmentManager.Ffu.IsCommunicationStarted)
            {
                EfemEquipmentManager.Ffu.StartCommunication();
            }

            if (!EfemEquipmentManager.LightTower.IsCommunicationStarted)
            {
                EfemEquipmentManager.LightTower.StartCommunication();
            }

            foreach (var processModule in EfemEquipmentManager.ProcessModules.Values)
            {
                if (!processModule.IsCommunicationStarted)
                {
                    processModule.StartCommunication();
                }
            }

            //Automatically init devices if init is required at startup
            if (Config.InitRequiredAtStartup)
            {
                if (EquipmentManager.Equipment.AllOfType<GenericDevice>()
                    .All(d => d.State == OperatingModes.Idle || d.State == OperatingModes.Maintenance))
                {
                    EfemEquipmentManager.Controller.InitializeAsync(!Config.UseWarmInit);
                }
                else
                {
                    _initRequired = true;
                    foreach (var device in EquipmentManager.Equipment.AllOfType<GenericDevice>())
                    {
                        device.StatusValueChanged += Device_StatusValueChanged;
                    }
                }
            }

            DeviceUiManagerService.Setup();
        }

        private bool _initRequired;
        private void Device_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (Config.InitRequiredAtStartup
                && _initRequired
                && EquipmentManager.Equipment.AllOfType<GenericDevice>()
                    .All(d => d.State == OperatingModes.Idle || d.State == OperatingModes.Maintenance))
            {
                _initRequired = false;
                foreach (var device in EquipmentManager.Equipment.AllOfType<GenericDevice>())
                {
                    device.StatusValueChanged -= Device_StatusValueChanged;
                }
                EfemEquipmentManager.Controller.InitializeAsync(!Config.UseWarmInit);
            }
        }

        private void Efem_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name.Equals(nameof(IEfem.OperationMode)))
            {
                OnPropertyChanged(nameof(IsMaintenanceMode));
            }
        }

        private void Robot_CommandConfirmationRequested(object sender, CommandConfirmationRequestedEventArgs e)
        {
            var robot                = sender as Robot;
            var armExtensionCommands = new List<string>
            {
                nameof(IRobot.Pick),
                nameof(IRobot.Place),
                nameof(IRobot.ExtendArm)
            };

            if (ControlState == ControlState.Local
                && SequenceProgramProcessor.ProgramExecutionState != ProgramExecutionState.Running
                && armExtensionCommands.Contains(e.CommandId))
            {
                var popup = new Popup(
                    nameof(L10N.POPUP_CONFIRMATION_TITLE),
                    nameof(L10N.POPUP_ROBOT_ARM_EXTENSION_CONFIRMATION_MESSAGE))
                {
                    SeverityLevel = MessageLevel.Warning
                };

                popup.Commands.Add(new PopupCommand(
                    Agileo.GUI.Properties.Resources.S_YES,
                    new DelegateCommand(() => robot?.CommandGranted(e.Uuid))));

                popup.Commands.Add(new PopupCommand(
                    Agileo.GUI.Properties.Resources.S_NO,
                    new DelegateCommand(() => robot?.CommandDenied(e.Uuid))));

                Current.UserInterface.Navigation.SelectedBusinessPanel?.Popups.Show(popup);
            }
            else
            {
                robot?.CommandGranted(e.Uuid);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetAdditionalLocalizationResources()
        {
            // Enable localization feature
            var xmlResources = new XmlDictionaryProvider(((ApplicationConfiguration)ConfigurationManager.Current).Path.LocalizationPath);
            LocalizationManager.AddLocalizationProvider(xmlResources, true);

            return new List<Type>
            {
                typeof(L10N)
            };
        }

        protected override void DisposeAdditionalComponents()
        {
            ConfigurationManager.CurrentChanged -= ConfigurationManager_CurrentChanged;

            EquipmentFactory.CloseSimulationWindow();

            HostDriver.Disconnect();
            HostDriver.Dispose();
            EquipmentManager?.Dispose();
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

        /// <summary>
        /// Useful to customize default <see cref = "IConfigManager" /> object
        /// </summary >
        /// <returns>
        /// Instance that implements<see cref= "IConfigManager" />
        /// </returns>
        protected override IConfigManager CreateConfigurationManager()
        {
            ConfigManager<EfemControllerConfiguration> configManager = new ConfigManager<EfemControllerConfiguration>(
                new XmlDataContractStoreStrategy(ConfigurationPath),
                new DataContractCloneStrategy(),
                new DataContractCompareStrategy<EfemControllerConfiguration>());

            return configManager;
        }

        protected override IConfigManager LoadDefaultConfiguration()
        {
            EfemControllerConfiguration config = new EfemControllerConfiguration();
            (ConfigurationManager as ConfigManager<EfemControllerConfiguration>)?.Load(config);
            return ConfigurationManager;
        }

        private void ConfigurationManager_CurrentChanged(object sender, ConfigurationChangedEventArgs e)
        {
            var oldConfig = e?.OldConfiguration as EfemControllerConfiguration;
            var newConfig = e?.NewConfiguration as EfemControllerConfiguration;

            if (oldConfig == null || newConfig == null)
            {
                // Setup with current config (i.e. first time at startup)
                HostDriver.Setup(EfemConfig.HostConfiguration);
                SwitchControlStateMode(ControlState.Remote);
            }
            else if (oldConfig.HostConfiguration.ToString() != newConfig.HostConfiguration.ToString())
            {
                // Update with new configuration (i.e. when there is an actual change on Host config parameters)
                // Need to reconnect for changes to take effect
                SwitchControlStateMode(ControlState.Local);
                HostDriver.Setup(newConfig.HostConfiguration);
                SwitchControlStateMode(ControlState.Remote);
            }
        }

        #endregion Configuration

        public event EventHandler ControlStateChanged;

        public void SwitchControlStateMode(ControlState mode)
        {
            if (mode == ControlState)
            {
                return;
            }

            ControlState = mode;
            switch (ControlState)
            {
                case ControlState.Remote:
                    if (!HostDriver.IsCommunicationEnabled)
                    {
                        HostDriver.EnableCommunications();
                    }

                    break;
            }

            ControlStateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override UnityScConfiguration GetConfig()
            => (EfemControllerConfiguration)ConfigurationManager?.Current;

        protected override ILogger GetApplicationLogger() => GetLogger(nameof(Controller));

        protected override string GetApplicationName() => "EFEM Controller";

        protected override EquipmentManager CreateEquipmentManager(
            EquipmentConfiguration equipmentConfig,
            string currentExecutionPath)
        {
            return EquipmentFactory.CreateEquipmentManager(equipmentConfig, currentExecutionPath);
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
