using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Ffu
{
    public class StateChangedMessage
    {
        public DeviceState State;
    }

    public class StatusChangedMessage
    {
        public string Status;
    }

    public class CurrentSpeedChangedMessage
    {
        public ushort CurrentSpeed;
    }

    public class TemperatureChangedMessage
    {
        public double Temperature;
    }

    public class WarningChangedMessage
    {
        public bool Warning;
    }

    public class AlarmChangedMessage
    {
        public bool Alarm;
    }

    public class CustomChangedMessage
    {
        public string Custom;
    }
}
