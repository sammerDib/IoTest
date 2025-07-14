using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChamberService : DuplexServiceBase<IChamberServiceCallback>, IChamberService
    {
        private HardwareManager _hardwareManager;

        public ChamberService(ILogger logger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();

            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<DataAttributesChamberMessage>(this, (r, m) => { UpdateDataAttributes(m.DataAttributes); });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Unsubscribe();
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Subscribe();
            });
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Chamber initialize update");
                _hardwareManager.Chamber.TriggerUpdateEvent();
                messageContainer.Add(new Message(MessageLevel.Information, "Chamber initialize update"));
            });
        }

        public void UpdateDataAttributes(List<DataAttribute> values)
        {
            InvokeCallback(i => i.UpdateDataAttributesCallback(values));
        }

        public Response<List<string>> GetWebcamUrls()
        {
            return InvokeDataResponse(messageContainer => _hardwareManager.Chamber.Configuration.WebcamUrls);
        }

        public async Task<Response<VoidResult>> ResetProcess()
        {
            _logger.Information("Reset Process");
            return await InvokeVoidResponseAsync(async messaVgeContainer =>
            {
                if (_hardwareManager.Chamber is IANAChamber anaChamber)
                {
                    anaChamber.InitProcess();
                }
            });
        }

        public Response<bool> SetChamberLightState(bool value)
        {
            return InvokeDataResponse(messaVgeContainer =>
            {
                _logger.Information("Toogle Light");

                try
                {
                    if (_hardwareManager.Chamber is IANAChamber anaChamber)
                    {
                        anaChamber.SetChamberLightState(value);
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}
