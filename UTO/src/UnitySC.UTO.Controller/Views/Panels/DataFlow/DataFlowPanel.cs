using System;
using System.Linq;
using System.Text;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.SemiDefinitions;

using CommunityToolkit.Mvvm.Messaging;
using UnitsNet;

using UnitySC.Dataflow.Service.Interface;
using UnitySC.Dataflow.UI.Shared;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Devices.Controller;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.PM.Shared.UI.Recipes.Management;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.TC.UI.Dataflow.ViewModel;
using UnitySC.UTO.Controller.Views.Panels.DataFlow.Integration;
using UnitySC.UTO.Controller.Views.Panels.DataFlow.Popup;
using UnitySC.UTO.Controller.Views.Panels.Integration;
using UnitySC.UTO.Controller.Views.Panels.Production.Equipment;

namespace UnitySC.UTO.Controller.Views.Panels.DataFlow
{
    public class DataFlowPanel : BaseUnityIntegrationPanel
    {
        #region Fields

        private UnitySC.Equipment.Devices.Controller.Controller _controller;
        private readonly BusinessPanelToggleCommand _engineeringModeCommand;
        #endregion

        #region Properties

        public ControllerEquipmentManager EquipmentManager
            => App.ControllerInstance.ControllerEquipmentManager;

        public UserMessageDisplayer MessageDisplayer { get; } = new();

        #endregion

        public DataflowManagementViewModel DataFlow
            => ClassLocator.Default.GetInstance<DataflowManagementViewModel>();

        static DataFlowPanel()
        {
            DataTemplateGenerator.Create(typeof(DataFlowPanel), typeof(DataFlowPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(DataFlowResources)));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(EquipmentResources)));
        }

        public DataFlowPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            var enterEngineeringMode = new ContextualBusinessPanelCommand(
                nameof(DataFlowResources.DATAFLOW_ENTER_ENGINEERING_MODE),
                RequestEngineeringModeCommand,
                PathIcon.Manual);

            var exitEngineeringMode = new ContextualBusinessPanelCommand(
                nameof(DataFlowResources.DATAFLOW_EXIT_ENGINEERING_MODE),
                ExitEngineeringModeCommand,
                PathIcon.Manual);

            _engineeringModeCommand = new BusinessPanelToggleCommand(
                nameof(DataFlowResources.DATAFLOW_ENGINEERING_MODE),
                enterEngineeringMode,
                exitEngineeringMode);

            Commands.Add(_engineeringModeCommand);

            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(DataFlowResources.DATAFLOW_LOAD_PM),
                    LoadPmCommand,
                    PathIcon.ArrowUp));

            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(DataFlowResources.DATAFLOW_UNLOAD_PM),
                    UnloadPmCommand,
                    PathIcon.ArrowDown));
        }

        #region Overrides of BaseUnityIntegrationPanel

        public override void OnSetup()
        {
            base.OnSetup();

            _controller = EquipmentManager.Controller as UnitySC.Equipment.Devices.Controller.Controller;
            if (_controller != null)
            {
                _controller.PropertyChanged += Controller_PropertyChanged;
            }

            UpdateEngineeringModeCommand();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _controller.PropertyChanged -= Controller_PropertyChanged;
            }
        }

        protected override void Register()
        {
            RegisterUserSupervisor();

            // TC Status
            ClassLocator.Default.Register(typeof(ITcStatus), typeof(DummyTCStatus), true);

            // DataflowManagementViewModel
            ClassLocator.Default.Register<DataflowManagementViewModel>(true);

            DataFlowRegister();
        }

        private void DataFlowRegister()
        {
            // Similar to UnitySC.TC.UI.Dataflow.Bootstrapper

            ClassLocator.Default.Register<DataflowViewModel>(true);
            ClassLocator.Default.Register<DataflowUTOSimulatorViewModel>(true);

            RegisterIDbRecipeService();

            ClassLocator.Default.Register<ServiceInvoker<IDAP>>(() => new ServiceInvoker<IDAP>("DAP", ClassLocator.Default.GetInstance<SerilogLogger<IDAP>>(), ClassLocator.Default.GetInstance<IMessenger>()));

            RegisterSharedSupervisors();
            RegisterExternalUserControls();
        }

        #endregion

        #region Commands

        #region RequestEngineeringMode

        private ContextualSafeDelegateCommand _requestEngineeringModeCommand;

        private ContextualSafeDelegateCommand RequestEngineeringModeCommand
            => _requestEngineeringModeCommand ??= new ContextualSafeDelegateCommand(
                RequestEngineeringModeExecute,
                RequestEngineeringModeCanExecute);

        private void RequestEngineeringModeExecute()
        {
            _controller.RequestEngineeringModeAsync();
        }

        private Tuple<bool, string> RequestEngineeringModeCanExecute()
        {
            var canExecute = EquipmentManager.Controller.CanExecute(nameof(IController.RequestEngineeringMode), false, out var context);

            var sb = new StringBuilder();
            foreach (var error in context.Errors)
            {
                sb.AppendLine($"- {error}");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            if (EquipmentManager.Controller.State != OperatingModes.Maintenance
                && App.ControllerInstance.GemController.E87Std.Carriers.Count == 0)
            {
                canExecute = false;
                sb.AppendLine("- No carrier present");
            }

            if (EquipmentManager.Controller.State != OperatingModes.Maintenance
                && App.ControllerInstance.GemController.E87Std.Carriers.
                    Any(c=> c.SlotMapStatus == SlotMapStatus.WaitingForHost))
            {
                canExecute = false;
                sb.AppendLine("- At least one carrier loading is in progress");
            }

            if (EquipmentManager.LoadPorts.Values.All(lp =>
                    (lp.PhysicalState != LoadPortState.Open
                     && !lp.Configuration.CloseDoorAfterRobotAction)
                    || (lp.PhysicalState != LoadPortState.Docked
                        && lp.Configuration.CloseDoorAfterRobotAction)))
            {
                canExecute = false;
                sb.AppendLine("- No carrier ready for transfer");
            }

            if (EquipmentManager.Efem.State != OperatingModes.Idle
                || EquipmentManager.ProcessModules.Values.Any(pm=>pm.ProcessModuleState != ProcessModuleState.Idle))
            {
                canExecute = false;
                sb.AppendLine("- At least one device is not idle");
            }

            if (EquipmentManager.Robot.UpperArmLocation.Substrate != null
                || EquipmentManager.Robot.LowerArmLocation.Substrate != null
                || EquipmentManager.Aligner.Location.Substrate != null)
            {
                canExecute = false;
                sb.AppendLine("- Wafer still present in the EFEM (expect PM)");
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region ExitEngineeringMode

        private ContextualSafeDelegateCommand _exitEngineeringModeCommand;

        private ContextualSafeDelegateCommand ExitEngineeringModeCommand
            => _exitEngineeringModeCommand ??= new ContextualSafeDelegateCommand(
                ExitEngineeringModeExecute,
                ExitEngineeringModeCanExecute);

        private void ExitEngineeringModeExecute()
        {
            UpdateEngineeringModeCommand();

            var popup = new Agileo.GUI.Services.Popups.Popup(new LocalizableText(nameof(DataFlowResources.DATAFLOW_EXIT_ENGINEERING_MODE)),
                new LocalizableText(nameof(DataFlowResources.DATAFLOW_EXIT_ENGINEERING_MODE_MESSAGE)));

            popup.Commands.Add(
                new PopupCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_POPUP_YES),
                    new DelegateCommand(
                        () =>
                        {
                            try
                            {
                                MessageDisplayer.HideAll();
                                _controller.ActivityDone += Controller_ActivityDone;
                                _controller.CleanAsync();
                            }
                            catch (Exception e)
                            {
                                MessageDisplayer.Show(new UserMessage(Agileo.GUI.Services.Popups.MessageLevel.Error, e.Message));
                                _controller.RequestManualModeAsync();
                            }
                            UpdateEngineeringModeCommand();
                        },
                        () => true)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(ProductionEquipmentResources.CANCEL)));

            Popups.Show(popup);
        }

        private void UpdateEngineeringModeCommand()
        {
            _engineeringModeCommand.IsChecked = _controller.State != OperatingModes.Engineering;
        }

        private Tuple<bool, string> ExitEngineeringModeCanExecute()
        {
            bool canExecute = true;
            var sb = new StringBuilder();
            if (!App.UtoInstance.ControllerEquipmentManager.IsEquipmentEmpty())
            {
                canExecute = EquipmentManager.Controller.CanExecute(nameof(IController.Clean), false, out var context);

                foreach (var error in context.Errors)
                {
                    sb.AppendLine($"- {error}");
                }
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Load

        private ContextualSafeDelegateCommand _loadPmCommand;

        private ContextualSafeDelegateCommand LoadPmCommand
            => _loadPmCommand ??= new ContextualSafeDelegateCommand(LoadPmExecute, LoadPmCanExecute);

        private void LoadPmExecute()
        {
            var loadPopup = new LoadPmPopupViewModel();
            var popup =
                new Agileo.GUI.Services.Popups.Popup(
                    new LocalizableText(nameof(DataFlowResources.DATAFLOW_LOAD_PM)))
                {
                    IsFullScreen = true, Content = loadPopup
                };
            popup.Commands.Add(
                new PopupCommand(
                    nameof(DataFlowResources.DATAFLOW_LOAD),
                    new DelegateCommand(
                        () =>
                        {
                            var selectedSubstrate = loadPopup.SubstrateSelectionViewers.Where(
                                viewModel => viewModel.LpSelectedSlots.Count > 0).ToList();

                            if (selectedSubstrate.Count != 1)
                            {
                                return;
                            }

                            var processModule = loadPopup.SelectedProcessModule;
                            var angle = processModule.GetAlignmentAngle();
                            var substrate = selectedSubstrate[0].LpSelectedSlots[0].Substrate;
                            var loadPort = selectedSubstrate[0].LoadPort;

                            _controller.LoadProcessModuleAsync(
                                loadPort,
                                substrate.SourceSlot,
                                loadPopup.RobotArm,
                                Angle.FromDegrees(angle), 
                                AlignType.AlignWaferWithoutCheckingSubO_FlatLocation,
                                loadPopup.RobotArm == RobotArm.Arm1
                                    ? EquipmentManager.Robot.Configuration.UpperArm.EffectorType
                                    : EquipmentManager.Robot.Configuration.LowerArm.EffectorType,
                                processModule);

                            loadPopup.Dispose();
                            loadPopup = null;
                            popup.Close();
                        },
                        () =>
                        {
                            if (loadPopup?.SelectedProcessModule == null)
                            {
                                return false;
                            }

                            var processModule = loadPopup.SelectedProcessModule;
                            var selectedSubstrate = loadPopup.SubstrateSelectionViewers.Where(
                                viewModel => viewModel.LpSelectedSlots.Count > 0).ToList();

                            if (selectedSubstrate.Count != 1)
                            {
                                return false;
                            }

                            var substrate = selectedSubstrate[0].LpSelectedSlots[0].Substrate;
                            var loadPort = selectedSubstrate[0].LoadPort;

                            return _controller.CanExecute(
                                nameof(IController.LoadProcessModule),
                                true,
                                out _,
                                loadPort,
                                substrate.SourceSlot,
                            loadPopup.RobotArm,
                                Angle.FromDegrees(0),
                                AlignType.AlignWaferWithoutCheckingSubO_FlatLocation,
                                loadPopup.RobotArm == RobotArm.Arm1
                                    ? EquipmentManager.Robot.Configuration.UpperArm.EffectorType
                                    : EquipmentManager.Robot.Configuration.LowerArm.EffectorType,
                                processModule);

                        })));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(ProductionEquipmentResources.CANCEL),
                    new DelegateCommand(
                        () =>
                        {
                            loadPopup.Dispose();
                            popup.Close();
                        })));
            Popups.Show(popup);
        }

        private Tuple<bool, string> LoadPmCanExecute()
        {
            var canExecute = true;
            var sb = new StringBuilder();

            if (_controller.State != OperatingModes.Engineering)
            {
                canExecute = false;
                sb.AppendLine("- Controller is not in engineering mode");
            }

            if (EquipmentManager.LoadPorts.Values.All(lp => lp.Carrier == null))
            {
                canExecute = false;
                sb.AppendLine("- No carrier present");
            }

            if (EquipmentManager.LoadPorts.Values.All(lp =>
                    (lp.PhysicalState != LoadPortState.Open
                     && !lp.Configuration.CloseDoorAfterRobotAction)
                    || (lp.PhysicalState != LoadPortState.Docked
                        && lp.Configuration.CloseDoorAfterRobotAction)))
            {
                canExecute = false;
                sb.AppendLine("- No carrier ready for transfer");
            }

            if (App.UtoInstance.ControllerEquipmentManager.ProcessModules.Values.All(pm => pm.Location.Material != null))
            {
                canExecute = false;
                sb.AppendLine("- All process modules are already occupied");
            }

            if (App.UtoInstance.ControllerEquipmentManager.CommunicatingDevices.Any(d => d.State != OperatingModes.Idle))
            {
                canExecute = false;
                sb.AppendLine("- All devices are not idle");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Unload

        private ContextualSafeDelegateCommand _unloadPmCommand;

        private ContextualSafeDelegateCommand UnloadPmCommand
            => _unloadPmCommand ??=
                new ContextualSafeDelegateCommand(UnloadPmExecute, UnloadPmCanExecute);

        private void UnloadPmExecute()
        {
            var popup = new Agileo.GUI.Services.Popups.Popup(new LocalizableText(nameof(DataFlowResources.DATAFLOW_UNLOAD_PM)),
                                                            new LocalizableText(nameof(DataFlowResources.DATAFLOW_UNLOAD_PM_MESSAGE)));
            popup.Commands.Add(
              new PopupCommand(
                  nameof(Agileo.GUI.Properties.Resources.S_POPUP_YES),
                  new DelegateCommand(
                      () =>
                      {
                          var processModule = App.UtoInstance.ControllerEquipmentManager.ProcessModule1;
                          var substrate = processModule.Location.Substrate;
                          var loadPort =
                              App.UtoInstance.ControllerEquipmentManager.LoadPorts[substrate.SourcePort];

                          _controller.UnloadProcessModuleAsync(
                              processModule,
                              RobotArm.Arm2,
                              EquipmentManager.Robot.Configuration.LowerArm.EffectorType,
                              loadPort,
                              substrate.SourceSlot);
                          popup.Close();
                      },
                      () =>
                      {
                          var processModule = App.UtoInstance.ControllerEquipmentManager.ProcessModule1;
                          var substrate = processModule.Location.Substrate;
                          var loadPort = App.UtoInstance.ControllerEquipmentManager.LoadPorts[substrate.SourcePort];

                          return _controller.CanExecute(
                              nameof(IController.UnloadProcessModule),
                              true,
                              out _,
                              processModule,
                              RobotArm.Arm2,
                              EquipmentManager.Robot.Configuration.LowerArm.EffectorType,
                              loadPort,
                              substrate.SourceSlot);
                      })));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(ProductionEquipmentResources.CANCEL),
                    new DelegateCommand(
                        () => popup.Close())));
            Popups.Show(popup);
        }

        private Tuple<bool, string> UnloadPmCanExecute()
        {
            var canExecute = true;
            var sb = new StringBuilder();

            if (_controller.State != OperatingModes.Engineering)
            {
                canExecute = false;
                sb.AppendLine("- Controller is not in engineering mode");
            }

            if (EquipmentManager.LoadPorts.Values.All(lp => lp.Carrier == null))
            {
                canExecute = false;
                sb.AppendLine("- No carrier present");
            }

            if (EquipmentManager.LoadPorts.Values.All(lp =>
                    (lp.PhysicalState != LoadPortState.Open
                     && !lp.Configuration.CloseDoorAfterRobotAction)
                    || (lp.PhysicalState != LoadPortState.Docked
                        && lp.Configuration.CloseDoorAfterRobotAction)))
            {
                canExecute = false;
                sb.AppendLine("- No carrier ready for transfer");
            }

            if (App.UtoInstance.ControllerEquipmentManager.ProcessModules.Values.All(
                    pm => pm.Location.Material == null))
            {
                canExecute = false;
                sb.AppendLine("- All process modules are empty");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #endregion

        #region Event handler

        private void Controller_ActivityDone(object sender, Equipment.Abstractions.Vendor.Devices.Activities.ActivityEventArgs e)
        {
            _controller.ActivityDone -= Controller_ActivityDone;
            _controller.Initialize(false);
        }

        private void Controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DispatcherHelper.DoInUiThread(
                () =>
                {
                    if (e.PropertyName == nameof(IController.State))
                    {
                        UpdateEngineeringModeCommand();
                    }
                });
        }
        #endregion
    }
}
