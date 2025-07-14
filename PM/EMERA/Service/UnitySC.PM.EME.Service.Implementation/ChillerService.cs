using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Chiller;
using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChillerService : DuplexServiceBase<IChillerServiceCallback>, IChillerService
    {
        private readonly EmeHardwareManager _hardwareManager;
        public ChillerService(ILogger logger, IMessenger messenger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            messenger.Register<ChillerTemperatureChangedMessage>(this, (r, m) => { UpdateTemperature(m.Value); });
            messenger.Register<ChillerFanSpeedChangedMessage>(this, (r, m) => { UpdateFanSpeed(m.Value); });
            messenger.Register<ChillerMaxCompressionSpeedChangedMessage>(this, (r, m) => { UpdateMaxCompressionSpeed(m.Value); });
            messenger.Register<ChillerConstantFanSpeedModeChangedMessage>(this, (r, m) => { UpdateConstantFanSpeedMode(m.Value); });
            messenger.Register<ChillerModeChangedMessage>(this, (r, m) => { UpdateChillerMode(m.Value); });
        }

        private void UpdateChillerMode(ChillerMode value)
        {
            InvokeCallback(i => i.UpdateMode(value));
        }

        private void UpdateConstantFanSpeedMode(ConstFanSpeedMode value)
        {
            InvokeCallback(i => i.UpdateConstantFanSpeedModeCallback(value));
        }

        private void UpdateMaxCompressionSpeed(double value)
        {
            InvokeCallback(i => i.UpdateMaxCompressionSpeedCallback(value));
        }

        private void UpdateTemperature(double messageTemperature)
        {
            InvokeCallback(i => i.UpdateTemperatureCallback(messageTemperature));
        }
        
        private void UpdateFanSpeed(double messageTemperature)
        {
            InvokeCallback(i => i.UpdateFanSpeedCallback(messageTemperature));
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Subscribe();
            });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Unsubscribe();
            });
        }
        
        public Response<VoidResult> SetTemperature(double value)
        {
            return InvokeVoidResponse(x => _hardwareManager.Chiller.SetTemperature(value));
        }

        public Response<VoidResult> SetConstFanSpeedMode(ConstFanSpeedMode value)
        {
            return InvokeVoidResponse(x => _hardwareManager.Chiller.SetConstantFanSpeedMode(value));
        }
        
        public Response<VoidResult> SetFanSpeed(double value)
        {
            return InvokeVoidResponse(x => _hardwareManager.Chiller.SetFanSpeed(value));
        }

        public Response<VoidResult> SetMaxCompressionSpeed(double value)
        {
            return InvokeVoidResponse(x => _hardwareManager.Chiller.SetMaxCompressionSpeed((int)value));
        }
        
        public Response<VoidResult> Reset()
        {
            return InvokeVoidResponse(x => _hardwareManager.Chiller.Reset());
        }
        
        public Response<VoidResult> SetChillerMode(ChillerMode value)
        {
            return InvokeVoidResponse(x => _hardwareManager.Chiller.SetChillerMode(value));
        }
    }
}
