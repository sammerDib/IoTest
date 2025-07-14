using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Shutters;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Shutter
{
    public abstract class ShutterBase : DeviceBase
    {
#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        private ILogger _logger;

        public override DeviceFamily Family => DeviceFamily.Shutter;

        protected ShutterController ShutterController;

        public ShutterConfig Config { get; set; }

        // For test purpose only
        public ShutterBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger) { }

        public ShutterBase(IGlobalStatusServer globalStatusServer, ILogger logger, ShutterConfig config, ShutterController shutterController)
            : base(globalStatusServer, logger)
        {
            Config = config;
            ShutterController = shutterController;
        }

        public virtual void Init()
        {
            Name = Config.Name;
            DeviceID = Config.DeviceID;
            Logger.Information($"Init device {Family}-{Name}");
        }

        public abstract void OpenIris();

        public abstract void CloseIris();

        public abstract void TriggerUpdateEvent();
    }
}
