using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Chamber
{
    public abstract class ChamberBase : DeviceBase
    {
#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public override DeviceFamily Family => DeviceFamily.Chamber;

        public ChamberConfig Configuration { get; set; }

        // For test purpose only
        public ChamberBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger) { }

        public ChamberBase(IGlobalStatusServer globalStatusServer, ILogger logger, ChamberConfig config)
            : base(globalStatusServer, logger)
        {
            Configuration = config;
        }

        public virtual void Init()
        {
            Name = Configuration.Name;
            DeviceID = Configuration.DeviceID;
            Logger.Information($"Init device {Family}-{Name}");
        }

        public static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        public abstract void TriggerUpdateEvent();
    }
}
