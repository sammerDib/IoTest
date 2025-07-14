using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.OpticalPowermeter
{
    public abstract class OpticalPowermeterBase : IDevice
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        public string Name { get; set; }
        public DeviceState State { get; set; } = new DeviceState(DeviceStatus.Unknown);
        public DeviceFamily Family => DeviceFamily.OpticalPowermeter;
        public string DeviceID { get; set; }

        protected ILogger Logger;

#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event StateChangedEventHandler OnStatusChanged;

        public OpticalPowermeterConfig Configuration { get; set; }

        public abstract void Init(OpticalPowermeterConfig config);

        public abstract void Connect();

        public abstract void TriggerUpdateEvent();

        public virtual void ReadPower() { }

        public virtual void CustomCommand(string custom) { }

        public virtual void EnableAutoRange(bool activate) { }

        public virtual void RangesVariation(string range) { }

        public virtual void StartDarkAdjust() { }

        public virtual void CancelDarkAdjust() { }

        public virtual void EditResponsivity(double responsivity_mA_W) { }
    }
}
