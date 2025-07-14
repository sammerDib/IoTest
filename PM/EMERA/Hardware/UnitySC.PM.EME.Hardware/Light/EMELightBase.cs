using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Hardware.Light
{
    public abstract class EMELightBase : DeviceBase
    {
        private ILogger Logger { get; set; }

        public readonly EMELightConfig Config;

        public virtual bool IsMainLight => Config.IsMainLight;

        // For test purpose only
        protected EMELightBase() : base(null, ClassLocator.Default.GetInstance<ILogger>())
        {
        }

        protected EMELightBase(EMELightConfig config, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(globalStatusServer, logger)
        {
            Logger = logger;
            Config = config;
        }

        public virtual void Init()
        {
            Name = Config.Name;
            DeviceID = Config.DeviceID;
            Logger = new HardwareLogger(Config.LogLevel.ToString(), Family.ToString(), Name);
            Logger.Information($"Init device {Family}-{Name}");
        }

        public abstract void InitLightSources();

        public abstract void SwitchOn(bool powerOn);

        public abstract void SetPower(double power);
        public abstract double GetPower();
        public abstract void RefreshPower();

        public abstract void RefreshSwitchOn();

        public abstract void RefreshLightSource();
    }
}
