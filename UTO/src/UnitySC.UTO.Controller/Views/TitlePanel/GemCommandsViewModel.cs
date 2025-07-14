using System;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.UTO.Controller.Remote.Services;

namespace UnitySC.UTO.Controller.Views.TitlePanel
{
    public class GemCommandsViewModel : Notifier, IDisposable
    {
        private ControlServices ControlServices => IsInDesignMode ? null : App.ControllerInstance.GemController?.ControlServices;

        public GemCommandsViewModel()
        {
            ClosePopupCommand = new DelegateCommand<object>(ClosePopupCommandExecute);
            EnableCommand = new DelegateCommand(EnableCommandExecute, GemControlCommandCanExecute);
            DisableCommand = new DelegateCommand(DisableCommandExecute, GemControlCommandCanExecute);
            OnLineCommand = new DelegateCommand(OnLineCommandExecute, GemControlCommandCanExecute);
            OffLineCommand = new DelegateCommand(OffLineCommandExecute, OffLineCommandCanExecute);
            LocalCommand = new DelegateCommand(LocalCommandExecute, LocalRemoteCommandCanExecute);
            RemoteCommand = new DelegateCommand(RemoteCommandExecute, LocalRemoteCommandCanExecute);
        }

        private bool GemControlCommandCanExecute()
        {
            if (IsInDesignMode)
            {
                return true;
            }

            if (App.ControllerInstance.GemController == null || !App.ControllerInstance.GemController.IsSetupDone)
            {
                return false;
            }

            return App.ControllerInstance.GemController?.ControlServices != null;
        }

        #region Properties

        /// <summary>
        /// Defined if the Enable button is activated
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                switch (ControlServices?.CommunicationState)
                {
                    case CommunicationState.Enabled:
                    case CommunicationState.NotCommunicating:
                    case CommunicationState.Communicating:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Defined if the Disable button is activated
        /// </summary>
        public bool IsDisabled => ControlServices?.CommunicationState == CommunicationState.Disabled;

        /// <summary>
        /// Defined if the On-line button is activated
        /// </summary>
        public bool IsOnLine
        {
            get
            {
                switch (ControlServices?.ControlState)
                {
                    case ControlState.OnLine:
                    case ControlState.AttemptOnLine:
                    case ControlState.Local:
                    case ControlState.Remote:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Defined if the Off-line button is activated
        /// </summary>
        public bool IsOffLine => !IsOnLine;

        /// <summary>
        /// Defined if the Local button is activated
        /// </summary>
        public bool IsLocal => ControlServices?.ControlState == ControlState.Local;

        /// <summary>
        /// Defined if the Remote button is activated
        /// </summary>
        public bool IsRemote => ControlServices?.ControlState == ControlState.Remote;

        private bool _enableInProgress;

        public bool EnableInProgress
        {
            get => _enableInProgress;
            set => SetAndRaiseIfChanged(ref _enableInProgress, value);
        }

        private bool _disableInProgress;

        public bool DisableInProgress
        {
            get => _disableInProgress;
            set => SetAndRaiseIfChanged(ref _disableInProgress, value);
        }

        private bool _onLineInProgress;

        public bool OnLineInProgress
        {
            get => _onLineInProgress;
            set => SetAndRaiseIfChanged(ref _onLineInProgress, value);
        }

        private bool _offLineInProgress;

        public bool OffLineInProgress
        {
            get => _offLineInProgress;
            set => SetAndRaiseIfChanged(ref _offLineInProgress, value);
        }

        private bool _localInProgress;

        public bool LocalInProgress
        {
            get => _localInProgress;
            set => SetAndRaiseIfChanged(ref _localInProgress, value);
        }

        private bool _remoteInProgress;
        public bool RemoteInProgress
        {
            get => _remoteInProgress;
            set => SetAndRaiseIfChanged(ref _remoteInProgress, value);
        }

        #endregion

        #region Commands

        public ICommand ClosePopupCommand { get; }

        // Do not use Command<Popup> because the designer passes an incompatible object as a parameter
        private static void ClosePopupCommandExecute(object arg)
        {
            if (arg is Popup popup && popup.IsOpen)
            {
                popup.IsOpen = false;
            }
        }

        /// <summary>
        /// Enable equipment communication with the remote connection.
        /// </summary>
        public ICommand EnableCommand { get; }

        private void EnableCommandExecute()
        {
            if (IsEnabled || EnableInProgress || DisableInProgress)
            {
                return;
            }

            EnableInProgress = true;
            App.ControllerInstance.GemController.ControlServices.SetCommunicationStateEnable();
        }

        /// <summary>
        /// Disable equipment communication with the remote connection.
        /// </summary>
        public ICommand DisableCommand { get; }

        private void DisableCommandExecute()
        {
            if (IsDisabled || EnableInProgress || DisableInProgress)
            {
                return;
            }

            DisableInProgress = true;
            App.ControllerInstance.GemController.ControlServices.SetCommunicationStateDisable();
        }

        /// <summary>
        /// Switch to Online mode (equipment can be controlled by the host).
        /// </summary>
        public ICommand OnLineCommand { get; }

        private void OnLineCommandExecute()
        {
            if (IsOnLine)
            {
                return;
            }

            OnLineInProgress = true;
            App.ControllerInstance.GemController.ControlServices.SetEquipmentStateOnLine();
        }

        /// <summary>
        /// Switch to Offline mode (equipment controlled from the operator console).
        /// </summary>
        public ICommand OffLineCommand { get; }

        private void OffLineCommandExecute()
        {
            if (IsOffLine)
            {
                return;
            }

            OffLineInProgress = true;
            App.ControllerInstance.GemController.ControlServices.SetEquipmentStateOffLine();
        }

        private bool OffLineCommandCanExecute()
        {
            if (!GemControlCommandCanExecute())
            {
                return false;
            }

            switch (ControlServices?.ControlState)
            {
                case ControlState.HostOffLine:
                case ControlState.EquipmentOffLine:
                case ControlState.OnLine:
                case ControlState.Local:
                case ControlState.Remote:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Switch to Online mode (equipment can be controlled by the host).
        /// </summary>
        public ICommand LocalCommand { get; }

        private void LocalCommandExecute()
        {
            if (IsLocal)
            {
                return;
            }

            LocalInProgress = true;
            App.ControllerInstance.GemController.ControlServices.SetLocalMode();
        }

        /// <summary>
        /// Switch to Offline mode (equipment controlled from the operator console).
        /// </summary>
        public ICommand RemoteCommand { get; }

        private void RemoteCommandExecute()
        {
            if (IsRemote)
            {
                return;
            }

            RemoteInProgress = true;
            App.ControllerInstance.GemController.ControlServices.SetRemoteMode();
        }

        private bool LocalRemoteCommandCanExecute()
        {
            if (!GemControlCommandCanExecute())
            {
                return false;
            }

            switch (ControlServices?.ControlState)
            {
                case ControlState.OnLine:
                case ControlState.Local:
                case ControlState.Remote:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Handlers

        private void ControlServices_OnCommunicationStateChanged(object sender, CommunicationStateChangedEventArgs e)
        {
            EnableInProgress = false;
            DisableInProgress = false;

            OnPropertyChanged(nameof(IsEnabled));
            OnPropertyChanged(nameof(IsDisabled));
        }

        private void ControlServices_OnControlStateChanged(object sender, ControlStateChangedEventArgs e)
        {
            OnLineInProgress = false;
            OffLineInProgress = false;
            LocalInProgress = false;
            RemoteInProgress = false;

            OnPropertyChanged(nameof(IsOnLine));
            OnPropertyChanged(nameof(IsOffLine));
            OnPropertyChanged(nameof(IsLocal));
            OnPropertyChanged(nameof(IsRemote));
        }

        #endregion

        public void OnSetup()
        {
            if (App.ControllerInstance.GemController == null || !App.ControllerInstance.GemController.IsSetupDone)
            {
                return;
            }

            App.ControllerInstance.GemController.ControlServices.OnCommunicationStateChanged +=
                ControlServices_OnCommunicationStateChanged;
            App.ControllerInstance.GemController.ControlServices.OnControlStateChanged += ControlServices_OnControlStateChanged;

            OnPropertyChanged(null);
        }

        #region IDisposable

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    App.ControllerInstance.GemController.ControlServices.OnCommunicationStateChanged -=
                        ControlServices_OnCommunicationStateChanged;
                    App.ControllerInstance.GemController.ControlServices.OnControlStateChanged -= ControlServices_OnControlStateChanged;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Application Commands

        public ApplicationCommand CommunicationApplicationCommand { get; private set; }

        public ApplicationCommand EquipmentApplicationCommand { get; private set; }

        public ApplicationCommand ControlApplicationCommand { get; private set; }

        public bool GemPopupCanBeOpen
        {
            get
            {
                var isGemEnabled = App.ControllerInstance?.IsGemEnabled;
                if (!isGemEnabled.HasValue || !isGemEnabled.Value)
                {
                    return false;
                }

                if (CommunicationApplicationCommand?.IsVisible == true
                    || CommunicationApplicationCommand?.IsEnabled == true)
                {
                    return true;
                }

                if (EquipmentApplicationCommand?.IsVisible == true || EquipmentApplicationCommand?.IsEnabled == true)
                {
                    return true;
                }

                if (ControlApplicationCommand?.IsVisible == true || ControlApplicationCommand?.IsEnabled == true)
                {
                    return true;
                }

                return false;
            }
        }

        public void InitilizeApplicationCommands(
            ApplicationCommand communicationCommand,
            ApplicationCommand equipmentCommand,
            ApplicationCommand controlCommand)
        {
            CommunicationApplicationCommand = communicationCommand;
            EquipmentApplicationCommand = equipmentCommand;
            ControlApplicationCommand = controlCommand;
        }

        public void RefreshAccessibility() => OnPropertyChanged(nameof(GemPopupCanBeOpen));

        #endregion
    }
}
