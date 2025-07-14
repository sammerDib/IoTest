using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Laser
{
    public abstract class LaserBase : DeviceBase
    {
#pragma warning disable CS0067 // Event not used. Not detected by the compiler


        public override DeviceFamily Family => DeviceFamily.Laser;

        protected LaserController LaserController;

        public LaserConfig Config { get; set; }

        // For test purpose only
        public LaserBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger) { }

        public LaserBase(IGlobalStatusServer globalStatusServer, ILogger logger, LaserConfig config, LaserController laserController)
            : base(globalStatusServer, logger)
        {
            Config = config;
            LaserController = laserController;
        }

        public virtual void Init()
        {
            Name = Config.Name;
            DeviceID = Config.DeviceID;
            Logger.Information($"Init device {Family}-{Name}");
        }

        public abstract void PowerOn();

        public abstract void PowerOff();

        public abstract void SetPower(double power);

        public abstract void ReadPower();

        public abstract void TriggerUpdateEvent();

        public abstract void CustomCommand(string custom);
    }
}
