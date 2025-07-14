using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.LIGHTSPEED.Client.Proxy.FeedbackLoop
{
    public class PowerChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double Power;
        public double PowerCal_mW;
        public double RFactor;
    }

    public class WavelengthChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public uint Wavelength;
    }

    public class BeamDiameterChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public uint BeamDiameter;
    }

    public class PowerLaserChangedMessage
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

    public class ShutterIrisPositionChangedMessages
    {
        public string ShutterIrisPosition;
    }

    public class AttenuationPositionChangedMessages
    {
        public double AttenuationPosition;
    }
}
