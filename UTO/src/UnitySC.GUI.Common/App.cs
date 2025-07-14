using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

using Agileo.AlarmModeling.Configuration;
using Agileo.Alarms;
using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.Common.Tracing.Listeners;
using Agileo.DataMonitoring;
using Agileo.EquipmentModeling;
using Agileo.GUI.Helpers;
using Agileo.GUI.Services.UserMessages;
using Agileo.Recipes.Management.StorageFormats;
using Agileo.Recipes.Management.StorageStrategies;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.Configuration.DataCollection;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.ProcessExecution;
using UnitySC.GUI.Common.Vendor.Recipes;
using UnitySC.GUI.Common.Vendor.Scenarios;
using UnitySC.GUI.Common.Vendor.UIComponents.Components;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.GUI.Common.Configuration;
using UnitySC.GUI.Common.Listeners;
using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Views.Splashscreen;

namespace UnitySC.GUI.Common
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public abstract class App : AgilControllerApplication
    {
        #region Properties

        public UnityScConfiguration Config => GetConfig();

        /// <summary>
        /// Gets the Alarm Center System
        /// </summary>
        public AlarmCenter AlarmCenter { get; private set; }

        /// <summary>
        /// Gets the equipment manager.
        /// </summary>
        public EquipmentManager EquipmentManager { get; private set; }

        /// <summary>
        /// Gets the Recipe Manager
        /// </summary>
        public RecipeManager RecipeManager { get; private set; }

        /// <summary>
        /// Gets the Scenario Manager
        /// </summary>
        public ScenarioManager ScenarioManager { get; private set; }

        /// <summary>
        /// The librarian managing all <see cref="DataCollectionPlan"/>.
        /// </summary>
        public DataCollectionPlanLibrarian DataCollectionPlanLibrarian { get; private set; }

        public AppProgramProcessor SequenceProgramProcessor { get; private set; }

        /// <summary>
        /// Gets the user interface manager
        /// </summary>
        public UserInterfaceManager UserInterfaceManager { get; private set; }

        /// <summary>
        /// Gets the current instance of <see cref="App"/>.
        /// </summary>
        public static App Instance => IsInDesignMode ? null : (App)Application.Current;

        /// <inheritdoc />
        public override string ApplicationName => GetApplicationName();

        public ILogger Logger => GetApplicationLogger();

        public override ILogger UserInterfaceLogger => GetLogger(nameof(Agileo.GUI.Components.UserInterface));

        public ILogger GetLogger(string name) => Agileo.Common.Logging.Logger.GetLogger(name, Tracer);

        public UserMessageDisplayer MainUserMessageDisplayer { get; } = new();

        public static bool IsInDesignMode { get; private set; } = true;

        #endregion Properties

        protected override void OnStartup(StartupEventArgs e)
        {
            IsInDesignMode = false;
            base.OnStartup(e);
        }

        /// <inheritdoc />
        protected override Window CreateSplashScreenWindow()
        {
            return new SplashScreenWindow();
        }

        /// <inheritdoc />
        protected override void CreateAdditionalComponents()
        {
            ///////////////////////////////////////////////////////////////////////
            //
            // All creations are protected and errors must be set through Exceptions
            //
            ///////////////////////////////////////////////////////////////////////

            #region Tracer
            
            ((TraceStorage)TraceManager.Instance().Listeners[nameof(TraceStorage)]).IsArchiveDeleteProcessEnable = true;

            var traceStorage = (TraceStorage)TraceManager.Instance().Listeners["TraceStorage"];
            TraceManager.Instance().RemoveListener(traceStorage);
            TraceManager.Instance().AddListener(new UnityTraceStorage(traceStorage));
            
            var bufferListener =
                (BufferListener)TraceManager.Instance().Listeners["BufferListener"];
            TraceManager.Instance().RemoveListener(bufferListener);
            TraceManager.Instance().AddListener(new UnityBufferListener(bufferListener));

            #endregion Tracer

            var currentExecutingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // User interface management
            // The initialization of the UserInterfaceManager must be carried out before the creation
            // of the additional components because it impacts SplashScreen's graphic rendering.
            UserInterfaceManager = new UserInterfaceManager();
            try
            {
                UserInterfaceManager.Initialize(Instance.Config.UserInterfaceConfiguration);
            }
            catch (Exception e)
            {
                Tracer.Trace(TraceLevelType.Warning, "Unable to apply the configured graphic theme.", new TraceParam(e.ToString()));
            }

            // Alarm Management
            LogOnSplashScreen("Alarm Center creation ...", true);
            AlarmCenterConfiguration alarmConfiguration = Config.Alarms;
            AlarmCenter = new AlarmCenter(alarmConfiguration);

            // Equipment
            LogOnSplashScreen("Equipment creation ...", true);
            EquipmentManager = CreateEquipmentManager(Config.EquipmentConfig, currentExecutingPath);

            // Recipe Manager
            var storageStrategy =
                new OnDiskRecipeStorageStrategy<ProcessRecipe>((Config?.RecipeConfiguration.StorageFormat) ?? StorageFormat.XML)
                {
                    StorageFolderPath = Config?.RecipeConfiguration.Path,
                    FileExtension = Config?.RecipeConfiguration.FileExtension
                };

            RecipeManager = new RecipeManager(storageStrategy)
            {
                RecipeGroups = Config?.RecipeConfiguration.Groups // XML format and OnDisk storage by default
            };

            var recipeManagerSetupErrors = RecipeManager.Setup(Tracer, AccessRights).ToList();
            if (recipeManagerSetupErrors.Count > 0)
            {
                // Do not throw an exception because if a recipe is incoherent, all other recipes can be loaded instead. The error is not a blocking error.
                // Only need to inform operator...
                Logger.Error(
                    string.Join(Environment.NewLine, recipeManagerSetupErrors).AsAttachment(),
                    "Recipe Manager Setup failed.");
            }

            // Scenario Manager
            // XML format and OnDisk storage by default
            var scenarioStorageStrategy =
                new OnDiskRecipeStorageStrategy<SequenceRecipe>((Config?.ScenarioConfiguration.StorageFormat) ?? StorageFormat.XML)
                {
                    StorageFolderPath = Config?.ScenarioConfiguration.Path,
                    FileExtension = Config?.ScenarioConfiguration.FileExtension
                };

            ScenarioManager = new ScenarioManager(scenarioStorageStrategy)
            {
                RecipeGroups = Config?.ScenarioConfiguration?.Groups
            };

            var scenarioManagerSetupErrors = ScenarioManager.Setup(Tracer, AccessRights, nameof(ScenarioManager)).ToList();
            if (scenarioManagerSetupErrors.Count > 0)
            {
                // Do not throw an exception because if a recipe is incoherent, all other recipes can be loaded instead. The error is not a blocking error.
                // Only need to inform operator...
                Logger.Error(
                    string.Join(Environment.NewLine, scenarioManagerSetupErrors).AsAttachment(),
                    "Scenario Manager Setup failed.");
            }

            // Sequence Program Processor
            SequenceProgramProcessor = new AppProgramProcessor(Tracer);

            if (ScenarioManager == null)
            {
                throw new InvalidOperationException($"Initialized instance of {nameof(ScenarioManager)} is 'null'");
            }

            if (AlarmCenter == null)
            {
                throw new InvalidOperationException($"Initialized instance of {nameof(AlarmCenter)} is 'null'");
            }

            if (EquipmentManager == null)
            {
                throw new InvalidOperationException($"Initialized instance of {nameof(EquipmentManager)} is 'null'");
            }

            if (EquipmentManager.Equipment == null)
            {
                throw new InvalidOperationException(
                    $"Initialized instance of {nameof(EquipmentManager.Equipment)} is 'null'");
            }

            CreateDataCollectionPlans();
        }

        private void CreateDataCollectionPlans()
        {
            DataCollectionPlanLibrarian = new DataCollectionPlanLibrarian();

            DispatcherHelper.DoInUiThread(
                () =>
                {
                    // Color generator component
                    var redColor = Brushes.SeverityErrorBrush;
                    var greenColor = Brushes.SeveritySuccessBrush;
                    var blueColor = Brushes.SeverityInformationBrush;
                    ColorGenerator.Initialize(redColor.Color, greenColor.Color, blueColor.Color);
                });

            DataCollectionPlanLibrarian.Setup<DCPConfiguration>(Tracer, ((ApplicationConfiguration)ConfigurationManager.Current).ApplicationPath.DataMonitoringPath);
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
            EquipmentManager?.Dispose();
        }

        public override void ShutdownActions()
        {
            var loadPorts = EquipmentManager.Equipment.AllOfType<LoadPort>();
            foreach (var loadPort in loadPorts)
            {
                loadPort.DisableE84Async();
            }
            base.ShutdownActions();
        }

        #region Abstract Methods

        protected abstract UnityScConfiguration GetConfig();

        protected abstract ILogger GetApplicationLogger();

        protected abstract string GetApplicationName();

        protected abstract EquipmentManager CreateEquipmentManager(
            EquipmentConfiguration equipmentConfig,
            string currentExecutionPath);

        #endregion
    }
}
