using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.Mppc
{
    public class StateMessage
    {
        public MppcCollector Collector;
        public string State;
    }

    public class MonitorInfoStatusMessage
    {
        public MppcCollector Collector;
        public string MonitorInfoStatus;
    }

    public class OutputCurrentMessage
    {
        public MppcCollector Collector;
        public double OutputCurrent;
    }

    public class OutputVoltageMessage
    {
        public MppcCollector Collector;
        public double OutputVoltage;
    }

    public class OutputVoltageSettingMessage
    {
        public MppcCollector Collector;
        public double OutputVoltageSetting;
    }

    public class HighVoltageStatusMessage
    {
        public MppcCollector Collector;
        public string HighVoltageStatus;
    }

    public class StateSignalsMessage
    {
        public MppcCollector Collector;
        public MppcStateModule StateSignals;
    }

    public class TemperatureMessage
    {
        public MppcCollector Collector;
        public double Temperature;
    }

    public class SensorTemperatureMessage
    {
        public MppcCollector Collector;
        public double SensorTemperature;
    }

    public class PowerFctReadMessage
    {
        public MppcCollector Collector;
        public double PowerFctRead;
    }

    public class TempCorrectionFactorReadMessage
    {
        public MppcCollector Collector;
        public double TempCorrectionFactorRead;
    }

    public class FirmwareMessage
    {
        public MppcCollector Collector;
        public string Firmware;
    }

    public class IdentifierMessage
    {
        public MppcCollector Collector;
        public string Identifier;
    }
}
