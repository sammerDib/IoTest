using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ffus;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Ffu
{
    public abstract class FfuBase : DeviceBase
    {

        public override DeviceFamily Family => DeviceFamily.Ffu;

        protected FfuController FfuController;

        public FfuConfig Config { get; set; }

        // For test purpose only
        public FfuBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger) { }

        public FfuBase(IGlobalStatusServer globalStatusServer, ILogger logger, FfuConfig config, FfuController ffuController)
            : base(globalStatusServer, logger)
        {
            Config = config;
            FfuController = ffuController;
        }

        public virtual void Init()
        {
            Name = Config.Name;
            DeviceID = Config.DeviceID;
            Logger.Information($"Init device {Family}-{Name}");
        }

        public abstract void PowerOn();

        public abstract void PowerOff();

        public abstract void SetSpeed(ushort speedPercent);

        public abstract void CustomCommand(string custom);

        public abstract void TriggerUpdateEvent();
        public abstract Dictionary<string, ushort> GetDefaultFfuValues();
    }
}
