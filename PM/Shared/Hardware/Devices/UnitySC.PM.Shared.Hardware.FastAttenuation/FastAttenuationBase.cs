using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.FastAttenuation
{
    public abstract class FastAttenuationBase : IDevice
    {
        protected static GalaSoft.MvvmLight.Messaging.IMessenger Messenger => ClassLocator.Default.GetInstance<GalaSoft.MvvmLight.Messaging.IMessenger>();

        public string Name { get; set; }
        public DeviceState State { get; set; } = new DeviceState(DeviceStatus.Unknown);
        public DeviceFamily Family => DeviceFamily.FastAttenuation;
        public string DeviceID { get; set; }

        protected ILogger Logger;

#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event StateChangedEventHandler OnStatusChanged;

        public FastAttenuationConfig Configuration { get; set; }

        public abstract void Init(FastAttenuationConfig config);

        public abstract void Connect();

        public abstract void TriggerUpdateEvent();

        public abstract void MoveAbsPosition(double position);
    }
}
