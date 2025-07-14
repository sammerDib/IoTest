using System.ComponentModel;
using System.Linq;
using System.Windows;

using Agileo.AlarmModeling;
using Agileo.Common.Access;
using Agileo.Common.Access.Users;
using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Components.Tools;
using Agileo.GUI.Configuration;
using Agileo.GUI.Services.Saliences;
using Agileo.GUI.Services.UserMessages;
using Agileo.MessageDataBus;
using Agileo.SemiDefinitions;

using UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Maintenance.Communication;
using UnitySC.EFEM.Rorze.GUI.Views.Tools.StatusComparer;
using UnitySC.Equipment.Abstractions.Devices.Controller;
using UnitySC.Equipment.Abstractions.Devices.LightTower;
using UnitySC.Equipment.Abstractions.Devices.LightTower.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.UIComponents.Popup;
using UnitySC.GUI.Common.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.Recipes.Resources;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.TagsSpy;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Alarms;
using UnitySC.GUI.Common.Vendor.Views.Panels.Alarms.Analysis;
using UnitySC.GUI.Common.Vendor.Views.Panels.Diagnostic.TraceViewer;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.About;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.BusinessPanelCommand;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Controls;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTable;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTree;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.MessageArea;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.NavigateToPanelTester;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Popup;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Resources.IconsViewer;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Salience;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Toolbox;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.FileViewer;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.SystemInformation;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.WebVisu;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.DataAnalysis;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Library;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.RealTimeAnalysis;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Library;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AlarmCenter;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Appearance;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.DataCollection;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Diagnostic;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.FileConfiguration;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.GlobalSettings;
using UnitySC.GUI.Common.Vendor.Views.TitlePanel;
using UnitySC.GUI.Common.Vendor.Views.Tools.ThemeBuilder;
using UnitySC.GUI.Common.Views.Panels.Sample;
using UnitySC.GUI.Common.Views.Panels.Setup.EquipmentSettings;
using UnitySC.Shared.Tools;
using UnitySC.UTO.Controller.Remote.Observers;
using UnitySC.UTO.Controller.Views.Panels.DataFlow;
using UnitySC.UTO.Controller.Views.Panels.EquipmentHandling.Equipment;
using UnitySC.UTO.Controller.Views.Panels.Gem;
using UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary;
using UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary.Alarms;
using UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary.Reports;
using UnitySC.UTO.Controller.Views.Panels.Gem.EPT;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.Carriers;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts;
using UnitySC.UTO.Controller.Views.Panels.Gem.Jobs;
using UnitySC.UTO.Controller.Views.Panels.Gem.SequenceLog;
using UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking;
using UnitySC.UTO.Controller.Views.Panels.Integration;
using UnitySC.UTO.Controller.Views.Panels.Integration.Dialog;
using UnitySC.UTO.Controller.Views.Panels.Maintenance.Counters;
using UnitySC.UTO.Controller.Views.Panels.Maintenance.Scenario.Runner;
using UnitySC.UTO.Controller.Views.Panels.Maintenance.Wafer;
using UnitySC.UTO.Controller.Views.Panels.Production.Equipment;
using UnitySC.UTO.Controller.Views.Panels.Production.RecipeRun;
using UnitySC.UTO.Controller.Views.Panels.Results;
using UnitySC.UTO.Controller.Views.Panels.Setup.AccessRights;
using UnitySC.UTO.Controller.Views.Panels.Setup.DataFlow;
using UnitySC.UTO.Controller.Views.Panels.Setup.HostCommunication;
using UnitySC.UTO.Controller.Views.Panels.Setup.OcrProfile;
using UnitySC.UTO.Controller.Views.Panels.Setup.UtoAppPathConfig;
using UnitySC.UTO.Controller.Views.TitlePanel;
using UnitySC.UTO.Controller.Views.Tools.Notifier;
using UnitySC.UTO.Controller.Views.Tools.TerminalMessages;

namespace UnitySC.UTO.Controller.Views
{
    public class MainWindowViewModel : UserInterface
    {
        private Menu _setupNavigationMenu;
        private TagsSpyViewModel _tagSpy;
        private ApplicationCommand _openTagsSpyCommand;
        private ServiceModePanel _serviceModePanel;

        public DialogOwner DialogOwner => ClassLocator.Default.GetInstance<DialogOwner>();

        public MainWindowViewModel()
            : base(new AccessManager(), new LocalizationManager(), Logger.GetLogger("DesignTime"))
        {
            //Localized Resources for Validation Rules
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(RecipeValidationResources)));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(E84Resources)));
        }

        public MainWindowViewModel(
            AccessManager accessManager,
            LocalizationManager localizationManager,
            ILogger userInterfaceLogger)
            : base(accessManager, localizationManager, userInterfaceLogger)
        {
            DialogOwner.Register();
        }

        public override AgileoGuiConfiguration GetCurrentConfiguration()
        {
            return GUI.Common.App.Instance.Config;
        }

        /// <inheritdoc />
        protected override void CreatePanels()
        {
            UserMessage.CloseCommandIcon = PathIcon.Close;

            _openTagsSpyCommand = new ApplicationCommand(
                nameof(L10N.TAGS_SPY),
                new DelegateCommand(OpenTagsSpyWindow, CanOpenTagsSpyWindow),
                PathIcon.Spy);
            ApplicationCommands.Add(_openTagsSpyCommand);

            // Dissociate each Navigation tree definition within private methods to clarify how the GUI tree is built.
            CreateJobPanels();
            CreateMaintenancePanels();
            CreateProcessPanels();
            CreateResultsPanels();
            CreateLogPanels();
            CreateGemPanels();
            CreateConfigurationPanels();
            CreateAlarmPanels();
            CreateHelpPanel();

            if (GUI.Common.App.Instance.Config.GlobalSettings.IsDeveloperDebugModeEnabled)
            {
                var statusComparer = new StatusComparer(nameof(L10N.TOOL_STATUS_COMPARER));
                ToolManager.Tools.Add(statusComparer);

                // Add status comparer tool in service mode and log panels.
                foreach (var businessPanel in BusinessPanels.Where(
                             panel => panel.RelativeId.Equals(nameof(L10N.BP_SERVICE_MODE))
                                      || panel.RelativeId.Equals(nameof(L10N.ROOT_DIAGNOSTICS))
                                      || panel.RelativeId.Equals(
                                          nameof(L10N.BP_COMMUNICATION_VISUALIZER))))
                {
                    businessPanel.ToolElements.Add(new ToolReference(statusComparer));
                }
            }

            if (App.ControllerInstance.IsGemEnabled)
            {
                var terminalMessagesTool = new TerminalMessagesTool();
                ToolManager.Tools.Add(terminalMessagesTool);
                foreach (var businessPanel in BusinessPanels)
                {
                    businessPanel.ToolElements.Add(new ToolReference(terminalMessagesTool));
                }
            }

            foreach (var businessPanel in BusinessPanels)
            {
                businessPanel.Saliences.PropertyChanged += OnBusinessPanelSaliencesChanged;
            }

            #region Theme Editor Tools

            if (GUI.Common.App.Instance.Config?.GlobalSettings?.IsColorizationToolboxVisible
                ?? false)
            {
                var rapidColorTool = new ThemeBuilderTool(
                    nameof(L10N.TOOL_RAPIDCOLORS),
                    PathIcon.RapidColor);
                var rapidColorToolSaveCommand = new ToolCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_SETUP_HOST_SAVEAS),
                    rapidColorTool.SaveCommand,
                    PathIcon.Save);
                rapidColorTool.Commands.Add(rapidColorToolSaveCommand);

                ToolManager.Tools.Add(rapidColorTool);

                // Add tool in all configuration panels
                foreach (var toolElements in BusinessPanels.Select(bp => bp.ToolElements))
                {
                    toolElements.Add(new ToolReference(rapidColorTool));
                }
            }

            #endregion Theme Editor Tools

            #region Unity Notifier Tool

            var notifierTool = new NotifierTool(nameof(L10N.TOOL_NOTIFIER));
            ToolManager.Tools.Add(notifierTool);
            foreach (var businessPanel in BusinessPanels)
            {
                businessPanel.ToolElements.Add(new ToolReference(notifierTool));
            }

            #endregion Unity Notifier Tool

            if (IsInDesignMode)
            {
                Navigation.TryNavigateTo(Navigation.RootMenu.Items.First() as BusinessPanel);
            }
        }

        private void CreateJobPanels()
        {
            var productionModeMenu = new Menu(nameof(L10N.ROOT_MAIN), PathIcon.Home);
            Navigation.RootMenu.Items.Add(productionModeMenu);

            var processPanel = new EquipmentPanel(
                nameof(L10N.ROOT_MAIN_EQUIPMENT),
                PathIcon.Production);

            if (App.ControllerInstance?.ControllerEquipmentManager.LoadPorts != null
                && App.ControllerInstance.ControllerEquipmentManager.LoadPorts.Count > 2)
            {
                ToolManager.Tools.Add(processPanel.JobQueueViewModel);
                processPanel.ToolElements.Insert(
                    0,
                    new ToolReference(processPanel.JobQueueViewModel));
            }

            productionModeMenu.Items.Add(processPanel);

            foreach (var driveableProcessModule in App.ControllerInstance.ControllerEquipmentManager
                         .ProcessModules)
            {
                if (driveableProcessModule.Value is UnityProcessModule)
                {
                    var recipeRuneLive = new RecipeRunLiveManagementPanel(
                        $"ProcessModule{driveableProcessModule.Key}",
                        driveableProcessModule.Value,
                        PathIcon.Show);

                    productionModeMenu.Items.Add(recipeRuneLive);
                }
            }
        }

        private void CreateMaintenancePanels()
        {
            var enterMaintenanceModeCommand = new ApplicationCommand(
                nameof(L10N.BP_ENTER_MAINTENANCE_MODE),
                new DelegateCommand(
                    EnterMaintenanceModeCommandExecute,
                    EnterMaintenanceModeCommandCanExecute),
                PathIcon.Maintenance);

            ApplicationCommands.Add(enterMaintenanceModeCommand);

            // MAINTENANCE Menu
            var systemNavigationMenu =
                new Menu(nameof(L10N.ROOT_MAINTENANCE), PathIcon.Maintenance);
            Navigation.RootMenu.Items.Add(systemNavigationMenu);

            //Manual substrate handling
            BusinessPanel equipmentHandling = new EquipmentHandlingPanelViewModel(
                nameof(L10N.BP_EQUIPMENT_HANDLING),
                PathIcon.Manual);
            equipmentHandling.Commands.Insert(
                0,
                new ApplicationCommandReference(enterMaintenanceModeCommand));
            systemNavigationMenu.Items.Add(equipmentHandling);

            // Panels
            _serviceModePanel = new ServiceModePanel(
                GUI.Common.App.Instance.EquipmentManager.Equipment,
                nameof(L10N.BP_SERVICE_MODE),
                PathIcon.Maintenance);
            _serviceModePanel.Commands.Insert(
                0,
                new ApplicationCommandReference(enterMaintenanceModeCommand));

            systemNavigationMenu.Items.Add(_serviceModePanel);

            _serviceModePanel.AreCommandsEnabled =
                App.ControllerInstance.ControllerEquipmentManager.Controller.State
                == OperatingModes.Maintenance;

            var communicationVisualizer = new CommunicationVisualizerPanel(
                nameof(L10N.BP_COMMUNICATION_VISUALIZER),
                PathIcon.Data);
            systemNavigationMenu.Items.Add(communicationVisualizer);

            // Add scenario panel
            var scenarioManager = new ScenarioLibraryPanel(
                nameof(L10N.BP_SCENARIO_LIBRARY),
                GUI.Common.App.Instance.ScenarioManager,
                PathIcon.RecipeManager);
            systemNavigationMenu.Items.Add(scenarioManager);

            var scenarioRunner = new UnityScenarioRunnerPanel(
                nameof(L10N.BP_SCENARIO_SEQUENCER),
                GUI.Common.App.Instance.ScenarioManager,
                PathIcon.PlayDocument);
            scenarioRunner.Commands.Insert(
                0,
                new ApplicationCommandReference(enterMaintenanceModeCommand));
            systemNavigationMenu.Items.Add(scenarioRunner);

            //TODO Reactivate when we handle 64 bits for web component
            //var webVisu = new WebVisuPanel("Network Camera", PathIcon.Web);
            //webVisu.LinkList.Add(new WebLink("http://20.20.249.52/view/viewer_index.shtml?id=42", "Network camera"));
            //systemNavigationMenu.Items.Add(webVisu);

            var wafersPanel = new WaferPanel(nameof(L10N.BP_WAFERS), CustomPathIcon.Wafer);
            systemNavigationMenu.Items.Add(wafersPanel);

            var countersPanel = new CountersPanel(nameof(L10N.BP_COUNTERS), CustomPathIcon.Reservation);
            systemNavigationMenu.Items.Add(countersPanel);

            // Add data collection library panel
            var dcpMenu = new Menu(nameof(DataCollectionLibraryResources.DATA_MONITORING));
            var dcpLibrarian = GUI.Common.App.Instance.DataCollectionPlanLibrarian;

            dcpMenu.Items.Add(
                new DataCollectionLibraryPanel(
                    GUI.Common.App.Instance.EquipmentManager.Equipment,
                    nameof(DataCollectionLibraryResources.PERMANENT_COLLECTION_PLANS),
                    dcpLibrarian,
                    false,
                    PathIcon.Chart));

            dcpMenu.Items.Add(
                new DataCollectionLibraryPanel(
                    GUI.Common.App.Instance.EquipmentManager.Equipment,
                    nameof(DataCollectionLibraryResources.TEMPORARY_COLLECTION_PLANS),
                    dcpLibrarian,
                    true,
                    PathIcon.TemporaryChart));

            // Add data collection real time monitoring panel
            dcpMenu.Items.Add(
                new RealTimeAnalysisPanel(
                    nameof(RealTimeAnalysisPanelResources.REAL_TIME_ANALYSIS),
                    dcpLibrarian,
                    PathIcon.RealTimeAnalysis));

            // Add data collection history panel
            dcpMenu.Items.Add(
                new DataAnalysisPanel(
                    nameof(DataAnalysisPanelResources.DATA_ANALYSIS),
                    dcpLibrarian,
                    PathIcon.DataAnalysis));

            systemNavigationMenu.Items.Add(dcpMenu);

            AddTagsSpyCommand(systemNavigationMenu);
        }

        private void CreateProcessPanels()
        {
            if (App.ControllerInstance.ControllerEquipmentManager.Equipment
                .AllDevices<DataFlowManager>()
                .Any())
            {
                Navigation.RootMenu.Items.Add(
                    new DataFlowPanel(nameof(L10N.BP_PROCESS), UnityScIcons.DataFlow));
            }
        }

        private void CreateResultsPanels()
        {
            if (App.ControllerInstance.ControllerEquipmentManager.Equipment
                .AllDevices<DataFlowManager>()
                .Any())
            {
                Navigation.RootMenu.Items.Add(
                    new ResultsPanel(nameof(L10N.BP_RESULTS), UnityScIcons.WaferResult));
            }
        }

        private void CreateLogPanels()
        {
            BusinessPanel traceViewer = new TraceViewerPanel(
                nameof(L10N.ROOT_DIAGNOSTICS),
                PathIcon.Datalog);
            Navigation.RootMenu.Items.Add(traceViewer);
        }

        private void CreateGemPanels()
        {
            if (!App.ControllerInstance.IsGemEnabled)
            {
                return;
            }

            var gemMenu = new Menu(nameof(L10N.ROOT_GEM), PathIcon.Gem);

            #region Data Dictionary

            var dataDictionaryMenu = new Menu(nameof(L10N.MENU_GEM_DATA_DICTIONARY));
            dataDictionaryMenu.Items.Add(
                new StatusVariablesPanel(
                    nameof(DataDictionaryPanelsResources.GEMPANELS_STATUS_VARIABLES),
                    PathIcon.InProgress));
            dataDictionaryMenu.Items.Add(
                new DataVariablesPanel(
                    nameof(DataDictionaryPanelsResources.GEMPANELS_DATA_VARIABLES),
                    PathIcon.Data));
            dataDictionaryMenu.Items.Add(
                new EquipmentConstantsPanel(
                    nameof(DataDictionaryPanelsResources.GEMPANELS_EQUIPMENT_CONSTANTS),
                    PathIcon.Pi));
            dataDictionaryMenu.Items.Add(
                new CollectionEventsPanel(
                    nameof(DataDictionaryPanelsResources.GEMPANELS_COLLECTION_EVENTS),
                    PathIcon.Lightning));
            dataDictionaryMenu.Items.Add(
                new E30ReportsPanel(
                    nameof(DataDictionaryPanelsResources.GEMPANELS_REPORTS),
                    PathIcon.Link));
            dataDictionaryMenu.Items.Add(
                new E30AlarmsPanel(
                    nameof(DataDictionaryPanelsResources.GEMPANELS_ALARMS),
                    PathIcon.Alarm));
            gemMenu.Items.Add(dataDictionaryMenu);

            #endregion Data Dictionary

            #region Equipment

            if (App.ControllerInstance.IsGem300Supported)
            {
                #region Equipment

                gemMenu.Items.Add(
                    new LoadPortsViewerPanel(
                        nameof(L10N.BP_LOADPORTS_OVERVIEW),
                        PathIcon.LoadPorts));
                gemMenu.Items.Add(
                    new CarriersViewerPanel(nameof(L10N.BP_CARRIERS), PathIcon.Carrier));

                #endregion Equipment

                #region Job

                gemMenu.Items.Add(
                    new JobsPanel(
                        nameof(GemGeneralRessources.GEM_JOBSMENU),
                        CustomPathIcon.ControlAndProcessJob));

                #endregion Job

                #region Substrate tracking

                gemMenu.Items.Add(
                    new SubstrateTrackingPanelModel(
                        nameof(SubstrateTrackingRessources.SUB_TRACK),
                        PathIcon.Tracking));

                #endregion Substrate tracking

                #region Equipment performance tracking

                gemMenu.Items.Add(
                    new EptPanelModel(nameof(EPTRessources.EPT_TAB_TITLE), PathIcon.Spy));

                #endregion

                #region Spy

                gemMenu.Items.Add(
                    new SecsMessagesLogPanel(nameof(L10N.BP_SECS_MESSAGE), PathIcon.Spy));
                gemMenu.Items.Add(
                    new SmnViewerPanel(nameof(L10N.BP_SMN_VIEWER), PathIcon.DataAnalysis));

                #endregion Spy
            }

            #endregion Equipment

            Navigation.RootMenu.Items.Add(gemMenu);
        }

        private void CreateConfigurationPanels()
        {
            // SETUP RootMenu
            _setupNavigationMenu = new Menu(nameof(L10N.ROOT_SETUP), PathIcon.Setup);
            Navigation.RootMenu.Items.Add(_setupNavigationMenu);

            ///////////////// Application Sub Menu /////////////////
            var applicationSubMenu = new Menu(nameof(L10N.MENU_APPLICATION));
            _setupNavigationMenu.Items.Add(applicationSubMenu);

            /* AccessRights */
            var accessRightsPanel = new AccessRightsPanel(
                this,
                GUI.Common.App.Instance.AccessRights,
                nameof(L10N.BP_ACCESSRIGHTS),
                PathIcon.AccessRights);

            applicationSubMenu.Items.Add(accessRightsPanel);

            /* PathConfig */
            applicationSubMenu.Items.Add(
                new UtoAppPathConfigPanel(nameof(L10N.BP_APP_CONFIG), PathIcon.Folder));

            /* Global Settings */
            applicationSubMenu.Items.Add(
                new GlobalSettingsPanel(nameof(L10N.BP_GLOBAL_PARAM), PathIcon.Setup));

            /* Logs */
            applicationSubMenu.Items.Add(
                new DiagnosticPanel(nameof(L10N.BP_SETUP_LOGS), PathIcon.Datalog));

            /* Alarms */
            applicationSubMenu.Items.Add(new AlarmCenter(nameof(L10N.ROOT_ALARMS), PathIcon.Alarm));

            applicationSubMenu.Items.Add(
                new AppearancePanelViewModel(nameof(L10N.BP_APPEARANCE), PathIcon.Responsive));

            ///////////////// Process Sub Menu /////////////////
            var processSubMenu = new Menu(nameof(L10N.PROCESS));
            _setupNavigationMenu.Items.Add(processSubMenu);

            /* Recipes Configuration */
            processSubMenu.Items.Add(
                new FileConfigurationPanel(
                    nameof(L10N.BP_PROCESS_PROGRAM_CONFIG),
                    PathIcon.Recipes));

            /* Scenarios Configuration */
            processSubMenu.Items.Add(
                new FileConfigurationPanel(
                    nameof(L10N.BP_SCENARIOS_CONFIGURATION),
                    PathIcon.RecipeManager,
                    FileConfigurationType.Scenario));

            /* DCPs */
            processSubMenu.Items.Add(
                new DataCollectionSetupPanel(
                    nameof(DataCollectionLibraryResources.DATA_MONITORING),
                    GUI.Common.App.Instance?.DataCollectionPlanLibrarian,
                    PathIcon.Chart));

            processSubMenu.Items.Add(
                new DataFlowSetupPanel(nameof(L10N.BP_PROCESS), UnityScIcons.DataFlow));

            if (App.ControllerInstance.ControllerEquipmentManager.SubstrateIdReaderFront != null
                || App.ControllerInstance.ControllerEquipmentManager.SubstrateIdReaderBack != null)
            {
                processSubMenu.Items.Add(
                    new OcrProfilesPanel(nameof(L10N.BP_OCR_PROFILE), PathIcon.RecipeEditor));
            }

            /* Host Communication */
            if (App.ControllerInstance is { IsGemEnabled: true })
            {
                _setupNavigationMenu.Items.Add(
                    new HostCommunicationSetupPanel(nameof(L10N.BP_HOST_COMM_SETUP), PathIcon.Lan));
            }

            /* Device Settings Submenu */
            var deviceSettingsSubMenu = new Menu(nameof(L10N.MENU_DEVICES_SETTINGS));
            _setupNavigationMenu.Items.Add(deviceSettingsSubMenu);

            /* Equipment */
            deviceSettingsSubMenu.Items.Add(
                new EquipmentSettingsPanel(
                    nameof(EquipmentSettingsResources.BP_EQUIPMENT_SETTINGS),
                    PathIcon.Setup));


            foreach (var device in App.UtoInstance.EquipmentManager.Equipment.AllDevices())
            {
                if (App.ControllerInstance.DeviceUiManagerService.CreatePanelFrom(device) is { } panel
                    && !deviceSettingsSubMenu.Items.Select(x => x.RelativeId).Contains(panel.RelativeId))
                {
                    deviceSettingsSubMenu.Items.Add(panel);
                }
            }

            var saveAppConfigCommand = new ApplicationCommand(
                nameof(SetupPanelResources.SETUP_SAVE_CONFIG_COMMAND),
                new DelegateCommand(SaveAppConfigExecute, SaveAppConfigCommandCanExecute),
                PathIcon.Save);
            var undoAppConfigCommand = new ApplicationCommand(
                nameof(SetupPanelResources.SETUP_UNDO_CHANGES_COMMAND),
                new DelegateCommand(UndoAppConfigExecute, UndoAppConfigCommandCanExecute),
                PathIcon.Undo);

            ApplicationCommands.Add(saveAppConfigCommand);
            ApplicationCommands.Add(undoAppConfigCommand);

            // add Save/Undo config app commands to all setup panels
            _setupNavigationMenu.GetChildrenAsFlattenedCollection()
                .OfType<ISetupPanel>()
                .ToList()
                .ForEach(
                    setupPanel =>
                    {
                        setupPanel.Commands.Add(
                            new ApplicationCommandReference(saveAppConfigCommand));
                        setupPanel.Commands.Add(
                            new ApplicationCommandReference(undoAppConfigCommand));
                    });
        }

        private void CreateAlarmPanels()
        {
            // Application Command
            var muteBuzzerApplicationCommand = new ApplicationCommand(
                Actives.NameofMuteBuzzer,
                new DelegateCommand(
                    () =>
                    {
                        var lightTower = GUI.Common.App.Instance.EquipmentManager.Equipment
                            .AllDevices<LightTower>()
                            .First();
                        lightTower.DefineBuzzerModeAsync(BuzzerState.Off);
                    },
                    () =>
                    {
                        var lightTower = GUI.Common.App.Instance.EquipmentManager.Equipment
                            .AllDevices<LightTower>()
                            .FirstOrDefault();
                        return lightTower != null
                               && lightTower.IsCommunicating
                               && lightTower.BuzzerState != BuzzerState.Off;
                    }),
                PathIcon.AudioOn);
            ApplicationCommands.Add(muteBuzzerApplicationCommand);

            // Menu
            var alarmsNavigationMenu = new Menu(nameof(L10N.ROOT_ALARMS), PathIcon.Alarm);
            Navigation.RootMenu.Items.Add(alarmsNavigationMenu);

            if (IsInDesignMode)
            {
                alarmsNavigationMenu.Items.Add(new Actives());
                alarmsNavigationMenu.Items.Add(new History());
                alarmsNavigationMenu.Items.Add(new AnalysisPanel());
                alarmsNavigationMenu.Items.Add(new Catalog());
            }
            else
            {
                alarmsNavigationMenu.Items.Add(new Actives(App.ControllerInstance.AlarmCenter));
                alarmsNavigationMenu.Items.Add(new History(App.ControllerInstance.AlarmCenter));
                alarmsNavigationMenu.Items.Add(
                    new AnalysisPanel(
                        GUI.Common.App.Instance.AlarmCenter,
                        nameof(L10N.BP_ALARM_ANALYSIS),
                        PathIcon.Analysis));
                alarmsNavigationMenu.Items.Add(new Catalog(App.ControllerInstance.AlarmCenter));
            }

            // Add MuteBuzzer command for all alarm panels
            foreach (var businessPanel in alarmsNavigationMenu.GetChildrenAsFlattenedCollection()
                         .OfType<BusinessPanel>())
            {
                businessPanel.Commands.Add(
                    new ApplicationCommandReference(muteBuzzerApplicationCommand));
            }

            GUI.Common.App.Instance.AlarmCenter.Services.AlarmOccurrenceStateChanged +=
                OnAlarmCenterOccurrenceStateChanged;
        }

        private void CreateHelpPanel()
        {
            // Menu
            var aboutNavigationMenu = new Menu(nameof(L10N.ROOT_HELP), PathIcon.Help);
            Navigation.RootMenu.Items.Add(aboutNavigationMenu);

            // Panels
            var about = new About(nameof(L10N.BP_ABOUT), PathIcon.About);

            about.Supplier = new CompanyData
            {
                Address = @"611 rue Aristide Berges
Z.A. de Pré Millet
38330 Montbonnot-Saint-Martin - FRANCE",
                Name = "UnitySC",
                WebMailContact = "services@unity-sc.com",
                WebSite = "https://www.unity-sc.com/",
                Description = "Innovative Technology and Solutions for Semiconductor"
            };

            aboutNavigationMenu.Items.Add(about);

            if (IsInDesignMode)
            {
                BusinessPanel userManualOnLine = new FileViewer();
                aboutNavigationMenu.Items.Add(userManualOnLine);
            }
            else
            {
                //BusinessPanel userManualOnLine = new FileViewer(
                //    GUI.Common.App.Instance.Config?.ApplicationPath.UserManualPath,
                //    nameof(L10N.BP_USER_MANUAL),
                //    PathIcon.FileViewer);
                //aboutNavigationMenu.Items.Add(userManualOnLine);
            }

            BusinessPanel systemInformation = new SystemInformationViewModel(
                nameof(L10N.BP_SYSTEMINFO),
                PathIcon.SystemInformation);
            aboutNavigationMenu.Items.Add(systemInformation);

            if (GUI.Common.App.Instance.Config?.GlobalSettings?.IsDeveloperDebugModeEnabled
                ?? false)
            {
                // Developer sub-menu
                var developersSubMenu = new Menu(nameof(L10N.MENU_DEVELOPERS), PathIcon.Developer);
                aboutNavigationMenu.Items.Add(developersSubMenu);

                var sample = new SamplePanel(nameof(SamplePanelResources.BP_SAMPLE), PathIcon.Code);
                developersSubMenu.Items.Add(sample);

                var guiComponentsMenu = new Menu(nameof(L10N.MENU_GUI_COMPONENTS), PathIcon.Agileo)
                {
                    Items =
                    {
                        new PopupPanel(nameof(L10N.BP_POPUP), PathIcon.Popup),
                        new BusinessPanelCommandPanel(
                            nameof(L10N.BP_PANEL_COMMANDS),
                            PathIcon.UserInterfaceControl),
                        new MessageAreaPanel(nameof(L10N.BP_MESSAGE_AREA), PathIcon.Sms),
                        new NavigateToPanel(
                            nameof(L10N.BP_NAVIGATION),
                            PathIcon.Navigation),
                        new SaliencePanel(nameof(L10N.BP_SALIENCE), PathIcon.Salience),
                        new ToolboxPanel(nameof(L10N.BP_TOOLBOX), PathIcon.ToolBox)
                    }
                };
                developersSubMenu.Items.Add(guiComponentsMenu);

                var vendorComponentsMenu =
                    new Menu(nameof(L10N.MENU_VENDOR_COMPONENTS), PathIcon.Responsive)
                    {
                        Items =
                        {
                            new DataTablePanel(
                                nameof(L10N.BP_DATA_TABLE),
                                PathIcon.ListView),
                            new DataTreePanel(nameof(L10N.BP_DATA_TREE), PathIcon.DataTree)
                        }
                    };
                developersSubMenu.Items.Add(vendorComponentsMenu);

                var controlsMenu = new Menu(nameof(L10N.MENU_CONTROLS), PathIcon.SliderControl)
                {
                    Items =
                    {
                        new WpfControlsPanel(nameof(L10N.BP_WPF_CONTROLS)),
                        new CustomControlsPanel(nameof(L10N.BP_CUSTOM_CONTROLS))
                    }
                };
                developersSubMenu.Items.Add(controlsMenu);

                var resourcesMenu = new Menu(nameof(L10N.MENU_RESOURCES), PathIcon.Responsive)
                {
                    Items = { new IconsViewerPanel(nameof(L10N.BP_ICONS), PathIcon.Star) }
                };
                developersSubMenu.Items.Add(resourcesMenu);

                var webVisu = new WebVisuPanel(nameof(L10N.BP_WEBVISU), PathIcon.Web);
                webVisu.LinkList.Add(new WebLink("https://www.agileo.com", "Agileo"));
                developersSubMenu.Items.Add(webVisu);
            }
        }

        /// <inheritdoc />
        protected override Agileo.GUI.Components.TitlePanel CreateTitlePanel()
        {
            var communicationCommand = new ApplicationCommand(
                nameof(TitlePanelResources.TITLE_PANEL_COMMUNICATION_COMMAND),
                new DelegateCommand(() => { }),
                PathIcon.Doorbell);
            var equipmentCommand = new ApplicationCommand(
                nameof(TitlePanelResources.TITLE_PANEL_EQUIPMENT_COMMAND),
                new DelegateCommand(() => { }),
                PathIcon.Doorbell);
            var controlCommand = new ApplicationCommand(
                nameof(TitlePanelResources.TITLE_PANEL_CONTROL_COMMAND),
                new DelegateCommand(() => { }),
                PathIcon.Doorbell);

            ApplicationCommands.Add(communicationCommand);
            ApplicationCommands.Add(equipmentCommand);
            ApplicationCommands.Add(controlCommand);

            var titlePanel = new UnityTitlePanel(this);

            titlePanel.GemCommandsViewModel.InitilizeApplicationCommands(
                communicationCommand,
                equipmentCommand,
                controlCommand);
            return titlePanel;
        }

        #region Commands

        #region Validate Popup

        private ApplicationCommand _validateCurrentPopupCommand;

        public ApplicationCommand ValidateCurrentPopupCommand
        {
            get
            {
                if (_validateCurrentPopupCommand == null)
                {
                    _validateCurrentPopupCommand = new ApplicationCommand(
                        "Validate current popup",
                        new DelegateCommand(ValidateCurrentPopupCommandExecute));
                }

                return _validateCurrentPopupCommand;
            }
        }

        private void ValidateCurrentPopupCommandExecute()
        {
            if (AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel
                    ?.Popups.Current is ValidablePopup validablePopup)
            {
                validablePopup.Validate();
            }
        }

        #endregion

        #region Cancel Popup

        private ApplicationCommand _cancelCurrentPopupCommand;

        public ApplicationCommand CancelCurrentPopupCommand
        {
            get
            {
                if (_cancelCurrentPopupCommand == null)
                {
                    _cancelCurrentPopupCommand = new ApplicationCommand(
                        "Cancel current popup",
                        new DelegateCommand(CancelCurrentPopupCommandExecute));
                }

                return _cancelCurrentPopupCommand;
            }
        }

        private void CancelCurrentPopupCommandExecute()
        {
            if (AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel
                    ?.Popups.Current is ValidablePopup validablePopup)
            {
                validablePopup.Cancel();
            }
        }

        #endregion

        #endregion

        #region Private

        #region Tags Spy

        private static bool CanOpenTagsSpyWindow()
        {
            var tagsSpyWindow = GUI.Common.App.Instance.Windows.Cast<Window>()
                .FirstOrDefault(w => w.Content is TagsSpyView);
            return tagsSpyWindow is not { IsVisible: true };
        }

        private void OpenTagsSpyWindow()
        {
            _tagSpy ??= new TagsSpyViewModel(MessageDataBus.Instance);
            var tagsSpyWindow = new Window
            {
                Content = new TagsSpyView(),
                Title = nameof(L10N.TAGS_SPY),
                DataContext = _tagSpy
            };
            tagsSpyWindow.Show();
        }

        private void AddTagsSpyCommand(Menu menu)
        {
            var panels = menu.GetChildrenAsFlattenedCollection().OfType<BusinessPanel>();
            foreach (var panel in panels)
            {
                AddOpenTagsSpyWindowCommand(panel);
            }
        }

        private void AddOpenTagsSpyWindowCommand(BusinessPanel businessPanel)
        {
            if (_openTagsSpyCommand != null
                && (GUI.Common.App.Instance.Config?.GlobalSettings?.IsDeveloperDebugModeEnabled
                    ?? false))
            {
                businessPanel?.Commands.Add(new ApplicationCommandReference(_openTagsSpyCommand));
            }
        }

        #endregion Tags Spy

        /// <inheritdoc />
        public override void OnSetup()
        {
            base.OnSetup();
            GUI.Common.App.Instance.AccessRights.UserLogon += AccessRights_UserLogon;
            GUI.Common.App.Instance.AccessRights.UserLogoff += AccessRights_UserLogoff;

            App.ControllerInstance.ControllerEquipmentManager.Controller.StatusValueChanged +=
                Controller_StatusValueChanged;

            if (App.ControllerInstance.ControllerConfig.GlobalSettings.IsDeveloperDebugModeEnabled)
            {
                App.ControllerInstance.AccessRights.Logon("SUPERVISOR", string.Empty);
            }

            if (App.ControllerInstance.ControllerEquipmentManager.Equipment
                .AllDevices<DataFlowManager>()
                .Any(df => df.ExecutionMode == ExecutionMode.Real))
            {
                BaseUnityIntegrationPanel.FinalizeSetup();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GUI.Common.App.Instance.AccessRights.UserLogon -= AccessRights_UserLogon;
                GUI.Common.App.Instance.AccessRights.UserLogoff -= AccessRights_UserLogoff;

                App.ControllerInstance.ControllerEquipmentManager.Controller.StatusValueChanged -=
                    Controller_StatusValueChanged;

                GUI.Common.App.Instance.AlarmCenter.Services.AlarmOccurrenceStateChanged -=
                    OnAlarmCenterOccurrenceStateChanged;
                foreach (var businessPanel in BusinessPanels)
                {
                    businessPanel.Saliences.PropertyChanged -= OnBusinessPanelSaliencesChanged;
                }
            }

            base.Dispose(disposing);
        }

        private void Controller_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(IController.State))
            {
                _serviceModePanel.AreCommandsEnabled =
                    App.ControllerInstance.ControllerEquipmentManager.Controller.State
                    == OperatingModes.Maintenance;
            }
        }

        private void AccessRights_UserLogoff(User user, UserEventArgs e)
        {
            ((GUI.Common.Vendor.Views.TitlePanel.TitlePanel)TitlePanel)?.AccessRights_UserLogoff(
                user,
                e);
        }

        private void AccessRights_UserLogon(User user, UserEventArgs e)
        {
            ((GUI.Common.Vendor.Views.TitlePanel.TitlePanel)TitlePanel)?.AccessRights_UserLogon(
                user,
                e);
        }

        private static void DefineLightsState(LightTowerState state)
        {
            var lightTower = GUI.Common.App.Instance.EquipmentManager.Equipment
                .AllDevices<LightTower>()
                .FirstOrDefault();
            if (lightTower is { IsCommunicating: true })
            {
                lightTower.DefineStateAsync(state);
            }
        }

        #region Setup Panels Commands

        private bool UndoAppConfigCommandCanExecute()
        {
            return _setupNavigationMenu.GetChildrenAsFlattenedCollection()
                .OfType<ISetupPanel>()
                .Any(setupPanel => setupPanel.UndoCommandCanExecute());
        }

        private void UndoAppConfigExecute()
        {
            foreach (var setupPanel in _setupNavigationMenu.GetChildrenAsFlattenedCollection()
                         .OfType<ISetupPanel>())
            {
                setupPanel.UndoCommandExecute();
            }
        }

        private bool SaveAppConfigCommandCanExecute()
        {
            var setupPanels = _setupNavigationMenu.GetChildrenAsFlattenedCollection()
                .OfType<ISetupPanel>()
                .ToList();
            setupPanels.ForEach(setupPanel => setupPanel.RefreshSaveRequired());
            return setupPanels.Any(setupPanel => setupPanel.SaveCommandCanExecute());
        }

        private void SaveAppConfigExecute()
        {
            foreach (var setupPanel in _setupNavigationMenu.GetChildrenAsFlattenedCollection()
                         .OfType<ISetupPanel>())
            {
                setupPanel.SaveCommandExecute();
            }
        }

        #region Maintenance Panels Commands

        private bool EnterMaintenanceModeCommandCanExecute()
        {
            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                return false;
            }

            if (App.ControllerInstance.ControllerEquipmentManager.Controller is
                Equipment.Devices.Controller.Controller controller)
            {
                var context = controller.NewCommandContext(nameof(controller.RequestManualMode));
                return controller.CanExecute(context);
            }

            return false;
        }

        private void EnterMaintenanceModeCommandExecute()
        {
            if (App.ControllerInstance.ControllerEquipmentManager.Controller is
                Equipment.Devices.Controller.Controller controller)
            {
                controller.RequestManualModeAsync();
            }
        }

        #endregion Maintenance Panels Commands

        #endregion Setup Panels Commands

        #endregion Private

        #region Event handlers

        private static void OnBusinessPanelSaliencesChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SalienceCounter.UserAttentionCount)
                && sender is SalienceCounter salienceCounter)
            {
                DefineLightsState(
                    salienceCounter.UserAttentionCount == 0
                        ? LightTowerState.InterventionACK
                        : LightTowerState.InterventionNACK);
            }
        }

        private static void OnAlarmCenterOccurrenceStateChanged(
            object sender,
            AlarmOccurrenceEventArgs args)
        {
            var allCleared = GUI.Common.App.Instance.AlarmCenter.Repository.GetAlarmOccurrences()
                .All(a => a.State == AlarmState.Cleared);
            var allAcknowledged = GUI.Common.App.Instance.AlarmCenter.Repository
                .GetAlarmOccurrences()
                .Where(al => al.State == AlarmState.Set)
                .All(a => a.Acknowledged);

            //Change lightState if new alarm or all alarm acknowledged
            if ((args.AlarmOccurrence.State == AlarmState.Set && !args.AlarmOccurrence.Acknowledged)
                || (allAcknowledged && !allCleared))
            {
                DefineLightsState(
                    allAcknowledged
                        ? LightTowerState.HazardousACK
                        : LightTowerState.HazardousNACK);
            }

            if (allCleared)
            {
                (App.ControllerInstance.ControllerEquipmentManager.Controller as
                    Equipment.Devices.Controller.Controller)?.UpdateLightTower();
            }
        }

        #endregion Event handlers
    }
}
