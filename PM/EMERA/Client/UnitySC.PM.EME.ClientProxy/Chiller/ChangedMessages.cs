using UnitySC.PM.EME.Service.Interface.Chiller;

namespace UnitySC.PM.EME.Client.Proxy.Chiller
{
    public class CompressionChangedMessage
    {
        public double Compression { get; set; }
    }

    public class TemperatureChangedMessage
    {
        public double Temperature { get; set; }
    }

    public class FanSpeedModeChangedMessage
    {
        public ConstFanSpeedMode ConstFanSpeedMode { get; set; }
    }

    public class FanSpeedChangedMessage
    {
        public double FanSpeed { get; set; }
    }

    public class LeakDetectionChangedMessage
    {
        public LeakDetection Leak { get; set; }
    }

    public class AlarmChangedMessage
    {
        public AlarmDetection Alarm { get; set; }
    }

    public class ChillerModeChangedMessage
    {
        public ChillerMode Mode { get; set; }
    }
}
