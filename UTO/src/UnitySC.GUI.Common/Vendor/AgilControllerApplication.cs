using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Agileo.Common.Access;
using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.Common.Tracing;
using Agileo.GUI;
using Agileo.GUI.Components;
using Agileo.GUI.Components.AccessRights;
using Agileo.GUI.Components.Visitors;
using Agileo.GUI.Configuration;
using Agileo.GUI.Extensions;
using Agileo.GUI.Properties;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Recipes.Annotations;

using Humanizer;

using UnitySC.GUI.Common.Vendor.Views.Splashscreen;

namespace UnitySC.GUI.Common.Vendor
{
    public abstract class AgilControllerApplication : AgileoGuiApplication
    {
        #region Fields

        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        private Mutex _mutex;

        #endregion

        #region Properties

        /// <summary>
        /// Get the access manager.
        /// </summary>
        public AccessManager AccessRights { get; protected set; }

        /// <summary>
        /// Get the application name.
        /// </summary>
        public abstract string ApplicationName { get; }

        /// <summary>
        /// Provides version information for current <see cref="AgilControllerApplication"/>
        /// </summary>
        public FileVersionInfo ApplicationVersion { get; private set; }

        /// <summary>
        /// Get the config manager
        /// </summary>
        public IConfigManager ConfigurationManager { get; protected set; }

        /// <summary>
        /// Get the current application as <see cref="AgilControllerApplication" />
        /// </summary>
        public new static AgilControllerApplication Current => (AgilControllerApplication)Application.Current;

        /// <summary>
        /// Get the <see cref="LocalizationManager" />
        /// </summary>
        public LocalizationManager Localizer { get; protected set; }

        /// <summary>
        /// Define if the application was launched in debug mode
        /// </summary>
        public bool IsDebugModeEnabled { get; set; }

        /// <summary>
        /// Get the <see cref="UserInterface" /> that correspond to main UI container and MainWindow DataContext
        /// </summary>
        public UserInterface UserInterface { get; private set; }

        /// <summary>
        /// Get the <see cref="TraceManager" />
        /// </summary>
        public ITracer Tracer { get; protected set; }

        /// <summary>
        /// Get or set the configuration path
        /// </summary>
        public string ConfigurationPath { get; set; } = @"Configuration\XML\Configuration.xml";

        #region SplashScreen

        /// <summary>
        /// Gets the splash screen.
        /// </summary>
        protected Window SplashScreenWindow { get; set; }

        /// <summary>
        /// Gets the splash view model.
        /// </summary>
        private SplashScreenViewModel SplashScreen { get; set; }

        /// <summary>
        /// In A²ECF system, commands CanExecute method is automatically called by timer.
        /// This timer (defined in <see cref="StartAutoRaiseCanExecute" /> method) interval can be customized with a value
        /// expressed in millisecond.
        /// <remarks>
        /// By default, value is set to 300 ms.
        /// If customized value is lower or equals to zero, timer will not started and it will be necessary to call
        /// <see cref="CommandManager.InvalidateRequerySuggested()" /> manually.
        /// </remarks>
        /// </summary>
        protected virtual int CanExecuteRefreshTimerInterval { get; } = 300;

        #endregion

        #endregion Properties

        #region Creation methods

        private void ThrowMandatoryComponentsMustNotBeNull(string componentName)
        {
            throw new InvalidOperationException($"Mandatory component {componentName} must not be null.");
        }

        /// <summary>
        /// Agileo default components creation sequence
        /// </summary>
        private void CreateMandatoryComponents()
        {
            //DEFAULT AGILEO FRAMEWORK COMPONENTS CREATION

            #region ConfigurationManager

            //Config Manager
            LogOnSplashScreen("Configuration Manager creation...", true);

            ConfigurationManager = CreateConfigurationManager();

            #endregion ConfigurationManager

            #region Localizer

            LogOnSplashScreen("Localizer creation...", true);
            // For now, LocalizationManager is a singleton
            Localizer = CreateLocalizer();

            #endregion Localizer

            #region Tracer

            //Trace Manager (required for all layers)
            LogOnSplashScreen("Tracer creation...", true);
            Tracer = CreateTracer();

            #endregion Tracer

            #region Accessrights

            //Access Manager
            LogOnSplashScreen("AccessRights creation...", true);
            AccessRights = CreateAccessRights();

            #endregion Accessrights

            // Check if all mandatory components have been instantiated
            if (ConfigurationManager == null) ThrowMandatoryComponentsMustNotBeNull(nameof(ConfigurationManager));
            if (Localizer == null) ThrowMandatoryComponentsMustNotBeNull(nameof(Localizer));
            if (Tracer == null) ThrowMandatoryComponentsMustNotBeNull(nameof(Tracer));
            if (AccessRights == null) ThrowMandatoryComponentsMustNotBeNull(nameof(AccessRights));
        }

        /// <summary>
        /// Useful to customize default <see cref="AccessRights" /> object
        /// </summary>
        /// <returns>Instance that implements <see cref="IAccessManager" /></returns>
        protected virtual AccessManager CreateAccessRights()
        {
            return new AccessManager();
        }

        /// <summary>
        /// Useful to customize default <see cref="ConfigurationManager" /> object
        /// </summary>
        /// <returns>Instance that implements <see cref="IConfigManager" /></returns>
        protected virtual IConfigManager CreateConfigurationManager()
        {
            return new ConfigManager<AgileoGuiConfiguration>(
                new XmlDataContractStoreStrategy(ConfigurationPath),
                new DataContractCloneStrategy(),
                new DataContractCompareStrategy<AgileoGuiConfiguration>());
        }

        /// <summary>
        /// Useful to customize default <see cref="Localizer" /> object
        /// </summary>
        /// <returns>Instance of type <see cref="LocalizationManager" /></returns>
        protected virtual LocalizationManager CreateLocalizer()
        {
            return LocalizationManager.Instance;
        }

        /// <summary>
        /// Useful to customize default <see cref="Tracer" /> object
        /// </summary>
        /// <returns>Instance that implements <see cref="ITracer" /></returns>
        protected virtual ITracer CreateTracer()
        {
            return TraceManager.Instance();
        }

        /// <summary>
        /// Creates the splash screen view model.
        /// </summary>
        protected virtual SplashScreenViewModel CreateSplashScreenViewModel()
        {
            return new SplashScreenViewModel(16)
            {
                EquipmentName = ApplicationName,
                SoftwareName = ApplicationVersion?.ProductName,
                SoftwareVersion = ApplicationVersion?.ProductVersion
            };
        }

        /// <summary>
        /// Creates the splash screen.
        /// </summary>
        /// <returns></returns>
        protected abstract Window CreateSplashScreenWindow();

        /// <summary>
        /// Create custom component that will be accessible from <see cref="ApplicationName" />
        /// </summary>
        protected abstract void CreateAdditionalComponents();

        /// <summary>
        /// Safely creates a window
        /// </summary>
        /// <returns>A window</returns>
        /// *
        [NotNull]
        private Window InternalCreateMainWindow()
        {
            Window mainWindow;

            try
            {
                mainWindow = CreateMainWindow();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"API {nameof(CreateMainWindow)} generate the following exception : " + e.Message);
            }

            if (mainWindow == null)
                throw new InvalidOperationException($"API {nameof(CreateMainWindow)} must return a window");
            return mainWindow;
        }

        /// <summary>
        /// Create the MainView (Window)
        /// </summary>
        protected abstract Window CreateMainWindow();

        /// <summary>
        /// Safely creates a main
        /// </summary>
        /// <returns>A <see cref="UserInterface" /> instance </returns>
        private UserInterface InternalCreateUserInterface()
        {
            UserInterface userInterface;

            try
            {
                userInterface = CreateUserInterface();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"API {nameof(CreateUserInterface)} generate the following exception : " + e.Message);
            }

            if (userInterface == null)
                throw new InvalidOperationException(
                    $"API {nameof(CreateUserInterface)} must return a {nameof(UserInterface)}");
            return userInterface;
        }

        /// <summary>
        /// Create the <see cref="UserInterface" /> property that will be the <see cref="Application.MainWindow" />
        /// dataContext.
        /// </summary>
        protected abstract UserInterface CreateUserInterface();

        #endregion Creation methods

        #region Setup methods

        /// <summary>
        /// Add additional localization resources to the localization manager.
        /// </summary>
        private static void AddLocalizationResources(Type resource)
        {
            var localizationResources = new ResourceFileProvider(resource);
            LocalizationManager.AddLocalizationProvider(localizationResources);
        }

        /// <summary>
        /// Add Resources .rsx files as LocalizationProviders into <see cref="Localizer" />
        /// </summary>
        internal static void AddLocalizationInternalResources()
        {
            // Add default Agileo GUI resources to Localizer
            AddLocalizationResources(typeof(Agileo.GUI.Properties.Resources));
            AddLocalizationResources(typeof(InfoStringResources));
            AddLocalizationResources(typeof(InfoResources));
        }

        /// <summary>
        /// Give the types of resx resources to be included in the localization manager.
        /// </summary>
        /// <returns>The list of resx types</returns>
        // [FTa] Why not scan a dedicated namespace so that the integrator does not have to implement this method.
        protected abstract IEnumerable<Type> GetAdditionalLocalizationResources();

        /// <summary>
        /// Agileo default components setup sequence
        /// </summary>
        private void SetupMandatoryComponents()
        {
            //AGILEO FRAMEWORK COMPONENTS SETUP

            #region ConfigurationManager

            LogOnSplashScreen("ConfigurationManager setup...", true);
            LoadConfiguration();

            #endregion

            #region Localizer

            //LocalizationManager (should be configured before creating HMI, otherwise there will be many trace indicating that resource string wasn't found)
            LogOnSplashScreen("Localizer setup...", true);
            AddLocalizationInternalResources();
            AddAdditionalLocalizationResources();

            #endregion Localizer

            #region Accessrights

            var config = ConfigurationManager?.Current as AgileoGuiConfiguration;
            if (config == null) throw new NullReferenceException(nameof(config));

            LogOnSplashScreen("Accessrights setup...", true);
            // CAUTION : Set UI accessRights must be done BEFORE AccessRights component setup
            AccessRights?.Setup(config.Path.AccessRightsConfigurationPath,
                config.Path.AccessRightsSchemaPath, AccessRightsLoadingMode.NavigationAndUsers);

            #endregion
        }

        /// <summary>
        /// Add additional localization resources from the integration to the localization manager.
        /// </summary>
        private void AddAdditionalLocalizationResources()
        {
            IEnumerable<Type> resources = GetAdditionalLocalizationResources();
            if (resources != null)
                foreach (var resource in resources)
                    AddLocalizationResources(resource);
        }

        /// <summary>
        /// Useful to setup additional components that have been created in <see cref="CreateAdditionalComponents" />
        /// </summary>
        protected abstract void SetupAdditionalComponents();

        private void LoadConfiguration()
        {
            // Try to load the configuration from disk
            if (ConfigurationManager != null && ConfigurationManager.Load())
            {
                ValidateLoadedConfiguration();
                Tracer.Trace(ApplicationName,
                    TraceLevelType.Info,
                    $"Configuration loaded from '{ConfigurationPath}'.",
                    new TraceParam(ConfigurationManager.Loaded.ToString()));
                return;
            }

            // Loading configuration failed, ask user what to do
            string message =
                $@"Error while loading '{ApplicationName}' configuration.
            - Click '{MessageBoxResult.Yes}' to load the default configuration (WITH save).
            - Click '{MessageBoxResult.No}' to load the default configuration (WITHOUT save).
            - Click '{MessageBoxResult.Cancel}' to shutdown the application.";

            switch (MessageBox.Show(message, "Configuration error", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Error))
            {
                case MessageBoxResult.Yes:
                    ConfigurationManager = LoadDefaultConfiguration();
                    ValidateLoadedConfiguration();
                    ConfigurationManager.Save();
                    Tracer.Trace(ApplicationName, TraceLevelType.Info,
                        $"Default configuration loaded and saved to '{ConfigurationPath}'.",
                        new TraceParam(ConfigurationManager.Loaded.ToString()));
                    break;

                case MessageBoxResult.No:
                    ConfigurationManager = LoadDefaultConfiguration();
                    ValidateLoadedConfiguration();
                    Tracer.Trace(ApplicationName, TraceLevelType.Info, "Default configuration loaded.",
                        new TraceParam(ConfigurationManager.Loaded.ToString()));
                    break;

                default:
                    throw new ApplicationException("Failed to load application's configuration");
            }
        }

        /// <summary>
        /// Load default configuration. Is used if configuration file was not found or if an error has occured while configuration loading.
        /// </summary>
        /// <returns></returns>
        protected virtual IConfigManager LoadDefaultConfiguration()
        {
            var config = new AgileoGuiConfiguration();

            var configManager = new ConfigManager<AgileoGuiConfiguration>(
                new XmlDataContractStoreStrategy(ConfigurationPath),
                new DataContractCloneStrategy(),
                new DataContractCompareStrategy<AgileoGuiConfiguration>());

            configManager.Load(config);

            return configManager;
        }

        /// <summary>
        /// Validate the loaded configuration
        /// </summary>
        private void ValidateLoadedConfiguration()
        {
            if (ConfigurationManager == null)
                throw new ApplicationException(
                    $@"Failed to validate configuration for '{ApplicationName}'.
                    Configuration manager has not been created.");

            if (!string.IsNullOrEmpty(ConfigurationManager.Errors))
                throw new ApplicationException(
                    $@"Failed to validate configuration for '{ApplicationName}'.
                    Configuration manager detected errors : {ConfigurationManager.Errors}");

            AgileoGuiConfiguration configGui = ConfigurationManager.Loaded as AgileoGuiConfiguration;
            if (configGui == null)
                throw new ApplicationException(
                    $@"Configuration for '{
                            ApplicationName
                        }' must inherit from ApplicationConfiguration to configure the GUI component.");

            string validationResult = ConfigurationManager.Loaded.ValidatedParameters();
            if (!string.IsNullOrEmpty(validationResult))
                throw new ApplicationException(
                    $@"Failed to validate configuration for '{ApplicationName}'.
                    Configuration validated with warnings: {validationResult}");
            Tracer.Trace(ApplicationName, TraceLevelType.Info, "Configuration validated without warnings.");
            TraceManager.Instance().Setup(configGui.Diagnostics.TracingConfig, configGui.Diagnostics.DataLogFilters);
        }

        #endregion Setup methods

        #region Gui Execution methods

        /// <summary>
        /// Implements the automatic updating strategy of ICommands.
        /// By default, a <see cref="DispatcherTimer" /> runs the <see cref="CommandManager.InvalidateRequerySuggested" />
        /// method every 300 milliseconds.
        /// </summary>
        private void StartAutoRaiseCanExecute()
        {
            var cmdRaiseCanExeChangeTimer =
                new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(CanExecuteRefreshTimerInterval) };
            cmdRaiseCanExeChangeTimer.Tick += delegate { CommandManager.InvalidateRequerySuggested(); };
            cmdRaiseCanExeChangeTimer.Start();
        }

        /// <summary>
        /// Run the application
        /// </summary>
        private void RunApplication()
        {
            if (MainWindow == null
                || UserInterface == null
                || !(MainWindow.DataContext is UserInterface))
                throw new InvalidOperationException(
                    $"{nameof(MainWindow)} DataContext must be an instance of {nameof(Agileo.GUI.Components.UserInterface)} and these objects cannot be null.");

            MainWindow.Closing += MainWindow_Closed;

            if (IsDebugModeEnabled)
            {
                // ReSharper disable once PossibleNullReferenceException
                MainWindow.ResizeMode = ResizeMode.CanResize;
                MainWindow.WindowState = WindowState.Maximized;
                MainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
            }

            if (SplashScreenWindow != null && SplashScreen != null) // if splash screen is started
            {
                // ReSharper disable once PossibleNullReferenceException
                SplashScreenWindow.Closed += (_, _) => MainWindow.Show();
                SplashScreenWindow.Close();
                SplashScreen = null;
            }
            else
            {
                //if no splash screen is displayed, show mainWindow directly
                // ReSharper disable once PossibleNullReferenceException
                MainWindow.Show();
            }

            // Start Commands CanExecute timer if needed
            if (CanExecuteRefreshTimerInterval > 0) StartAutoRaiseCanExecute();
            UserInterface.Accept(new CallApplyAccessRightsVisitor(AccessRights));

            UserInterface.OnShow();
        }

        /// <summary>
        /// Allow to show SplashScreen.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        private void ShowSplashScreen()
        {
            // In case the splash screen is the initiator of a shutdown
            if (SplashScreen != null)
            {
                SplashScreen.HideException();
                return;
            }

            SplashScreen = CreateSplashScreenViewModel();
            if (SplashScreen == null) return;

            SplashScreenWindow = CreateSplashScreenWindow();
            if (SplashScreenWindow == null) throw new NullReferenceException(nameof(SplashScreenWindow));

            SplashScreenWindow.DataContext = SplashScreen;
            SplashScreenWindow.Show();
        }

        /// <summary>
        /// Display message on the Splash screen
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="increaseProgress">if set to <c>true</c> [increase progress].</param>
        protected void LogOnSplashScreen(string message, bool increaseProgress)
        {
            SplashScreen?.Increment(message, increaseProgress ? 1 : 0);
        }

        #endregion Gui Execution methods

        #region Dispose methods

        /// <summary>
        /// Method that will be automatically called by A²ECF SDK at application shutdown.
        /// You can add your component(s) dispose like : MyComponent.Dispose();
        /// Caution, some exit actions requires to be done from UI thread
        /// </summary>
        protected abstract void DisposeAdditionalComponents();

        /// <summary>
        /// Dispose mandatory components except TraceManager that must be dispose just before application shutdown
        /// </summary>
        protected void DisposeMandatoryComponents()
        {
            //DEFAULT AGILEO FRAMEWORK COMPONENTS DISPOSE

            #region Accessrights

            //Access Manager
            LogOnSplashScreen("Disposing AccessRights...", true);
            //AccessRights?.Dispose();

            #endregion Accessrights

            #region Localizer

            LogOnSplashScreen("Disposing Localizer...", true);
            // For now, LocalizationManager is a singleton
            //Localizer?.Dispose();

            #endregion Localizer

            #region ConfigurationManager

            //Config Manager
            LogOnSplashScreen("Disposing Configuration Manager...", true);
            //ConfigurationManager?.Dispose();

            #endregion ConfigurationManager
        }

        /// <summary>
        /// Dispose <see cref="UserInterface"/> instance before application shutdown.
        /// Must be called on the UI thread to affect the graphical elements
        /// </summary>
        protected void DisposeUserInterface()
        {
            // Use visitor to visit UI tree structure and call Dispose() method on each UI component
            UserInterface.Accept(new CallDisposeVisitor(msg => SplashScreen?.Display(msg)));
        }

        #endregion Dispose methods

        #region Startup/Shutdown methods

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;

            // Gets the process executable in the default application domain
            var entryAssembly = Assembly.GetEntryAssembly();

            // Gets the process file's version info
            FileVersionInfo fileVersionInfo = null;
            if (entryAssembly != null && File.Exists(entryAssembly.Location))
            {
                fileVersionInfo = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
            }

            ApplicationVersion = fileVersionInfo;

            _mutex = new Mutex(true, ApplicationName ?? fileVersionInfo?.ProductName,
                out createdNew); // private field affectation is needed to enable mutex behavior
            if (!createdNew)
            {
                MessageBox.Show($"{Assembly.GetExecutingAssembly().GetName().Name} already running", "Caution",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Current.Shutdown();
                return;
            }

            // Application is authorized to start
            // Register to exceptions for custom handling, and call base implementation
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            base.OnStartup(e);

            // Call Application.Current.Shutdown manually is needed to do some custom actions between MainWindow Closed event and application OnExit event
            // see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.shutdownmode?redirectedfrom=MSDN&view=netframework-4.7.2#System_Windows_Application_ShutdownMode for more details
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Set culture to "en" by default
            var enCulture = new CultureInfo("en");
            Thread.CurrentThread.CurrentUICulture = enCulture;
            CultureInfo.DefaultThreadCurrentUICulture = enCulture;

            // Execute startup actions
            StartupActions();
        }

        /// <summary>
        /// Creates and setup all components in proper order.
        /// </summary>
        private void StartupActions()
        {
            // [TLa] Starts a new asynchronous task to release the UI thread (Current.Dispatcher.BeginInvoke blocks the UI thread!)
            // This allows the UI thread to display and update the SplashScreen independently.
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // INITIAL STEP - Start the splash screen and show it on the screen
                    Current.Dispatcher.Invoke(() =>
                    {
                        ShowSplashScreen();
                        LogOnSplashScreen("Application creation...", true);
                    });

                    // STEP 1 - Create Agileo components
                    LogOnSplashScreen("Mandatory components creation...", true);
                    CreateMandatoryComponents();

                    // STEP 2 - Setup Agileo components
                    LogOnSplashScreen("Mandatory Components Setup...", true);
                    SetupMandatoryComponents();

                    // STEP 3 - Create integration components
                    LogOnSplashScreen("Additional component(s) creation...", true);
                    CreateAdditionalComponents();

                    // STEP 4 - Setup integration components (Optional)
                    LogOnSplashScreen("Additional Components Setup...", true);
                    SetupAdditionalComponents();

                    Current.Dispatcher.Invoke(() =>
                    {
                        // STEP 5 - Create application Main UI container
                        LogOnSplashScreen("MainWindow creation...", true);
                        UserInterface = InternalCreateUserInterface();
                        MainWindow = InternalCreateMainWindow();
                    });

                    // STEP 6 - Create UI elements (mainWindow, panels, etc..)
                    // [TLa] Returns in the UI thread to affect the graphical elements
                    Current.Dispatcher.Invoke(() =>
                    {
                        LogOnSplashScreen("HMI creation...", true);
                        UserInterface.CreateUiComponents(msg => LogOnSplashScreen(msg, false));

                        //STEP 7 - Setup application Main UI container
                        // ReSharper disable once PossibleNullReferenceException
                        MainWindow.DataContext = UserInterface;
                        if (string.IsNullOrEmpty(ApplicationName))
                            throw new InvalidOperationException(
                                $"The '{nameof(ApplicationName)}' property cannot be null or empty. Please, specify an application name in your {nameof(AgilControllerApplication)} inherited class.");

                        // ReSharper disable once PossibleNullReferenceException
                        MainWindow.Title = ApplicationName;
                    });

                    // STEP 8 - Setup UI elements
                    // [TLa] Returns in the UI thread to affect the graphical elements
                    Current.Dispatcher.Invoke(() =>
                    {
                        LogOnSplashScreen("HMI Setup...", true);
                        UserInterface.OnSetup();

                        // FINAL STEP - Run application
                        RunApplication();
                    });
                }
                catch (Exception ex)
                {
                    ExitOnException("Application exception", ex);
                }
            });
        }

        private void MainWindow_Closed(object sender, CancelEventArgs e)
        {
            // ShutDown is managed by main
            if (UserInterface is { ShutDownRequested: false })
            {
                e.Cancel = true;
                UserInterface.Close();
                return;
            }

            // Execute shutdown actions asynchronously
            ShutdownActions();
        }

        /// <summary>
        /// Disposes gracefully all created components (Last Created First Disposed),
        /// and ensures that <see cref="Application.Shutdown()" /> is called.
        /// </summary>
        public virtual void ShutdownActions()
        {
            // [TLa] Starts a new asynchronous task to release the UI thread (Current.Dispatcher.BeginInvoke blocks the UI thread!)
            // This allows the UI thread to display and update the SplashScreen independently.
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // [TLa] Returns in the UI thread to affect the graphical elements
                    Current.Dispatcher.Invoke(() =>
                    {
                        // INITIAL STEP - Start the splash screen and show it on the screen
                        ShowSplashScreen();
                        SplashScreen.Increment("Application shutdown...");
                        Tracer?.Trace($"{ApplicationName}", TraceLevelType.Info,
                            $"{ApplicationName} v{ApplicationVersion.ProductVersion} shut down");

                        // STEP 1 - Dispose UI Elements
                        SplashScreen.Display("Disposing HMI...");
                        DisposeUserInterface();
                    });

                    // STEP 2 - Dispose Agileo Components
                    SplashScreen.Display("Disposing mandatory components...");
                    DisposeMandatoryComponents();

                    // STEP 3 - Dispose Agileo Components
                    SplashScreen.Display("Disposing additional component(s)...");
                    DisposeAdditionalComponents();

                    // FINAL STEP - Dispose Trace Manager (to add a shutdown trace and flush trace queue on disk)
                    SplashScreen.Display("Disposing trace manager...");
                    DisposeTraceManager();
                }
                catch (Exception ex)
                {
                    ExitOnException("Error during application's shutdown.", ex);
                }
                finally
                {
                    Current.Dispatcher.Invoke(() =>
                    {
                        // Once all custom code is done, actually shutdown the application
                        // Environment.Exit needed to kill all background threads
                        Environment.Exit(0);
                    });
                }
            });
        }

        #endregion Startup/Shutdown methods

        #region Exception management

        /// <summary>
        /// Event catch when application dispatcher (UI thread) exception was thrown.
        /// </summary>
        /// <param name="sender"> The sender of this event. </param>
        /// <param name="e"> <see cref="DispatcherUnhandledExceptionEventArgs"/></param>
        protected virtual void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // In all cases, trace exception message
            Tracer?.Trace(ApplicationName, TraceLevelType.Fatal,
                $"Application Dispatcher Unhandled Exception. Message={e.Exception.Message}",
                new TraceParam(e.Exception.ToString()));

            // if exception occurs DURING ui creation and BEFORE its display, process will running in background and the MainWindow will not shown.
            if (MainWindow == null || !MainWindow.IsLoaded)
            {
                MessageBox.Show(e.Exception.Message);
                Environment.Exit(1);
            }
            else
            {
                UserInterface?.Messages.Show(new UserMessage(MessageLevel.Error, nameof(InfoResources.I_APPLICATION_UNSTABLE_TITLE_MSG)));
                UserInterface?.Popups.Show(
                    new Popup("Error", nameof(InfoResources.I_APPLICATION_UNSTABLE_POPUP_MSG))
                    {
                        Commands =
                        {
                            new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_OK))
                        }
                    });

                // If the source comes from PresentationFramework or PresentationCore the event cannot be Handled under penalty of creating a loop
                if (e.Exception.Source == "PresentationFramework" || e.Exception.Source == "PresentationCore")
                {
                    ExitOnException("Application Dispatcher exception", e.Exception);
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Event catch when application thread exception was thrown.
        /// </summary>
        /// <param name="sender"> The sender of this event. </param>
        /// <param name="e"> <see cref="UnhandledExceptionEventArgs"/></param>
        protected virtual void AppDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception exception = e.ExceptionObject as Exception;

                // In all cases, trace exception message
                Tracer?.Trace(ApplicationName, TraceLevelType.Fatal,
                    $"Application Dispatcher Unhandled Exception. Message={exception?.Message}",
                    new TraceParam(exception?.ToString()));

                // Show exception
                GuiInstance.Dispatcher.Invoke(() =>
                {

                    UserInterface?.Messages.Show(new UserMessage(MessageLevel.Error, nameof(InfoResources.I_APPLICATION_UNSTABLE_TITLE_MSG)));
                    UserInterface?.Popups.Show(
                        new Popup("Error", nameof(InfoResources.I_APPLICATION_UNSTABLE_POPUP_MSG))
                        {
                            Commands =
                            {
                                new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_OK))
                            }
                        });
                });
            }
            catch (Exception ex)
            {
                ExitOnException("Application Thread exception", ex);
            }
        }

        /// <summary>
        /// Application shutdown caused by the raising of an exception which cannot be managed
        /// </summary>
        /// <param name="mainCause">Main cause</param>
        /// <param name="ex">Exception</param>
        protected virtual void ExitOnException(string mainCause, Exception ex)
        {
            var exceptionDetails = (ex?.ToString() ?? "Exception caught. No Details on the exception are available.")
                .RemoveEmptyLines();
            var isTraceManagerSetupDone = (Tracer as TraceManager)?.IsSetupDone ?? false;

            Tracer?.Trace(ApplicationName, TraceLevelType.Fatal,
                $"Application Dispatcher Unhandled Exception. Message={ex?.Message}",
                new TraceParam(ex?.ToString()));

            if (SplashScreen != null)
            {
                SplashScreen.DisplayException(mainCause, exceptionDetails);
                return;
            }

            try
            {
                // Show exception
                GuiInstance.Dispatcher.Invoke(() => MessageBox.Show("Application will be shutdown.\r\n\r\nReason:\r\n" + exceptionDetails, mainCause,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation));

                // Dispose UI
                UserInterface.Accept(new CallDisposeVisitor(msg =>
                    SplashScreen
                        ?.Display(msg))); // Use visitor to visit UI tree structure and call OnSetup() method on each UI component

                // Dispose mandatory components
                DisposeMandatoryComponents();

                // Dispose additional components
                DisposeAdditionalComponents();

                // Dispose TraceManager
                if (isTraceManagerSetupDone)
                {
                    DisposeTraceManager();
                }
                else
                {
                    ErrorTracer.ErrorMessagePath = @"..\OutputFiles\Logs\EquipmentController";
                    ErrorTracer.Error(mainCause + Environment.NewLine + exceptionDetails);
                }
            }
            catch (Exception e)
            {
                ErrorTracer.ErrorMessagePath = @"..\OutputFiles\Logs\EquipmentController";
                ErrorTracer.Error(mainCause + Environment.NewLine + exceptionDetails);
            }
            finally
            {
                // Shutdown application
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Adds a final trace indicating software shutdown, then dispose the TraceManager (flush all remaining traces on
        /// disk).
        /// </summary>
        protected void DisposeTraceManager()
        {
            // Trace application shut down. "Dispose()" will flush the queue on disk.
            // We have to create a dedicated thread because "Dispose" hangs and block application shut down.
            // After the Thread.Sleep, application will be terminated (and "Dispose" thread killed)
            Thread.Sleep(5000);
            (Tracer as TraceManager)?.Trace(ApplicationName, TraceLevelType.Info,
                $"{ApplicationName} v{ApplicationVersion.ProductVersion} shut down.");
            (Tracer as TraceManager)?.Dispose();
        }

        #endregion Exception management
    }
}
