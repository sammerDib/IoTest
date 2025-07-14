using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Ionizer
{
    public abstract class IonizerBase : DeviceBase
    {
#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        private ILogger _logger;

        public override DeviceFamily Family => DeviceFamily.Ionizer;

        public IonizerConfig Configuration { get; set; }

        // For test purpose only
        public IonizerBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger) { }

        public IonizerBase(IGlobalStatusServer globalStatusServer, ILogger logger, IonizerConfig config)
            : base(globalStatusServer, logger)
        {
            Configuration = config;
        }

        public virtual void Init()
        {
            Name = Configuration.Name;
            DeviceID = Configuration.DeviceID;
            _logger.Information($"Init device {Family}-{Name}");
        }

        public static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        public abstract void OpenAirPneumaticValve();

        public abstract void CloseAirPneumaticValve();

        public abstract void TriggerUpdateEvent();
    }
}
