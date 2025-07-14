using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.DistanceSensor
{
    public class StateChangedMessage
    {
        public DeviceState State;
    }

    public class DistanceChangedMessages
    {
        public double Distance;
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
