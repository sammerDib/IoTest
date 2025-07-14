using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Mppc
{
    public class StateChangedMessage
    {
        public MppcCollector Collector;
        public string State;
    }

    public class MonitorInfoStatusChangedMessage
    {
        public MppcCollector Collector;
        public string MonitorInfoStatus;
    }

    public class OutputCurrentChangedMessage
    {
        public MppcCollector Collector;
        public double OutputCurrent;
    }

    public class OutputVoltageChangedMessage
    {
        public MppcCollector Collector;
        public double OutputVoltage;
    }

    public class OutputVoltageSettingChangedMessage
    {
        public MppcCollector Collector;
        public double OutputVoltageSetting;
    }

    public class HighVoltageStatusChangedMessage
    {
        public MppcCollector Collector;
        public string HighVoltageStatus;
    }

    public class StateSignalsChangedMessage
    {
        public MppcCollector Collector;
        public MppcStateModule StateSignals;
    }

    public class TemperatureChangedMessage
    {
        public MppcCollector Collector;
        public double Temperature;
    }

    public class SensorTemperatureChangedMessage
    {
        public MppcCollector Collector;
        public double SensorTemperature;
    }

    public class PowerFctReadChangedMessage
    {
        public MppcCollector Collector;
        public double PowerFctRead;
    }

    public class TempCorrectionFactorReadChangedMessage
    {
        public MppcCollector Collector;
        public double TempCorrectionFactorRead;
    }

    public class FirmwareChangedMessage
    {
        public MppcCollector Collector;
        public string Firmware;
    }

    public class IdentifierChangedMessage
    {
        public MppcCollector Collector;
        public string Identifier;
    }
}
