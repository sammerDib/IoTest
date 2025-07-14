using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.Equipment.Devices.Controller;
using UnitySC.GUI.Common.Equipment.Aligner.Enhanced;
using UnitySC.GUI.Common.Equipment.DriveableProcessModule;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.Equipment.Robot;
using UnitySC.GUI.Common.Equipment.SubstrateIdReader;
using UnitySC.GUI.Common.UIComponents.Commands;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.UTO.Controller.Views.Panels.EquipmentHandling.Clean;
using UnitySC.UTO.Controller.Views.Panels.Gem;

using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.UTO.Controller.Views.Panels.EquipmentHandling.Equipment
{
    public class EquipmentHandlingPanelViewModel : BusinessPanel
    {
        #region Fields

        private UnitySC.Equipment.Devices.Controller.Controller _controller;
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        static EquipmentHandlingPanelViewModel()
        {
            DataTemplateGenerator.Create(typeof(EquipmentHandlingPanelViewModel), typeof(EquipmentHandlingPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(EquipmentHandlingResources)));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(GemGeneralRessources)));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(CleanResources)));
        }

        public EquipmentHandlingPanelViewModel()
            : this($"{nameof(EquipmentHandlingPanelViewModel)} DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public EquipmentHandlingPanelViewModel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            _logger = Logger.GetLogger("EquipmentHandling");

            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_INIT),
                    InitCommand,
                    PathIcon.Actives));

            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_CLEAN),
                    CleanCommand,
                    PathIcon.Clean));

            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_LOAD_PM),
                    LoadPMCommand,
                    PathIcon.ArrowUp));

            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_UNLOAD_PM),
                    UnloadPMCommand,
                    PathIcon.ArrowDown));

            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_ABORT),
                    AbortCommand,
                    PathIcon.Abort));
        }

        #endregion

        #region Override

        public override void OnSetup()
        {
            base.OnSetup();

            foreach (var device in App.UtoInstance.EquipmentManager.Equipment.AllDevices())
            {
                if (App.ControllerInstance.DeviceUiManagerService.GetEquipmentHandlingCardViewModel(device) is { } viewModel)
                {
                    switch (viewModel)
                    {
                        case LoadPortCardViewModel loadPortCardViewModel:
                            LoadPortCards.Add(loadPortCardViewModel);
                            break;
                        case EnhAlignerCardViewModel alignerCardViewModel:
                            AlignerCardViewModel = alignerCardViewModel;
                            AlignerCardViewModel.Setup(EquipmentManager.ProcessModules.Values.ToList());
                            break;
                        case RobotCardViewModel robotCardViewModel:
                            RobotCardViewModel = robotCardViewModel;
                            RobotCardViewModel.Setup();
                            break;
                        case IProcessModuleCardViewModel processModuleCardViewModel:
                            ProcessModuleCards.Add(processModuleCardViewModel);
                            break;
                        case SubstrateIdReaderCardViewModel substrateIdReaderCardViewModel:
                            if (device == EquipmentManager.SubstrateIdReaderFront)
                            {
                                SubstrateIdReaderFrontCardViewModel = substrateIdReaderCardViewModel;
                                SubstrateIdReaderFrontCardViewModel.Setup(EquipmentManager.Efem);
                            }

                            if (device == EquipmentManager.SubstrateIdReaderBack)
                            {
                                SubstrateIdReaderBackCardViewModel = substrateIdReaderCardViewModel;
                                SubstrateIdReaderBackCardViewModel.Setup(EquipmentManager.Efem);
                            }
                            break;
                    }
                }
            }

            if (InvertProcessModules)
            {
                ProcessModuleCards.Reverse();
            }

            MachineViewModel = new MachineViewModel(EquipmentManager, App.ControllerInstance.DeviceUiManagerService, this);
            MachineViewModel.SourceDestinationChanged += OnMachineViewModelSourceDestinationChanged;

            _controller = EquipmentManager.Controller as UnitySC.Equipment.Devices.Controller.Controller;
            if (_controller != null)
            {
                _controller.StatusValueChanged += Controller_StatusValueChanged;
            }
        }

        #endregion

        #region Properties

        public MachineViewModel MachineViewModel { get; set; }

        public ControllerEquipmentManager EquipmentManager
            => App.ControllerInstance.ControllerEquipmentManager;

        public bool IsMaintenanceMode
            => EquipmentManager.Controller.State == OperatingModes.Maintenance;

        public UserMessageDisplayer UserMessageDisplayer
            => App.ControllerInstance.MainUserMessageDisplayer;

        public bool InvertProcessModules
            => GUI.Common.App.Instance.Config.EquipmentConfig.InvertPmOnUserInterface;

        #region LP

        public List<LoadPortCardViewModel> LoadPortCards { get; } = new();

        #endregion

        #region Aligner

        public EnhAlignerCardViewModel AlignerCardViewModel { get; private set; }

        #endregion

        #region Robot

        public RobotCardViewModel RobotCardViewModel { get; private set; }

        #endregion

        #region Process module

        public List<IProcessModuleCardViewModel> ProcessModuleCards { get; } = new();

        #endregion

        #region Substrate ID Reader

        public SubstrateIdReaderCardViewModel SubstrateIdReaderBackCardViewModel
        {
            get;
            private set;
        }

        public bool IsSubstrateIdReaderBackAvailable => SubstrateIdReaderBackCardViewModel != null;

        public SubstrateIdReaderCardViewModel SubstrateIdReaderFrontCardViewModel
        {
            get;
            private set;
        }

        public bool IsSubstrateIdReaderFrontAvailable
            => SubstrateIdReaderFrontCardViewModel != null;

        #endregion

        #endregion

        #region Commands

        #region Init

        private ContextualSafeDelegateCommandAsync _initCommand;

        private ContextualSafeDelegateCommandAsync InitCommand
            => _initCommand ??= new ContextualSafeDelegateCommandAsync(
                InitCommandExecute,
                InitCommandCanExecute);

        private Task InitCommandExecute()
        {
            var task = Task.Run(
                () =>
                {
                    try
                    {
                        UserMessageDisplayer.HideAll();
                        EquipmentManager.Controller.Initialize(
                            !GUI.Common.App.Instance.Config.UseWarmInit);
                    }
                    catch (Exception e)
                    {
                        UserMessageDisplayer.Show(new UserMessage(MessageLevel.Error, e.Message));
                    }
                });
            return task;
        }

        private Tuple<bool, string> InitCommandCanExecute()
        {
            var canExecute = EquipmentManager.Controller.CanExecute(
                nameof(IController.Initialize),
                true,
                out var context,
                false);

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

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Clean

        private ContextualSafeDelegateCommand _cleanCommand;

        private ContextualSafeDelegateCommand CleanCommand
            => _cleanCommand ??= new ContextualSafeDelegateCommand(
                CleanCommandExecute,
                CleanCommandCanExecute);

        private void CleanCommandExecute()
        {
            _logger.Info("Clean setup started.");

            var cleanPopupViewModel = new CleanPopupViewModel(_controller, _logger);
            var popup = new Popup(new LocalizableText(nameof(CleanResources.CLEAN)))
            {
                Content = cleanPopupViewModel,
                IsFullScreen = true,
                Commands =
                {
                    new PopupCommand(
                        nameof(CleanResources.CLEAN_CANCEL),
                        new DelegateCommand(
                            () => _logger.Info("Cleaning procedure setup canceled by user."))),
                    new PopupCommand(
                        nameof(CleanResources.CLEAN_START),
                        new DelegateCommand(
                            () =>
                            {
                                Task.Run(
                                    () =>
                                    {
                                        try
                                        {
                                            UserMessageDisplayer.HideAll();
                                            _controller.Clean();
                                        }
                                        catch (Exception e)
                                        {
                                            UserMessageDisplayer.Show(
                                                new UserMessage(MessageLevel.Error, e.Message)
                                                {
                                                    CanUserCloseMessage = true
                                                });
                                        }
                                    });
                            },
                            () => CheckCleanCanBeExecute(cleanPopupViewModel)))
                }
            };

            Popups.Show(popup);
        }

        private Tuple<bool, string> CleanCommandCanExecute()
        {
            var canExecute = EquipmentManager.Controller.CanExecute(
                nameof(IController.Clean),
                false,
                out var context);

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

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        private bool CheckCleanCanBeExecute(CleanPopupViewModel cleanPopupViewModel)
        {
            if (!cleanPopupViewModel.SubstrateLocations.Any())
            {
                var message = new UserMessage(
                    MessageLevel.Warning,
                    new LocalizableText(nameof(CleanResources.CLEAN_NO_SUBSTRATE_TO_CLEAN)));
                cleanPopupViewModel.Messages.Show(message);
                return false;
            }

            //All substrate locations must be idle to allow clean
            if (cleanPopupViewModel.SubstrateLocations.Any(
                    x => x.Container is GenericDevice genericDevice
                         && genericDevice.State != OperatingModes.Idle))
            {
                var message = new UserMessage(
                    MessageLevel.Warning,
                    new LocalizableText(nameof(CleanResources.CLEAN_ALL_LOCATIONS_MUST_BE_IDLE)));
                cleanPopupViewModel.Messages.Show(message);
                return false;
            }

            //All source ports and slots must be set in order to allow clean
            if (cleanPopupViewModel.SubstrateLocations.Any(
                    x => x.Substrate != null
                         && (x.Substrate.SourcePort <= 0 || x.Substrate.SourceSlot <= 0)))
            {
                var message = new UserMessage(
                    MessageLevel.Warning,
                    new LocalizableText(
                        nameof(CleanResources.CLEAN_ALL_SUBSTRATE_MUST_HAVE_DESTINATION)));
                cleanPopupViewModel.Messages.Show(message);
                return false;
            }

            var filteredSubstrates = cleanPopupViewModel.SubstrateLocations.Select(
                    x => new { x.Substrate.SourcePort, x.Substrate.SourceSlot })
                .ToList();

            //All substrates must have a unique source port and source slot
            if (filteredSubstrates.Count != filteredSubstrates.Distinct().Count())
            {
                var message = new UserMessage(
                    MessageLevel.Warning,
                    new LocalizableText(
                        nameof(CleanResources.CLEAN_MANY_SUBSTRATES_SAME_DESTINATION)));
                cleanPopupViewModel.Messages.Show(message);
                return false;
            }

            if (cleanPopupViewModel.SubstrateLocations.Cast<WaferLocation>()
                .Any(w => w.Wafer.MaterialType == MaterialType.Unknown))
            {
                var message = new UserMessage(
                    MessageLevel.Warning,
                    new LocalizableText(
                        nameof(CleanResources.CLEAN_ALL_SUBSTRATE_MUST_HAVE_MATERIAL_TYPE)));
                cleanPopupViewModel.Messages.Show(message);
                return false;
            }

            foreach (var substrate in cleanPopupViewModel.SubstrateLocations.Select(
                         s => s.Substrate))
            {
                var loadPort = _controller.AllDevices<LoadPort>()
                    .FirstOrDefault(x => x.InstanceId == substrate.SourcePort);

                //All required load ports for clean command must be opened
                if (loadPort != null && (loadPort.PhysicalState != LoadPortState.Open
                                         && !loadPort.Configuration.CloseDoorAfterRobotAction))
                {
                    var message = new UserMessage(
                        MessageLevel.Warning,
                        new LocalizableText(
                            nameof(CleanResources.CLEAN_LOAD_PORTS_MUST_BE_OPENED)));
                    cleanPopupViewModel.Messages.Show(message);
                    return false;
                }

                //Slot must be empty to be able to clean
                if (loadPort != null
                    && loadPort.Carrier != null
                    && (loadPort.Carrier.MappingTable == null
                        || loadPort.Carrier.MappingTable[substrate.SourceSlot - 1]
                        != SlotState.NoWafer))
                {
                    var message = new UserMessage(
                        MessageLevel.Warning,
                        new LocalizableText(
                            nameof(CleanResources.CLEAN_LOAD_PORT_SLOT_NOT_AVAILABLE)));
                    cleanPopupViewModel.Messages.Show(message);
                    return false;
                }

                if (substrate.MaterialDimension == SampleDimension.NoDimension)
                {
                    var message = new UserMessage(
                        MessageLevel.Warning,
                        new LocalizableText(nameof(CleanResources.CLEAN_NO_SIZE_WARNING)));
                    cleanPopupViewModel.Messages.Show(message);
                    return false;
                }
            }

            cleanPopupViewModel.Messages.HideAll();
            return true;
        }

        #endregion

        #region Load

        private ContextualSafeDelegateCommandAsync _loadPMCommand;

        private ContextualSafeDelegateCommandAsync LoadPMCommand
            => _loadPMCommand ??=
                new ContextualSafeDelegateCommandAsync(LoadPMExecute, LoadPMCanExecute);

        private Task LoadPMExecute()
        {
            if (MachineViewModel.SelectedDestination is not DriveableProcessModule pm)
            {
                return Task.CompletedTask;
            }

            var task = Task.Run(
                () =>
                {
                    try
                    {
                        UserMessageDisplayer.HideAll();
                        _controller.LoadProcessModule(
                            MachineViewModel.SelectedSource,
                            MachineViewModel.SelectedSlotSource,
                            RobotCardViewModel.Arm,
                            Angle.FromDegrees(pm.GetAlignmentAngle()),
                            AlignerCardViewModel.AlignType,
                            RobotCardViewModel.Arm == RobotArm.Arm1
                                ? EquipmentManager.Robot.Configuration.UpperArm.EffectorType
                                : EquipmentManager.Robot.Configuration.LowerArm.EffectorType,
                            MachineViewModel.SelectedDestination);
                    }
                    catch (Exception e)
                    {
                        UserMessageDisplayer.Show(
                            new UserMessage(MessageLevel.Error, e.Message)
                            {
                                CanUserCloseMessage = true
                            });
                    }
                });
            return task;
        }

        private Tuple<bool, string> LoadPMCanExecute()
        {
            var context = EquipmentManager.Controller.NewCommandContext(
                nameof(IController.LoadProcessModule));
            context.AddArgument("loadPort", MachineViewModel.SelectedSource);
            context.AddArgument("sourceSlot", MachineViewModel.SelectedSlotSource);
            context.AddArgument("robotArm", RobotCardViewModel.Arm);
            context.AddArgument("alignAngle", Angle.FromDegrees(0));
            context.AddArgument("alignType", AlignerCardViewModel.AlignType);
            context.AddArgument("processModule", MachineViewModel.SelectedDestination);
            context.AddArgument(
                "effectorType",
                RobotCardViewModel.Arm == RobotArm.Arm1
                    ? EquipmentManager.Robot.Configuration.UpperArm.EffectorType
                    : EquipmentManager.Robot.Configuration.LowerArm.EffectorType);

            var canExecute = EquipmentManager.Controller.CanExecute(context);

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

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Unload

        private ContextualSafeDelegateCommandAsync _unloadPMCommand;

        private ContextualSafeDelegateCommandAsync UnloadPMCommand
            => _unloadPMCommand ??=
                new ContextualSafeDelegateCommandAsync(UnloadPMExecute, UnloadPMCanExecute);

        private Task UnloadPMExecute()
        {
            var task = Task.Run(
                () =>
                {
                    try
                    {
                        UserMessageDisplayer.HideAll();
                        _controller.UnloadProcessModule(
                            MachineViewModel.SelectedSource,
                            RobotCardViewModel.Arm,
                            RobotCardViewModel.Arm == RobotArm.Arm1
                                ? EquipmentManager.Robot.Configuration.UpperArm.EffectorType
                                : EquipmentManager.Robot.Configuration.LowerArm.EffectorType,
                            MachineViewModel.SelectedDestination,
                            MachineViewModel.SelectedSlotDestination);
                    }
                    catch (Exception e)
                    {
                        UserMessageDisplayer.Show(
                            new UserMessage(MessageLevel.Error, e.Message)
                            {
                                CanUserCloseMessage = true
                            });
                    }
                });
            return task;
        }

        private Tuple<bool, string> UnloadPMCanExecute()
        {
            var context = EquipmentManager.Controller.NewCommandContext(
                nameof(IController.UnloadProcessModule));
            context.AddArgument("loadPort", MachineViewModel.SelectedDestination);
            context.AddArgument("destinationSlot", MachineViewModel.SelectedSlotDestination);
            context.AddArgument("robotArm", RobotCardViewModel.Arm);
            context.AddArgument(
                "effectorType",
                RobotCardViewModel.Arm == RobotArm.Arm1
                    ? EquipmentManager.Robot.Configuration.UpperArm.EffectorType
                    : EquipmentManager.Robot.Configuration.LowerArm.EffectorType);
            context.AddArgument("processModule", MachineViewModel.SelectedSource);

            var canExecute = EquipmentManager.Controller.CanExecute(context);

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

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Abort

        private ContextualSafeDelegateCommandAsync _abortCommand;

        private ContextualSafeDelegateCommandAsync AbortCommand
            => _abortCommand ??= new ContextualSafeDelegateCommandAsync(
                AbortCommandExecute,
                AbortCommandCanExecute);

        private Task AbortCommandExecute()
        {
            return _controller.InterruptAsync(InterruptionKind.Abort);
        }

        private Tuple<bool, string> AbortCommandCanExecute()
        {
            var canExecute = true;
            var sb = new StringBuilder();

            if (_controller.CurrentActivity == null)
            {
                canExecute = false;
                sb.AppendLine("- No activity is running");
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

        #region Event Handlers

        private void Controller_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(UnitySC.Equipment.Devices.Controller.Controller.State))
            {
                OnPropertyChanged(nameof(IsMaintenanceMode));
            }
        }

        private void OnMachineViewModelSourceDestinationChanged(object sender, EventArgs e)
        {
            RobotCardViewModel.SelectedSource = MachineViewModel.SelectedSource;
            RobotCardViewModel.SelectedSlotSource = MachineViewModel.SelectedSlotSource;

            RobotCardViewModel.SelectedDestination = MachineViewModel.SelectedDestination;
            RobotCardViewModel.SelectedSlotDestination = MachineViewModel.SelectedSlotDestination;
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var loadPortCard in LoadPortCards)
                {
                    loadPortCard.Dispose();
                }

                if (_controller != null)
                {
                    _controller.StatusValueChanged -= Controller_StatusValueChanged;
                }

                if (MachineViewModel != null)
                {
                    MachineViewModel.SourceDestinationChanged -= OnMachineViewModelSourceDestinationChanged;
                    MachineViewModel.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
