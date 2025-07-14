using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;

using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.GUI.Common.Equipment.Popup;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

namespace UnitySC.GUI.Common.Equipment.DriveableProcessModule
{
    public abstract class DriveableProcessModuleCardViewModel<T> : UnityDeviceCardViewModel, IProcessModuleCardViewModel where T : UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.DriveableProcessModule
    {
        #region Constructor

        protected DriveableProcessModuleCardViewModel(T processModule)
        {
            ProcessModule = processModule;
            ProcessModule.CommandExecutionStateChanged += ProcessModule_CommandExecutionStateChanged;
        }

        #endregion

        #region Properties

        private T _processModule;

        public T ProcessModule
        {
            get => _processModule;
            set => SetAndRaiseIfChanged(ref _processModule, value);
        }

        #endregion

        #region Commands

        #region Init

        private SafeDelegateCommandAsync _initializeCommand;

        public SafeDelegateCommandAsync InitializeCommand
            => _initializeCommand ??= new SafeDelegateCommandAsync(
                InitializeCommandExecute,
                InitializeCommandCanExecute);

        private Task InitializeCommandExecute() => ProcessModule.InitializeAsync(false);

        private bool InitializeCommandCanExecute()
        {
            if (ProcessModule == null)
            {
                return false;
            }

            var context = ProcessModule.NewCommandContext(nameof(ProcessModule.Initialize))
                .AddArgument("mustForceInit", false);
            return ProcessModule.CanExecute(context);
        }

        #endregion

        #region Set Wafer Presence

        private SafeDelegateCommand _setWaferPresenceCommand;

        public SafeDelegateCommand SetWaferPresenceCommand
            => _setWaferPresenceCommand ??= new SafeDelegateCommand(
                SetWaferPresenceCommandExecute,
                SetWaferPresenceCommandCanExecute);

        private void SetWaferPresenceCommandExecute()
        {
            var popupContent = new SetWaferPresencePopup(ProcessModule.Location);
            var popup = new Agileo.GUI.Services.Popups.Popup(new LocalizableText(nameof(EquipmentResources.POPUP_SET_WAFER_PRESENCE)))
            {
                Content = popupContent
            };

            popup.Commands.Add(new PopupCommand(Agileo.GUI.Properties.Resources.S_OK,
                new DelegateCommand(() =>
                {
                    popupContent.ValidateModifications();
                    ProcessModule.CheckSubstrateDetectionError(true);
                })));
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));

            AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel?.Popups.Show(popup);
        }

        private bool SetWaferPresenceCommandCanExecute()
            => ProcessModule.ProcessModuleState == ProcessModuleState.Idle
               || ProcessModule.ProcessModuleState == ProcessModuleState.Error;

        #endregion

        #endregion

        #region Event Handlers

        private void ProcessModule_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            switch (e.NewState)
            {
                case ExecutionState.Success:
                    if (e.Execution.Context.Command.Name == nameof(IGenericDevice.Initialize))
                    {
                        ProcessModule.CheckSubstrateDetectionError();
                    }
                    break;
            }
        }

        #endregion
    }

    public interface IProcessModuleCardViewModel
    {
        SafeDelegateCommandAsync InitializeCommand { get; }

        SafeDelegateCommand SetWaferPresenceCommand { get; }
    }
}
