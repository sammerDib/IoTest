using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Controller;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.GUI.Common.Equipment.Aligner;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.Equipment.ProcessModule;
using UnitySC.GUI.Common.Equipment.Robot;
using UnitySC.GUI.Common.Equipment.SubstrateIdReader;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

using ProcessModule = UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule;

namespace UnitySC.EFEM.Controller.Views.Panels.Main
{
    public class MainViewModel : BusinessPanel
    {
        static MainViewModel()
        {
            DataTemplateGenerator.Create(typeof(MainViewModel), typeof(MainView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(MainResources)));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(EquipmentResources)));
        }

        public MainViewModel()
            : this($"{nameof(MainViewModel)} DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public MainViewModel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            if (App.EfemAppInstance.EfemEquipmentManager is not { } efemEquipmentManager)
            {
                throw new InvalidOperationException(
                    "The current loaded equipment manager is not an EfemEquipmentManager");
            }

            EquipmentManager = efemEquipmentManager;

            var controlStateCommand = new BusinessPanelToggleCommand(
                nameof(MainResources.MAIN_CONTROLSTATE_COMMAND),
                new BusinessPanelCommand(
                    nameof(MainResources.MAIN_SWITCHTOLOCAL),
                    SwitchToLocalModeCommand,
                    IconFactory.PathGeometryFromRessourceKey("UserInterfaceControlIcon")),
                new BusinessPanelCommand(
                    nameof(MainResources.MAIN_SWITCHTOREMOTE),
                    SwitchToRemoteModeCommand,
                    IconFactory.PathGeometryFromRessourceKey("LanIcon")));
            // Makes the stop command visible
            controlStateCommand.IsChecked = _controlState == ControlState.Remote;
            Commands.Add(controlStateCommand);

            Commands.Add(
                new BusinessPanelCommand(
                    nameof(MainResources.MAIN_GLOBALINIT_COMMAND),
                    InitCommand,
                    PathIcon.Actives));
        }

        public override void OnSetup()
        {
            base.OnSetup();

            foreach (var device in EquipmentManager.Equipment.AllDevices())
            {
                var needBase = device is Aligner or ProcessModule;

                if (App.EfemAppInstance.DeviceUiManagerService.GetEquipmentHandlingCardViewModel(device, needBase) is { } viewModel)
                {
                    switch (viewModel)
                    {
                        case LoadPortCardViewModel loadPortCardViewModel:
                            LoadPortCards.Add(loadPortCardViewModel);
                            break;
                        case AlignerCardViewModel alignerCardViewModel:
                            AlignerCardViewModel = alignerCardViewModel;
                            break;
                        case RobotCardViewModel robotCardViewModel:
                            RobotCardViewModel = robotCardViewModel;
                            RobotCardViewModel.Setup();
                            break;
                        case ProcessModuleCardViewModel processModuleCardViewModel:
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

            MachineViewModel = new MachineViewModel(EquipmentManager, App.EfemAppInstance.DeviceUiManagerService, this);
            MachineViewModel.SourceDestinationChanged += OnMachineViewModelSourceDestinationChanged;

            Controller = EquipmentManager.Controller as UnitySC.Equipment.Devices.Controller.Controller;
            if (Controller != null)
            {
                Controller.PropertyChanged += Controller_PropertyChanged;
                Controller.CommandExecutionStateChanged += Controller_CommandExecutionStateChanged;
            }
        }

        #region Fields

        private ControlState _controlState => App.EfemAppInstance.ControlState;

        private HostDriver _hostDriver => App.EfemAppInstance.HostDriver;

        #endregion

        #region Properties
        public MachineViewModel MachineViewModel { get; set; }

        public bool IsViewEnable => _controlState == ControlState.Local;

        public EfemEquipmentManager EquipmentManager { get; }

        public UnitySC.Equipment.Devices.Controller.Controller Controller { get; private set; }

        public bool InvertProcessModules
            => GUI.Common.App.Instance.Config.EquipmentConfig.InvertPmOnUserInterface;

        #region LP

        public List<LoadPortCardViewModel> LoadPortCards { get; } = new();

        #endregion

        #region Aligner

        public AlignerCardViewModel AlignerCardViewModel { get; private set; }

        #endregion

        #region Robot

        public RobotCardViewModel RobotCardViewModel { get; private set; }

        #endregion

        #region Process module

        public List<ProcessModuleCardViewModel> ProcessModuleCards { get; } = new();

        #endregion

        #region Substrate ID Reader

        public SubstrateIdReaderCardViewModel SubstrateIdReaderBackCardViewModel { get; private set; }

        public bool IsSubstrateIdReaderBackAvailable => SubstrateIdReaderBackCardViewModel != null;

        public SubstrateIdReaderCardViewModel SubstrateIdReaderFrontCardViewModel { get; private set; }

        public bool IsSubstrateIdReaderFrontAvailable => SubstrateIdReaderFrontCardViewModel != null;

        #endregion

        #endregion

        #region Commands

        #region SwitchToLocalMode

        private ICommand _switchToLocalModeCommand;

        /// <summary>
        /// Switch to local control mode (meaning Host communication is stopped).
        /// </summary>
        public ICommand SwitchToLocalModeCommand => _switchToLocalModeCommand ??=
            new DelegateCommand(SwitchToLocalModeCommandExecuteMethod, SwitchToLocalModeCommandCanExecuteMethod);

        private void SwitchToLocalModeCommandExecuteMethod()
        {
            App.EfemAppInstance.SwitchControlStateMode(ControlState.Local);
            OnPropertyChanged(nameof(IsViewEnable));
        }

        private bool SwitchToLocalModeCommandCanExecuteMethod()
        {
            return _controlState != ControlState.Local && _hostDriver != null;
        }

        #endregion SwitchToLocalMode

        #region SwitchToRemoteMode

        private ICommand _switchToRemoteModeCommand;

        /// <summary>
        /// Switch to remote control mode (meaning Host communication is started).
        /// </summary>
        public ICommand SwitchToRemoteModeCommand => _switchToRemoteModeCommand ??=
            new DelegateCommand(SwitchToRemoteModeCommandExecuteMethod, SwitchToRemoteModeCommandCanExecuteMethod);

        private void SwitchToRemoteModeCommandExecuteMethod()
        {
            App.EfemAppInstance.SwitchControlStateMode(ControlState.Remote);
            OnPropertyChanged(nameof(IsViewEnable));
        }

        private bool SwitchToRemoteModeCommandCanExecuteMethod()
        {
            return _controlState != ControlState.Remote && _hostDriver != null;
        }

        #endregion SwitchToRemoteMode

        #region Init

        private SafeDelegateCommandAsync _initCommand;

        private SafeDelegateCommandAsync InitCommand
            => _initCommand ??= new SafeDelegateCommandAsync(InitCommandExecute, InitCommandCanExecute);

        private Task InitCommandExecute() => EquipmentManager.Controller.InitializeAsync(!GUI.Common.App.Instance.Config.UseWarmInit);

        private bool InitCommandCanExecute()
        {
            var context = EquipmentManager.Controller.NewCommandContext(nameof(EquipmentManager.Controller.Initialize));
            context.AddArgument("mustForceInit", !GUI.Common.App.Instance.Config.UseWarmInit);
            var canExecute = EquipmentManager.Controller.CanExecute(context) && _controlState == ControlState.Local;
            return canExecute;
        }

        #endregion

        #endregion

        #region Event handler

        private void Controller_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(IController.State)
                && Controller.State == OperatingModes.Maintenance
                && Messages != null)
            {
                Messages.Show(BuildUserMessage(EquipmentResources.EQUIPMENT_CONTROLLER_MAINTENANCE));
            }

            if (e.PropertyName == nameof(IController.State)
                && Controller.State != OperatingModes.Maintenance)
            {
                Messages?.HideAll();
            }
        }

        private void Controller_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (e.NewState != ExecutionState.Failed)
            {
                return;
            }

            Messages.Show(BuildUserMessage(e.ExceptionThrown.Message));
        }

        private void OnMachineViewModelSourceDestinationChanged(object sender, EventArgs e)
        {
            RobotCardViewModel.SelectedSource = MachineViewModel.SelectedSource;
            RobotCardViewModel.SelectedSlotSource = MachineViewModel.SelectedSlotSource;

            RobotCardViewModel.SelectedDestination = MachineViewModel.SelectedDestination;
            RobotCardViewModel.SelectedSlotDestination = MachineViewModel.SelectedSlotDestination;
        }

        #endregion

        #region Private 

        private UserMessage BuildUserMessage(string message)
        {
            var userMessage = new UserMessage(MessageLevel.Error, new InvariantText(message))
            {
                Icon = PathIcon.Maintenance,
            };

            userMessage.Commands.Add(new UserMessageCommand(nameof(EquipmentResources.EQUIPMENT_CANCEL))
            {
                CloseMessageAfterExecute = true
            });

            return userMessage;
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing && MachineViewModel != null)
            {
                MachineViewModel.SourceDestinationChanged -= OnMachineViewModelSourceDestinationChanged;
                MachineViewModel.Dispose();
            }
           
            base.Dispose(disposing);
        }

        #endregion
    }
}
