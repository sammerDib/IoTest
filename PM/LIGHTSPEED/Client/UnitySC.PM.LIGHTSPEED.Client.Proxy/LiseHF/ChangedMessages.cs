using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.LIGHTSPEED.Client.Proxy.LiseHF
{ 
    public class LaserPowerStatusChangedMessage
    {
        public bool LaserPowerOn;
    }

    public class InterlockStatusChangedMessage
    {
        public string InterlockStatus;
    }

    public class LaserTemperatureChangedMessage
    {
        public double LaserTemperature;
    }

    public class CrystalTemperatureChangedMessage
    {
        public double CrystalTemperature;
    }

    public class AttenuationPositionChangedMessage
    {
        public double AttenuationPosition;
    }

    public class FastAttenuationPositionChangedMessage
    {
        public double FastAttenuationPosition;
    }

    public class ShutterIrisPositionChangedMessages
    {
        public string ShutterIrisPosition;
    }
}
