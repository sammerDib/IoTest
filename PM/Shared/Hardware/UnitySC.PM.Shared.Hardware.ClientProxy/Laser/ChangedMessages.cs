using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Laser
{
    public class StateChangedMessage
    {
        public DeviceState State;
    }

    public class PowerChangedMessage
    {
        public double Power;
    }

    public class InterlockStatusChangedMessage
    {
        public string InterlockStatus;
    }

    public class LaserTemperatureChangedMessage
    {
        public double LaserTemperature;
    }

    public class PsuTemperatureChangedMessage
    {
        public double PsuTemperature;
    }

    public class CrystalTemperatureChangedMessage
    {
        public double CrystalTemperature;
    }

    public class LaserPowerStatusChangedMessage
    {
        public bool LaserPowerStatus;
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
