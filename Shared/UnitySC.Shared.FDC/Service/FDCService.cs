using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.FDC.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FDCService : DuplexServiceBase<IFDCServiceCallback>, IFDCService
    {
        private FDCManager _fdcManager;

        public FDCService(ILogger logger, FDCManager fdcManager) : base(logger, ExceptionType.HardwareException)
        {
            _fdcManager = fdcManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<SendFDCMessage>(this, (r, m) => { UpdateFDCData(m.Data); });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
            });
        }


        public Response<List<FDCItemConfig>> GetFDCsConfig()
        {
            return InvokeDataResponse(messageContainer =>
            {
                return _fdcManager.GetFDCsConfig();
            });
        }

        public Response<VoidResult> SetFDCsConfig(List<FDCItemConfig> fdcItemsConfig)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _fdcManager.SetFDCsConfig(fdcItemsConfig);
            });
        }

        public Response<VoidResult> ResetFDC(string fdcName)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _fdcManager.ResetFDC(fdcName);
            });
        }

        public Response<VoidResult> SetInitialCountdownValue(string fdcName, double initialCountdownValue)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _fdcManager.SetInitialCountdownValue(fdcName, initialCountdownValue);
            });
        }
        public Response<FDCData> GetFDC(string fdcName)
        {
            return InvokeDataResponse(messageContainer =>
            {
                return _fdcManager.GetFDC(fdcName);
            });
        }

        public void UpdateFDCData(FDCData fdcData)
        {
            InvokeCallback(i => i.UpdateFDCDataCallback(fdcData));
        }

    }
}
