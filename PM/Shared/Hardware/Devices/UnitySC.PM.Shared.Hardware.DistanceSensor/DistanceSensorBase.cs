using UnitySC.PM.Shared.Hardware.Controllers.Controllers.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.DistanceSensor
{
    public abstract class DistanceSensorBase : DeviceBase
    {
#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public override DeviceFamily Family => DeviceFamily.DistanceSensor;

        protected DistanceSensorController DistanceSensorController;

        public DistanceSensorConfig Config { get; set; }

        // For test purpose only
        public DistanceSensorBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger) { }

        public DistanceSensorBase(IGlobalStatusServer globalStatusServer, ILogger logger, DistanceSensorConfig config, DistanceSensorController distanceSensorController)
            : base(globalStatusServer, logger)
        {
            Config = config;
            DistanceSensorController = distanceSensorController;
        }

        public virtual void Init()
        {
            Name = Config.Name;
            DeviceID = Config.DeviceID;
            Logger.Information($"Init device {Family}-{Name}");
        }

        public abstract double GetDistanceSensorHeight();

        public abstract void TriggerUpdateEvent();        

        public abstract void CustomCommand(string custom);
    }
}
