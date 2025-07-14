using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Plc;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Plc
{
    public abstract class PlcBase : DeviceBase
    {
#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        private ILogger _logger;

        public override DeviceFamily Family => DeviceFamily.Plc;

        protected PlcController PlcController;

        public PlcConfig Config { get; set; }

        // For test purpose only
        public PlcBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger) { }

        public PlcBase(IGlobalStatusServer globalStatusServer, ILogger logger, PlcConfig config, PlcController plcController)
            : base(globalStatusServer, logger)
        {
            Config = config;
            PlcController = plcController;
        }

        public virtual void Init()
        {
            Name = Config.Name;
            DeviceID = Config.DeviceID;
            _logger = new HardwareLogger(Config.LogLevel.ToString(), Family.ToString(), Name);
            _logger.Information($"Init device {Family}-{Name}");
        }

        public abstract void TriggerUpdateEvent();

        public abstract void Restart();

        public abstract void Reboot();

        public abstract void StartTriggerOutEmitSignal(double pulseDuration_ms = 1);

        public abstract void SmokeDetectorResetAlarm();

        public abstract void CustomCommand(string custom);
    }
}
