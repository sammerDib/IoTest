using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.GUI.Common.Equipment.Popup;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Equipment.UnityDevice;

namespace UnitySC.GUI.Common.Equipment.ProcessModule
{
    public class ProcessModuleCardViewModel : UnityDeviceCardViewModel
    {
        #region Constructor

        public ProcessModuleCardViewModel(
            UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule processModule)
        {
            ProcessModule = processModule;
        }

        #endregion

        #region Properties

        private UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule _processModule;

        public UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule ProcessModule
        {
            get => _processModule;
            set => SetAndRaiseIfChanged(ref _processModule, value);
        }

        #endregion

        #region Commands

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
                })));
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));

            App.Instance.UserInterface.Navigation.SelectedBusinessPanel?.Popups.Show(popup);
        }

        private bool SetWaferPresenceCommandCanExecute() =>
            ProcessModule.State == OperatingModes.Idle || ProcessModule.State == OperatingModes.Maintenance;

        #endregion

        #endregion
    }
}
