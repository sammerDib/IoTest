using System.Windows.Input;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Equipment.UnityDevice
{
    public class UnityDeviceCardViewModel : NotifyDataError
    {
        #region ToggleDeviceConnectCommand

        private ICommand _toggleDeviceConnectCommand;

        public ICommand ToggleDeviceConnectCommand
            => _toggleDeviceConnectCommand ??= new SafeDelegateCommand<object>(ToggleDeviceConnectExecute);

        #endregion

        private static void ToggleDeviceConnectExecute(object o)
        {
            if (o is not UnityCommunicatingDevice communicatingDevice)
            {
                return;
            }

            if (communicatingDevice.IsCommunicationStarted)
            {
                DisconnectDeviceExecute(o);
            }
            else
            {
                ConnectDeviceExecute(o);
            }
        }

        private static void ConnectDeviceExecute(object obj)
        {
            if (obj is UnityCommunicatingDevice { IsCommunicationStarted: false } communicatingDevice)
            {
                communicatingDevice.StartCommunicationAsync();
            }
        }

        private static void DisconnectDeviceExecute(object obj)
        {
            if (obj is UnityCommunicatingDevice { IsCommunicating: true } communicatingDevice)
            {
                communicatingDevice.StopCommunicationAsync();
            }
        }
    }
}
