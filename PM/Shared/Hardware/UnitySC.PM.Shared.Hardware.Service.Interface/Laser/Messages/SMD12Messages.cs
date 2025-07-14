namespace UnitySC.PM.Shared.Hardware.Service.Interface.Laser.LaserQuantum
{
    public class StateMessage
    {
        public DeviceState State;
    }

    public class PowerMessage
    {
        public double Power;
    }

    public class LaserStatusMessage
    {
        public bool LaserStatus;
    }

    public class InterlockStatusMessage
    {
        public string InterlockStatus;
    }

    public class LaserTemperatureStatusMessage
    {
        public bool LaserTemperatureStatus;
    }

    public class PsuTemperatureMessage
    {
        public double PsuTemperature;
    }

    public class LaserTemperatureMessage
    {
        public double LaserTemperature;
    }

    public class IdMessage
    {
        public string Id;
    }

    public class CustomMessage
    {
        public string Custom;
    }

    public class PowerOnMessage
    { }

    public class PowerOffMessage
    { }

    public class PowerSetpointMessage
    {
        public int PowerSetpoint;
    }
}
