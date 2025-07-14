namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ffu
{
    public class StateMessage
    {
        public DeviceState State;
    }

    public class StatusMessage
    {
        public string Status;
    }

    public class CurrentSpeedMessage
    {
        public ushort CurrentSpeed_percentage;
    }

    public class TemperatureMessage
    {
        public double Temperature;
    }

    public class WarningMessage
    {
        public bool Triggered { get; set; }

        public override string ToString()
        {
            return $"Warning message triggered:{Triggered}";
        }
    }

    public class AlarmMessage
    {
        public bool Triggered { get; set; }

        public override string ToString()
        {
            return $"Alarm message triggered:{Triggered}";
        }
    }

    public class CustomMessage
    {
        public string Custom;
    }
}
