using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.PlcScreen
{
    public abstract class PlcScreenBase : DeviceBase
    {
#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public override DeviceFamily Family => DeviceFamily.Screen;

        protected ScreenController ScreenController;

        public ScreenConfig Config { get; set; }

        // For test purpose only
        public PlcScreenBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger) { }

        public PlcScreenBase(IGlobalStatusServer globalStatusServer, ILogger logger, ScreenConfig config, ScreenController controller)
            : base(globalStatusServer, logger)
        {
            Config = config;
            ScreenController = controller;
        }

        public virtual void Init()
        {
            Name = Config.Name;
            DeviceID = Config.DeviceID;
            Logger.Information($"Init device {Family}-{Name}");
        }

        public abstract void PowerOn();

        public abstract void PowerOff();

        public abstract void TriggerUpdateEvent();
    }
}
