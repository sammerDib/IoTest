using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Chiller;
using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeChillerSupervisor : IChillerSupervisor
    {
        private readonly IMessenger _messenger;

        public FakeChillerSupervisor(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> Reset()
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> SetTemperature(double value)
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> SetConstFanSpeedMode(ConstFanSpeedMode mode)
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> SetChillerMode(ChillerMode mode)
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> SetFanSpeed(double value)
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> SetMaxCompressionSpeed(double value)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateFanSpeedCallback(double value)
        {
            _messenger.Send(new FanSpeedChangedMessage() { FanSpeed = value });
        }

        public void UpdateMaxCompressionSpeedCallback(double value)
        {
            _messenger.Send(new CompressionChangedMessage { Compression = value });
        }

        public void UpdateTemperatureCallback(double value)
        {
            _messenger.Send(new TemperatureChangedMessage { Temperature = value });
        }

        public void UpdateConstantFanSpeedModeCallback(ConstFanSpeedMode value)
        {
            _messenger.Send(new FanSpeedModeChangedMessage { ConstFanSpeedMode = value });
        }

        public void UpdateAlarms(AlarmDetection value)
        {
            _messenger.Send(new AlarmChangedMessage { Alarm = value });
        }

        public void UpdateLeaks(LeakDetection value)
        {
            _messenger.Send(new LeakDetectionChangedMessage { Leak = value });
        }

        public void UpdateMode(ChillerMode value)
        {
            _messenger.Send(new ChillerModeChangedMessage { Mode = value });
        }
    }
}
