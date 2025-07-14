using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.FiberSwitch
{
    public abstract class FiberSwitchBase : IDevice
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        public string Name { get; set; }
        public DeviceState State { get; set; } = new DeviceState(DeviceStatus.Unknown);
        public DeviceFamily Family => DeviceFamily.FiberSwitch;
        public string DeviceID { get; set; }

        protected ILogger Logger;

#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event StateChangedEventHandler OnStatusChanged;

        public FiberSwitchConfig Configuration { get; set; }

        public abstract void Init(FiberSwitchConfig config);

        public abstract void Connect();

        public abstract void SetPosition(int position);

        public abstract void TriggerUpdateEvent();

        public abstract void GetPosition();

        public abstract void CustomCommand(string custom);
    }
}
