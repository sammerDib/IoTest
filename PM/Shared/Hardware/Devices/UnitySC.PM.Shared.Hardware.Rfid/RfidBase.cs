using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Rfids;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Rfid;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Rfid
{
    public abstract class RfidBase : DeviceBase
    {
#pragma warning disable CS0067 // Event not used. Not detected by the compiler


        public override DeviceFamily Family => DeviceFamily.Rfid;

        protected RfidController RfidController;

        public RfidConfig Config { get; set; }

        // For test purpose only
        public RfidBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger) { }

        public RfidBase(IGlobalStatusServer globalStatusServer, ILogger logger, RfidConfig config, RfidController controller)
             : base(globalStatusServer, logger)
        {
            Config = config;
            RfidController = controller;
        }

        public virtual void Init()
        {
            Name = Config.Name;
            DeviceID = Config.DeviceID;
            Logger.Information($"Init device {Family}-{Name}");
        }

        public abstract RfidTag GetTag();

        public abstract void TriggerUpdateEvent();
    }
}
