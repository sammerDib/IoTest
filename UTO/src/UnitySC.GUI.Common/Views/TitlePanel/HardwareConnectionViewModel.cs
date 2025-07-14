using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

namespace UnitySC.GUI.Common.Views.TitlePanel
{
    public class HardwareConnectionViewModel : Notifier, IDisposable
    {

        #region Event Handlers

        private void Device_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(UnityCommunicatingDevice.IsCommunicating))
            {
                OnPropertyChanged(nameof(AllDevicesAreConnected));
            }
        }

        #endregion

        #region Constructor

        public HardwareConnectionViewModel(IEnumerable<UnityCommunicatingDevice> communicatingDevices)
        {
            LoadCommunicatingDevices(communicatingDevices);
        }

        #endregion

        #region Properties

        public ObservableCollection<UnityCommunicatingDevice> CommunicatingDevices { get; } = new();

        public bool AllDevicesAreConnected => CommunicatingDevices.All(d => d.IsCommunicating);

        private bool _isCommunicatingDevicesPopupVisible;

        public bool IsCommunicatingDevicesPopupVisible
        {
            get => _isCommunicatingDevicesPopupVisible;
            set => SetAndRaiseIfChanged(ref _isCommunicatingDevicesPopupVisible, value);
        }

        #endregion

        #region Commands

        #region OpenDevicesStateViewerCommand

        private ICommand _openDevicesStateViewerCommand;

        public ICommand OpenDevicesStateViewerCommand
            => _openDevicesStateViewerCommand ??= new SafeDelegateCommand(
                () =>
                {
                    IsCommunicatingDevicesPopupVisible = !IsCommunicatingDevicesPopupVisible;
                });

        #endregion

        #region CloseDevicesStateViewerCommand

        private ICommand _closeDevicesStateViewerCommand;

        public ICommand CloseDevicesStateViewerCommand
            => _closeDevicesStateViewerCommand ??= new SafeDelegateCommand(
                () => IsCommunicatingDevicesPopupVisible = false);

        #endregion

        #region ConnectDeviceConnectCommand

        private ICommand _connectDeviceConnectCommand;

        public ICommand ConnectDeviceConnectCommand
            => _connectDeviceConnectCommand ??= new SafeDelegateCommand<object>(ConnectDeviceExecute);

        #endregion

        #region DisconnectDeviceConnectCommand

        private ICommand _disconnectDeviceConnectCommand;

        public ICommand DisconnectDeviceConnectCommand
            => _disconnectDeviceConnectCommand ??= new SafeDelegateCommand<object>(DisconnectDeviceExecute);

        #endregion

        #region ToggleDeviceConnectCommand

        private ICommand _toggleDeviceConnectCommand;

        public ICommand ToggleDeviceConnectCommand
            => _toggleDeviceConnectCommand ??= new SafeDelegateCommand<object>(ToggleDeviceConnectExecute);

        #endregion

        #endregion

        #region Private methods

        private void LoadCommunicatingDevices(IEnumerable<UnityCommunicatingDevice> communicatingDevices)
        {
            foreach (var communicatingDevice in communicatingDevices)
            {
                CommunicatingDevices.Add(communicatingDevice);
                communicatingDevice.StatusValueChanged += Device_StatusValueChanged;
            }
        }

        private static void ToggleDeviceConnectExecute(object o)
        {
            if (o is UnityCommunicatingDevice communicatingDevice)
            {
                if (communicatingDevice.IsCommunicationStarted)
                {
                    DisconnectDeviceExecute(o);
                }
                else
                {
                    ConnectDeviceExecute(o);
                }
            }
        }

        private static void ConnectDeviceExecute(object obj)
        {
            if (obj is UnityCommunicatingDevice communicatingDevice)
            {
                if (!communicatingDevice.IsCommunicationStarted)
                {
                    communicatingDevice.StartCommunicationAsync();
                }

                App.Instance.GetLogger(nameof(Vendor.Views.TitlePanel.TitlePanel))
                    .Info(
                        "Command Connect was clicked by operator for device {Device}",
                        communicatingDevice.Name);
            }
        }

        private static void DisconnectDeviceExecute(object obj)
        {
            if (obj is UnityCommunicatingDevice communicatingDevice)
            {
                if (communicatingDevice.IsCommunicating)
                {
                    communicatingDevice.StopCommunicationAsync();
                }

                App.Instance.GetLogger(nameof(Vendor.Views.TitlePanel.TitlePanel))
                    .Info(
                        "Command Disconnect was clicked by operator for device {Device}",
                        communicatingDevice.Name);
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (var communicatingDevice in CommunicatingDevices)
            {
                communicatingDevice.StatusValueChanged -= Device_StatusValueChanged;
            }
        }

        #endregion
    }
}
