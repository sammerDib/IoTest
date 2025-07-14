using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.GUI.Common.Equipment.Popup;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Equipment.Aligner
{
    public class AlignerCardViewModel : UnityDeviceCardViewModel
    {
        #region Constructor

        static AlignerCardViewModel()
        {
            DataTemplateGenerator.Create(typeof(AlignerCardViewModel), typeof(AlignerCard));
        }

        public AlignerCardViewModel(UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner aligner)
        {
            Aligner = aligner;
        }

        #endregion

        #region Properties

        private UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner _aligner;
        public UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner Aligner
        {
            get => _aligner;
            set => SetAndRaiseIfChanged(ref _aligner, value);
        }

        private Angle _currentAngle;

        public double CurrentAngleAsDouble
        {
            get => _currentAngle.Degrees;
            set
            {
                _currentAngle = Angle.FromDegrees(value);
                SetAndRaiseIfChanged(ref _currentAngle, _currentAngle);
            }
        }

        public Angle CurrentAngle => _currentAngle;


        private AlignType _alignType;
        public AlignType AlignType
        {
            get => _alignType;
            set => SetAndRaiseIfChanged(ref _alignType, value);
        }
        #endregion

        #region Command

        #region Align

        private SafeDelegateCommandAsync _alignCommand;

        public SafeDelegateCommandAsync AlignCommand
            => _alignCommand ??= new SafeDelegateCommandAsync(
                AlignCommandExecute,
                AlignCommandCanExecute);

        private Task AlignCommandExecute() => Aligner.AlignAsync(_currentAngle, AlignType);

        private bool AlignCommandCanExecute()
        {
            if (Aligner == null)
            {
                return false;
            }

            var context = Aligner.NewCommandContext(nameof(Aligner.Align))
                .AddArgument("target", _currentAngle)
                .AddArgument("alignType", AlignType);
            return Aligner.CanExecute(context);
        }

        #endregion

        #region Init

        private SafeDelegateCommandAsync _initCommand;

        public SafeDelegateCommandAsync InitCommand
            => _initCommand ??= new SafeDelegateCommandAsync(
                InitProcessModuleCommandExecute,
                InitProcessModuleCommandCanExecute);

        private Task InitProcessModuleCommandExecute() => Aligner.InitializeAsync(false);

        private bool InitProcessModuleCommandCanExecute()
        {
            if (Aligner == null)
            {
                return false;
            }

            var context = Aligner.NewCommandContext(nameof(Aligner.Initialize))
                .AddArgument("mustForceInit", false);
            return Aligner.CanExecute(context);
        }

        #endregion

        #region Abort

        private SafeDelegateCommandAsync _abortCommand;

        public SafeDelegateCommandAsync AbortCommand
            => _abortCommand ??= new SafeDelegateCommandAsync(
                AbortCommandExecute,
                AbortCommandCanExecute);

        private Task AbortCommandExecute() => Aligner.InterruptAsync(InterruptionKind.Abort);

        private bool AbortCommandCanExecute()
        {
            if (Aligner == null)
            {
                return false;
            }

            return Aligner.State != OperatingModes.Maintenance
                   && Aligner.State != OperatingModes.Idle;
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
            var popupContent = new SetWaferPresencePopup(Aligner.Location);
            var popup = new Agileo.GUI.Services.Popups.Popup(new LocalizableText(nameof(EquipmentResources.POPUP_SET_WAFER_PRESENCE)))
            {
                Content = popupContent
            };

            popup.Commands.Add(new PopupCommand(Agileo.GUI.Properties.Resources.S_OK,
                new DelegateCommand(() =>
                {
                    popupContent.ValidateModifications();
                    Aligner.CheckSubstrateDetectionError(true);
                })));
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));

            App.Instance.UserInterface.Navigation.SelectedBusinessPanel?.Popups.Show(popup);
        }

        private bool SetWaferPresenceCommandCanExecute() =>
            Aligner.State == OperatingModes.Idle || Aligner.State == OperatingModes.Maintenance;

        #endregion

        #region Centering

        private SafeDelegateCommandAsync _centeringCommand;

        public SafeDelegateCommandAsync CenteringCommand
            => _centeringCommand ??= new SafeDelegateCommandAsync(
                CenteringCommandExecute,
                CenteringCommandCanExecute);

        private Task CenteringCommandExecute() => Aligner.CenteringAsync();

        private bool CenteringCommandCanExecute()
        {
            if (Aligner == null)
            {
                return false;
            }

            var context = Aligner.NewCommandContext(nameof(Aligner.Centering));
            return Aligner.CanExecute(context);
        }

        #endregion

        #endregion
    }
}
