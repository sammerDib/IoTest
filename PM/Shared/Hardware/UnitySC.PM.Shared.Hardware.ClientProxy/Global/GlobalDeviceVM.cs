using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface.Global;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Global
{
    public class GlobalDeviceVM : ObservableObject
    {
        private IMessenger _messenger;

        public GlobalDeviceVM(IMessenger messenger)
        {
            _messenger = messenger;
        }

        private List<GlobalDevice> _devices;

        public List<GlobalDevice> Devices
        {
            get => _devices; set { if (_devices != value) { _devices = value; OnPropertyChanged(); } }
        }

        private GlobalDevice _selectedDevice;

        public GlobalDevice SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if ((value != null) && (_selectedDevice != value))
                {
                    _selectedDevice = value;
                    OnPropertyChanged();
                    _messenger.Send(_selectedDevice);
                }
            }
        }

        public void Update(List<GlobalDevice> devices)
        {
            Devices = devices;
        }
    }
}
