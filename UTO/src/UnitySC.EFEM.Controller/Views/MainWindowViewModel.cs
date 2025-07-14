using System.Linq;
using System.Windows;

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
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.UserMessages;
using Agileo.MessageDataBus;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EFEM.Controller.Views.Panels.Main;
using UnitySC.EFEM.Controller.Views.Panels.Setup.HostInterface;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Maintenance.Communication;
using UnitySC.EFEM.Rorze.GUI.Views.Tools.StatusComparer;
using UnitySC.Equipment.Abstractions.Devices.LightTower;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Recipes.Resources;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.TagsSpy;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Diagnostic.TraceViewer;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.About;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.FileViewer;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.SystemInformation;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Library;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Runner;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Appearance;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Diagnostic;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.FileConfiguration;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.GlobalSettings;
using UnitySC.GUI.Common.Vendor.Views.TitlePanel;
using UnitySC.GUI.Common.Vendor.Views.Tools.ThemeBuilder;
using UnitySC.GUI.Common.Views.Panels.Setup.AppPathConfig;
using UnitySC.GUI.Common.Views.Panels.Setup.EquipmentSettings;

namespace UnitySC.EFEM.Controller.Views
{
    public class MainWindowViewModel : UserInterface
    {
        private Menu _setupNavigationMenu;
        private TagsSpyViewModel _tagSpy;
        private ApplicationCommand _openTagsSpyCommand;
        private ServiceModePanel _serviceModePanel;

        public MainWindowViewModel()
            : base(new AccessManager(), new LocalizationManager(), Logger.GetLogger("DesignTime"))
        {
            //Localized Resources for Validation Rules
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(RecipeValidationResources)));
        }

        public MainWindowViewModel(
            AccessManager accessManager,
            LocalizationManager localizationManager,
            ILogger userInterfaceLogger)
            : base(accessManager, localizationManager, userInterfaceLogger)
        {
        }

        public override AgileoGuiConfiguration GetCurrentConfiguration()
        {
            return App.EfemAppInstance.EfemConfig;
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
            CreateLogPanels();
            CreateConfigurationPanels();
            CreateHelpPanel();

            if (App.EfemAppInstance.EfemConfig.GlobalSettings.IsDeveloperDebugModeEnabled)
            {
                var statusComparer = new StatusComparer(nameof(L10N.TOOL_STATUS_COMPARER));
                ToolManager.Tools.Add(statusComparer);

                // Add status comparer tool in service mode and log panels.
                foreach (var businessPanel in BusinessPanels.Where(
                             panel => panel.RelativeId.Equals(nameof(L10N.BP_SERVICE_MODE))
                                      || panel.RelativeId.Equals(nameof(L10N.ROOT_LOGS))
                                      || panel.RelativeId.Equals(
                                          nameof(L10N.BP_COMMUNICATION_VISUALIZER))))
                {
                    businessPanel.ToolElements.Add(new ToolReference(statusComparer));
                }
            }

            #region Theme Editor Tools

            if (App.EfemAppInstance.EfemConfig?.GlobalSettings?.IsColorizationToolboxVisible
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

            #endregion

            if (IsInDesignMode)
            {
                Navigation.NavigateTo(Navigation.RootMenu.Items.First() as BusinessPanel);
            }
        }

        private void CreateJobPanels()
        {
            BusinessPanel equipmentHandling = new MainViewModel(
                nameof(L10N.ROOT_MAIN),
                IconFactory.PathGeometryFromRessourceKey("HomeIcon"));
            Navigation.RootMenu.Items.Add(equipmentHandling);
        }

        private void CreateMaintenancePanels()
        {
            // MAINTENANCE Menu
            var systemNavigationMenu = new Menu(
                nameof(L10N.ROOT_MAINTENANCE),
                IconFactory.PathGeometryFromRessourceKey("MaintenanceIcon"));
            Navigation.RootMenu.Items.Add(systemNavigationMenu);

            _serviceModePanel = new ServiceModePanel(
                App.EfemAppInstance.EquipmentManager.Equipment,
                nameof(L10N.BP_SERVICE_MODE),
                PathIcon.Maintenance);

            systemNavigationMenu.Items.Add(_serviceModePanel);

            // Add scenario panel
            var scenarioManager = new ScenarioLibraryPanel(
                nameof(L10N.BP_SCENARIO_LIBRARY),
                App.EfemAppInstance.ScenarioManager,
                IconFactory.PathGeometryFromRessourceKey("RecipeManagerIcon"));
            systemNavigationMenu.Items.Add(scenarioManager);

            var scenarioRunner = new ScenarioRunnerPanel(
                nameof(L10N.BP_SCENARIO_SEQUENCER),
                App.EfemAppInstance.ScenarioManager,
                IconFactory.PathGeometryFromRessourceKey("PlayDocumentIcon"));
            systemNavigationMenu.Items.Add(scenarioRunner);

            var communicationVisualizer = new CommunicationVisualizerPanel(
                nameof(L10N.BP_COMMUNICATION_VISUALIZER),
                IconFactory.PathGeometryFromRessourceKey("DataIcon"));
            systemNavigationMenu.Items.Add(communicationVisualizer);

            AddTagsSpyCommand(systemNavigationMenu);
        }

        private void CreateLogPanels()
        {
            // Root & unique Panel in the Navigation Area
            BusinessPanel dataLog = new TraceViewerPanel(
                nameof(L10N.ROOT_LOGS),
                IconFactory.PathGeometryFromRessourceKey("DatalogIcon"));
            Navigation.RootMenu.Items.Add(dataLog);
        }

        private void CreateConfigurationPanels()
        {
            // SETUP RootMenu
            _setupNavigationMenu = new Menu(
                nameof(L10N.ROOT_SETUP),
                IconFactory.PathGeometryFromRessourceKey("SetupIcon"));
            Navigation.RootMenu.Items.Add(_setupNavigationMenu);

            ///////////////// Not in Sub Menu /////////////////
            /* Host Interface */
            _setupNavigationMenu.Items.Add(
                new HostInterfacePanel(
                    nameof(L10N.BP_HOST_COMM_SETUP),
                    IconFactory.PathGeometryFromRessourceKey("LanIcon")));

            ///////////////// Application Sub Menu /////////////////
            var applicationSubMenu = new Menu(
                nameof(L10N.MENU_APPLICATION),
                IconFactory.PathGeometryFromRessourceKey("SetupBoxIcon"));
            _setupNavigationMenu.Items.Add(applicationSubMenu);

            /* AccessRights */
            var accessRightsPanel = new AccessRightsPanel(
                this,
                App.EfemAppInstance.AccessRights,
                nameof(L10N.BP_ACCESSRIGHTS),
                PathIcon.AccessRights);

            applicationSubMenu.Items.Add(accessRightsPanel);

            /* PathConfig */
            applicationSubMenu.Items.Add(
                new AppPathConfigPanel(
                    nameof(L10N.BP_APP_CONFIG),
                    IconFactory.PathGeometryFromRessourceKey("FolderIcon")));

            /* Global Settings */
            applicationSubMenu.Items.Add(
                new GlobalSettingsPanel(
                    nameof(L10N.BP_GLOBAL_PARAM),
                    IconFactory.PathGeometryFromRessourceKey("SetupIcon")));

            /* Logs */
            applicationSubMenu.Items.Add(
                new DiagnosticPanel(
                    nameof(L10N.BP_SETUP_LOGS),
                    IconFactory.PathGeometryFromRessourceKey("DatalogIcon")));

            applicationSubMenu.Items.Add(
                new AppearancePanelViewModel(
                    nameof(L10N.BP_APPEARANCE),
                    IconFactory.PathGeometryFromRessourceKey("ResponsiveIcon")));

            /* Scenarios Configuration */
            applicationSubMenu.Items.Add(
                new FileConfigurationPanel(
                    nameof(L10N.BP_SCENARIOS),
                    PathIcon.RecipeManager,
                    FileConfigurationType.Scenario));

            /* Device Settings Submenu */
            var deviceSettingsSubMenu = new Menu(nameof(L10N.MENU_DEVICES_SETTINGS));
            _setupNavigationMenu.Items.Add(deviceSettingsSubMenu);

            /* Equipment */
            deviceSettingsSubMenu.Items.Add(
                new EquipmentSettingsPanel(
                    nameof(EquipmentSettingsResources.BP_EQUIPMENT_SETTINGS),
                    PathIcon.Setup));

            foreach (var device in App.EfemAppInstance.EquipmentManager.Equipment.AllDevices())
            {
                if (device is LightTower)
                {
                    //No light tower panel in Efem controller 
                    continue;
                }

                if (App.EfemAppInstance.DeviceUiManagerService.CreatePanelFrom(device) is { } panel
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

        private void CreateHelpPanel()
        {
            // Menu
            var aboutNavigationMenu = new Menu(
                nameof(L10N.ROOT_HELP),
                IconFactory.PathGeometryFromRessourceKey("HelpIcon"));
            Navigation.RootMenu.Items.Add(aboutNavigationMenu);

            // Panels
            var about = new About(
                nameof(L10N.BP_ABOUT),
                IconFactory.PathGeometryFromRessourceKey("AboutIcon"));

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
                BusinessPanel userManualOnLine = new FileViewer(
                    App.EfemAppInstance.EfemConfig?.ApplicationPath.UserManualPath,
                    nameof(L10N.BP_USER_MANUAL),
                    PathIcon.FileViewer);
                aboutNavigationMenu.Items.Add(userManualOnLine);
            }

            BusinessPanel systemInformation = new SystemInformationViewModel(
                nameof(L10N.BP_SYSTEMINFO),
                IconFactory.PathGeometryFromRessourceKey("SystemInformationIcon"));
            aboutNavigationMenu.Items.Add(systemInformation);
        }

        /// <inheritdoc />
        protected override Agileo.GUI.Components.TitlePanel CreateTitlePanel()
        {
            var equipmentCommand = new ApplicationCommand(
                nameof(TitlePanelResources.TITLE_PANEL_EQUIPMENT_COMMAND),
                new DelegateCommand(() => { }),
                IconFactory.PathGeometryFromRessourceKey("DoorbellIcon"));

            ApplicationCommands.Add(equipmentCommand);

            var titlePanel = new TitlePanel.TitlePanel(this);
            return titlePanel;
        }

        #region Tags Spy

        private static bool CanOpenTagsSpyWindow()
        {
            var tagsSpyWindow = App.EfemAppInstance.Windows.Cast<Window>()
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
                && (App.EfemAppInstance.EfemConfig?.GlobalSettings?.IsDeveloperDebugModeEnabled
                    ?? false))
            {
                businessPanel?.Commands.Add(new ApplicationCommandReference(_openTagsSpyCommand));
            }
        }

        #endregion Tags Spy

        #region Private

        /// <inheritdoc />
        public override void OnSetup()
        {
            base.OnSetup();
            App.EfemAppInstance.AccessRights.UserLogon += AccessRights_UserLogon;
            App.EfemAppInstance.AccessRights.UserLogoff += AccessRights_UserLogoff;

            App.EfemAppInstance.ControlStateChanged += EfemAppInstance_ControlStateChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.EfemAppInstance.AccessRights.UserLogon -= AccessRights_UserLogon;
                App.EfemAppInstance.AccessRights.UserLogoff -= AccessRights_UserLogoff;

                App.EfemAppInstance.ControlStateChanged -= EfemAppInstance_ControlStateChanged;
            }

            base.Dispose(disposing);
        }

        private void EfemAppInstance_ControlStateChanged(object sender, System.EventArgs e)
        {
            _serviceModePanel.AreCommandsEnabled =
                App.EfemAppInstance.ControlState == ControlState.Local;
        }

        private void AccessRights_UserLogoff(User user, UserEventArgs e)
        {
            ((TitlePanel.TitlePanel)TitlePanel)?.AccessRights_UserLogoff(user, e);
        }

        private void AccessRights_UserLogon(User user, UserEventArgs e)
        {
            ((TitlePanel.TitlePanel)TitlePanel)?.AccessRights_UserLogon(user, e);
        }

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

        #endregion Private
    }
}
