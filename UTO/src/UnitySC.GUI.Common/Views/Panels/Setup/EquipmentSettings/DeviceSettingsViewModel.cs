using System;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Views.Panels.Setup.EquipmentSettings
{
    public class DeviceSettingsViewModel : Notifier
    {
        #region Fields

        private readonly Device _device;

        #endregion

        #region Constructor

        public DeviceSettingsViewModel(Device device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (device.DeviceType == null)
            {
                throw new ArgumentException(@"DeviceType must not be null", nameof(device));
            }

            _device = device;
            Devices = new ObservableCollection<DeviceSettingsViewModel>(
                _device.Devices.Select(d => new DeviceSettingsViewModel(d)));
        }

        #endregion

        #region Properties

        public string Name => _device.Name;

        public int InstanceId => _device.InstanceId;

        public ObservableCollection<DeviceSettingsViewModel> Devices { get; }

        public ExecutionMode ExecutionMode
        {
            get => _device.ExecutionMode;
            set
            {
                _device.ExecutionMode = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public void Refresh()
        {
            OnPropertyChanged();
        }
    }
}
