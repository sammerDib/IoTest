namespace UnitySC.PM.Shared.Hardware.Service.Interface.Laser.Leukos
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

    public class PeltierRegulationStatusMessage
    {
        public bool PeltierRegulationStatus;
    }

    public class LaserHeadComStatusMessage
    {
        public bool LaserHeadComStatus;
    }

    public class LaserDiodeStatusMessage
    {
        public int LaserDiodeStatus;
    }

    public class RunningTimeLaserOnMessage
    {
        public double RunningTimeLaserOn;
    }

    public class RunningTimeElectroOnMessage
    {
        public double RunningTimeElectroOn;
    }

    public class CrystalTemperatureMessage
    {
        public double CrystalTemperature;
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
