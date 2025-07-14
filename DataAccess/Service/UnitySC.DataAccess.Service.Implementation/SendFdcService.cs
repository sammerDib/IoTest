using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]

    public class SendFdcService : DataAccesDuplexServiceBase<ISendFdcServiceCallback>, ISendFdcService
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();
        public SendFdcService(ILogger logger) : base(logger)
        {
            Messenger.Register<SendFDCListMessage>(this, (r, m) => { SendFDCsData(m.FDCsData); });
        }

        private void SendFDCsData(List<FDCData> fDCsData)
        {
            InvokeCallback(x => x.SendFDCs(fDCsData));
        }

        public Response<VoidResult> RequestAllFDCsUpdate()
        {
            return InvokeVoidResponse<object>(() =>
            {
                ClassLocator.Default.GetInstance<FDCManager>().RequestAllFDCsUpdate();
                return null;
            });
        }
    }
}
