using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Axes
{
    public class DeviceStateChangedMessage
    {
        public DeviceState State;
    }

    public class IdChangedMessage
    {
        public string Id;
    }

    public class CustomChangedMessage
    {
        public string Custom;
    }
}
