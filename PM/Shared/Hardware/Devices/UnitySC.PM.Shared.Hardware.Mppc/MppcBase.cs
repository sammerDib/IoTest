using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Data.Enum;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Mppc
{
    public abstract class MppcBase : IDevice
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        public string Name { get; set; }
        public DeviceState State { get; set; } = new DeviceState(DeviceStatus.Unknown);
        public DeviceFamily Family => DeviceFamily.Mppc;
        public string DeviceID { get; set; }

        protected ILogger Logger;

#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event StateChangedEventHandler OnStatusChanged;

        public MppcConfig Configuration { get; set; }

        public MppcVoltageStabilityStatus VoltageStabilityStatus;

        public abstract void Init(MppcConfig config);

        public abstract void Connect();

        public abstract void TriggerUpdateEvent();

        public abstract void SetOutputVoltage(double voltage);

        public abstract void ManageRelays(bool relayActivated);

        public abstract void TempCorrectionFactorSetting();

        public abstract void SwitchTempCompensationMode();

        public abstract void RefVoltageTempSetting();

        public abstract void PowerFctSetting();

        public abstract void OutputVoltageOn();

        public abstract void OutputVoltageOff();

        public abstract void PowerReset();

        public abstract void CustomCommand(string custom);
    }
}
